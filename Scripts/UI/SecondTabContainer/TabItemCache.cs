using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using WLDDZ;

namespace NGame
{
    /// <summary>
    /// 单个TabItem缓存池
    /// </summary>
    public class TabItemCache<T> : _ACacheControllerBase<T, T> where T : _ATabItemMono
    {
        // 是否缓存初始化完成
        private bool _m_bInit;

        public TabItemCache()
            : base(3, 30)
        {
        }

        public float itemHeight
        {
            get { return null == _m_tTemplateObj ? 0 : _m_tTemplateObj.normalHeight; }
        }

        public bool isInit()
        {
            return _m_bInit;
        }

        protected override string _warningTxt
        {
            get { return "TabItemCache"; }
        }

        protected override void _onInit(T _template)
        {
            _m_bInit = true;
        }

        protected override T _createItem(T _template)
        {
            return UGUICommon.cloneGameObj(_template);
        }

        protected override void _discardItem(T itemMono)
        {
            if (null == itemMono)
                return;

            GameObject.Destroy(itemMono);
        }

        protected override void _resetItem(T itemMono)
        {
            if (null == itemMono)
                return;

            itemMono.reset();
            UGUICommon.setGameObjDisable(itemMono.gameObject);
            Transform cacheParTrans = UIItemCacheMono.instance.getCacheParTrans();
            if (null != cacheParTrans)
                itemMono.gameObject.SetParent(cacheParTrans);
        }
    }
}