using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NGame
{
    public enum EGridViewLayoutStyle
    {
        HORIZONTAL,
        VERTICAL,
    }

    /// <summary>
    /// 循环组件的基类
    /// </summary>
    public abstract class _AGridMonoBase : _ABaseUIEntity
    {
        [Header("滑动容器")]
        public ScrollRect scrollRect;

        [Header("遮罩 显示内容区域")]
        public RectTransform gridAreaMaskObj;

        [Header("滑动区域 一般拖入content")]
        public RectTransform gridAreaUIObj;

        [Header("数量为空显示的GoList - 有数量隐藏")]
        public List<GameObject> emptyShowGoList;

        [Header("数量为空隐藏的GoList - 有数量显示")]
        public List<GameObject> emptyHideGoList;

        [Header("是否从右到左排列")]
        public bool isRTL = false;

        public bool isInit
        {
            get { return _m_bIsInit; }
        }

        private bool _m_bIsInit;

        //cell总数量
        private int _m_iTotalCount = 0;

        // 展示的cell总数量
        private int _m_iShowTotlaCount;

        private EGridViewLayoutStyle _m_layoutStyle;

        //每行或每列的数量，当横向时表示每列数量，当纵向时表示每行数量
        private int _m_perLineItemCount;

        //间隔
        private Vector2 _m_space;

        //当前复用的cell模板
        private _AGridMonoCellBase _m_template;

        /** 窗口显示对象当前尺寸 */
        private float _m_fWidth;

        private float _m_fHeight;

        /** 所有内容对象的显示尺寸 */
        private float _m_fAllWidth;

        private float _m_fAllHeight;

        /** 是否需要强制重新计算窗口size */
        private bool _m_bNeedForceRefreshSize;

        /** 是否需要强制刷新 */
        private bool _m_bNeedForceRefresh;

        /** 当行或当列的对象数量 */
        private int _m_iPerUnitItemCount;

        /** 行或列的数量 */
        private int _m_iShowLineCount;

        //是否需要自适应，或动态平分间隔
        private bool _m_autoFixLine;

        /** 根据当前窗口尺寸创建出来的需要显示的子对象队列 */
        private List<_AGridMonoCellBase> _m_lShowItemList;

        /** 根据当前窗口尺寸创建出来的所有可能需要使用到的对象队列 */
        private List<_AGridMonoCellBase> _m_lTotalItemList;

        /** 当前显示范围的区域信息 */
        private Vector2 _m_vGridPreShowRect;

        private Vector2 _m_vGridCurShowRect;

        /** 起始到结束的对象位置索引信息 */
        private int _m_iStartItemIdx;

        private int _m_iFinalItemIdx;
        private int _m_iTmpi;
        private int _m_iTmpi2;
        private _AGridMonoCellBase _m_wTmpItem;

        protected _AGridMonoBase()
        {
            _m_bIsInit = false;
            _m_bNeedForceRefresh = false;
            _m_fWidth = 0;
            _m_fHeight = 0;
            _m_fAllWidth = 0;
            _m_fAllHeight = 0;
            _m_space = Vector2.zero;
            _m_iTotalCount = 0;
            _m_iPerUnitItemCount = 0;
            _m_iStartItemIdx = 0;
            _m_iFinalItemIdx = 0;
            _m_lShowItemList = new List<_AGridMonoCellBase>();
            _m_lTotalItemList = new List<_AGridMonoCellBase>();
            _m_autoFixLine = false;
        }

        //TODO 这里可以优化
        private void Update()
        {
            if (!gameObject.activeInHierarchy || !_m_bIsInit)
                return;

            frameRefresh();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_template">模板cell</param>
        /// <param name="_totalCount">总数量</param>
        /// <param name="_layoutStyle">布局方式</param>
        /// <param name="_spaceSize">间隔</param>
        /// <param name="_isAutoFixLine">是否自动布局</param>
        public void init(_AGridMonoCellBase _template, int _totalCount, EGridViewLayoutStyle _layoutStyle,
            Vector2 _spaceSize, bool _isAutoFixLine = false)
        {
            if (null == _template || null == scrollRect || null == gridAreaMaskObj || null == gridAreaUIObj)
                return;

            if (_m_bIsInit)
                return;

            _m_template = _template;
            _m_layoutStyle = _layoutStyle;
            _m_autoFixLine = _isAutoFixLine;
            setTotalCount(_totalCount, _spaceSize, false);
            _refreshGridSize();
        }

        public void setTotalCount(int _totalCount,Vector2 _spaceSize,bool _isForceRefresh = true)
        {
            _m_space = _spaceSize;
            _m_iTotalCount = _totalCount;
            _m_bNeedForceRefresh = _isForceRefresh;
            _refreshEmptyShow();
        }

        private void _refreshEmptyShow()
        {
            UGUICommon.setGameObjEnable(emptyShowGoList, _m_iTotalCount == 0);
            UGUICommon.setGameObjEnable(emptyHideGoList, _m_iTotalCount != 0);
        }

        /// <summary>
        /// 清除所有子窗口
        /// </summary>
        public void discard()
        {
            _m_bIsInit = false;

            _m_fWidth = 0;
            _m_fHeight = 0;
            _m_iTotalCount = 0;
            _m_iPerUnitItemCount = 0;
            _m_iStartItemIdx = 0;
            _m_iFinalItemIdx = 0;
            //释放物品格的对象
            foreach (_AGridMonoCellBase cell in _m_lTotalItemList)
            {
                //使子窗口的位置可以正确显示
                cell.transform.SetParent(null);
                cell.discardGridItem();
                //删除对应的对象
                UGUICommon.releaseGameObj(cell);
            }

            //清空数据集
            _m_lShowItemList.Clear();
            _m_lTotalItemList.Clear();
            _m_wTmpItem = null;
            _m_template = null;
            _m_layoutStyle = EGridViewLayoutStyle.HORIZONTAL;
            _m_space = Vector2.zero;
        }

        /// <summary>
        /// 使用对应的刷新函数刷新所有子对象
        /// </summary>
        /// <param name="_refreshAction"></param>
        public void refreshAllItem(Action<_AGridMonoCellBase, int> _refreshAction)
        {
            if (null == _refreshAction || null == _m_lShowItemList)
                return;

            //释放物品格的对象
            for (int i = 0; i < _m_lShowItemList.Count; i++)
            {
                _refreshAction(_m_lShowItemList[i], i + _m_iStartItemIdx);
            }
        }

        /// <summary>
        /// 强制刷新对应对象
        /// </summary>
        public void forceRefreshAllItem()
        {
            //刷新每个对象窗口
            refreshAllItem(_refreshItemwnd);
        }

        public void forceRefreshItem(int _index)
        {
            //释放物品格的对象
            for (int i = 0; i < _m_lShowItemList.Count; i++)
            {
                if (i + _m_iStartItemIdx == _index)
                    _refreshItemwnd(_m_lShowItemList[i], i + _m_iStartItemIdx);
            }
        }

        #region 移动

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

        #endregion


        /// <summary>
        /// 添加一个子窗口到容器中
        /// </summary>
        protected void _addItemWnd()
        {
            //判断对应数据是否有效
            if (null == gridAreaUIObj || null == _m_template)
                return;

            //实例化一个子窗口对象
            _AGridMonoCellBase itemWndObj = UGUICommon.cloneGameObj(_m_template);
            if (null == itemWndObj)
                return;

            _createItemWnd(itemWndObj);
            //将子窗口添加到容器中
            itemWndObj.transform.SetParent(gridAreaUIObj.transform);
            itemWndObj.transform.localPosition = Vector3.zero;
            itemWndObj.transform.localScale = Vector3.one;

            //重置数据
            itemWndObj.resetGridItem();

            //添加到数据集合
            _m_lTotalItemList.Add(itemWndObj);
        }

        /// <summary>
        /// 从数据集中删除对应的子窗口对象
        /// </summary>
        /// <param name="_itemWnd"></param>
        protected void _removeItemWnd(_AGridMonoCellBase _itemWnd)
        {
            _AGridMonoCellBase tmpWnd = null;
            //遍历查找，匹配成功则跳出循环
            for (int i = 0; i < _m_lTotalItemList.Count; i++)
            {
                tmpWnd = _m_lTotalItemList[i];
                if (null == tmpWnd)
                    continue;

                if (tmpWnd == _itemWnd)
                {
                    //移除对应位置节点
                    _m_lTotalItemList.RemoveAt(i);
                    break;
                }
            }

            //判断数据是否有效
            if (_itemWnd != tmpWnd)
                return;

            //使子窗口的位置可以正确显示
            tmpWnd.transform.SetParent(null);
            //释放窗口对象
            tmpWnd.discardGridItem();

            //删除对应的对象
            UGUICommon.releaseGameObj(tmpWnd);
        }


        /// <summary>
        /// 检测content的宽高,不一致则重置
        /// </summary>
        private void _checkContentSize()
        {
            if (null == gridAreaMaskObj || null == gridAreaUIObj || null == _m_template)
            {
                Debug.LogError(
                    "_AGridViewMono _m_gridAreaMaskObj = null or _m_gridAreaUIObj = null or _m_template = null ");
                return;
            }

            if (_m_fWidth == gridAreaMaskObj.rect.width && _m_fHeight == gridAreaMaskObj.rect.height &&
                _m_iShowTotlaCount == _m_iTotalCount && !_m_bNeedForceRefreshSize)
                return;

            _m_bNeedForceRefreshSize = false;
            //此时需要重新计算所有对象
            _m_fWidth = gridAreaMaskObj.rect.width;
            _m_fHeight = gridAreaMaskObj.rect.height;
            if (_m_template.height <= 0 || _m_template.width <= 0)
            {
                Debug.LogError("_AGridViewMono _checkContentSize() template.height = 0 template.width = 0 ");
                return;
            }

            //此时需要重新计算所有对象的数量
            if (_m_layoutStyle == EGridViewLayoutStyle.HORIZONTAL)
            {
                //实际可以放入的数量
                _m_iPerUnitItemCount = (int)(((_m_fHeight - _m_space.y) / (_m_template.height + _m_space.y)) + 0.001f);
                //实际的行数
                _m_iShowLineCount = (int)(((_m_fWidth - _m_space.x) / (_m_template.width + _m_space.x)) + 0.001f) + 2;

                if (0 != _m_perLineItemCount && _m_perLineItemCount < _m_iPerUnitItemCount)
                    _m_iPerUnitItemCount = _m_perLineItemCount;

                //最少每行一个
                if (_m_iPerUnitItemCount <= 0)
                    _m_iPerUnitItemCount = 1;

                //如果窗口需要自适应尺寸则在此进行计算
                if (_m_autoFixLine)
                {
                    //重新计算高度
                    _m_space.y = (_m_fHeight - (_m_iPerUnitItemCount * _m_template.height)) /
                                 (_m_iPerUnitItemCount + 1);
                }
            }
            else
            {
                _m_iPerUnitItemCount = (int)(((_m_fWidth - _m_space.x) / (_m_template.width + _m_space.x)) + 0.001f);
                _m_iShowLineCount = (int)(((_m_fHeight - _m_space.y) / (_m_template.height + _m_space.y)) + 0.001f) + 2;

                if (0 != _m_perLineItemCount && _m_perLineItemCount < _m_iPerUnitItemCount)
                    _m_iPerUnitItemCount = _m_perLineItemCount;

                //最少每行一个
                if (_m_iPerUnitItemCount <= 0)
                    _m_iPerUnitItemCount = 1;

                //如果窗口需要自适应尺寸则在此进行计算
                if (_m_autoFixLine)
                {
                    //重新计算高度
                    _m_space.x = (_m_fWidth - _m_iPerUnitItemCount * _m_template.width) / (_m_iPerUnitItemCount + 1);
                }
            }

            _m_iShowTotlaCount = _m_iTotalCount;
            int totalLineCount = (_m_iShowTotlaCount + _m_iPerUnitItemCount - 1) / _m_iPerUnitItemCount;
            //设置区域对象的尺寸
            if (_m_layoutStyle == EGridViewLayoutStyle.HORIZONTAL)
            {
                //设置总宽度
                _m_fAllWidth = (totalLineCount * (_m_template.width + _m_space.x)) + _m_space.x;
                _m_fAllHeight = (_m_iPerUnitItemCount * (_m_template.height + _m_space.y)) + _m_space.y;
                gridAreaUIObj.sizeDelta = new Vector2(_m_fAllWidth, _m_fAllHeight);
            }
            else
            {
                _m_fAllWidth = (_m_iPerUnitItemCount * (_m_template.width + _m_space.x)) + _m_space.x;
                //设置总高度
                _m_fAllHeight = (totalLineCount * (_m_template.height + _m_space.y)) + _m_space.y;
                gridAreaUIObj.sizeDelta = new Vector2(_m_fAllWidth, _m_fAllHeight);
            }

            //计算需要的总数量
            int totalCacheItemCount = (_m_iPerUnitItemCount * _m_iShowLineCount);

            totalCacheItemCount = totalCacheItemCount < _m_iTotalCount ? totalCacheItemCount : _m_iTotalCount;
            //初始化所有显示对象队列
            _AGridMonoCellBase tmpItem = null;
            while (_m_lShowItemList.Count < totalCacheItemCount)
                _m_lShowItemList.Add(null);
            while (_m_lShowItemList.Count > totalCacheItemCount)
            {
                tmpItem = _m_lShowItemList[_m_lShowItemList.Count - 1];
                if (null != tmpItem)
                    tmpItem.resetGridItem();

                _m_lShowItemList.RemoveAt(_m_lShowItemList.Count - 1);
            }

            //根据需要的总数量从数据集合中删除多余，增加缺少的
            while (_m_lTotalItemList.Count < totalCacheItemCount)
            {
                _addItemWnd();
            }

            //删除多余的
            while (_m_lTotalItemList.Count > totalCacheItemCount)
            {
                _removeItemWnd(_m_lTotalItemList[_m_lTotalItemList.Count - 1]);
            }

            //设置强制刷新
            _m_bNeedForceRefresh = true;
        }


        /// <summary>
        /// 刷新窗口尺寸，重新计算显示数量
        /// </summary>
        private void _refreshGridSize()
        {
            if (_m_layoutStyle == EGridViewLayoutStyle.HORIZONTAL)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            }
            else
            {
                scrollRect.vertical = true;
                scrollRect.horizontal = false;
            }

            //开启刷新任务
            _m_bIsInit = true;
        }

        /***************
         * 每帧刷新操作
         **/
        public void frameRefresh()
        {
            //检查窗口尺寸
            _checkContentSize();

            //计算区域
            _recalculateCurRect();
        }

        /// <summary>
        /// 刷新处理
        /// </summary>
        private void _recalculateCurRect()
        {
            if (null == _m_template)
                return;

            //设置上一次的区域
            _m_vGridPreShowRect = _m_vGridCurShowRect;
            //根据当前
            _m_vGridCurShowRect.x = -gridAreaUIObj.anchoredPosition.x;
            _m_vGridCurShowRect.y = gridAreaUIObj.anchoredPosition.y;

            if (!_m_bNeedForceRefresh && _m_vGridCurShowRect.x == _m_vGridPreShowRect.x &&
                _m_vGridCurShowRect.y == _m_vGridPreShowRect.y)
                return;

            //设置强制刷新无效
            bool isForceRefresh = _m_bNeedForceRefresh;
            _m_bNeedForceRefresh = false;

            //根据显示区域计算开始显示的窗口索引
            if (_m_layoutStyle == EGridViewLayoutStyle.HORIZONTAL)
            {
                if (_m_vGridCurShowRect.x <= 0)
                    _m_iStartItemIdx = 0;
                else
                    _m_iStartItemIdx = (int)(_m_vGridCurShowRect.x / (_m_template.width + _m_space.x)) *
                                       _m_iPerUnitItemCount;

                _m_iFinalItemIdx = _m_iStartItemIdx + (_m_iPerUnitItemCount * _m_iShowLineCount) - 1;
                //判断最后的索引是否有效
                if (_m_iFinalItemIdx >= _m_iTotalCount)
                    _m_iFinalItemIdx = _m_iTotalCount - 1;
            }
            else
            {
                if (_m_vGridCurShowRect.y <= 0)
                    _m_iStartItemIdx = 0;
                else
                    _m_iStartItemIdx = (int)((_m_vGridCurShowRect.y) / (_m_template.height + _m_space.y)) *
                                       _m_iPerUnitItemCount;

                _m_iFinalItemIdx = _m_iStartItemIdx + (_m_iPerUnitItemCount * _m_iShowLineCount) - 1;
                //判断最后的索引是否有效
                if (_m_iFinalItemIdx >= _m_iTotalCount)
                    _m_iFinalItemIdx = _m_iTotalCount - 1;
            }

            //根据显示的对象范围逐个调整对象位置
            //先清空所有显示队列
            for (_m_iTmpi = 0; _m_iTmpi < _m_lShowItemList.Count; _m_iTmpi++)
                _m_lShowItemList[_m_iTmpi] = null;
            //逐个将显示范围内的对象放入，此处处理的是可能的重复对象
            for (_m_iTmpi = 0; _m_iTmpi < _m_lTotalItemList.Count; _m_iTmpi++)
            {
                _m_wTmpItem = _m_lTotalItemList[_m_iTmpi];
                //判断显示对象是否在索引区域内，是则直接设置数据索引
                if (_m_wTmpItem.itemIdx >= _m_iStartItemIdx && _m_wTmpItem.itemIdx <= _m_iFinalItemIdx)
                {
                    //此时设置显示对象对应索引的对象为本对象
                    _m_iTmpi2 = _m_wTmpItem.itemIdx - _m_iStartItemIdx;

                    //设置对应显示队列中对象
                    if (null == _m_lShowItemList[_m_iTmpi2])
                    {
                        _m_lShowItemList[_m_iTmpi2] = _m_wTmpItem;
                    }
                    else
                    {
                        _m_wTmpItem.resetGridItem();
                    }
                }
                else
                {
                    _m_wTmpItem.resetGridItem();
                }
            }

            //两个队列分别逐个遍历，将未使用的对象放入显示队列对应空位
            _m_iTmpi2 = 0; //此时设置为所有对象的索引位置
            for (_m_iTmpi = 0; _m_iTmpi < _m_lShowItemList.Count; _m_iTmpi++)
            {
                if (null == _m_lShowItemList[_m_iTmpi])
                {
                    //此时需要寻找对象并设置
                    while (_m_lTotalItemList[_m_iTmpi2].itemIdx != -1)
                    {
                        _m_iTmpi2++;
                    }

                    _m_wTmpItem = _m_lTotalItemList[_m_iTmpi2];
                    _m_iTmpi2++;
                    //此时设置对象
                    _m_lShowItemList[_m_iTmpi] = _m_wTmpItem;
                }
            }

            //将显示队列遍历，设置无效的显示信息，并刷新每个的新位置
            for (_m_iTmpi = 0; _m_iTmpi < _m_lShowItemList.Count; _m_iTmpi++)
            {
                _m_wTmpItem = _m_lShowItemList[_m_iTmpi];
                if (null == _m_wTmpItem)
                    continue;

                _m_iTmpi2 = _m_iTmpi + _m_iStartItemIdx;
                //超出范围则中断
                if (_m_iTmpi2 >= _m_iTotalCount)
                    break;

                //在0数据之前的数据都是无效数据
                if (_m_iTmpi2 < 0)
                {
                    _m_wTmpItem.resetGridItem();
                    continue;
                }

                //显示窗口
                _m_wTmpItem.showGridItem();
                //设置显示对象位置
                if (_m_layoutStyle == EGridViewLayoutStyle.HORIZONTAL)
                {
                    //横向位置设置
                    UGUICommon.setUIPos(
                        _m_wTmpItem.gameObject
                        , ((_m_iTmpi2 / _m_iPerUnitItemCount) * (_m_template.width + _m_space.x)) + _m_space.x
                        , -(((_m_iTmpi2 % _m_iPerUnitItemCount) * (_m_template.height + _m_space.y)) + _m_space.y));
                }
                else
                {
                    //处理从右往左排序
                    float x = ((_m_iTmpi2 % _m_iPerUnitItemCount) * (_m_template.width + _m_space.x)) + _m_space.x;
                    if (isRTL)
                        x = _m_fAllWidth - x - (_m_template.width + _m_space.x);
                    //纵向位置设置
                    UGUICommon.setUIPos(
                        _m_wTmpItem.gameObject
                        , x
                        , -(((_m_iTmpi2 / _m_iPerUnitItemCount) * (_m_template.height + _m_space.y)) + _m_space.y));
                }

                //判断序号是否一致，并刷新显示
                if (_m_wTmpItem.itemIdx == _m_iTmpi2)
                {
                    //判断是否需要强制刷新，是则刷新显示项
                    if (isForceRefresh)
                        _refreshItemwnd(_m_wTmpItem, _m_iTmpi2);
                    continue;
                }

                //设置索引
                _m_wTmpItem.setItemIdx(_m_iTmpi2);
                //刷新显示信息
                _refreshItemwnd(_m_wTmpItem, _m_iTmpi2);
            }
        }

        /// <summary>
        /// 根据带入的已经实例化的图标对象，创建一个对应子窗口的管理对象
        /// </summary>
        /// <returns></returns>
        protected abstract void _createItemWnd(_AGridMonoCellBase _itemMono);

        /// <summary>
        /// 根据带入的窗口对象以及索引刷新相关的显示信息
        /// </summary>
        /// <param name="_itemMono"></param>
        /// <param name="_itemIdx"></param>
        protected abstract void _refreshItemwnd(_AGridMonoCellBase _itemMono, int _itemIdx);
    }
}