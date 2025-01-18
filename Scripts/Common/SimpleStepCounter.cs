using System;

namespace WLDDZ
{
    /// <summary>
    /// 一个简单的步骤控制器，用于多个步骤完成的统一回调
    /// </summary>
    public class SimpleStepCounter
    {
        /** 所有步骤完成后的回调对象 */
        private Action _m_dOnAllStepDone;

        /** 总的需要的步骤数量 */
        private int _m_iTotalStepCount;

        /** 目前完成的步骤数量 */
        private int _m_iCurDoneStepCount;

        public SimpleStepCounter()
        {
            _m_dOnAllStepDone = null;

            resetStepInfo();
        }

        /****************
         * 重置所有步骤的统计信息
         **/
        public void resetStepInfo()
        {
            //初始化步骤数量变量
            lock (this)
            {
                _m_iTotalStepCount = 0;
                _m_iCurDoneStepCount = 0;
            }
        }

        /****************
         * 重置
         **/
        public void resetAll()
        {
            resetStepInfo();
            _m_dOnAllStepDone = null;
        }

        public int totalStep
        {
            get { return _m_iTotalStepCount; }
        }

        public int doneStep
        {
            get { return _m_iCurDoneStepCount; }
        }

        /****************
         * 修改总的需要完成的步骤数
         **/
        public void chgTotalStepCount(int _chgStepCount)
        {
            _m_iTotalStepCount += _chgStepCount;
        }

        /****************
         * 增加一个已经完成的步骤数
         **/
        public void addDoneStepCount()
        {
            Action needDealAction = null;

            lock (this)
            {
                _m_iCurDoneStepCount++;

                //判断步骤数量是否达到总数量，达到则需要调用加载完成的事件函数
                if (_m_iCurDoneStepCount >= _m_iTotalStepCount)
                {
                    //获取当前需要处理的回调对象
                    needDealAction = _m_dOnAllStepDone;
                    _m_dOnAllStepDone = null;
                }
            }

            //判断是否需要执行操作
            if (null != needDealAction)
                needDealAction();
            needDealAction = null;
        }

        /*******************
         * 注册监听是否全部加载完的回调函数
         **/
        public void regAllDoneDelegate(Action _delegate)
        {
            if (null == _delegate)
                return;

            if (_m_iCurDoneStepCount >= _m_iTotalStepCount)
                _delegate();
            else if (null == _m_dOnAllStepDone)
                _m_dOnAllStepDone = _delegate;
            else
                _m_dOnAllStepDone += _delegate;
        }
    }
}