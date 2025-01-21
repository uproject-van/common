using System;
using YooAsset;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
#endif

namespace UTGame
{
    /****************
     * 资源下载成功的回调函数
     * _isSuc : 是否下载成功
     * _assetInfo : 资源信息
     **/
    public delegate void _assetDownloadedDelegate(bool _isSuc, Object _assetObj);

    /** 加载本地资源管理对象 */
    public class UTResLoaderMgr
    {
        private static UTResLoaderMgr _g_instance = new UTResLoaderMgr();

        public static UTResLoaderMgr instance
        {
            get
            {
                if (null == _g_instance)
                    _g_instance = new UTResLoaderMgr();

                return _g_instance;
            }
        }

        private bool _m_bIsInit;

        public void init()
        {
            if(_m_bIsInit)
                return;
            
            _initYooAsset();
            _m_bIsInit = true;
        }
        /// <summary>
        /// 初始话资源管理
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
            
        }
        
        /// <summary>
        /// 加载本地资源对象的处理函数
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_delegate"></param>
        public void loadRefdataObjAsset(string _assetName,
            _assetDownloadedDelegate _delegate)
        {
            AssetHandle handle = YooAssets.LoadAssetAsync(_assetName);
            if (null != handle)
            {
                handle.Completed += (_handle) =>
                {
                    if (null == _handle)
                    {
                        if (null != _delegate)
                            _delegate(false, null);
                    }
                    else
                    {
                        if (null != _delegate)
                            _delegate(true, _handle.AssetObject);
                    }
                };
            }
        }
    }
}