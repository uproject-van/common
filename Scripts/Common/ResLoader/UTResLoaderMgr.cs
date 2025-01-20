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

        /*****************
         * 加载本地资源对象的处理函数
         **/
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

        public void loadSynRefdataObjAsset<T>(string _assetPath,
            string _objName,
            _assetDownloadedDelegate _delegate,
            _assetDownloadedDelegate _lateDelegate,
            Action<Object> _localLoadDelegate
        )
            where T : Object
        {
        }
    }
}