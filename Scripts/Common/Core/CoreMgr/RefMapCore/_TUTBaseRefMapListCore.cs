using System;
using System.Collections.Generic;
using UnityEngine;

/**********************
 * 游戏中的基本信息对象存储对象
 **/
namespace UTGame
{
    public class _TUTBaseRefMapListCore<T> where T : _IUTBaseRefObj
    {
        /** 存储对应id索引的数据映射表 */
        private Dictionary<long, object> _m_dicRef;
        private List<T> _m_lRefList;
        /// <summary>
        /// 是否已经初始化数据，没有初始化的时候就访问的话会报错
        /// </summary>
        private bool _m_bIsInit = false;

        /** 取值失败时返回的默认值 */
        private T _m_oDefaultObj;

        public _TUTBaseRefMapListCore()
        {
            _m_dicRef = new Dictionary<long, object>();
            _m_lRefList = new List<T>();

            _m_oDefaultObj = default(T);
        }

        public List<T> refList
        {
            get
            {
                if (_m_bIsInit == false)
                {
                    Debug.LogError($"在配表还没初始化就尝试访问：{this.GetType()}");
                }
                return _m_lRefList;
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

        /*****************
         * 从数据集中初始化
         **/
        public void initData(_TUTSOBaseRefSet<T> _refSet)
        {
            if (null == _refSet)
                return;

            _m_bIsInit = true;
            _m_dicRef = new Dictionary<long, object>(_refSet.refList.Count);
            _m_lRefList = new List<T>(_refSet.refList.Count);
            //遍历数据集插入映射表
            for (int i = 0; i < _refSet.refList.Count; i++)
            {
                T refObj = _refSet.refList[i];
                if (null == refObj)
                    continue;

                if (_m_dicRef.ContainsKey(refObj._refId))
                {
#if UNITY_EDITOR
                    Debug.LogError($"[EDITOR]重复配表id！！！: {refObj._refId} 配表是这个: {typeof(T)}");
#endif
                    continue;
                }

                //放入映射表
                _m_dicRef.Add(refObj._refId, refObj);
                _m_lRefList.Add(refObj);
            }
        }
        /*******
         * 增加一个新的配表对象
         */
        public void addRef(T _refObj)
        {
            if (null == _refObj)
                return;

            _m_dicRef.Add(_refObj._refId, _refObj);
            _m_lRefList.Add(_refObj);
        }

        /*******************
         * 根据id获取对应的数据
         **/
        public T getRef(long _id, bool _checkFail = true)
        {
            if (_m_bIsInit == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"[EDITOR]在配表还没初始化就尝试访问：{this.GetType()}:{_id}");
#endif
            }
            if (!_m_dicRef.ContainsKey(_id))
            {
                if(_checkFail)
                {
                    _getRefFailed(_id);
                }
                return _m_oDefaultObj;
            }

            T obj = (T)_m_dicRef[_id];

            if (_checkFail && obj == null)
                _getRefFailed(_id);
            return obj;
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
                Debug.LogError($"在配表还没初始化就尝试访问：{this.GetType()}");
            }

            for (int i = 0; i < _m_lRefList.Count; i++)
            {
                _action(_m_lRefList[i]);
            }
        }

        /// <summary>
        /// 增加根据函数返回对应检索值的函数
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T findIf(Predicate<T> predicate)
        {
            if (null == predicate)
                return default(T);

            if (_m_bIsInit == false)
            {
                Debug.LogError($"在配表还没初始化就尝试访问：{this.GetType()}");
            }

            for (int i = 0; i < _m_lRefList.Count; i++)
            {
                T item = _m_lRefList[i];
                if (null == item)
                    continue;

                if (predicate(item))
                {
                    return item;
                }
            }

            return default(T);
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
