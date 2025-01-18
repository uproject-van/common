using System;
using UnityEngine;

namespace UTGame
{
    public class UTMonoCoroutineDealer : MonoBehaviour
    {

        /** 本类型脚本是否初始化过的操作，避免重复执行 */
        private static bool _g_bIsMonoStarted = false;
        /** 本脚本实际是否有效，如非重复初始化本脚本则有效 */
        private bool _m_bIsEnable = false;

        // Use this for initialization
        void Start()
        {
            if (_g_bIsMonoStarted)
                return;

            //设置已初始化
            _g_bIsMonoStarted = true;
            //设置本脚本有效
            _m_bIsEnable = true;
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                if (!_m_bIsEnable)
                    return;

                //将Coroutine管理队列中的Coroutine开启
                _IALCoroutineDealer coroutineDealer = UTCoroutineDealerMgr.instance.popCoroutineObj();
                while (null != coroutineDealer)
                {
                    StartCoroutine(coroutineDealer.dealCoroutine());

                    //取出下一个需要执行的Coroutine
                    coroutineDealer = UTCoroutineDealerMgr.instance.popCoroutineObj();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("UTMonoCoroutineDealer Update has Exception:\n" + e);
                if(GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }
    }
}