using System.Collections.Generic;

namespace NGame
{
    public interface _ICommonItem
    {
        public int itemId { get; }
        public long count { get; }
        public int enableDay { get; }
    }
    
    /// <summary>
    /// 通用的物品展示列表
    /// </summary>
    public abstract class _ACommonItemContainer<T_MONO,T_DATA> : _AContainerMonoBase<T_MONO> where T_MONO:_ACommonItemMono<T_DATA> where T_DATA:_ICommonItem
    {
        //所有item
        private List<T_MONO> _m_itemMonoList;

        //数据列表
        private List<T_DATA> _m_dataList;
        
        protected override void _OnInitEx()
        {
            _m_dataList = new List<T_DATA>();
            _m_itemMonoList = new List<T_MONO>();
        }

        protected override void _OnDestroyEx()
        {
            T_MONO temp = null;
            for (int i = 0; i < _m_itemMonoList.Count; i++)
            {
                temp = _m_itemMonoList[i];
                if(null == temp)
                    continue;
                
                UGUICommon.releaseGameObj(temp);
            }
            _m_itemMonoList.Clear();
            _m_dataList.Clear();
            
            _m_dataList = null;
            _m_itemMonoList = null;
        }

        protected override void _OnEnableEx()
        {
            _refresh();
        }

        protected override void _OnDisableEx()
        {

        }

        protected override void _onAddItemWnd(T_MONO _itemWnd)
        {
        }

        public void setData(IEnumerable<T_DATA> _dataList)
        {
            if(null == _dataList)
                return;
            
            _m_dataList.Clear();
            _m_dataList.AddRange(_dataList);
            _refresh();
        }
        
        private void _refresh()
        {
            if(null == _m_dataList)
                return;
            
            //逐个添加Item
            int itemIdx = 0;
            T_MONO itemMono = null;
            T_DATA temp = default(T_DATA);
            for (int i = 0; i < _m_dataList.Count; i++)
            {
                temp = _m_dataList[i];
                if (null == temp)
                    continue;
                
                if (itemIdx < _m_itemMonoList.Count)
                {
                    itemMono = _m_itemMonoList[itemIdx];
                }
                else
                {
                    itemMono = addItemWnd();
                    if (null != itemMono)
                        _m_itemMonoList.Add(itemMono);
                }
                //累加索引
                itemIdx++;

                if (null != itemMono)
                {
                    UGUICommon.setGameObjEnable(itemMono,true);
                    itemMono.setItem(temp);
                }
            }

            //隐藏容器中多余的视图
            for (int j = _m_itemMonoList.Count - 1; j >= itemIdx; j--)
            {
                itemMono = _m_itemMonoList[j];
                UGUICommon.setGameObjEnable(itemMono,false);
            }
            
        }
    }
}