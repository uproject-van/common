using System;

namespace UTGame
{
    public class UTFrameDelayActionMonoTask : _IUTBaseMonoTask, _IUTCommonTaskMonitorInterface
    {
        #region 对外接口
        //创建一个对应的任务
        protected static UTFrameDelayActionMonoTask _createTask(int _frameCount, Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
            UTFrameDelayActionMonoTask task = UTFrameDelayActionTaskCache.instance.popItem();

#if UNITY_EDITOR
            //添加到Editor监控
            if (null != _container)
                _container.addMonitor(task);
#endif

            task._setAction(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
                );

            return task;
        }
        /** 对外开放的任务创建操作函数 */
        public static void addMonoTask(int _frameCount, Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addMonoTask(_createTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        public static void addMonoTask(int _frameCount, Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addMonoTask(_createTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }

        public static void addScaleTimeDelayMonoTask(int _frameCount, Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayMonoTask(_createTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }
        public static void addScaleTimeDelayLaterMonoTask(int _frameCount, Action _delegate, float _delayTime
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayLaterMonoTask(_createTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
                ), _delayTime);
        }

        public static void addNextFrameTask(int _frameCount, Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFrameTask(_createTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
                ));
        }
        #endregion

        /** 对外开放的任务创建操作函数终结 */

        private int _m_iFrameCount;
        private Action _m_dAction;
#if UNITY_EDITOR 
        private UTCommonTaskMonitorContainer _m_tmcTaskMonitor;
#endif

        protected UTFrameDelayActionMonoTask()
        {
            _m_iFrameCount = 0;
            _m_dAction = null;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        protected void _setAction(int _frameCount, Action _delegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            )
        {
#if UNITY_EDITOR
            if(_delegate == null)
            {
                UnityEngine.Debug.LogError("ALFrameDelayActionMonoTask 传入Action为空");
            }
#endif
            _m_iFrameCount = _frameCount;
            _m_dAction = _delegate;

#if UNITY_EDITOR
            _m_tmcTaskMonitor = _container;
#endif
        }

        public void deal()
        {
            _m_iFrameCount--;

            if(_m_iFrameCount > 0)
            {
                UTMonoTaskMgr.instance.addNextFrameTask(this);
                return;
            }

            if (null != _m_dAction)
                _m_dAction();
            _m_dAction = null;

#if UNITY_EDITOR
            if (null != _m_tmcTaskMonitor)
                _m_tmcTaskMonitor.rmvMonitor(this);
#endif

            //放回缓存
            UTFrameDelayActionTaskCache.instance.pushBackCacheItem(this);
        }

        protected void _reset()
        {
            _m_iFrameCount = 0;
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
            _m_iFrameCount = 0;
            _m_dAction = null;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        public override string ToString()
        {
            if(_m_dAction != null)
            {
                return $"UTFrameDelayActionMonoTask:{GCommon.ToReadableString(_m_dAction)}";
            }
            else
            {
                return $"UTFrameDelayActionMonoTask:null";
            }
        }

        public class UTFrameDelayActionTaskCache : _ACacheControllerBase<UTFrameDelayActionMonoTask, UTFrameDelayActionMonoTask>
        {
            private static UTFrameDelayActionTaskCache _g_instance = new UTFrameDelayActionTaskCache();
            public static UTFrameDelayActionTaskCache instance
            {
                get
                {
                    if(null == _g_instance)
                        _g_instance = new UTFrameDelayActionTaskCache();
                    return _g_instance;
                }
            }

            public UTFrameDelayActionTaskCache() : base(32, 128)
            {
                init(new UTFrameDelayActionMonoTask());
            }

            protected override UTFrameDelayActionMonoTask _createItem(UTFrameDelayActionMonoTask _template)
            {
                return new UTFrameDelayActionMonoTask();
            }

            //警告信息文字
            protected override string _warningTxt { get { return "ALFrameDelayActionTaskCache"; } }

            protected override void _discardItem(UTFrameDelayActionMonoTask _item)
            {
                _item._reset();
                return;
            }

            protected override void _onInit(UTFrameDelayActionMonoTask _template)
            {
            }

            protected override void _resetItem(UTFrameDelayActionMonoTask _item)
            {
                _item._reset();
            }
        }
    }
}
