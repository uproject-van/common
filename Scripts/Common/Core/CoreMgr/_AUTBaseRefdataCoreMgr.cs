using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLDDZ;

namespace UTGame
{
    public interface _IUTInitRefObj
    {
        void init(Action _call, Action<Type, _IUTInitRefObj> _onFail);
        //获取最终调用函数
        Action finalDelegate { get; }
    }

    public abstract class _AUTBaseRefdataCoreMgr
    {
        private List<_IUTInitRefObj> _m_lLoadList = new List<_IUTInitRefObj>();

        public _AUTBaseRefdataCoreMgr()
        {
            _m_lLoadList = new List<_IUTInitRefObj>();
        }

        //存储是否初始化
        private bool _m_bIsInit = false;
        public bool isInited { get { return _m_bIsInit; } }
        
        public void InitAllRefCore(Action _completeCall)
        {
            if (_m_bIsInit)
            {
                if (null != _completeCall)
                    _completeCall();
                return;
            }

            //设置初始化完成
            _m_bIsInit = true;

            //初始化加载队列
            _initLoadList(_m_lLoadList);

            SimpleStepCounter stepCounter = new SimpleStepCounter();
            int max = _m_lLoadList.Count;
            if (max == 0)
            {
                _onInitAllRefCore();
                _completeCall();
                return;
            }
            stepCounter.chgTotalStepCount(max);
            stepCounter.regAllDoneDelegate(_onInitAllRefCore);
            stepCounter.regAllDoneDelegate(_completeCall);

            for (int i = 0; i < max; i++)
            {
//                Debug.Log($"{Time.frameCount} init {i}");
                _m_lLoadList[i].init(stepCounter.addDoneStepCount, _onRefInitFail);
            }
        }

        /// <summary>
        /// 逐步加载ref避免卡顿
        /// </summary>
        /// <param name="_completeCall"></param>
        public void InitAllRefCoreAsync(Action _completeCall)
        {
            if(_m_bIsInit)
            {
                if(null != _completeCall)
                    _completeCall();
                return;
            }

            //设置初始化完成
            _m_bIsInit = true;

            //初始化加载队列
            _initLoadList(_m_lLoadList);

            UTCoroutineDealerMgr.instance.addCoroutine(new UTCoroutineWrapper(InitAllRefCoreAsyncCoroutine(_completeCall)));
        }

        IEnumerator InitAllRefCoreAsyncCoroutine(Action _completeCall)
        {
            SimpleStepCounter stepCounter = new SimpleStepCounter();
            int max = _m_lLoadList.Count;
            if(max == 0)
            {
                _onInitAllRefCore();
                _completeCall();
                yield break;
            }
            stepCounter.chgTotalStepCount(max);
            stepCounter.regAllDoneDelegate(_onInitAllRefCore);
            stepCounter.regAllDoneDelegate(_completeCall);

            //记录时间，用于避免过长加载
            long timeStampMs = GCommon.getNowTimeMill();
#if UNITY_EDITOR
            int thisFrameInitCount = 0;
#endif
            for(int i = 0; i < max; i++)
            {
                //                Debug.Log($"{Time.frameCount} init {i}");
                long startInitTimeMs = GCommon.getNowTimeMill();
                //加载操作
                _m_lLoadList[i].init(stepCounter.addDoneStepCount, _onRefInitFail);

                //计算耗时
                long endInitTimeMs = GCommon.getNowTimeMill();
#if UNITY_EDITOR
                thisFrameInitCount++;
#endif

                //Editor或者showDebugOutput都输出，这样在手机上才能看到耗时
#if !UNITY_EDITOR
                if (GameMain.instance.showDebugOutput)
                {
#endif
                    long loadUseTime = endInitTimeMs - startInitTimeMs;
                    if(loadUseTime > 20)
                        Debug.LogWarning($"【Init Use Time {loadUseTime} - {_m_lLoadList[i].ToString()}】 ");
                    if(loadUseTime > 10)
                        Debug.Log($"【Init Use Time {loadUseTime} - {_m_lLoadList[i].ToString()}】 ");
#if !UNITY_EDITOR
                }
#endif

                //按照游戏是30帧来算，超过30毫秒则停止加载等待显示处理，让界面不至于完全卡主不动
                if (endInitTimeMs - timeStampMs > 30)
                {
                    timeStampMs = GCommon.getNowTimeMill();
#if UNITY_EDITOR
                    Debug.LogWarning($"本轮加载Refdata 加载数据集数量【{thisFrameInitCount}】 ");
                    thisFrameInitCount = 0;
#endif
                    yield return null; //有设置限制个数，并且这一帧init的个数大于等于限制个数时，就等下一帧再init
                }
            }
        }
        
        /** 加载失败处理 */
        protected abstract void _onRefInitFail(Type _class, _IUTInitRefObj _obj);

        //初始化加载队列
        protected abstract void _initLoadList(List<_IUTInitRefObj> _list);

        //初始化所有数据处理
        protected abstract void _onInitAllRefCore();
    }
}

