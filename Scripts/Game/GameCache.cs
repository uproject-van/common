using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    ///  UI缓存的挂载父节点
    /// </summary>
    public class GameCache : MonoBehaviour
    {
        private static GameCache _g_instance = null;

        public static GameCache instance
        {
            get { return _g_instance; }
        }

        [Header("UI缓存放置的父节点")]
        public Transform cacheParTrans;

        //二级视图的缓存存储字典
        [NotNull] private Dictionary<string, TabItemCache<_ATabItemMono>> _m_cacheDic;

        protected internal void Start()
        {
            if (null == _g_instance)
                _g_instance = this;
            else
            {
                Debug.LogError("Multiple UIItemCacheMono Mono!!!");
            }

            _m_cacheDic = new Dictionary<string, TabItemCache<_ATabItemMono>>();
            DontDestroyOnLoad(this);
        }

        public Transform getCacheParTrans()
        {
            return cacheParTrans;
        }

        public TabItemCache<_ATabItemMono> getTabItemCache(string _key)
        {
            TabItemCache<_ATabItemMono> cache;
            if (_m_cacheDic.ContainsKey(_key))
            {
                _m_cacheDic.TryGetValue(_key, out cache);
                if (null == cache)
                {
                    cache = new TabItemCache<_ATabItemMono>();
                    _m_cacheDic[_key] = cache;
                }
            }
            else
            {
                cache = new TabItemCache<_ATabItemMono>();
                _m_cacheDic.Add(_key, cache);
            }
            
            return cache;
        }
        
    }
}