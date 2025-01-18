
using System;

namespace UTGame
{
    public class UTCommonEnableLateTickActionMonoTask : UTCommonTaskController._AUTEnableMonoTask, _IUTBaseMonoTask, _IUTCommonTaskMonitorInterface
    {
        #region 对外接口
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        protected static UTCommonEnableLateTickActionMonoTask _createTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
            UTCommonEnableLateTickActionMonoTask task = UTCommonEnableLateTickActionTaskCache.instance.popItem();

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
        public static UTCommonEnableTaskController addLaterMonoTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableLateTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addLaterMonoTask(task);

            return new UTCommonEnableTaskController(task.serialize);
        }
        public static UTCommonEnableTaskController addLaterMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableLateTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addLaterMonoTask(task, _delayTime);

            return new UTCommonEnableTaskController(task.serialize);
        }
        public static UTCommonEnableTaskController addNextFrameLaterTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTCommonEnableLateTickActionMonoTask task = _createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            UTMonoTaskMgr.instance.addNextFrameLaterTask(task);

            return new UTCommonEnableTaskController(task.serialize);
        }
        #endregion

        /** 对外开放的任务创建操作函数终结 */

        private Action _m_dAction;
#if UNITY_EDITOR 
        private UTCommonTaskMonitorContainer _m_tmcTaskMonitor;
#endif

        protected UTCommonEnableLateTickActionMonoTask()
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
                UnityEngine.Debug.LogError("UTCommonEnableLateTickActionMonoTask 传入Action为空");
            }
#endif
            _m_dAction = _delegate;

#if UNITY_EDITOR
            _m_tmcTaskMonitor = _container;
#endif
        }

        public override void deal()
        {
            if (!_m_bIsEnable)
            {
#if UNITY_EDITOR
                if (null != _m_tmcTaskMonitor)
                    _m_tmcTaskMonitor.rmvMonitor(this);
#endif

                //注销
                UTCommonTaskController._AUTEnableMonoTask.UTEnableMonoTaskMgr.instance.popTask(serialize);
                //放回缓存
                UTCommonEnableLateTickActionTaskCache.instance.pushBackCacheItem(this);
                _m_dAction = null;
                return;
            }
            
            if (null != _m_dAction)
                _m_dAction();

            //放入下一个LateTask
            UTMonoTaskMgr.instance.addNextFrameLaterTask(this);
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
                return $"UTCommonEnableLateTickActionMonoTask:{GCommon.ToReadableString(_m_dAction)}";
            }
            else
            {
                return $"UTCommonEnableLateTickActionMonoTask:null";
            }
        }

        public class UTCommonEnableLateTickActionTaskCache : _ACacheControllerBase<UTCommonEnableLateTickActionMonoTask, UTCommonEnableLateTickActionMonoTask>
        {
            private static UTCommonEnableLateTickActionTaskCache _g_instance = new UTCommonEnableLateTickActionTaskCache();
            public static UTCommonEnableLateTickActionTaskCache instance
            {
                get
                {
                    if(null == _g_instance)
                        _g_instance = new UTCommonEnableLateTickActionTaskCache();
                    return _g_instance;
                }
            }

            public UTCommonEnableLateTickActionTaskCache() : base(4, 64)
            {
                init(new UTCommonEnableLateTickActionMonoTask());
            }

            protected override UTCommonEnableLateTickActionMonoTask _createItem(UTCommonEnableLateTickActionMonoTask _template)
            {
                return new UTCommonEnableLateTickActionMonoTask();
            }

            //警告信息文字
            protected override string _warningTxt { get { return "UTCommonEnableLateTickActionTaskCache"; } }

            protected override void _discardItem(UTCommonEnableLateTickActionMonoTask _item)
            {
                _item._reset();
                return;
            }

            protected override void _onInit(UTCommonEnableLateTickActionMonoTask _template)
            {
            }

            protected override void _resetItem(UTCommonEnableLateTickActionMonoTask _item)
            {
                _item._reset();
            }
        }
    }
}
