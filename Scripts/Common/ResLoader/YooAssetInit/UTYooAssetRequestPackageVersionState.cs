using System.Collections;
using UnityEngine;
using UTGame;
using YooAsset;

internal class UTYooAssetRequestPackageVersionState :_ASimpleState<EYooInitType>, _IUTProgressnterface
{
    private float _m_curProcess;

    public UTYooAssetRequestPackageVersionState(SimpleStateMachine<EYooInitType> _machine) : base(_machine)
    {
    }

    public float curProcess
    {
        get { return _m_curProcess; }
    }

    public override EYooInitType state
    {
        get { return EYooInitType.RequestPackageVersion; }
    }

    protected override void _onExit()
    {
    }

    public override bool canEnterState(EYooInitType _newState)
    {
        return true;
    }

    protected override void _onEnter(params object[] _params)
    {
        _m_curProcess = 0.1f;
        UTCoroutineDealerMgr.instance.addCoroutine(new UTCoroutineWrapper(UpdatePackageVersion()));
    }

    private IEnumerator UpdatePackageVersion()
    {
        var package = YooAssets.GetPackage(GameMain.instance.packageName);
        if (null == package)
            yield break;

        var operation = package.RequestPackageVersionAsync();
        _m_curProcess = 0.5f;
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
        }
        else
        {
            _m_curProcess = 1.0f;
            //TODO 把包的版本号存入本地
            Debug.Log($"Request package version : {operation.PackageVersion}");
            _m_machine.changeState(EYooInitType.UpdatePackageManifest,operation.PackageVersion);
        }
    }
}