using System;

namespace UTGame
{
    /// <summary>
    /// 步骤处理类，带入三个回调，分别是执行函数，完成回调，失败回调。
    /// 在执行完成执行函数后，将执行完成回调，在执行函数过程出错则执行失败回调
    /// </summary>
    public class UTCommonStepActionMonoTask : _IUTBaseMonoTask, _IUTCommonTaskMonitorInterface
    {
        #region 对外接口
        /// <summary>
        /// 创建一个对应的任务，最后一个参数代表是否失败也执行完成
        /// 切记不要重复添加到任务管理器中
        /// </summary>
        /// <param name="_action"></param>
        /// <param name="_doneDelegate"></param>
        /// <param name="_failDelegate"></param>
        /// <param name="_isFailDoDone"></param>
        /// <returns></returns>
        protected internal static UTCommonStepActionMonoTask _createTask(Action _action, Action _doneDelegate
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container
#endif
            , Action _failDelegate = null, bool _isFailDoDone = false)
        {
            UTCommonStepActionMonoTask task = UTCommonStepActionTaskCache.instance.popItem();

#if UNITY_EDITOR
            //添加到Editor监控
            if (null != _container)
                _container.addMonitor(task);
#endif

            task._setAction(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
                );

            return task;
        }
        /** 对外开放的任务创建操作函数 */
        public static void addMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone));
        }
        public static void addMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false, float _delayTime = 0f
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone), _delayTime);
        }

        public static void addLaterMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addLaterMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone));
        }
        public static void addLaterMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false, float _delayTime = 0f
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addLaterMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone), _delayTime);
        }

        public static void addFixedMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addFixedMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone));
        }
        public static void addFixedMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false, float _delayTime = 0f
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addFixedMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone), _delayTime);
        }

        public static void addScaleTimeDelayMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false, float _delayTime = 0f
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone), _delayTime);
        }
        public static void addScaleTimeDelayLaterMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false, float _delayTime = 0f
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayLaterMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone), _delayTime);
        }
        public static void addScaleTimeDelayFixedMonoTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false, float _delayTime = 0f
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addScaleTimeDelayFixedMonoTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone), _delayTime);
        }

        public static void addNextFrameTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFrameTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone));
        }
        public static void addNextFrameLaterTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFrameLaterTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone));
        }
        public static void addNextFixedUpdateTask(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
            UTMonoTaskMgr.instance.addNextFixedUpdateTask(_createTask(_action, _doneDelegate
#if UNITY_EDITOR
                , _container
#endif
                , _failDelegate, _isFailDoDone));
        }
        #endregion

        /** 对外开放的任务创建操作函数终结 */

        //执行回调
        private Action _m_dAction;
        //完成回调
        private Action _m_dDoneAction;
        //失败回调
        private Action _m_dFailAction;
        //是否失败的时候也执行完成函数
        private bool _m_bIsFailDoDone;
#if UNITY_EDITOR 
        private UTCommonTaskMonitorContainer _m_tmcTaskMonitor;
#endif

        protected UTCommonStepActionMonoTask()
        {
            _m_dAction = null;
            _m_dDoneAction = null;
            _m_dFailAction = null;
            _m_bIsFailDoDone = false;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        protected void _setAction(Action _action, Action _doneDelegate, Action _failDelegate = null, bool _isFailDoDone = false
#if UNITY_EDITOR 
            , UTCommonTaskMonitorContainer _container = null
#endif
            )
        {
#if UNITY_EDITOR
            if(_action == null)
            {
                UnityEngine.Debug.LogError("UTCommonStepActionMonoTask 传入Action为空");
            }
#endif
            _m_dAction = _action;
            _m_dDoneAction = _doneDelegate;
            _m_dFailAction = _failDelegate;
            _m_bIsFailDoDone = _isFailDoDone;

#if UNITY_EDITOR
            _m_tmcTaskMonitor = _container;
#endif
        }

        public void deal()
        {
            try
            {
                if(null != _m_dAction)
                    _m_dAction();

                //执行完成后执行完成函数，根据结果判断是否正常
                if(null != _m_dDoneAction)
                    _m_dDoneAction();
            }
            catch(Exception _ex)
            {
                UnityEngine.Debug.LogError(_ex.ToString());

                //执行失败函数
                if(null != _m_dFailAction)
                    _m_dFailAction();

                //判断失败是否需要执行成功函数
                if(_m_bIsFailDoDone)
                {
                    //执行成功函数
                    if(null != _m_dDoneAction)
                        _m_dDoneAction();
                }
                else
                {
                    UTLog.Sys("One Process Fail!");
                }
            }
            finally
            {
                _m_dAction = null;
                _m_dDoneAction = null;
                _m_dFailAction = null;
                _m_bIsFailDoDone = false;
            }

#if UNITY_EDITOR
            if (null != _m_tmcTaskMonitor)
                _m_tmcTaskMonitor.rmvMonitor(this);
#endif

            //放回缓存
            UTCommonStepActionTaskCache.instance.pushBackCacheItem(this);
        }

        protected void _reset()
        {
            _m_dAction = null;
            _m_dDoneAction = null;
            _m_dFailAction = null;
            _m_bIsFailDoDone = false;
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
            _m_dDoneAction = null;
            _m_dFailAction = null;
            _m_bIsFailDoDone = false;
#if UNITY_EDITOR
            _m_tmcTaskMonitor = null;
#endif
        }

        public override string ToString()
        {
            if(_m_dAction != null)
            {
                return $"UTCommonStepActionMonoTask:{GCommon.ToReadableString(_m_dAction)}";
            }
            else
            {
                return $"UTCommonStepActionMonoTask:null";
            }
        }
        
        public class UTCommonStepActionTaskCache : _ACacheControllerBase<UTCommonStepActionMonoTask, UTCommonStepActionMonoTask>
        {
            private static UTCommonStepActionTaskCache _g_instance = new UTCommonStepActionTaskCache();
            public static UTCommonStepActionTaskCache instance
            {
                get
                {
                    if(null == _g_instance)
                        _g_instance = new UTCommonStepActionTaskCache();
                    return _g_instance;
                }
            }

            public UTCommonStepActionTaskCache() : base(32, 128)
            {
                init(new UTCommonStepActionMonoTask());
            }

            protected override UTCommonStepActionMonoTask _createItem(UTCommonStepActionMonoTask _template)
            {
                return new UTCommonStepActionMonoTask();
            }

            //警告信息文字
            protected override string _warningTxt { get { return "UTCommonStepActionTaskCache"; } }

            protected override void _discardItem(UTCommonStepActionMonoTask _item)
            {
                _item._reset();
                return;
            }

            protected override void _onInit(UTCommonStepActionMonoTask _template)
            {
            }

            protected override void _resetItem(UTCommonStepActionMonoTask _item)
            {
                _item._reset();
            }
        }
    }
}
