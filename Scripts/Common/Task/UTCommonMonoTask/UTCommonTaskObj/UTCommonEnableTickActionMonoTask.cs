using System;

namespace UTGame
{
    public class UTCommonEnableTickActionMonoTask : UTCommonTaskController._AUTEnableMonoTask, _IUTBaseMonoTask, _IUTCommonTaskMonitorInterface
    {
        #region 对外接口
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        protected static UTCommonEnableTickActionMonoTask _createTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
            UTCommonEnableTickActionMonoTask task = UTCommonEnableTickActionTaskCache.instance.popItem();

#if UNITY_EDITOR
            //添加到Editor监控
            if (null != _container)
                _container.addMonitor(task);
#endif

            task.setAction(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            //注册
            UTCommonTaskController._AUTEnableMonoTask.UTEnableMonoTaskMgr.instance.regTask(task);

            return task;
        }
        /** 对外开放的任务创建操作函数 */
        public static UTCommonEnableTaskController addMonoTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addMonoTask(task);

            return new UTCommonEnableTaskController(task.serialize);
        }
        public static UTCommonEnableTaskController addMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addMonoTask(task, _delayTime);

            return new UTCommonEnableTaskController(task.serialize);
        }

        public static UTCommonEnableTaskController addScaleTimeDelayMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addScaleTimeDelayMonoTask(task, _delayTime);

            return new UTCommonEnableTaskController(task.serialize);
        }
        public static UTCommonEnableTaskController addScaleTimeDelayLaterMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addScaleTimeDelayLaterMonoTask(task, _delayTime);

            return new UTCommonEnableTaskController(task.serialize);
        }

        public static UTCommonEnableTaskController addNextFrameTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addNextFrameTask(task);

            return new UTCommonEnableTaskController(task.serialize);
        }
        #endregion

        /** 对外开放的任务创建操作函数终结 */

        private Action _m_dAction;
#if UNITY_EDITOR 
        private UTCommonTaskMonitorContainer _m_tmcTaskMonitor;
#endif

        protected UTCommonEnableTickActionMonoTask()
            : base()
        {
            _m_dAction = null;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        public void setAction(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
#if UNITY_EDITOR
            if(_delegate == null)
            {
                UnityEngine.Debug.LogError("UTCommonEnableGUITickActionMonoTask 传入Action为空");
            }
#endif
            _m_dAction = _delegate;

#if UNITY_EDITOR
            _m_tmcTaskMonitor = _container;
#endif
        }

        public override void deal()
        {
            if(!_m_bIsEnable)
            {
#if UNITY_EDITOR
                if (null != _m_tmcTaskMonitor)
                    _m_tmcTaskMonitor.rmvMonitor(this);
#endif

                //注销
                UTCommonTaskController._AUTEnableMonoTask.UTEnableMonoTaskMgr.instance.popTask(serialize);
                //放回缓存
                UTCommonEnableTickActionTaskCache.instance.pushBackCacheItem(this);
                _m_dAction = null;
                return;
            }

            if (null != _m_dAction)
                _m_dAction();

            //放入下一帧
            UTMonoTaskMgr.instance.addNextFrameTask(this);
        }

        /// <summary>
        /// 重载几个重置接口
        /// </summary>
        protected override void _onDisable()
        {
            _m_dAction = null;
#if UNITY_EDITOR
            if (null != _m_tmcTaskMonitor)
                _m_tmcTaskMonitor.rmvMonitor(this);
            _m_tmcTaskMonitor = null;
#endif
        }
        protected override void _onReset()
        {
            _m_dAction = null;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        /// <summary>
        /// 释放本任务对象的相关资源或者关联
        /// </summary>
        public void discard()
        {
            _m_dAction = null;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        public override string ToString()
        {
            if(_m_dAction != null)
            {
                return $"ALCommonEnableTickActionMonoTask:{GCommon.ToReadableString(_m_dAction)}";
            }
            else
            {
                return $"ALCommonEnableTickActionMonoTask:null";
            }
        }
        
        public class UTCommonEnableTickActionTaskCache : _ACacheControllerBase<UTCommonEnableTickActionMonoTask, UTCommonEnableTickActionMonoTask>
        {
            private static UTCommonEnableTickActionTaskCache _g_instance = new UTCommonEnableTickActionTaskCache();
            public static UTCommonEnableTickActionTaskCache instance
            {
                get
                {
                    if(null == _g_instance)
                        _g_instance = new UTCommonEnableTickActionTaskCache();
                    return _g_instance;
                }
            }

            public UTCommonEnableTickActionTaskCache() : base(64, 256)
            {
                init(new UTCommonEnableTickActionMonoTask());
            }

            protected override UTCommonEnableTickActionMonoTask _createItem(UTCommonEnableTickActionMonoTask _template)
            {
                return new UTCommonEnableTickActionMonoTask();
            }

            //警告信息文字
            protected override string _warningTxt { get { return "UTCommonEnableTickActionTaskCache"; } }

            protected override void _discardItem(UTCommonEnableTickActionMonoTask _item)
            {
                _item._reset();
                return;
            }

            protected override void _onInit(UTCommonEnableTickActionMonoTask _template)
            {
            }

            protected override void _resetItem(UTCommonEnableTickActionMonoTask _item)
            {
                _item._reset();
            }
        }
    }
}
