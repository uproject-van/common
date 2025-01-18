using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
#endif

namespace UTGame
{
    public class UTMonoTaskMono : MonoBehaviour
    {
#if UNITY_EDITOR && UNITY_STANDALONE
        [SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long performanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long frequency);
#endif

        /** 本类型脚本是否初始化过的操作，避免重复执行 */
        private static bool _g_bIsMonoStarted = false;
        /** 本脚本实际是否有效，如非重复初始化本脚本则有效 */
        private bool _m_bIsEnable = false;
        /** 用于存储本帧是否已经处理过OnGUI事件 */
        //private bool _m_bDealOnGUI = false;

#if UNITY_EDITOR
        private long _m_lTimeTickPerSec;
        private long _m_lTickStart;
        private long _m_lTickEnd;
        
        private int _m_thisFrameTotalTask = 0;
#endif

        // Use this for initialization
        void Start()
        {
            try
            {
                if (_g_bIsMonoStarted)
                    return;

                //设置已初始化
                _g_bIsMonoStarted = true;
                //设置本脚本有效
                _m_bIsEnable = true;

#if UNITY_EDITOR && UNITY_STANDALONE
                QueryPerformanceFrequency(out _m_lTimeTickPerSec);
#endif
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UTMonoTaskMono Start has Exception:\n" + e);
                if(GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
#if UNITY_EDITOR
                _m_thisFrameTotalTask = 0;
#endif
                if (!_m_bIsEnable)
                    return;

                //_m_bDealOnGUI = false;

                //取出需要执行的任务
                _IUTBaseMonoTask monoTask = UTMonoTaskMgr.instance.popMonoTask();
                while (null != monoTask)
                {
                    //进行处理
                    _dealTask(monoTask);

                    //取出下一个需要执行的Task
                    monoTask = UTMonoTaskMgr.instance.popMonoTask();
                }

                //取出需要执行的可缩放时间任务
                monoTask = UTMonoTaskMgr.instance.popScaletimeMonoTask();
                while (null != monoTask)
                {
                    //进行处理
                    _dealTask(monoTask);

                    //取出下一个需要执行的Task
                    monoTask = UTMonoTaskMgr.instance.popScaletimeMonoTask();
                }

                //处理任务管理器中的定时任务
                UTMonoTaskMgr.instance.dealTimerMonoTask();
                //处理根据时间缩放的延迟任务
                UTMonoTaskMgr.instance.dealScaleTimeTimerMonoTask();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UTMonoTaskMono Update has Exception:\n" + e);
                if(GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }

        //将下帧任务放入队列
        void LateUpdate()
        {
            try
            {
                if (!_m_bIsEnable)
                    return;

                //取出需要执行的任务
                _IUTBaseMonoTask monoTask = UTMonoTaskMgr.instance.popLaterMonoTask();
                while (null != monoTask)
                {
                    _dealTask(monoTask);

                    //取出下一个需要执行的Task
                    monoTask = UTMonoTaskMgr.instance.popLaterMonoTask();
                }

                //取出需要执行的可缩放时间任务
                monoTask = UTMonoTaskMgr.instance.popScaletimeLaterMonoTask();
                while (null != monoTask)
                {
                    //进行处理
                    _dealTask(monoTask);

                    //取出下一个需要执行的Task
                    monoTask = UTMonoTaskMgr.instance.popScaletimeLaterMonoTask();
                }

                //处理任务管理器中的定时任务
                UTMonoTaskMgr.instance.dealTimerLaterMonoTask();
                UTMonoTaskMgr.instance.dealScaleTimeTimerLaterMonoTask();

                //将本帧的延迟一帧处理任务转换到下一帧执行
                UTMonoTaskMgr.instance.swapNextFrameTask();
                
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UTMonoTaskMono LateUpdate has Exception:\n" + e);
                if(GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }

        private void FixedUpdate()
        {
            try
            {
                if (!_m_bIsEnable)
                    return;

                //取出需要执行的任务
                _IUTBaseMonoTask monoTask = UTMonoTaskMgr.instance.popFixedMonoTask();
                while (null != monoTask)
                {
                    //进行处理
                    _dealTask(monoTask);

                    //取出下一个需要执行的Task
                    monoTask = UTMonoTaskMgr.instance.popFixedMonoTask();
                }

                //取出需要执行的可缩放时间任务
                monoTask = UTMonoTaskMgr.instance.popScaletimeFixedMonoTask();
                while (null != monoTask)
                {
                    //进行处理
                    _dealTask(monoTask);

                    //取出下一个需要执行的Task
                    monoTask = UTMonoTaskMgr.instance.popScaletimeFixedMonoTask();
                }

                //处理任务管理器中的定时任务
                UTMonoTaskMgr.instance.dealTimerFixedMonoTask();
                //处理根据时间缩放的延迟任务
                UTMonoTaskMgr.instance.dealScaleTimeTimerFixedMonoTask();
                //将本帧的延迟一帧处理任务转换到下一帧执行
                UTMonoTaskMgr.instance.swapNextFixedUpdateTask();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UTMonoTaskMono FixedUpdate has Exception:\n" + e);
                if(GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }
        
        /****************
         * 处理执行函数的操作
         **/
        protected void _dealTask(_IUTBaseMonoTask _monoTask)
        {
            if (null == _monoTask)
                return;

#if UNITY_EDITOR && UNITY_STANDALONE
            _m_thisFrameTotalTask++;
            QueryPerformanceCounter(out _m_lTickStart);
            string taskDebugString = _monoTask.ToString();//需要先记录调试string，否则deal之后可能被清空，就不知道是谁了
#endif
            //进行处理
            _monoTask.deal();

#if UNITY_EDITOR && UNITY_STANDALONE
            QueryPerformanceCounter(out _m_lTickEnd);

            //判断时间长度
            double absoluteTime = (double)(_m_lTickEnd - _m_lTickStart) / (double)_m_lTimeTickPerSec;
            if(absoluteTime > 0.01f && null != GameMain.instance && GameMain.instance.isMonitorTaskTime)
            {
                Debug.LogWarning("Mono Task Deal Time too Long! task: " + taskDebugString + " time: " + absoluteTime.ToString());
            }
#endif
        }
    }
}