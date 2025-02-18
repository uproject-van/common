using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGame
{
    /// <summary>
    /// 带选择的滑动列表基类
    /// </summary>
    public abstract class _ASelectGridMonoBase<T_CELL,T_DATA> : _AGridMonoBase<T_CELL> where T_CELL:_ADataGridMonoCell<T_DATA> where T_DATA:_IBaseItem
    {
        //数据
        private List<T_DATA> _m_dataList;

        //当前选中下标
        private int _m_selectIdx;

        //选中回调
        private Action<T_DATA> _m_selectDelegate;
        
        //刷新选中
        protected abstract void _refreshSelect(T_DATA _data);
        protected override void _OnDestroyEx()
        {
            discard();
        }
        
        protected override void _createItemWnd(T_CELL _itemMono)
        {
        }

        protected override void _refreshItemwnd(T_CELL _itemMono, int _itemIdx)
        {
            if (null == _itemMono || null == _m_dataList || _itemIdx < 0 || _itemIdx >= _m_dataList.Count)
                return;

            if (null == _itemMono)
                return;
            
            _itemMono.setData(_m_dataList[_itemIdx]);
            _itemMono.setClickDelegate(_cellDidClickDelegate);
            _itemMono.setSelectStatus(GCommon.GetStatus(_itemIdx == _m_selectIdx));
        }


        public void setSelectDelegate(Action<T_DATA> _selectDelegate)
        {
            _m_selectDelegate = _selectDelegate;
        }

        /// <summary>
        /// 需要传入cell模板
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_cellTemplate"></param>
        protected void _setData(List<T_DATA> _list ,T_CELL _cellTemplate,EGridViewLayoutStyle _layoutStyle,Vector2 _spaceSize)
        {
            if (null == _list || null == _cellTemplate)
                return;

            _m_dataList = _list;
            _m_selectIdx = 0;
            if (!isInit)
                init(_cellTemplate, _m_dataList.Count, _layoutStyle, _spaceSize, true);
            else
                setTotalCount(_m_dataList.Count,_spaceSize,_layoutStyle,true);
            
            _sendSelectDelegate();
        }
        
        //cell点击事件
        private void _cellDidClickDelegate(_ABaseItemMono _itemMono)
        {
            if (null == _itemMono)
                return;

            T_CELL cell = _itemMono as T_CELL;
            if (null == cell)
                return;
            
            //刷新显示
            int lastIdx = _m_selectIdx;
            _m_selectIdx = cell.itemIdx;
            forceRefreshItem(lastIdx);
            forceRefreshItem(_m_selectIdx);
            _sendSelectDelegate();
        }

        /// <summary>
        /// 把数据传出去
        /// </summary>
        private void _sendSelectDelegate()
        {
            if (null == _m_dataList || _m_selectIdx < 0 || _m_selectIdx >= _m_dataList.Count)
                return;

            T_DATA goodData = _m_dataList[_m_selectIdx];
            if (null != goodData && null != _m_selectDelegate)
                _m_selectDelegate(goodData);
            
            _refreshSelect(goodData);
        }
    }
}