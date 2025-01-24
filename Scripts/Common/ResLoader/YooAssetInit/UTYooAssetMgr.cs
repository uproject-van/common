using System;
using UnityEngine.SceneManagement;
using YooAsset;

namespace UTGame
{
    /// <summary>
    /// 初始话进度枚举
    /// </summary>
    enum EYooInitType
    {
        Initialize,
        RequestPackageVersion,
        UpdatePackageManifest,
        CreateDownloader,
        InitDone
    }

    /// <summary>
    /// 资源管理对象
    /// </summary>
    public class UTYooAssetMgr : UTProgressNode
    {
        private static UTYooAssetMgr _g_instance = new UTYooAssetMgr();

        private bool _m_bIsInit;
        private SimpleStateMachine<EYooInitType> _m_stateMachine;
        private Action _m_initDoneAction;

        public static UTYooAssetMgr instance
        {
            get
            {
                if (null == _g_instance)
                    _g_instance = new UTYooAssetMgr();

                return _g_instance;
            }
        }

        public void init(Action _doneAction = null)
        {
            if (_m_bIsInit)
                return;

            _m_bIsInit = true;
            _m_initDoneAction = _doneAction;

            if (null == _m_stateMachine)
            {
                _m_stateMachine = new SimpleStateMachine<EYooInitType>();
                _m_stateMachine.onStateChg += _stateMachieStateChg;
            }

            UTYooAssetInitializeState initState = new UTYooAssetInitializeState(_m_stateMachine);
            UTYooAssetRequestPackageVersionState reqVState = new UTYooAssetRequestPackageVersionState(_m_stateMachine);
            UTYooAssetUpdatePackageManifestState updateManiState =
                new UTYooAssetUpdatePackageManifestState(_m_stateMachine);
            UTYooAssetCreateDownloaderNode createDownState = new UTYooAssetCreateDownloaderNode(_m_stateMachine);
            UTYooAssetInitializeDoneState initDoneState = new UTYooAssetInitializeDoneState(_m_stateMachine);
            _m_stateMachine.clear();
            _m_stateMachine.addState(initState);
            _m_stateMachine.addState(reqVState);
            _m_stateMachine.addState(updateManiState);
            _m_stateMachine.addState(createDownState);
            _m_stateMachine.addState(initDoneState);

            //进度监控
            addChidNode(initState, 1);
            addChidNode(reqVState, 1);
            addChidNode(updateManiState, 1);
            addChidNode(createDownState, 1);
            addChidNode(initDoneState, 1);

            //开始加载
            _m_stateMachine.changeState(EYooInitType.Initialize);
        }

        /// <summary>
        /// 状态机变化回调
        /// </summary>
        /// <param name="_lastState"></param>
        /// <param name="_curState"></param>
        private void _stateMachieStateChg(EYooInitType _lastState, EYooInitType _curState)
        {
            if (_curState != EYooInitType.InitDone)
                return;

            //初始话全部完成的回调，如果出现异常 应该立即弹窗进行重新处理
            if (null != _m_initDoneAction)
                _m_initDoneAction();
        }

        /// <summary>
        /// 加载本地资源对象的处理函数
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_delegate"></param>
        public void loadRefdataObjAsset(string _assetName,
            _assetDownloadedDelegate _delegate)
        {
            if (!_m_bIsInit)
                return;

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

        /// <summary>
        /// 加载场景处理函数
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_delegate"></param>
        public void loadSceneAsset(string _assetName,
            Action _doneAction)
        {
            if (!_m_bIsInit)
                return;

            SceneHandle handle = YooAssets.LoadSceneAsync(_assetName, LoadSceneMode.Additive,LocalPhysicsMode.None,true);
            if (null != handle)
            {
                handle.Completed += (_handle) =>
                {
                    if (null != _doneAction)
                        _doneAction();
                };
            }
        }
    }
}