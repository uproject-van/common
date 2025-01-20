using System;
using System.Collections;
using UnityEngine;
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

        [Header("是否进行任务超时监控")]
        public bool isMonitorTaskTime;

        [Header("非编辑器环境下是否输出日志")]
        public bool showDebugOutput;

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

            _initYooAsset();
        }

        /// <summary>
        /// 初始话YooAsset
        /// </summary>
        private void _initYooAsset()
        {
            YooAssets.Initialize();
            
            // 创建资源包裹类
            string packageName = "DefaultPackage";
            ResourcePackage package = YooAssets.TryGetPackage(packageName);
            if (package == null)
                package = YooAssets.CreatePackage(packageName);

            EPlayMode playMode = EPlayMode.EditorSimulateMode;
            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = buildResult.PackageRootDirectory;
                var createParameters = new EditorSimulateModeParameters();
                createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            initializationOperation.Completed += ((_oprera) =>
            {
                // 如果初始化失败弹出提示界面
                if (_oprera.Status != EOperationStatus.Succeed)
                {
                    Debug.LogWarning($"{_oprera.Error}");
                    return;
                }
            
                YooAssets.SetDefaultPackage(package);
                RequestPackageVersionOperation reqOperation = package.RequestPackageVersionAsync();
                reqOperation.Completed += ((_op) =>
                {
                    if (_op.Status != EOperationStatus.Succeed)
                    {
                        Debug.LogWarning($"{_op.Error}");
                        return;
                    }

                    UpdatePackageManifestOperation manifestOp = package.UpdatePackageManifestAsync(reqOperation.PackageVersion);
                    manifestOp.Completed += (_op2) =>
                    {
                        if (_op2.Status != EOperationStatus.Succeed)
                        {
                            Debug.LogWarning($"{_op2.Error}");
                            return;
                        }
                        
                        AssetHandle handle1 =  YooAssets.LoadAssetAsync("general");
                        handle1.Completed += (_handle) =>
                        {
                            Debug.LogError($"{_handle}");
                        };
                    };

                });
                
            });


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