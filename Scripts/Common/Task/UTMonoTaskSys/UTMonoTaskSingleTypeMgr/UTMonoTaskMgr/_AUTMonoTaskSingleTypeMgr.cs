using System.Collections.Generic;

/*****************************
 * 单独类型的任务管理对象
 **/
namespace UTGame
{
    public abstract class _AUTMonoTaskSingleTypeMgr
    {
        /** 当前任务队列由于此队列只从队列头抽取对象，并且仅在队列尾插入对象 */
        private List<_IUTBaseMonoTask> _m_lCurrentTaskList;
        private int _m_iCurTaskIndex = 0;

        /** 每秒检测的次数 */
        private int _m_iCheckTimePerSec;
        /** 定时任务的大区间长度，根据精度和长度可以决定一个区间的时间跨度 */
        private int _m_iTimingTaskCheckAreaSize;
        /** 定时任务开启处理的时间标记 */
        private float _m_fTimingTaskMgrStartTime;

        /** 最后一次检测的回合数和对应下标 */
        private int _m_iLastCheckRound;
        private int _m_iLastCheckTick;

        /** 记录固定队列长度的定时任务存储队列，用数组便于根据下标查询 */
        private UTMonoTaskTimingNode[] _m_arrTimingTaskNodeList;

        /** 临时用于检测时间标记所使用的临时任务队列，只在一处调用，因此不加锁 */
        private List<_IUTBaseMonoTask> _m_lTmpTimingTaskList;

        public _AUTMonoTaskSingleTypeMgr(int _checkTimePerSec, int _checkAreaNodeSize)
        {
            _m_lCurrentTaskList = new List<_IUTBaseMonoTask>();

            //读取配置
            _m_iCheckTimePerSec = _checkTimePerSec;
            if (_m_iCheckTimePerSec < 1)
                _m_iCheckTimePerSec = 1;
            if (_m_iCheckTimePerSec > 100)
                _m_iCheckTimePerSec = 100;

            _m_iTimingTaskCheckAreaSize = _checkAreaNodeSize;
            if (_m_iTimingTaskCheckAreaSize < 300)
                _m_iTimingTaskCheckAreaSize = 300;
            if (_m_iTimingTaskCheckAreaSize > 2000)
                _m_iTimingTaskCheckAreaSize = 2000;

            //获取开启管理对象的时间
            _m_fTimingTaskMgrStartTime = _getNowTime();
            //初始化最后一次检测的位置信息
            _m_iLastCheckRound = 0;
            //从-1开始，避免错过第一个节点
            _m_iLastCheckTick = -1;

            //创建固定区间长度的精度队列
            _m_arrTimingTaskNodeList = new UTMonoTaskTimingNode[_m_iTimingTaskCheckAreaSize];
            for (int i = 0; i < _m_iTimingTaskCheckAreaSize; i++)
            {
                _m_arrTimingTaskNodeList[i] = new UTMonoTaskTimingNode(0);
            }

            _m_lTmpTimingTaskList = new List<_IUTBaseMonoTask>();
        }

        /*****************
         * 注册一个直接执行的任务
         * 
         * @author alzq.z
         * @time   Feb 18, 2013 11:01:18 PM
         */
        public void regTask(_IUTBaseMonoTask _task)
        {
            lock (_m_lCurrentTaskList)
            {
                //向链表中添加执行任务
                _m_lCurrentTaskList.Add(_task);
            }
        }
        public void regTask(List<_IUTBaseMonoTask> _taskList)
        {
            lock (_m_lCurrentTaskList)
            {
                //向链表中添加执行任务
                _m_lCurrentTaskList.AddRange(_taskList);
            }
        }

        /*********************
         * 注册定时执行的任务
         * 
         * @author alzq.z
         * @time   Feb 18, 2013 11:04:48 PM
         */
        public void regTask(_IUTBaseMonoTask _task, float _time)
        {
            lock (_m_arrTimingTaskNodeList)
            {
                //判断定时时间，当非法，则直接当作当前执行任务插入
                if (_time <= 0)
                {
                    regTask(_task);
                }
                else
                {
                    //获取当前时间
                    float nowTime = _getNowTime();
                    //计算需要注册的时间差
                    float deltaTime = nowTime + _time - _m_fTimingTaskMgrStartTime;
                    if(deltaTime < 0)
                    {
                        //报错
                        UnityEngine.Debug.LogError($"Reg Timing Task time err： _time[{_time}] - task:[{_task.ToString()}]");
                        //此时注册时间数据异常，我们按照5秒直接注册
                        deltaTime = 5f;
                        return;
                    }

                    //注册定时任务，将任务添加到表中等待插入到执行队列
                    if (!_regTimingTask(nowTime + _time - _m_fTimingTaskMgrStartTime, _task))
                        regTask(_task);
                }
            }
        }

