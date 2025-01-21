using UnityEngine;
using UTGame;
using YooAsset;

internal class UTYooAssetCreateDownloaderNode : _ASimpleState<EYooInitType>, _IUTProgressnterface
{
    private float _m_curProcess;

    private string _m_packageVerson;
    
    public UTYooAssetCreateDownloaderNode(SimpleStateMachine<EYooInitType> _machine) : base(_machine)
    {
    }

    public float curProcess
    {
        get { return _m_curProcess; }
    }

    public override EYooInitType state
    {
        get { return EYooInitType.CreateDownloader; }
    }

    protected override void _onEnter(params object[] _args)
    {
        ResourcePackage package = YooAssets.GetPackage(GameMain.instance.packageName);
        if(null == package)
            return;
        
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        ResourceDownloaderOperation downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("Not found any download files !");
            _m_curProcess = 1f;
            _m_machine.changeState(EYooInitType.InitDone);
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            PatchEventDefine.FoundUpdateFiles.SendEventMessage(totalDownloadCount, totalDownloadBytes);
        }
    }
    protected override void _onExit()
    {
    }

    public override bool canEnterState(EYooInitType _newState)
    {
        return true;
    }
}