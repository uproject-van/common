using System;
using System.Collections.Generic;

namespace UTGame
{
    /// <summary>
    /// 根据序列号控制对应有效变量的控制对象
    /// </summary>
    public struct UTCommonEnableTaskController
    {
        private long _m_lTaskSerialize;

        public UTCommonEnableTaskController(long _serialize)
        {
            _m_lTaskSerialize = _serialize;
        }

        /// <summary>
        /// 设置任务无效
        /// </summary>
        public void setDisable()
        {
            UTCommonTaskController._AUTEnableMonoTask task =
                UTCommonTaskController._AUTEnableMonoTask.UTEnableMonoTaskMgr.instance.popTask(_m_lTaskSerialize);
            if (null == task)
                return;

            task.setDisable();
        }
    }

    /// <summary>
    /// 通用任务的控制对象，当每次这个对象处理后，任务将会被设置为空
    /// </summary>
    public class UTCommonTaskController
    {
        /** UTCommonActionMonoTask */
        public static void CommonActionAddMonoTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addMonoTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddLaterMonoTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addLaterMonoTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddLaterMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addLaterMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddFixedMonoTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addFixedMonoTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddFixedMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addFixedMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddScaleTimeDelayMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addScaleTimeDelayMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddScaleTimeDelayFixedMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addScaleTimeDelayFixedMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddNextFrameTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addNextFrameTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddNextFrameLaterTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addNextFrameLaterTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonActionAddNextFixedUpdateTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonActionMonoTask.addNextFixedUpdateTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }
        
