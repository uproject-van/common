using System;
using System.Collections.Generic;

using UnityEngine;
using UTGame;

/**********************
 * 游戏中的基本信息对象存储对象
 **/
namespace UTGame
{
    public class _TUTBaseRefListCore<T> where T : _IUTBaseRefObj
    {
        /** 存储对应id索引的数据映射表 */
        private List<T> _m_refList;
        /// <summary>
        /// 是否已经初始化数据，没有初始化的时候就访问的话会报错
        /// </summary>
        private bool _m_bIsInit = false;

        /** 取值失败时返回的默认值 */
        private T _m_oDefaultObj;

        public _TUTBaseRefListCore()
        {
            _m_refList = new List<T>();

            _m_oDefaultObj = default(T);
        }

        public List<T> RefList
        {
            get
            {
                if (_m_bIsInit == false)
                {
                    UnityEngine.Debug.LogError($"在配表还没初始化就尝试访问：{this.GetType()}");
                }
                return _m_refList;
            }
        }

        /// <summary>
        /// 设置默认值对象
        /// </summary>
        /// <param name="_v"></param>
        public void setDefaultValue(T _v)
        {
            _m_oDefaultObj = _v;
        }

        /// <summary>
        /// 从数据集中初始化
        /// </summary>
        /// <param name="_refSet"></param>
        public void initData(_TUTSOBaseRefSet<T> _refSet)
        {
            if (null == _refSet)
                return;

            //遍历数据集插入映射表
            initData(_refSet.refList);
        }

        /// <summary>
        /// 直接带入数据集进行初始化
        /// </summary>
        /// <param name="_refList"></param>
        public void initData(List<T> _refList)
        {
            if(null == _refList)
                return;

            _m_bIsInit = true;

            _m_refList = new List<T>(_refList.Count);
            //遍历数据集插入映射表
            for(int i = 0; i < _refList.Count; i++)
            {
                T refObj = _refList[i];
                if(null == refObj)
                    continue;

                //放入映射表
                _m_refList.Add(refObj);
            }
        }

        /*******************
         * 根据id获取对应的数据
         **/
        public T getRef(long _id, bool _checkFail = true)
        {
            if (_m_bIsInit == false)
            {
#if UNITY_EDITOR
                UnityEngine.Debug.LogError($"[EDITOR]在配表还没初始化就尝试访问：{this.GetType()}：{_id}");
#endif
            }
            T tmp = default(T);
            for (int i = 0; i < _m_refList.Count; i++)
            {
                tmp = _m_refList[i];
                if(tmp == null)
                    continue;

                if (tmp._refId == _id)
                    return _m_refList[i];
            }

            if(_checkFail)
            {
                _getRefFailed(_id);
            }
            return _m_oDefaultObj;
        }
        public T getRef(Func<T, bool> _findFunc)
        {
            if (_m_bIsInit == false)
            {
                UnityEngine.Debug.LogError($"在配表还没初始化就尝试访问：{this.GetType()}");
            }
            for(int i = 0; i < _m_refList.Count; i++)
            {
                if (_findFunc(_m_refList[i]))
                    return _m_refList[i];
            }

            return default(T);
        }

        /*******************
         * 逐个处理
         **/
        public void dealAllRef(Action<T> _action)
        {
            if (null == _action)
                return;

            if (_m_bIsInit == false)
            {
                UnityEngine.Debug.LogError($"在配表还没初始化就尝试访问：{this.GetType()}");
            }
            
            for(int i = 0; i < _m_refList.Count; i++)
            {
                _action(_m_refList[i]);
            }
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        protected void _clear()
        {
            if(null != _m_refList)
                _m_refList.Clear();
        }

        /// <summary>
        /// 当数据获取失败的时候调用的触发函数，可通过重载在子类处理
        /// </summary>
        /// <param name="_id"></param>
        protected virtual void _getRefFailed(long _id)
        {
        }
    }
}
