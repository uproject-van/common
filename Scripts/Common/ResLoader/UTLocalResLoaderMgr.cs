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
    public delegate void _assetDownloadedDelegate(bool _isSuc, ALAssetBundleObj _assetObj);

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

        /** 本地资源加载的根目录 */
        private string _m_sLocalResRootPath;
        /** 是否从本地加载窗口资源对象 */
        private bool _m_bUseLocalUI;
        private bool _m_bUseLocalGameObject;
        private bool _m_bUseLocalScene;
        private bool _m_bLoadRefdataFromLocal;
#if AL_ILRUNTIME
        private bool _m_useLocalHotfix;
#endif
        private bool _m_bIfLocalDisableUseRemote;

        protected UTLocalResLoaderMgr()
        {
            _m_sLocalResRootPath = "Resources";
            _m_bUseLocalUI = false;
            _m_bUseLocalGameObject = false;
            _m_bUseLocalScene = false;
            _m_bLoadRefdataFromLocal = false;
#if AL_ILRUNTIME
            _m_useLocalHotfix = false;
#endif
            _m_bIfLocalDisableUseRemote = false;
        }

        public string localResRootPath { get { return _m_sLocalResRootPath; } }
        public bool isLoadUIFromLocal { get { return _m_bUseLocalUI; } }
        public bool isLoadGameObjectFromLocal { get { return _m_bUseLocalGameObject; } }
        public bool isLoadSceneFromLocal { get { return _m_bUseLocalScene; } }
        public bool isLoadRefdataFromLocal { get { return _m_bLoadRefdataFromLocal; } }
#if AL_ILRUNTIME
        public bool useLocalHotfix { get { return _m_useLocalHotfix; } }
