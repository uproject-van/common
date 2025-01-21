using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UTGame;
using YooAsset;

internal class UTYooAssetInitializeDoneState : _ASimpleState<EYooInitType>, _IUTProgressnterface
{
    private float _m_curProcess;

    public UTYooAssetInitializeDoneState(SimpleStateMachine<EYooInitType> _machine) : base(_machine)
    {
    }

    public float curProcess
    {
        get { return _m_curProcess; }
    }

    public override EYooInitType state
    {
        get { return EYooInitType.InitDone; }
    }


    protected override void _onEnter(params object[] _params)
    {
        _m_curProcess = 1.0f;
        
        //加载配表
        GRefdataCoreMgr.instance.InitAllRefCore(() => { });
        //TODO YooAsset加载完成
        
    }

    public override bool canEnterState(EYooInitType _newState)
    {
        return true;
    }

    protected override void _onExit()
    {
    }
}