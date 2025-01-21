using System.Collections;
using UnityEngine;
using UTGame;
using YooAsset;

internal class UTYooAssetUpdatePackageManifestState : _ASimpleState<EYooInitType>, _IUTProgressnterface
{
    private float _m_curProcess;

    private string _m_packageVerson;
    
    public UTYooAssetUpdatePackageManifestState(SimpleStateMachine<EYooInitType> _machine) : base(_machine)
    {
    }

    public float curProcess
    {
        get { return _m_curProcess; }
    }

    public override EYooInitType state
    {
        get { return EYooInitType.UpdatePackageManifest; }
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
        if(_params.Length == 0)
            return;
        
        _m_packageVerson = _params[0] as string;
        _m_curProcess = 0.1f;
        UTCoroutineDealerMgr.instance.addCoroutine(new UTCoroutineWrapper(UpdateManifest()));
    }

    private IEnumerator UpdateManifest()
    {
        ResourcePackage package = YooAssets.GetPackage(GameMain.instance.packageName);
        if (null == package)
            yield break;
        
        var operation = package.UpdatePackageManifestAsync(_m_packageVerson);
        _m_curProcess = 0.5f;
        yield return operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
        }
        else
        {
            _m_curProcess = 1f;
            _m_machine.changeState(EYooInitType.CreateDownloader);
        }
    }
}