#endif
        public bool ifLocalDisableUseRemote { get { return _m_bIfLocalDisableUseRemote; } }

        /** 从本地加载UI */
        public void setLoadFromLocal(string _localResRootPath, bool _useLocalUI, bool _useLocalGameObject, bool _useLocalScene, bool _useLocalRefdata
#if AL_ILRUNTIME
            , bool _useLocalHotfix
#endif
            , bool _ifLocalDisableUseRemote)
        {
            _m_sLocalResRootPath = _localResRootPath;
            _m_bUseLocalUI = _useLocalUI;
            _m_bUseLocalGameObject = _useLocalGameObject;
            _m_bUseLocalScene = _useLocalScene;
            _m_bLoadRefdataFromLocal = _useLocalRefdata;
#if AL_ILRUNTIME
            _m_useLocalHotfix = _useLocalHotfix;
#endif
            _m_bIfLocalDisableUseRemote = _ifLocalDisableUseRemote;
        }
        /** 设置不从本地加载 */
        public void resetLoadFromLocal()
        {
            _m_sLocalResRootPath = "Resources";
            _m_bUseLocalUI = false;
            _m_bUseLocalGameObject = false;
            _m_bUseLocalScene = false;
            _m_bLoadRefdataFromLocal = false;
            _m_bIfLocalDisableUseRemote = false;
#if AL_ILRUNTIME
            _m_useLocalHotfix = false;
#endif
        }

        /*****************
         * 加载对应资源
         **/
        public void loadUIAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<GameObject> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if (!_m_bUseLocalUI || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                GameObject finalGo = _loadPrefab(_assetPath, _objName);
                
                //GameObject go = Resources.Load<GameObject>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if (null != finalGo)
                {
                    if (null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if(_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynUIAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<GameObject> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(!_m_bUseLocalUI || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                GameObject finalGo = _loadPrefab(_assetPath, _objName);

                //GameObject go = Resources.Load<GameObject>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadGameObjectAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<GameObject> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                GameObject finalGo = _loadPrefab(_assetPath, _objName);

                //T go = Resources.Load<T>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源T: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynGameObjectAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<GameObject> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                GameObject finalGo = _loadPrefab(_assetPath, _objName);

                //T go = Resources.Load<T>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源T: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadTemplateObjectAsset<T>(string _assetPath, string _objName, string _exname, string _unitySiftStr, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<T> _localLoadDelegate, _AALResourceCore _resourceCore)
            where T : Object
        {
            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                T finalGo = _loadPrefab<T>(_assetPath, _objName, _exname, _unitySiftStr);

                //T go = Resources.Load<T>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源T: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + _exname + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynTemplateObjectAsset<T>(string _assetPath, string _objName, string _exname, string _unitySiftStr, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<T> _localLoadDelegate, _AALResourceCore _resourceCore)
            where T : Object
        {
            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                T finalGo = _loadPrefab<T>(_assetPath, _objName, _exname, _unitySiftStr);

                //T go = Resources.Load<T>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源T: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + _exname + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadTemplateObjectAsset<T>(string _assetPath, string _objName, string[] _exname, string _unitySiftStr, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<T> _localLoadDelegate, _AALResourceCore _resourceCore)
            where T : Object
        {
            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                T finalGo = null;
                finalGo = _loadPrefab<T>(_assetPath, _objName, _exname, _unitySiftStr);

                //T go = Resources.Load<T>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源T: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynTemplateObjectAsset<T>(string _assetPath, string _objName, string[] _exname, string _unitySiftStr, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<T> _localLoadDelegate, _AALResourceCore _resourceCore)
            where T : Object
        {
            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                //加载本地资源
                T finalGo = null;
                finalGo = _loadPrefab<T>(_assetPath, _objName, _exname, _unitySiftStr);

                //T go = Resources.Load<T>(_m_sLocalGUIRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != finalGo)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(finalGo);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地资源T: " + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }

        /*****************
         * 加载本地资源对象的处理函数
         **/
        public void loadRefdataObjAsset<T>(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<Object> _localLoadDelegate)
            where T : Object
        {
            if(null == _resourceCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "本地Refdata: " + "/" + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            if(!_m_bLoadRefdataFromLocal || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                T obj = _loadScriptObject<T>(_assetPath, _objName);
                //T obj = Resources.Load<T>(_m_sLocalRefdataRootPath + "/" + _objName);
                if(null != obj)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(obj);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地Refdata: " + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynRefdataObjAsset<T>(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<Object> _localLoadDelegate, _AALResourceCore _resourceCore)
            where T : Object
        {
            if(null == _resourceCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "本地Refdata: " + "/" + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            if(!_m_bLoadRefdataFromLocal || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                T obj = _loadScriptObject<T>(_assetPath, _objName);
                //T obj = Resources.Load<T>(_m_sLocalRefdataRootPath + "/" + _objName);
                if(null != obj)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(obj);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地Refdata: " + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }

        public void loadTextAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<Object> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(null == _resourceCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "本地TextAsset: " + "/" + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            if(!_m_bLoadRefdataFromLocal || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                TextAsset obj = _loadTextAsset(_assetPath, _objName);
                //T obj = Resources.Load<T>(_m_sLocalRefdataRootPath + "/" + _objName);
                if(null != obj)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(obj);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地TextAsset: " + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynTextAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<Object> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(null == _resourceCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "本地TextAsset: " + "/" + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            if(!_m_bLoadRefdataFromLocal || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                TextAsset obj = _loadTextAsset(_assetPath, _objName);
                //T obj = Resources.Load<T>(_m_sLocalRefdataRootPath + "/" + _objName);
                if(null != obj)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(obj);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地TextAsset: " + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }

        /*****************
         * 加载本地资源对象的处理函数
         **/
        public void loadGameResObjAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<Object> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(null == _resourceCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "本地GameRes: " + "/" + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                Object obj = _loadAsset(_assetPath, _objName);
                //UnityEngine.Object obj = Resources.Load(_m_sLocalGameResRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != obj)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(obj);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地GameRes: " + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }
        public void loadSynGameResObjAsset(string _assetPath, string _objName, _assetDownloadedDelegate _delegate, _assetDownloadedDelegate _lateDelegate, Action<Object> _localLoadDelegate, _AALResourceCore _resourceCore)
        {
            if(null == _resourceCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "本地GameRes: " + "/" + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            if(!_m_bUseLocalGameObject || !_resourceCore.canLoadLocalRes)
            {
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
            }
            else
            {
#if UNITY_EDITOR
                Object obj = _loadAsset(_assetPath, _objName);
                //UnityEngine.Object obj = Resources.Load(_m_sLocalGameResRootPath + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName);
                if(null != obj)
                {
                    if(null != _localLoadDelegate)
                        _localLoadDelegate(obj);
                }
                else
                {
                    Debug.LogError("加载" + _resourceCore.ToString() + "本地GameRes: " + "/" + _assetPath.Replace(".unity3d", "") + "/" + _objName + " 失败！");

                    //加载rescore资源
                    if (_m_bIfLocalDisableUseRemote)//只有允许加载远程才使用
                        _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
                    else
                    {
                        if(null != _localLoadDelegate)
                            _localLoadDelegate(null);
                    }
                }
#else
                //加载rescore资源
                _resourceCore.loadSynAsset(_assetPath, _delegate, _lateDelegate);
#endif
            }
        }

        /***********
         * 加载场景前的Asset加载
         **/
        public void loadSceneAsset(_AALResourceCore _resCore, string _assetPath, _assetDownloadedDelegate _delegate)
        {
            if(null == _resCore)
            {
#if UNITY_EDITOR
                Debug.LogError("加载" + _assetPath + "场景: " + _assetPath.Replace(".unity3d", "") + " 失败！");
#endif
                if(null != _delegate)
                {
                    _delegate(false, null);
                }
                return;
            }

            //判断本地scene路径是否有效
            if(!_m_bUseLocalScene || !_resCore.canLoadLocalRes)
            {
                //开始进行资源加载
                _resCore.loadAsset(_assetPath, _delegate, null);
            }
            else
            {
                if(null != _delegate)
                {
                    _delegate(true, null);
                }
            }
        }

#if UNITY_EDITOR
        /** 记录已经检索的数据缓存 */
        private Dictionary<string, List<string>> _m_dAllkeyValue = new Dictionary<string, List<string>>();
        protected void _addKV(string _key, List<string> _values) { _m_dAllkeyValue.Add(_key, _values); }
        protected List<string> _getValues(string _key) { List<string> value; if(_m_dAllkeyValue.TryGetValue(_key, out value)) { return value; } else { return null; } }

        /** 从根目录中加载本地资源 */
        protected Object _loadAsset(string _assetPath, string _objName)
        {
            return _loadPrefab<Object>(_assetPath, _objName, ".asset", "");
        }
        /** 从根目录中加载本地资源 */
        protected GameObject _loadPrefab(string _assetPath, string _objName)
        {
            return _loadPrefab<GameObject>(_assetPath, _objName, ".prefab", "t:Prefab");
        }
        /** 从根目录中加载本地数据 */
        protected T _loadScriptObject<T>(string _assetPath, string _objName)
            where T : Object
        {
            return _loadPrefab<T>(_assetPath, _objName, ".asset", "t:ScriptableObject");
        }
        
        protected TextAsset _loadTextAsset(string _assetPath, string _objName)
        {
            return _loadPrefab<TextAsset>(_assetPath, _objName, ".txt", "t:TextAsset");
        }

        /** 从根目录中加载本地资源 */
        protected T _loadPrefab<T>(string _assetPath, string _objName, string _exname, string _unitySiftStr) where T : Object
        {
            string judgeS = _objName + _exname;
            judgeS = judgeS.ToLowerInvariant();

            //声明临时变量
            T finalGo = null;
            
            //使用GetAssetPathsFromAssetBundleAndAssetName方式加载比FindAssets匹配加载快很多，FindAssets底层逻辑是全遍历去匹配
            //直接先使用这个方式获取ab路径，不需要缓存，耗时很短可以忽略
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(_assetPath, _objName);
            foreach (string pathItem in paths)
            {
                if(string.IsNullOrEmpty(pathItem))
                    continue;
                
                finalGo = _getPathAssetsMatch<T>(pathItem, _objName, judgeS);
                //如果有对象直接返回   
                if (null != finalGo)
                    return finalGo;
            }

            return _loadPrefabByFindAssets<T>(_assetPath, _objName, _exname, _unitySiftStr);
        }
        
        protected T _loadPrefab<T>(string _assetPath, string _objName, string[] _exname, string _unitySiftStr) where T : Object
        {
            //声明临时变量
            T finalGo = null;
            
            // 先遍历所有拓展名, 查找是否存在资源
            for(int i = 0; i < _exname.Length; i++)
            {
                string judgeS = _objName + _exname[i];
                judgeS = judgeS.ToLowerInvariant();
                
                //使用GetAssetPathsFromAssetBundleAndAssetName方式加载比FindAssets匹配加载快很多，FindAssets底层逻辑是全遍历去匹配
                //直接先使用这个方式获取ab路径，不需要缓存，耗时很短可以忽略
                string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(_assetPath, _objName);
                foreach (string pathItem in paths)
                {
                    if(string.IsNullOrEmpty(pathItem))
                        continue;
                
                    finalGo = _getPathAssetsMatch<T>(pathItem, _objName, judgeS);
                    //如果有对象直接返回   
                    if (null != finalGo)
                        return finalGo;
                }
            }

            // 若GetAssetPathsFromAssetBundleAndAssetName没有找到, 再遍历所有拓展名通过FindAssets查找
            for (int i = 0; i < _exname.Length; i++)
            {
                finalGo = _loadPrefabByFindAssets<T>(_assetPath, _objName, _exname[i], _unitySiftStr);
                if(null != finalGo)
                    return finalGo;
            }
            
            return finalGo;
        }

        private T _loadPrefabByFindAssets<T>(string _assetPath, string _objName, string _exname, string _unitySiftStr) where T : Object
        {
            string judgeS = _objName + _exname;
            judgeS = judgeS.ToLowerInvariant();

            //声明临时变量
            T finalGo = null;
            
            List<string> totalValue = _getValues(_assetPath + _exname);
            if(null != totalValue)
            {
                //已经有全部值则直接处理并返回
                for (int i = 0; i < totalValue.Count; i++)
                {
                    string path = totalValue[i];
                    if(null == path)
                        continue;

                    finalGo = _getPathAssetsMatch<T>(path, _objName, judgeS);
                    if(null == finalGo)
                        continue;

                    break;
                }

                return finalGo;
            }
            else
            {
                //创建新队列，等待后续赋值
                totalValue = new List<string>();
            }

            //检索目录名
            string[] guids = AssetDatabase.FindAssets("b:" + _assetPath, new string[] { "Assets/" + _m_sLocalResRootPath });

            if(guids.Length <= 0)
            {
                //将值队列加入数据
                _addKV(_assetPath + _exname, totalValue);
                return null;
            }

            //加入值队列

            string[] resFolders = new string[guids.Length];

            for(int f = 0; f < guids.Length; f++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[f]);
                resFolders[f] = path;
                if(null == path || totalValue.Contains(path))
                    continue;
                //加入值队列
                totalValue.Add(path);

                if (null == finalGo)
                    finalGo = _getPathAssetsMatch<T>(path, _objName, judgeS);
            }

            if(null == finalGo)
            {
                //检索文件夹内信息
                string[] ids = AssetDatabase.FindAssets(_unitySiftStr, resFolders);

                //遍历对象加入列表中
                for (int i = 0; i < ids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(ids[i]);
                    if(null == path || totalValue.Contains(path))
                        continue;
                    //加入值队列
                    totalValue.Add(path);
                    if (null == finalGo)
                        finalGo = _getPathAssetsMatch<T>(path, _objName, judgeS);
                }
            }
          
            //将值队列加入数据
            _addKV(_assetPath + _exname, totalValue);

            return finalGo;
        }

        // 检查资源是否匹配，增加了检查生成的物体是否名字一致的判断，来实现加载TexturePacker的sprite的功能
        private T _getPathAssetsMatch<T>(string _path, string _objName, string judgeS) where T : Object
        {
            T finalGo = default(T);
            if(_path.ToLowerInvariant().EndsWith(judgeS))
            {
                finalGo = AssetDatabase.LoadAssetAtPath(_path, typeof(T)) as T;
                if(finalGo != null )
                    return finalGo;
            }
  
            Object[] allAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(_path);
            if(allAssets == null)
                return null;

            for (int i = 0; i < allAssets.Length; i++)
            {
                Object obj = allAssets[i];
                if (null == obj || !(obj is T))
                    continue;

                finalGo = obj as T;
                if(finalGo != null && finalGo.name == _objName)
                    return finalGo;
            }

            return null;
        }
#endif
    }
}


