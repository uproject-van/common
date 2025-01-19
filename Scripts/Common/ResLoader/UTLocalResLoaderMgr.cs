using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UTGame
{
    /****************
     * 资源下载成功的回调函数
     * _isSuc : 是否下载成功
     * _assetInfo : 资源信息
     **/
    public delegate void _assetDownloadedDelegate(bool _isSuc, AssetBundle _assetObj);

    /** 加载本地资源管理对象 */
    public class UTLocalResLoaderMgr
    {
        private static UTLocalResLoaderMgr _g_instance = new UTLocalResLoaderMgr();

        public static UTLocalResLoaderMgr instance
        {
            get
            {
                if (null == _g_instance)
                    _g_instance = new UTLocalResLoaderMgr();

                return _g_instance;
            }
        }

        /*****************
         * 加载本地资源对象的处理函数
         **/
        public void loadRefdataObjAsset<T>(string _assetPath,
            string _objName,
            _assetDownloadedDelegate _delegate,
            _assetDownloadedDelegate _lateDelegate,
            Action<Object> _localLoadDelegate)
            where T : Object
        {



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