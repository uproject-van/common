using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using TMPro;

namespace NGame
{
    [Serializable]
    public class TabItemMonoStatusMono : CommonStatusMono<ESelectStatus>
    {
        [Header("图片")]
        public Image iconImg;

        [Header("背景图片")]
        public Image bgImg;
    }

    /// <summary>
    /// 单个TabItem
    /// </summary>
    public abstract class _ATabItemMono : UIEntity
    {
        [Header("初始高度")]
        public float normalHeight;
        
        [Header("状态列表")]
        public List<TabItemMonoStatusMono> statusMonoList;

        [Header("名字文本")]
        public RTLTextMeshPro nameTxt;

        [Header("点击按钮")]
        public Button clickBtn;

        //点击回调
        protected Action<_ATabItemMono> _m_dClickDelegate;

        //当前选中状态
        protected ESelectStatus _m_selectStatus;

        //数据
        protected _ITabItemData _m_data;

        //选中状态
        public ESelectStatus selectStatus
        {
            get { return _m_selectStatus; }
        }

        public _ITabItemData data
        {
            get { return _m_data; }
        }

        protected abstract void AwakeEx();
        protected abstract void OnDestroyEx();

        //选中或取消选中动画
        protected abstract void _doSelectStatusChgAnimation(Action _doneAction);
        protected abstract void resetEx();

        protected virtual void _refreshEx(){}
        public void Awake()
        {
            clickBtn.SetOnClickListener(_clickBtnDidClick);
            AwakeEx();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _refresh();
        }

        public override void OnDestroy()
        {
            reset();
            _m_dClickDelegate = null;
            clickBtn.RemoveAllListeners();
            OnDestroyEx();
        }

        /// <summary>
        /// 重置，放回缓存中的时候调用
        /// </summary>
        public void reset()
        {
            _m_data = null;
            setSelected(ESelectStatus.UN_SELECTED);
            _m_dClickDelegate = null;
            resetEx();
        }
        
        //设置数据
        public void setData(_ITabItemData _data)
        {
            _m_data = _data;
            _refresh();
        }

        //设置点击回调
        public void setClickDelegate(Action<_ATabItemMono> _clickDelegate)
        {
            _m_dClickDelegate = _clickDelegate;
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="_isSelect"></param>
        /// <param name="_isForce">强制设置</param>
        public void setSelected(ESelectStatus _selectStatus, bool _isForce = false)
        {
            if (!_isForce && _m_selectStatus == _selectStatus)
                return;

            _m_selectStatus = _selectStatus;
            _refresh();
            _doSelectStatusChgAnimation(() =>
            {
                _refreshEx();
            });
        }
        
        //刷新显示
        private void _refresh()
        {
            if (null == _m_data)
                return;

            UGUICommon.setLabelTxt(nameTxt,_m_data.name);
            TabItemMonoStatusMono.setStatus(statusMonoList, _m_selectStatus,
                (_statusMono) =>
                {
                    UGUICommon.setUIObjColor(nameTxt,ColorHelper.ColorFrom16(_statusMono.chgColor));
                    if (_statusMono.status == ESelectStatus.UN_SELECTED)
                    {
                        _setImg(_statusMono, _m_data.unSelectImg, _m_data.unSelectBgImg);
                    }
                    else
                    {
                        _setImg(_statusMono, _m_data.selectImg, _m_data.selectBgImg);
                    }
                });
        }

        private void _setImg(TabItemMonoStatusMono _statusMono, Sprite _iconImg, Sprite _bgImg)
        {
            if(null == _statusMono)
                return;
            
            UGUICommon.setSprite(_statusMono.iconImg,_iconImg);
            UGUICommon.setSprite(_statusMono.bgImg,_bgImg);
        }
        
        private void _clickBtnDidClick()
        {
            if (null != _m_dClickDelegate)
                _m_dClickDelegate(this);
        }
    }
}