        /*********************
         * 提取出第一个需要执行的任务
         * 
         * @author alzq.z
         * @time   Feb 18, 2013 11:17:58 PM
         */
        public _IUTBaseMonoTask popCurrentTask()
        {
            lock (_m_lCurrentTaskList)
            {
                //判断任务队列是否为空
                if (_m_iCurTaskIndex >= _m_lCurrentTaskList.Count)
                {
                    //当下标超过队列长度的时候，直接重置下标及队列，避免多线程在外围重置的时候可能出现的放入任务被清空的可能
                    _m_iCurTaskIndex = 0;
                    _m_lCurrentTaskList.Clear();
                    return null;
                }

                //取出并移除任务队列第一个任务
                _IUTBaseMonoTask task = _m_lCurrentTaskList[_m_iCurTaskIndex];
                _m_iCurTaskIndex++;

                return task;
            }
        }

        /********************
         * 每帧进行的检测函数
         **/
        public void frameCheckTask()
        {
            //取出所有时间队列
            _popTimerTask(_m_lTmpTimingTaskList);

            //将任务队列放入当前任务管理对象
            _registerTaskList(_m_lTmpTimingTaskList);
        }

        /****************
         * 注册整个队列的定时任务
         * 系统内接口不对外开放
         * 
         * @author alzq.z
         * @time   Feb 18, 2013 11:32:18 PM
         */
        protected void _registerTaskList(List<_IUTBaseMonoTask> _taskList)
        {
            //在循环内加锁有利于大量任务插入时长时间阻塞处理线程的情况
            lock (_m_lCurrentTaskList)
            {
                for(int i = 0; i < _taskList.Count; i++)
                {
                    _m_lCurrentTaskList.Add(_taskList[i]);
                }

                _taskList.Clear();
            }
        }

        /****************
         * 在定时任务处理表中添加定时执行的任务，带入的_dealTime是距离本任务管理对象开启的时间间隔
         * 
         * @author alzq.z
         * @time   Feb 18, 2013 11:06:12 PM
         */
        protected bool _regTimingTask(float _dealTime, _IUTBaseMonoTask _task)
        {
            lock(_m_arrTimingTaskNodeList)
            {
                //根据时间精度以及回合总时间区域计算对应的回合数以及时间节点
                int tick = ((int)(_dealTime * _m_iCheckTimePerSec)) + 1;
                int round = tick / _m_iTimingTaskCheckAreaSize;
                //计算实际的下标数
                tick = tick - (round * _m_iTimingTaskCheckAreaSize);

                //当下标和回合等于最后一次检测的数据时将任务移到下一个下标中进行处理
                if (tick <= _m_iLastCheckTick && round <= _m_iLastCheckRound)
                {
                    tick++;
                    if (tick >= _m_iTimingTaskCheckAreaSize)
                    {
                        tick -= _m_iTimingTaskCheckAreaSize;
                        round++;
                    }
                }

                //将任务添加到对应下标
                return _m_arrTimingTaskNodeList[tick].addTimingTask(round, _task);
            }
        }

        /********************
         * 将到目前为止的所有定时任务取出
         * 
         * @author alzq.z
         * @time   Feb 18, 2013 11:23:11 PM
         */
        protected void _popTimerTask(List<_IUTBaseMonoTask> _recList)
        {
            if (null == _recList)
                return;
            
            lock(_m_arrTimingTaskNodeList)
            {
                //获取运行的时间
                float dealTime = _getNowTime() - _m_fTimingTaskMgrStartTime;
                //根据运行时间计算出当前时间对应的下标
                int tick = ((int)(dealTime * _m_iCheckTimePerSec)) + 1;
                int round = tick / _m_iTimingTaskCheckAreaSize;
                //计算实际的下标数
                tick = tick - (round * _m_iTimingTaskCheckAreaSize);

                //将下标和round不断累加获取需要执行的任务
                while (_m_iLastCheckTick < tick || _m_iLastCheckRound < round)
                {
                    //累加下标
                    _m_iLastCheckTick++;
                    //判断下标是否越界
                    if (_m_iLastCheckTick >= _m_iTimingTaskCheckAreaSize)
                    {
                        _m_iLastCheckTick -= _m_iTimingTaskCheckAreaSize;
                        _m_iLastCheckRound++;
                    }

                    //获取对应数据
                    _m_arrTimingTaskNodeList[_m_iLastCheckTick].popAllRoundTaskAndMoveNextRound(_m_iLastCheckRound, _recList);
                }
            }
        }

        /**************
         * 获取时间标记
         **/
        protected abstract float _getNowTime();
    }
}
