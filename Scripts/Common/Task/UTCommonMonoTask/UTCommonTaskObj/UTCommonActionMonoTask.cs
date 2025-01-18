using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UTGame
{
    public class UTCommonActionMonoTask : _IUTBaseMonoTask, _IUTCommonTaskMonitorInterface
    {
        #region 对外接口
        //创建一个对应的任务
        protected static internal UTCommonActionMonoTask _createTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container 
#endif
            )
        {
            if(null == _delegate)
                return null;

            UTCommonActionMonoTask task = UTCommonActionTaskCache.instance.popItem();

#if UNITY_EDITOR
            //添加到Editor监控
            if (null != _container)
                _container.addMonitor(task);
#endif

            task._setAction(_delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            return task;
        }
        /** 对外开放的任务创建操作函数 */
        public static void addMonoTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        public static void addMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }

        public static void addLaterMonoTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addLaterMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        public static void addLaterMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addLaterMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }

        public static void addFixedMonoTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addFixedMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        public static void addFixedMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addFixedMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }

        public static void addScaleTimeDelayMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }
        public static void addScaleTimeDelayLaterMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayLaterMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }
        public static void addScaleTimeDelayFixedMonoTask(Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayFixedMonoTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }

        public static void addNextFrameTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFrameTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        public static void addNextFrameLaterTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFrameLaterTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        public static void addNextFixedUpdateTask(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFixedUpdateTask(_createTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        #endregion

        /** 对外开放的任务创建操作函数终结 */

        private Action _m_dAction;
#if UNITY_EDITOR 
        private UTCommonTaskMonitorContainer _m_tmcTaskMonitor;
#endif


        protected UTCommonActionMonoTask()
        {
            _m_dAction = null;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

    protected void _setAction(Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
#if UNITY_EDITOR
            if(_delegate == null)
            {
                Debug.LogError("UTCommonActionMonoTask 传入Action为空");
            }
#endif
            _m_dAction = _delegate;

#if UNITY_EDITOR
            _m_tmcTaskMonitor = _container;
#endif
        }

        public void deal()
        {
            if (null != _m_dAction)
                _m_dAction();
            _m_dAction = null;

#if UNITY_EDITOR
            if (null != _m_tmcTaskMonitor)
                _m_tmcTaskMonitor.rmvMonitor(this);
#endif

            //放回缓存
            UTCommonActionTaskCache.instance.pushBackCacheItem(this);
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

        protected void _reset()
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
                return $"UTCommonActionMonoTask:{GCommon.ToReadableString(_m_dAction)}";
            }
            else
            {
                return $"UTCommonActionMonoTask:null";
            }
        }

        public class UTCommonActionTaskCache : _ACacheControllerBase<UTCommonActionMonoTask, UTCommonActionMonoTask>
        {
            private static UTCommonActionTaskCache _g_instance = new UTCommonActionTaskCache();
            [NotNull]
            public static UTCommonActionTaskCache instance
            {
                get
                {
                    if(null == _g_instance)
                        _g_instance = new UTCommonActionTaskCache();
                    return _g_instance;
                }
            }

            public UTCommonActionTaskCache() : base(64, 256)
            {
                init(new UTCommonActionMonoTask());
            }

            protected override UTCommonActionMonoTask _createItem(UTCommonActionMonoTask _template)
            {
                return new UTCommonActionMonoTask();
            }

            //警告信息文字
            protected override string _warningTxt { get { return "ALCommonActionTaskCache"; } }

            protected override void _discardItem(UTCommonActionMonoTask _item)
            {
                _item._reset();
                return;
            }

            protected override void _onInit(UTCommonActionMonoTask _template)
            {
            }

            protected override void _resetItem(UTCommonActionMonoTask _item)
            {
                _item._reset();
            }
        }
    }
}
