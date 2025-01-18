using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UTGame
{
    /// <summary>
    /// 自定义toggle 直接控制go显影，不使用ugui的toggle组件
    /// </summary>
    public class SubToggle : _ABaseMono
    {
        [Header("默认的选中状态")]
        public ESelectStatus defaultSelectStatus;

        [Header("点击按钮")]
        public GameObject clickBtn;

        [Header("状态列表")]
        public List<CommonStatusMono<ESelectStatus>> statusMonoList;

        [Header("需要改变颜色的文本")]
        public Text chgColorTxt;


        protected Action<SubToggle> _m_dClickDelegate;
        protected ESelectStatus _m_selectStatus; //当前选中状态

        public Action<SubToggle> clickDelegate
        {
            get { return _m_dClickDelegate; }
        }

        public ESelectStatus selectStatus
        {
            get { return _m_selectStatus; }
        }

        protected override void _OnInitEx()
        {
            UGUICommon.combineBtnClick(clickBtn, _clickBtnDidClick);
            setSelected(defaultSelectStatus, true);
        }

        protected override void _OnDestroyEx()
        {
            _m_dClickDelegate = null;
            UGUICommon.uncombineBtnClick(clickBtn, _clickBtnDidClick);
        }

        protected override void _OnEnableEx()
        {
        }

        protected override void _OnDisableEx()
        {
        }

        //设置点击回调
        public void setClickDelegate(Action<SubToggle> _clickDelegate)
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
        }

        private void _refresh()
        {
            UGUICommon.setStatus(statusMonoList, _m_selectStatus,
                (_statusMono) => { chgColorTxt.color = _statusMono.chgColor; });
        }

        private void _clickBtnDidClick(GameObject _go)
        {
            if (null != _m_dClickDelegate)
                _m_dClickDelegate(this);
        }
    }
}