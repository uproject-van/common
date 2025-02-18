using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using WLDDZ;
using UnityEngine;
using UnityEngine.UI;

namespace NGame
{
    /// <summary>
    /// 带二级展开的通用容器 注意: 名字相同的prefab 应该是同一个prefab,否则缓存加载会有问题
    /// 界面上添加conainter再挂上这个脚本，拖入相关需要的配置即可，记得二级item脚本需要拖拽加载父节点
    /// </summary>
    public class SecondTabItemContainer : _ABaseUIEntity
    {
        [Header("展开收起是否需要使用相同时间")]
        [Header("使用的话需要配置总时间 不论几个子页签收起和展开的时间相同")]
        [Header("不使用的话需要配置单个子页签时间，展开收起时间随页签数量变化")]
        public bool isUseSameTime;

        [Header("单个子页签收起时间 - 秒")]
        public float perTabItemTimeS = 0.1f;

        [Header("总时间 - 秒")]
        public float totalTabItemTimeS = 0.3f;

        [Header("外层页签默认选中下标 从0开始")]
        public int secondDefaultIdx;

        [Header("外层页签生成模版 -从prefab拖")]
        public SecondTabItemMono secondTempItem;

        [Header("内层页签生成模版 -从prefab拖")]
        public TabItemMono tempItem;

        [Header("外层页签加载父节点")]
        public RectTransform initParTrans;

        //SecondTabItem缓存类
        private TabItemCache<_ATabItemMono> _m_secondTabItemCache;
        private TabItemCache<_ATabItemMono> _m_tabItemCache;

        //数据
        private List<_ITabItemData> _m_dataList;

        //当前选中的页签
        private _ATabItemMono _m_selectTabItem;

        //外层选中页签回调
        private Action<_ITabItemData> _m_secondSelectDelegate;

        //内层选中页签回调
        private Action<_ITabItemData> _m_firstSelectDelegate;

        protected override void _OnInitEx()
        {
            _m_secondTabItemCache = UIItemCacheMono.instance.getTabItemCache(secondTempItem.name);
            if (!_m_secondTabItemCache.isInit())
                _m_secondTabItemCache.init(secondTempItem);

            _m_tabItemCache = UIItemCacheMono.instance.getTabItemCache(tempItem.name);
            if (!_m_tabItemCache.isInit())
                _m_tabItemCache.init(tempItem);
        }

        protected override void _OnDestroyEx()
        {
            //这里去回收
            if(null != _m_tabItemCache)
                _m_tabItemCache.pushBackAllCacheItems();
            _m_tabItemCache = null;;
            
            if(null != _m_secondTabItemCache)
                _m_secondTabItemCache.pushBackAllCacheItems();
            _m_secondTabItemCache = null;;
        }

        protected override void _OnEnableEx()
        {
            _refresh();
        }

        protected override void _OnDisableEx()
        {
            if (null != _m_selectTabItem)
                _m_selectTabItem.setSelected(ESelectStatus.UN_SELECTED);
            _m_selectTabItem = null;
        }

        public void setData<T>(List<T> _dataList, Action<_ITabItemData> _firstSelectDelegate,
            Action<_ITabItemData> _secondSelectDelegate = null) where T : _ITabItemData
        {
            if (null == _dataList || _dataList.Count == 0)
                return;

            _m_firstSelectDelegate = _firstSelectDelegate;
            _m_secondSelectDelegate = _secondSelectDelegate;

            if (null == _m_dataList)
                _m_dataList = new List<_ITabItemData>();
            _m_dataList.Clear();
            _m_dataList.AddRange(_dataList.Cast<_ITabItemData>());
            _refresh();
        }

        //刷新全部
        private void _refresh()
        {
            if (null == initParTrans || null == _m_dataList || null == _m_secondTabItemCache ||
                !_m_secondTabItemCache.isInit())
                return;

            _m_secondTabItemCache.pushBackAllCacheItems();
            _ITabItemData temp = null;
            SecondTabItemMono selectItemMono = null;
            for (int i = 0; i < _m_dataList.Count; i++)
            {
                temp = _m_dataList[i];
                if (null == temp)
                    continue;

                SecondTabItemMono itemMono = _addTabItem(temp);
                if (null != itemMono && i == secondDefaultIdx)
                {
                    selectItemMono = itemMono;
                }
            }
            
            if(null != selectItemMono)
                _tabItemDidClick(selectItemMono);
        }

        /// <summary>
        /// 添加单个item
        /// </summary>
        /// <param name="_itemData"></param>
        private SecondTabItemMono _addTabItem(_ITabItemData _itemData)
        {
            if (null == _m_secondTabItemCache || !_m_secondTabItemCache.isInit())
                return null;

            SecondTabItemMono itemMono = (SecondTabItemMono)_m_secondTabItemCache.popItem();
            if (null == itemMono)
                return null;

            itemMono.gameObject.SetParent(initParTrans);
            UGUICommon.setGameObjEnable(itemMono.gameObject);
            itemMono.setData(_itemData);
            itemMono.setTimeData(isUseSameTime, perTabItemTimeS, totalTabItemTimeS);
            itemMono.setCache(_m_tabItemCache);
            itemMono.setSelected(ESelectStatus.UN_SELECTED);
            itemMono.setFirstSelectDelegate(_m_firstSelectDelegate);
            itemMono.setClickDelegate(_tabItemDidClick);
            return itemMono;
        }

        /// <summary>
        /// 单个item点击事件
        /// </summary>
        private void _tabItemDidClick(_ATabItemMono _tabItemMono)
        {
            if (null == _tabItemMono)
                return;

            if (null != _m_selectTabItem && _tabItemMono == _m_selectTabItem)
                return;

            if (null != _m_selectTabItem)
                _m_selectTabItem.setSelected(ESelectStatus.UN_SELECTED);

            _tabItemMono.setSelected(ESelectStatus.IS_SELECTED);
            _m_selectTabItem = _tabItemMono;
            if (null != _m_secondSelectDelegate)
                _m_secondSelectDelegate(_tabItemMono.data);

            //如果没有子页签 把自己数据传出去 - TODO 临时处理 看有没有更好的处理方式
            if (null != _m_firstSelectDelegate &&
                (null == _tabItemMono.data.subData || _tabItemMono.data.subData.Count == 0))
                _m_firstSelectDelegate(_tabItemMono.data);
        }
    }
}