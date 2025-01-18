using System.Collections.Generic;
using UnityEngine;

/**********************
 * 单个时间节点下的任务管理对象
 **/
namespace UTGame
{
    public class UTMonoTaskTimingNode
    {
        /** 当前回合标记 */
        private int _m_iCurRound;
        /** 当前回合的本节点定时任务队列 */
        private List<_IUTBaseMonoTask> _m_lCurRoundTimingTaskList;

        /** 下一回合标记 */
        private int _m_iNextRound;
        /** 固定记录下一回合的延迟任务队列集合 */
        private List<_IUTBaseMonoTask> _m_lNextRoundTimingTaskList;
    
        /** 非本回合与下一回合的较长时间延迟任务信息集合队列 */
        private LinkedList<UTMonoTaskTimingNodeFarDelayTaskInfo> _m_lFarDelayTaskList;

        public UTMonoTaskTimingNode(int _round)
        {
            _m_iCurRound = _round;
            _m_lCurRoundTimingTaskList = new List<_IUTBaseMonoTask>();
        
            _m_iNextRound = _round + 1;
            _m_lNextRoundTimingTaskList = new List<_IUTBaseMonoTask>();

            _m_lFarDelayTaskList = new LinkedList<UTMonoTaskTimingNodeFarDelayTaskInfo>();
        }

        /***************
         * 添加指定回合的本时间节点的任务，返回是否添加成功，不成功则表示需要马上执行
         * 
         * @author alzq.z
         * @time   Jul 16, 2015 11:26:06 PM
         */
        public bool addTimingTask(int _round, _IUTBaseMonoTask _task)
        {
            lock (_m_lCurRoundTimingTaskList)
            {
                if (null == _task)
                    return false;

                //当本回合的时候，直接进行添加
                if (_round == _m_iCurRound)
                {
                    _m_lCurRoundTimingTaskList.Add(_task);
                    return true;
                }
                else if (_round == _m_iNextRound)
                {
                    _m_lNextRoundTimingTaskList.Add(_task);
                    return true;
                }
                else if (_round > _m_iNextRound)
                {
                    //长时间延迟的任务，按照回合顺序放入对应队列
                    LinkedListNode<UTMonoTaskTimingNodeFarDelayTaskInfo> iterator = _m_lFarDelayTaskList.First;
                    while (null != iterator)
                    {
                        UTMonoTaskTimingNodeFarDelayTaskInfo taskInfo = iterator.Value;
                        if (taskInfo.getRound() == _round)
                        {
                            //匹配回合则加入本节点
                            taskInfo.addSynTask(_task);
                            break;
                        }
                        else if (taskInfo.getRound() > _round)
                        {
                            //当查询的节点序号比插入序号还要靠后时
                            //创建新节点，并插入当前查询的节点之前
                            UTMonoTaskTimingNodeFarDelayTaskInfo newInfo = new UTMonoTaskTimingNodeFarDelayTaskInfo(_round);
                            _m_lFarDelayTaskList.AddBefore(iterator, newInfo);
                            //插入任务
                            newInfo.addSynTask(_task);
                            break;
                        }

                        //查询下一个节点
                        iterator = iterator.Next;
                    }

                    //判断是否已经到了最后节点
                    if (null == iterator)
                    {
                        //在最后节点则往最后追加数据
                        //创建新节点
                        UTMonoTaskTimingNodeFarDelayTaskInfo newInfo = new UTMonoTaskTimingNodeFarDelayTaskInfo(_round);
                        _m_lFarDelayTaskList.AddLast(newInfo);
                        //插入任务
                        newInfo.addSynTask(_task);
                    }

                    return true;
                }
                else
                {
                    //当回合小于当前回合则表示失败，外部需要直接处理
                    return false;
                }
            }
        }

        /***************
         * 取出所有对应回合的本节点任务
         * 
         * @author alzq.z
         * @time   Jul 16, 2015 11:49:51 PM
         */
        public void popAllRoundTaskAndMoveNextRound(int _round, List<_IUTBaseMonoTask> _recList)
        {
            if(null == _recList)
                return ;
        
            lock(_m_lCurRoundTimingTaskList)
            {
                //判断回合是否在本回合之前，则不做处理
                if (_round < _m_iCurRound)
                    return;

                //将所有当前回合任务放入队列
                _recList.AddRange(_m_lCurRoundTimingTaskList);
                _m_lCurRoundTimingTaskList.Clear();

                //当回合在本回合之后则弹出错误警告，并将所有对应回合之后的任务放入队列
                if (_round > _m_iCurRound)
                {
                    //可能跳过某个回合，因此输出错误
                    Debug.LogError("Timing Syn Task Round Error! pop round: " + _round + " - current round: " + _m_iCurRound);

                    //移动到对应的回合
                    _moveToRound(_round);
                    //将所有当前回合任务放入队列
                    _recList.AddRange(_m_lCurRoundTimingTaskList);
                    _m_lCurRoundTimingTaskList.Clear();
                }

                //移动到下一回合
                _moveNextRound();
            }
        }
    
        /****************
         * 移动当前回合至指定回合
         * 
         * @author alzq.z
         * @time   Jul 16, 2015 11:57:27 PM
         */
        protected void _moveToRound(int _targetRound)
        {
            //在不匹配对应回合的时候不断往后推移
            while(_m_iCurRound < _targetRound)
            {
                _moveNextRound();
            }
        }
    
        /***************
         * 向下移动一个回合
         * 
         * @author alzq.z
         * @time   Jul 16, 2015 11:58:23 PM
         */
        protected void _moveNextRound()
        {
            //累加一个回合
            _m_iCurRound++;
            _m_iNextRound++;

            //将下一回合任务累加到当前队列
            _m_lCurRoundTimingTaskList.AddRange(_m_lNextRoundTimingTaskList);
            _m_lNextRoundTimingTaskList.Clear();
        
            //获取第一个长时间间隔任务队列节点，是否匹配下一回合，是则将任务放入
            if(_m_lFarDelayTaskList.Count > 0)
            {
                UTMonoTaskTimingNodeFarDelayTaskInfo farDelayInfo = _m_lFarDelayTaskList.First.Value;
                if(farDelayInfo.getRound() == _m_iNextRound)
                {
                    //从长时间间隔队列删除
                    _m_lFarDelayTaskList.RemoveFirst();
                    //将任务放入
                    farDelayInfo.popAllSynTask(_m_lNextRoundTimingTaskList);
                }
            }
        }
    }
}
