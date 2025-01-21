using System.Collections;
using UnityEditor;
using UnityEngine;
using UTGame;
using YooAsset;

internal class UTYooAssetInitializeState : _ASimpleState<EYooInitType>, _IUTProgressnterface
{
    private float _m_curProcess;

    public UTYooAssetInitializeState(SimpleStateMachine<EYooInitType> _machine) : base(_machine)
    {
    }

    public float curProcess
    {
        get { return _m_curProcess; }
    }

    public override EYooInitType state
    {
        get { return EYooInitType.Initialize; }
    }


    protected override void _onEnter(params object[] _params)
    {
        // 初始化资源系统
        _m_curProcess = 0.1f;
        UTCoroutineDealerMgr.instance.addCoroutine(new UTCoroutineWrapper(InitPackage()));
    }

    public override bool canEnterState(EYooInitType _newState)
    {
        return true;
    }

    protected override void _onExit()
    {
        
    }

    private IEnumerator InitPackage()
    {
        string packageName = GameMain.instance.packageName;
        ResourcePackage package = YooAssets.TryGetPackage(packageName);
        if (package == null)
            package = YooAssets.CreatePackage(packageName);

        YooAssets.SetDefaultPackage(package);
        EPlayMode playMode = GameMain.instance.playMode;
        // 编辑器下的模拟模式
        InitializationOperation initializationOperation = null;
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
            var packageRoot = buildResult.PackageRootDirectory;
            var createParameters = new EditorSimulateModeParameters();
            createParameters.EditorFileSystemParameters =
                FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 单机运行模式
        if (playMode == EPlayMode.OfflinePlayMode)
        {
            var createParameters = new OfflinePlayModeParameters();
            createParameters.BuildinFileSystemParameters =
                FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // 联机运行模式
        if (playMode == EPlayMode.HostPlayMode)
        {
            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var createParameters = new HostPlayModeParameters();
            createParameters.BuildinFileSystemParameters =
                FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            createParameters.CacheFileSystemParameters =
                FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            initializationOperation = package.InitializeAsync(createParameters);
        }

        // WebGL运行模式
        if (playMode == EPlayMode.WebPlayMode)
        {
            var createParameters = new WebPlayModeParameters();
#if UNITY_WEBGL && WEIXINMINIGAME && !UNITY_EDITOR
			string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            createParameters.WebServerFileSystemParameters =
 WechatFileSystemCreater.CreateWechatFileSystemParameters(remoteServices);
#else
            createParameters.WebServerFileSystemParameters =
                FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
#endif
            initializationOperation = package.InitializeAsync(createParameters);
        }

        _m_curProcess = 0.5f;
        yield return initializationOperation;

        // TODO 如果初始化失败弹出提示界面
        if (null ==  initializationOperation || initializationOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning($"{initializationOperation.Error}");
        }
        else
        {
            _m_curProcess = 1.0f;
            _m_machine.changeState(EYooInitType.RequestPackageVersion);
        }
    }

    /// <summary>
    /// 获取资源服务器地址
    /// </summary>
    private string GetHostServerURL()
    {
        //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
        string hostServerIP = "http://127.0.0.1";
        string appVersion = "v1.0";

#if UNITY_EDITOR
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}