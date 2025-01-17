using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UTGame
{
    /// <summary>
    /// 二级TabItem
    /// </summary>
    public class SecondTabItemMono : _ATabItemMono
    {
        [Header("子页签加载父节点")]
        public Transform subInitParTrans;

        //TabItem缓存类
        private TabItemCache<_ATabItemMono> _m_tabItemCache;

        private Action<_ITabItemData> _m_firstSelectAction;

        //当前使用中的tabItem
        private List<_ATabItemMono> _m_useTabItemList;

        //当前选中的页签
        private _ATabItemMono _m_selectTabItem;

        //是否使用总时间
        private bool _m_isUseSameTime;

        //单个tab展开收起时间
        private float _m_perTabItemTimeS;

        //所有tab展开收起时间
        private float _m_totalTabItemTimeS;

        protected override void _OnInitEx()
        {
            base._OnInitEx();
            _m_useTabItemList = new List<_ATabItemMono>();
        }


        protected override void _OnDestroyEx()
        {
            base._OnDestroyEx();
            _clear();
            _m_useTabItemList = null;
        }

        protected override void _OnDisableEx()
        {
        }

        protected override void resetEx()
        {
            _m_tabItemCache = null;
            _m_firstSelectAction = null;
        }

        private void _clear()
        {
            if (null != _m_tabItemCache)
            {
                foreach (_ATabItemMono item in _m_useTabItemList)
                {
                    _m_tabItemCache.pushBackCacheItem(item);
                }
            }

            _m_useTabItemList.Clear();
            _m_tabItemCache = null;

            if (null != _m_selectTabItem)
                _m_selectTabItem.setSelected(GCommon.GetStatus(false));
            _m_selectTabItem = null;

            _m_firstSelectAction = null;
        }
        
        /// <summary>
        /// 额外刷新 用于刷新子页签
        /// </summary>
        protected override void _refreshEx()
        {
            if (null == _m_data || null == _m_data.subData || null == _m_tabItemCache)
                return;

            //回收当前使用的tabItem
            foreach (_ATabItemMono item in _m_useTabItemList)
            {
                _m_tabItemCache.pushBackCacheItem(item);
            }

            _m_useTabItemList.Clear();
            if (_m_selectStatus == ESelectStatus.UN_SELECTED)
            {
                _m_selectTabItem = null;
                return;
            }

            List<_ITabItemData> subDataList = _m_data.subData;
            _ITabItemData temp = null;
            for (int i = 0; i < subDataList.Count; i++)
            {
                temp = subDataList[i];
                if (null == temp)
                    continue;

                TabItemMono itemMono = _addTabItem(temp,i);
                if (null != itemMono && i == 0)
                {
                    _tabItemDidClick(itemMono);
                }
            }
        }

        public void setTimeData(bool _isUseSameTime, float _perTabItemTimeS, float _totalTabItemTimeS)
        {
            _m_isUseSameTime = _isUseSameTime;
            _m_perTabItemTimeS = _perTabItemTimeS;
            _m_totalTabItemTimeS = _totalTabItemTimeS;
        }

        public void setCache(TabItemCache<_ATabItemMono> _tabItemCache)
        {
            _m_tabItemCache = _tabItemCache;
        }

        public void setFirstSelectDelegate(Action<_ITabItemData> _firstSelectAction)
        {
            _m_firstSelectAction = _firstSelectAction;
        }

        /// <summary>
        /// 添加单个item
        /// </summary>
        /// <param name="_itemData"></param>
        private TabItemMono _addTabItem(_ITabItemData _itemData,int _idx)
        {
            if (null == _m_tabItemCache || !_m_tabItemCache.isInit() || null == subInitParTrans)
                return null;

            TabItemMono itemMono = (TabItemMono)_m_tabItemCache.popItem();
            if (null == itemMono)
                return null;

            itemMono.transform.SetParent(subInitParTrans);
            itemMono.transform.SetSiblingIndex(_idx + 1);
            UGUICommon.setGameObjEnable(itemMono.gameObject);
            itemMono.setData(_itemData);
            itemMono.setSelected(ESelectStatus.UN_SELECTED);
            itemMono.setClickDelegate(_tabItemDidClick);
            _m_useTabItemList.Add(itemMono);
            return itemMono;
        }

        //点击的子TabItem
        private void _tabItemDidClick(_ATabItemMono _tabItemMono)
        {
            if (null != _m_selectTabItem && _tabItemMono == _m_selectTabItem)
                return;

            if (null != _m_selectTabItem)
                _m_selectTabItem.setSelected(ESelectStatus.UN_SELECTED);

            _tabItemMono.setSelected(ESelectStatus.IS_SELECTED);
            _m_selectTabItem = _tabItemMono;

            if (null != _m_firstSelectAction)
                _m_firstSelectAction(_tabItemMono.data);
        }

        #region 动画相关

        /// <summary>
        /// 执行动画 展开或收起
        /// </summary>
        /// <param name="_selectStatus"></param>
        protected override void _doSelectStatusChgAnimation(Action _doneAction)
        {
            if (null != _doneAction)
                _doneAction();
            return;
            // //收起动画
            // if (_m_selectStatus == ESelectStatus.UN_SELECTED)
            // {
            //     _doPackUpAni(() =>
            //     {
            //         //收起要先动画再隐藏
            //         if (null != _doneAction)
            //             _doneAction();
            //     });
            // }
            // //展开动画
            // else
            // {
            //     //展开要先显示出来再动画 添加了mask
            //     if (null != _doneAction)
            //         _doneAction();
            //     _doExpandAni();
            // }
        }

        /// <summary>
        /// 展开动画
        /// </summary>
        // private void _doExpandAni(TweenCallback _doneAction = null)
        // {
        //     if (null == _m_data || null == _m_data.subData)
        //     {
        //         if (null != _doneAction)
        //             _doneAction();
        //         return;
        //     }
        //
        //     float duration = 0;
        //     if (_m_isUseSameTime)
        //         duration = _m_totalTabItemTimeS;
        //     else
        //         duration = _m_data.subData.Count * _m_perTabItemTimeS;
        //
        //     //获取间距
        //     float space = 0;
        //     VerticalLayoutGroup layout = GetComponent<VerticalLayoutGroup>();
        //     if (null != layout)
        //         space = layout.spacing;
        //
        //     //展开的高度
        //     float packUpHeight = normalHeight + _m_data.subData.Count * (space + _m_tabItemCache.itemHeight);
        //     RectTransform rectTrans = GetComponent<RectTransform>();
        //     rectTrans.DOSizeDelta(new Vector2(rectTrans.GetWidth(), packUpHeight), duration).OnComplete(_doneAction);
        // }

        /// <summary>
        /// 收起动画
        /// </summary>
        // private void _doPackUpAni(TweenCallback _doneAction = null)
        // {
        //     if (null == _m_data || null == _m_data.subData)
        //     {
        //         if (null != _doneAction)
        //             _doneAction();
        //         return;
        //     }
        //
        //     float duration = 0;
        //     if (_m_isUseSameTime)
        //         duration = _m_totalTabItemTimeS;
        //     else
        //         duration = _m_data.subData.Count * _m_perTabItemTimeS;
        //     RectTransform rectTrans = GetComponent<RectTransform>();
        //     rectTrans.DOSizeDelta(cc.p(rectTrans.GetWidth(), normalHeight), duration).OnComplete(_doneAction);
        // }

        #endregion
    }
}