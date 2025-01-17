using UnityEngine;

namespace NGame
{
    /// <summary>
    /// 一个通用循环列表的循环cell基类
    /// </summary>
    public abstract class _AGridMonoCellBase : _ABaseUIEntity
    {
        [Header("宽度")]
        //尺寸设置
        public int width;

        [Header("高度")]
        public int height;
        
        protected int _m_iItemIdx;

        public int itemIdx { get { return _m_iItemIdx; } }

        /// <summary>
        /// 设置显示对象的位置索引
        /// </summary>
        /// <param name="_itemIdx"></param>
        public void setItemIdx(int _itemIdx)
        {
            _m_iItemIdx = _itemIdx;
        }
        
        /// <summary>
        /// 显示窗口数据
        /// </summary>
        public void showGridItem()
        {
            //默认是缩小为0
            UGUICommon.setUIObjScale(this, 1f);
        }
        
        /// <summary>
        /// 重置窗口数据
        /// </summary>
        public void resetGridItem()
        {
            _m_iItemIdx = -1;

            //默认是缩小为0
            UGUICommon.setUIObjScale(this, 0f);
        }

        public virtual void discardGridItem()
        {
            
        }
    }
}