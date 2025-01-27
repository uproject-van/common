namespace UTGame
{
    public class UTCycleMonoTask : _IUTBaseMonoTask
    {
        /** 刷新接口 */
        private _IUTBaseTaskInterface _m_iTaskInterface;

        /** 刷新的序列号 */
        private long _m_iShowSerialize;

        public UTCycleMonoTask(_IUTBaseTaskInterface _interface)
        {
            _m_iTaskInterface = _interface;
            _m_iShowSerialize = _interface.showOpSerialize;
        }

        /// <summary>
        /// 任务具体的执行函数
        /// </summary>
        public void deal()
        {
            if (_m_iTaskInterface.showOpSerialize != _m_iShowSerialize)
                return;

            //刷新操作
            _m_iTaskInterface.tick();

            //加入下一帧
            UTMonoTaskMgr.instance.addMonoTask(this, 0.1f);
        }
    }
    
    public class UTCycleFrameMonoTask : _IUTBaseMonoTask
    {
        /** 刷新接口 */
        private _IUTBaseTaskInterface _m_iTaskInterface;

        /** 刷新的序列号 */
        private long _m_iShowSerialize;

        public UTCycleFrameMonoTask(_IUTBaseTaskInterface _interface)
        {
            _m_iTaskInterface = _interface;
            _m_iShowSerialize = _interface.showOpSerialize;
        }

        /// <summary>
        /// 任务具体的执行函数
        /// </summary>
        public void deal()
        {
            if (_m_iTaskInterface.showOpSerialize != _m_iShowSerialize)
                return;

            //刷新操作
            _m_iTaskInterface.tick();

            //加入下一帧
            UTMonoTaskMgr.instance.addNextFrameTask(this);
        }
    }
    public class UTCycleFixedFrameMonoTask : _IUTBaseMonoTask
    {
        /** 刷新接口 */
        private _IUTBaseTaskInterface _m_iTaskInterface;


        /** 刷新的序列号 */
        private long _m_iShowSerialize;

        public UTCycleFixedFrameMonoTask(_IUTBaseTaskInterface _interface)
        {
            _m_iTaskInterface = _interface;
            _m_iShowSerialize = _interface.showOpSerialize;
        }

        /// <summary>
        /// 任务具体的执行函数
        /// </summary>
        public void deal()
        {
            if (_m_iTaskInterface.showOpSerialize != _m_iShowSerialize)
                return;

            //刷新操作
            _m_iTaskInterface.fixedTick();

            //加入下一帧
            UTMonoTaskMgr.instance.addNextFixedUpdateTask(this);
        }
    }
}