        /** 对外开放的任务创建操作函数终结 */
        /** UTCommonEnableLateTickActionMonoTask */
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableLateTickActionAddLaterMonoTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableLateTickActionMonoTask.addLaterMonoTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableLateTickActionAddLaterMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableLateTickActionMonoTask.addLaterMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableLateTickActionAddNextFrameLaterTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableLateTickActionMonoTask.addNextFrameLaterTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }
        /** 对外开放的任务创建操作函数终结 */

        #region ALCommonEnableTickActionMonoTask

        /** ALCommonEnableTickActionMonoTask */
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddMonoTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableTickActionMonoTask.addMonoTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableTickActionMonoTask.addMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddScaleTimeDelayMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableTickActionMonoTask.addScaleTimeDelayMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddNextFrameTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableTickActionMonoTask.addNextFrameTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /** 对外开放的任务创建操作函数终结 */

        #endregion

        #region ALCommonEnableDurationActionMonoTask

        /** ALCommonEnableDurationActionMonoTask */
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableDurationActionAddMonoTask(Action _delegate,
            float _duration
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableDurationActionMonoTask.addMonoTask(_delegate, _duration
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableDurationActionAddMonoTask(Action _delegate,
            float _duration,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableDurationActionMonoTask.addMonoTask(_delegate, _duration, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableDurationActionAddScaleTimeDelayMonoTask(Action _delegate,
            float _duration,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableDurationActionMonoTask.addScaleTimeDelayMonoTask(_delegate, _duration, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableDurationActionAddNextFrameTask(Action _delegate,
            float _duration
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableDurationActionMonoTask.addNextFrameTask(_delegate, _duration
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /** 对外开放的任务创建操作函数终结 */

        #endregion

        /** ALCommonEnableTickActionMonoTask */
        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddFixedMonoTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableFixedTickActionMonoTask.addFixedMonoTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddFixedMonoTask(Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableFixedTickActionMonoTask.addFixedMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <param name="_delayTime"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddScaleTimeDelayFixedMonoTask(
            Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableFixedTickActionMonoTask.addScaleTimeDelayFixedMonoTask(_delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /// <summary>
        /// 创建一个对应的任务，切记不可重复添加到任务管理器中
        /// </summary>
        /// <param name="_delegate"></param>
        /// <returns></returns>
        public static UTCommonEnableTaskController CommonEnableTickActionAddNextFixedUpdateTask(Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            return UTCommonEnableFixedTickActionMonoTask.addNextFixedUpdateTask(_delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /** 对外开放的任务创建操作函数终结 */
        /** UTCommonStepActionMonoTask */
        public static void CommonStepActionAddMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddLaterMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addLaterMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddLaterMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addLaterMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddFixedMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addFixedMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddFixedMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addFixedMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddScaleTimeDelayMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addScaleTimeDelayMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone,
                _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddScaleTimeDelayFixedMonoTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addScaleTimeDelayFixedMonoTask(_action, _doneDelegate, _failDelegate,
                _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddNextFrameTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addNextFrameTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddNextFrameLaterTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addNextFrameLaterTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepActionAddNextFixedUpdateTask(Action _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepActionMonoTask.addNextFixedUpdateTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }
        
        /** 对外开放的任务创建操作函数终结 */
        /** UTCommonStepProcessMonoTask */
        public static void CommonStepProcessAddMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddLaterMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addLaterMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddLaterMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addLaterMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddFixedMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addFixedMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddFixedMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addFixedMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }
        
        public static void CommonStepProcessAddScaleTimeDelayMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addScaleTimeDelayMonoTask(_action, _doneDelegate, _failDelegate, _isFailDoDone,
                _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddScaleTimeDelayFixedMonoTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false,
            float _delayTime = 0f
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addScaleTimeDelayFixedMonoTask(_action, _doneDelegate, _failDelegate,
                _isFailDoDone, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddNextFrameTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addNextFrameTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddNextFrameLaterTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addNextFrameLaterTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void CommonStepProcessAddNextFixedUpdateTask(Func<bool> _action,
            Action _doneDelegate,
            Action _failDelegate = null,
            bool _isFailDoDone = false
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTCommonStepFuncMonoTask.addNextFixedUpdateTask(_action, _doneDelegate, _failDelegate, _isFailDoDone
#if UNITY_EDITOR
                , _container
#endif
            );
        }
        
        /** 对外开放的任务创建操作函数终结 */
        /** 对外开放的任务创建操作函数 */
        public static void FrameDelayActionAddMonoTask(int _frameCount,
            Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTFrameDelayActionMonoTask.addMonoTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void FrameDelayActionAddMonoTask(int _frameCount,
            Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTFrameDelayActionMonoTask.addMonoTask(_frameCount, _delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void FrameDelayActionAddScaleTimeDelayMonoTask(int _frameCount,
            Action _delegate,
            float _delayTime
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTFrameDelayActionMonoTask.addScaleTimeDelayMonoTask(_frameCount, _delegate, _delayTime
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        public static void FrameDelayActionAddNextFrameTask(int _frameCount,
            Action _delegate
#if UNITY_EDITOR
            ,
            UTCommonTaskMonitorContainer _container = null
#endif
        )
        {
            UTFrameDelayActionMonoTask.addNextFrameTask(_frameCount, _delegate
#if UNITY_EDITOR
                , _container
#endif
            );
        }

        /** 对外开放的任务创建操作函数终结 */
        /// <summary>
        /// 可用于enable管理控制的monotask基类
        /// </summary>
        public abstract class _AUTEnableMonoTask : _IUTBaseMonoTask
        {
            private static long _g_lSerialize = 1;

            private long _m_lSerialize;
            protected bool _m_bIsEnable;

            protected _AUTEnableMonoTask()
            {
                _m_lSerialize = 0;
                _m_bIsEnable = true;
            }

            public long serialize { get { return _m_lSerialize; } }

            //设置是否有效
            public void setDisable()
            {
                _m_bIsEnable = false;

                //处理无效操作
                _onDisable();
            }

            /// <summary>
            /// 刷新序列号
            /// </summary>
            protected void _refreshSerialize()
            {
                _m_lSerialize = _g_lSerialize++;
            }

            /// <summary>
            /// 重置信息
            /// </summary>
            protected void _reset()
            {
                _m_bIsEnable = true;

                //处理重置操作
                _onReset();
            }

            //处理函数
            public abstract void deal();

            //无效时的处理函数，一般用于释放资源
            protected abstract void _onDisable();
            protected abstract void _onReset();


            /// <summary>
            /// 所用可用enable管理控制的任务注册管理器
            /// </summary>
            public class UTEnableMonoTaskMgr
            {
                private static UTEnableMonoTaskMgr _g_instance = new UTEnableMonoTaskMgr();

                public static UTEnableMonoTaskMgr instance
                {
                    get
                    {
                        if (null == _g_instance)
                            _g_instance = new UTEnableMonoTaskMgr();

                        return _g_instance;
                    }
                }

                //任务对象
                private Dictionary<long, _AUTEnableMonoTask> _m_dicTask;

                protected UTEnableMonoTaskMgr()
                {
                    _m_dicTask = new Dictionary<long, _AUTEnableMonoTask>();
                }

                /// <summary>
                /// 注册并执行一个任务
                /// </summary>
                /// <param name="_task"></param>
                public long regTask(_AUTEnableMonoTask _task)
                {
                    if (null == _task)
                        return 0;

                    if (_task._m_lSerialize != 0)
                    {
                        UTLog.Error("Reg a Task when the task is reged!");
                        return 0;
                    }

                    //刷新序列号
                    _task._refreshSerialize();
                    long serialize = _task._m_lSerialize;

                    //使用序列号注册
                    _m_dicTask.Add(serialize, _task);

                    return serialize;
                }

                /// <summary>
                /// 将一个任务无效并从队列取出
                /// </summary>
                /// <param name="_serialize"></param>
                public _AUTEnableMonoTask popTask(long _serialize)
                {
                    _AUTEnableMonoTask task = null;
                    //尝试获取值
                    if (!_m_dicTask.TryGetValue(_serialize, out task))
                        return null;

                    //从集合删除
                    _m_dicTask.Remove(_serialize);
                    //设置任务序列号为0
                    task._m_lSerialize = 0;
                    //返回
                    return task;
                }
            }
        }
    }
}
