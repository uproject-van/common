using System;
using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using YooAsset;

namespace UTGame
{
    public class GameMain : MonoBehaviour
    {
        private static GameMain _g_instance = null;

        public static GameMain instance
        {
            get { return _g_instance; }
        }

        [Header("主相机")]
        public Camera mainCamera;
        
        [Header("是否进行任务超时监控")]
        public bool isMonitorTaskTime;

        [Header("非编辑器环境下是否输出日志")]
        public bool showDebugOutput;

        [Header("输出日志等级")]
        public UTLogLevel logLevel = UTLogLevel.DEBUG;

        [Header("全局缺省图片")]
        public Sprite globeEmptySpr;

        [Header("当前运行模式")]
        public EPlayMode playMode;

        [Header("默认包名")]
        public string packageName = "DefaultPackage";

        public GameObject testGo;



        private void Start()
        {
            //设置处理对象
            if (null == _g_instance)
                _g_instance = this;
            else
            {
                UTLog.Error("Multiple GameMain Mono!!!");
                return;
            }

            YooAssets.Initialize();
            //设置对象不被删除
            DontDestroyOnLoad(this);
            //进行 task 初始化
            UTMonoTaskMgr.instance.addMonoTask(null);
            //判断本对象是否有加载ALMonoTaskMono脚本，无则自动添加
            if (null == gameObject.GetComponent<UTMonoTaskMono>())
                gameObject.AddComponent<UTMonoTaskMono>();
            //判断本对象是否有加载UTMonoCoroutineDealer脚本，无则自动添加
            if (null == gameObject.GetComponent<UTMonoCoroutineDealer>())
                gameObject.AddComponent<UTMonoCoroutineDealer>();

            //资源管理初始化
            UTYooAssetMgr.instance.init();
            UGUICommon.combineBtnClick(testGo, _testGoDidClick);

            //固定相机的参数 防止被改到
            GameMain.instance.mainCamera.transform.position = new Vector3(0, 0, -10);
        }

        private void _testGoDidClick(GameObject _go)
        {
            UTCommonTaskController.CommonActionAddNextFrameTask(() =>
            {
                //切换场景
                SceneHandle handle = YooAssets.LoadSceneAsync("Game", LoadSceneMode.Single);
                if (null != handle)
                {
                    handle.Completed += (_handle) =>
                    {
                        if (_handle.Status == EOperationStatus.Succeed)
                        {
                            Debug.Log("场景加载成功！");

                            // 查找场景中带有特定标签的相机并移除
                            GameObject[] cameras = GameObject.FindGameObjectsWithTag("cameraTag");
                            foreach (var camera in cameras)
                            {
                                Debug.Log($"移除相机: {camera.name}");
                                Destroy(camera);
                            }
                        }
                        else
                        {
                            Debug.LogError($"场景加载失败：{handle.LastError}");
                        }
                    };
                }
                //SceneManager.LoadScene("Game", LoadSceneMode.Additive);
            });
        }

        /// <summary>
        /// 出现异常情况调用
        /// </summary>
        /// <param name="_e"></param>
        public void onUnKnowErrorOccurred(Exception _e)
        {
        }
    }
}