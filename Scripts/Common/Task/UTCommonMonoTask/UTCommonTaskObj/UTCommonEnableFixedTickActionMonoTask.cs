using System;

namespace UTGame
{
    public class UTCommonEnableFixedTickActionMonoTask : UTCommonTaskController._AUTEnableMonoTask, _IUTBaseMonoTask, _IUTCommonTaskMonitorInterface
    {
        #region 对外接口
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        protected static UTCommonEnableFixedTickActionMonoTask _createTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
            UTCommonEnableFixedTickActionMonoTask task = UTCommonEnableFixedTickActionTaskCache.instance.popItem();

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
        public static UTCommonEnableTaskController addFixedMonoTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableFixedTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addFixedMonoTask(task);

            return new UTCommonEnableTaskController(task.serialize);
        }
        public static UTCommonEnableTaskController addFixedMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableFixedTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addFixedMonoTask(task, _delayTime);

            return new UTCommonEnableTaskController(task.serialize);
        }

        public static UTCommonEnableTaskController addNextFixedUpdateTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableFixedTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addNextFixedUpdateTask(task);

            return new UTCommonEnableTaskController(task.serialize);
        }
        
        public static UTCommonEnableTaskController addScaleTimeDelayFixedMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableFixedTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addScaleTimeDelayFixedMonoTask(task, _delayTime);

            return new UTCommonEnableTaskController(task.serialize);
        }
        #endregion

        /** 对外开放的任务创建操作函数终结 */

        private Action _m_dAction;
#if UNITY_EDITOR 
        private UTCommonTaskMonitorContainer _m_tmcTaskMonitor;
#endif

        protected UTCommonEnableFixedTickActionMonoTask()
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
                UnityEngine.Debug.LogError("GCommonEnableFixedTickActionMonoTask 传入Action为空");
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
                UTCommonEnableFixedTickActionTaskCache.instance.pushBackCacheItem(this);
                _m_dAction = null;
                return;
            }
            
            if(null != _m_dAction)
                _m_dAction();

            //放入下一帧
            UTMonoTaskMgr.instance.addNextFixedUpdateTask(this);
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
                return $"GCommonEnableFixedTickActionMonoTask:{GCommon.ToReadableString(_m_dAction)}";
            }
            else
            {
                return $"GCommonEnableFixedTickActionMonoTask:null";
            }
        }

        public class UTCommonEnableFixedTickActionTaskCache : _ACacheControllerBase<UTCommonEnableFixedTickActionMonoTask, UTCommonEnableFixedTickActionMonoTask>
        {
            private static UTCommonEnableFixedTickActionTaskCache _g_instance = new UTCommonEnableFixedTickActionTaskCache();
            public static UTCommonEnableFixedTickActionTaskCache instance
            {
                get
                {
                    if(null == _g_instance)
                        _g_instance = new UTCommonEnableFixedTickActionTaskCache();
                    return _g_instance;
                }
            }

            public UTCommonEnableFixedTickActionTaskCache() : base(64, 256)
            {
                init(new UTCommonEnableFixedTickActionMonoTask());
            }

            protected override UTCommonEnableFixedTickActionMonoTask _createItem(UTCommonEnableFixedTickActionMonoTask _template)
            {
                return new UTCommonEnableFixedTickActionMonoTask();
            }

            //警告信息文字
            protected override string _warningTxt { get { return "GCommonEnableTickActionTaskCache"; } }

            protected override void _discardItem(UTCommonEnableFixedTickActionMonoTask _item)
            {
                _item._reset();
                return;
            }

            protected override void _onInit(UTCommonEnableFixedTickActionMonoTask _template)
            {
            }

            protected override void _resetItem(UTCommonEnableFixedTickActionMonoTask _item)
            {
                _item._reset();
            }
        }
    }
}
