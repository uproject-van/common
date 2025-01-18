using System;
using UnityEngine;

namespace UTGame
{
    public class GameMain : MonoBehaviour
    {
        private static GameMain _g_instance = null;

        public static GameMain instance
        {
            get { return _g_instance; }
        }

        [Header("是否进行任务超时监控")]
        public bool isMonitorTaskTime;

        [Header("输出日志等级")]
        public UTLogLevel logLevel = UTLogLevel.DEBUG;

        [Header("全局缺省图片")]
        public Sprite globeEmptySpr;

        public GameObject testGo;

        private void Start()
        {
            //先进行 task 初始化
            UTMonoTaskMgr.instance.addMonoTask(null);

            //设置处理对象
            if (null == _g_instance)
            {
                _g_instance = this;
            }
            else
            {
                UTLog.Error("Multiple GameMain Mono!!!");
                return;
            }

            //判断本对象是否有加载ALMonoTaskMono脚本，无则自动添加
            if (null == gameObject.GetComponent<UTMonoTaskMono>())
                gameObject.AddComponent<UTMonoTaskMono>();

            //设置对象不被删除
            GameObject.DontDestroyOnLoad(this);

            UGUICommon.combineBtnClick(testGo, _testGoDidClick);
        }


        private void _testGoDidClick(GameObject _go)
        {
            Debug.LogError("点击的帧数111 = " + Time.frameCount);

            UTCommonTaskController.CommonActionAddNextFrameTask(() =>
            {
                Debug.LogError("执行的帧数 = " + Time.frameCount);
            });

            Debug.LogError("点击的帧数222 = " + Time.frameCount);
        }

        /// <summary>
        /// 出现异常情况调用
        /// </summary>
        /// <param name="_e"></param>
        public void onUnKnowErrorOccurred(Exception _e)
        {
        }

        //点击间隔帧数 表示3帧触发一次点击
        public int getOneClickPerFrame()
        {
            return 3;
        }
    }
}
