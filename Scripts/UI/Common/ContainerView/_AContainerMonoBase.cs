using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UTGame
{
    /// <summary>
    /// 相同item的容器 不需要循环的用这个
    /// </summary>
    public abstract class _AContainerMonoBase<_ITEM> : _ABaseMono where _ITEM : MonoBehaviour
    {
        //拖拽的scroll rect对象,请注意，当这个scroll rect有设置的时候，将会由代码调整 container 的大小以达到可被scroll rect拖拽的目的
        [Header("滑动容器")]
        public ScrollRect scrollRect;

        [Header("滑动区域 一般拖入content")]
        public LayoutGroup itemContainer;

        [Header("模板cell")]
        public _ITEM itemTemplate;

        /** 存储子窗口对象的队列 */
        private List<_ITEM> _m_lItemWndList = new List<_ITEM>();

        /** 刷新拖拽窗口尺寸的时间标记 */
        private float _m_fRefreshGridTimeTag;

        protected override void _OnInitEx()
        {
            _m_fRefreshGridTimeTag = 0;
        }

        /// <summary>
        /// 重置窗口数据，代替原先的discard函数
        /// </summary>
        public void resetWnd()
        {
            //清除所有子窗口对象并释放
            clearAll();
        }

        /// <summary>
        /// 释放窗口资源相关对象
        /// </summary>
        public void discard()
        {
            //清除所有子窗口对象并释放
            clearAll();
        }

        /// <summary>
        /// 获取第一个匹配的窗口
        /// </summary>
        /// <param name="_func"></param>
        /// <returns></returns>
        public _ITEM getFirstItemWnd(Func<_ITEM, bool> _func)
        {
            if (null == _func)
                return null;

            _ITEM tmpWnd = null;
            //遍历查找，匹配成功则跳出循环
            for (int i = 0; i < _m_lItemWndList.Count; i++)
            {
                tmpWnd = _m_lItemWndList[i];
                if (null == tmpWnd)
                    continue;

                if (_func(tmpWnd))
                {
                    return tmpWnd;
                }
            }

            return null;
        }

        /******************
         * 添加一个子窗口到容器中
         **/
        public _ITEM addItemWnd()
        {
            return addItemWnd(itemTemplate);
        }

        public _ITEM addItemWnd(_ITEM _templateWndMono)
        {
            //判断对应数据是否有效
            if (null == itemContainer || null == _templateWndMono)
                return null;

            //实例化一个子窗口对象
            _ITEM itemWnd = _instantiateItemMono(_templateWndMono);
            if (null == itemWnd)
                return null;

            //将子窗口添加到容器中
            itemWnd.transform.SetParent(itemContainer.transform);
            itemWnd.transform.localPosition = Vector3.zero;
            itemWnd.transform.localScale = Vector3.one;
            itemWnd.transform.localRotation = Quaternion.identity;

            //调用子窗口的显示函数
            itemWnd.gameObject.SetActive(true);

            //添加到数据集合
            _m_lItemWndList.Add(itemWnd);

            //在添加了窗口的时候调用的对应事件函数
            _onAddItemWnd(itemWnd);

            //刷新拖拽区域大小
            _refreshGrid();

            return itemWnd;
        }

        /**************
         * 从数据集中删除对应的子窗口对象
         * */
        public void removeItemWnd(_ITEM _itemWnd)
        {
            _ITEM tmpWnd = null;
            //遍历查找，匹配成功则跳出循环
            for (int i = 0; i < _m_lItemWndList.Count; i++)
            {
                tmpWnd = _m_lItemWndList[i];
                if (null == tmpWnd)
                    continue;

                if (tmpWnd == _itemWnd)
                {
                    //移除对应位置节点
                    _m_lItemWndList.RemoveAt(i);
                    break;
                }
            }

            //判断数据是否有效
            if (_itemWnd != tmpWnd || null == tmpWnd)
                return;

            //使子窗口的位置可以正确显示
            tmpWnd.transform.SetParent(null);
            //释放窗口对象
            _discardItem(tmpWnd);
            //刷新拖拽区域大小
            _refreshGrid();
        }

        /****************
         * 使用对应的刷新函数刷新所有子对象
         **/
        public void refreshAllItem(Action<_ITEM> _refreshAction)
        {
            if (null == _refreshAction)
                return;

            //释放物品格的对象
            foreach (_ITEM itemWnd in _m_lItemWndList)
            {
                //处理刷新函数
                _refreshAction(itemWnd);
            }
        }

        /****************
         * 清除所有子窗口
         **/
        public void clearAll()
        {
            //释放物品格的对象
            foreach (_ITEM itemWnd in _m_lItemWndList)
            {
                if (itemWnd == null)
                    continue;
                
                itemWnd.transform.SetParent(null);
                _discardItem(itemWnd);
            }

            //清空数据集
            _m_lItemWndList.Clear();

            //刷新拖拽区域大小
            _refreshGrid();
        }

        /*****************
         * 获取对应位置对象对应的实际显示对象
         **/
        public _ITEM getItemWnd(int _index)
        {
            if (0 <= _index && _index < _m_lItemWndList.Count)
            {
                return _m_lItemWndList[_index];
            }

            return null;
        }

        /**********
         * 移动到最底层
         **/
        public void moveToLeft()
        {
            if (null == scrollRect)
                return;

            scrollRect.horizontalNormalizedPosition = 0;
        }

        public void moveToRight()
        {
            if (null == scrollRect)
                return;

            scrollRect.horizontalNormalizedPosition = 1;
        }

        public void moveToTop()
        {
            if (null == scrollRect)
                return;

            scrollRect.verticalNormalizedPosition = 1;
        }

        public void moveToBottom()
        {
            if (null == scrollRect)
                return;

            scrollRect.verticalNormalizedPosition = 0;
        }

        public void moveToHorizontalRate(float _rate)
        {
            if (null == scrollRect)
                return;

            float finalH = Mathf.Clamp(1 - _rate, 0f, 1f);
            scrollRect.horizontalNormalizedPosition = finalH;
        }

        public void moveToVerticalRate(float _rate)
        {
            if (null == scrollRect)
                return;

            float finalV = Mathf.Clamp(1 - _rate, 0f, 1f);
            scrollRect.verticalNormalizedPosition = finalV;
        }

        /****************
         * 刷新grid的显示尺寸
         **/
        protected void _refreshGrid()
        {
            if (null == scrollRect)
                return;

            //判断时间标记
            if (_m_fRefreshGridTimeTag >= Time.realtimeSinceStartup)
                return;

            //设置时间标记
            _m_fRefreshGridTimeTag = Time.realtimeSinceStartup;
            //开启任务刷新
            if (itemContainer is VerticalLayoutGroup)
            {
                UTCommonTaskController.CommonActionAddNextFrameTask(() =>
                {
                    _refresh(GridLayoutGroup.Axis.Vertical);
                });
            }
            else if (itemContainer is HorizontalLayoutGroup)
            {
                UTCommonTaskController.CommonActionAddNextFrameTask(() =>
                {
                    _refresh(GridLayoutGroup.Axis.Horizontal);
                });
            }
            else if (itemContainer is GridLayoutGroup)
            {
                //横向布局是向下延申的，纵向布局是横向延申的
                //因此尺寸的设计是需要颠倒判断的
                GridLayoutGroup.Axis direction = ((GridLayoutGroup)itemContainer).startAxis;
                if (GridLayoutGroup.Axis.Horizontal == direction)
                {
                    UTCommonTaskController.CommonActionAddNextFrameTask(() =>
                    {
                        _refresh(GridLayoutGroup.Axis.Vertical);
                    });
                }
                else
                {
                    UTCommonTaskController.CommonActionAddNextFrameTask(() =>
                    {
                        _refresh(GridLayoutGroup.Axis.Horizontal);
                    });
                }
            }
        }

        protected void _refresh(GridLayoutGroup.Axis _axis)
        {
            RectTransform rectTrans = itemContainer.GetComponent<RectTransform>();
            if (GridLayoutGroup.Axis.Horizontal == _axis && itemContainer.preferredWidth > 0)
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemContainer.preferredWidth);
            else if (GridLayoutGroup.Axis.Vertical == _axis && itemContainer.preferredHeight > 0)
                rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemContainer.preferredHeight);
        }
        /***************
         * 创建本对象的脚本对象实例
         **/
        protected virtual _ITEM _instantiateItemMono(_ITEM _template)
        {
            if (null == _template)
                return null;

            //实例化一个子窗口对象
            return UGUICommon.cloneGameObj(_template);
        }

        /**************
         * 释放单个窗口资源的函数
         **/
        protected virtual void _discardItem(_ITEM _itemWnd)
        {
            if (null == _itemWnd)
                return;

            UGUICommon.releaseGameObj(_itemWnd);
        }

        /// <summary>
        /// 在添加了一个子窗口的时候调用的事件函数
        /// </summary>
        /// <param name="_itemWnd"></param>
        protected abstract void _onAddItemWnd(_ITEM _itemWnd);
    }
}