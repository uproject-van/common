using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UTGame;

public class GameMain : MonoBehaviour
{
    private static GameMain _g_instance = null;
    public static GameMain instance { get { return _g_instance; } }

    [Header("全局缺省图片")]
    public Sprite globeEmptySpr;
    
    public GameObject testGo;

    public UTMonoTaskMono taskMono;
    private void Start()
    {
        UGUICommon.combineBtnClick(testGo,_testGoDidClick);
    }

    private void _testGoDidClick(GameObject _go)
    {
        Debug.LogError("点击的帧数111 = " + Time.frameCount);

        taskMono.AddTask(() =>
        {
            Debug.LogError("执行的帧数 = " + Time.frameCount);
        });
        
        Debug.LogError("点击的帧数222 = " + Time.frameCount);
    }
    
    /// <summary>
    /// 出现异常情况调用
    /// </summary>
    /// <param name="_e"></param>
    public void onUnKnowErrorOccurred(Exception _e)
    {
        
    }

    //点击间隔帧数 表示3帧触发一次点击
    public int getOneClickPerFrame()
    {
        return 3;
    }
}
