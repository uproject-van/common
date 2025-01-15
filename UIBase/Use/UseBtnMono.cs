using System;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using WLDDZ;

namespace NGame
{
    public enum EItemUseStatus
    {
        NONE,
        UN_USE, //未使用
        IS_USING, //使用中
    }

    /// <summary>
    /// 简单的使用按钮
    /// </summary>
    public class UseBtnMono : _ABaseUIEntity
    {
        [Header("使用状态类型列表")]
        public List<CommonStatusMono<EItemUseStatus>> statusMonoList;

        [Header("使用中文本")]
        public RTLTextMeshPro usingTxt;

        [Header("使用文本")]
        public RTLTextMeshPro usedTxt;

        [Header("使用按钮")]
        public ClickButton useBtn;

        //当前使用状态
        private EItemUseStatus _m_eUseStatus;

        private Action<EItemUseStatus> _m_aUseDelegate;
        
        protected override void _OnInitEx()
        {
            if (null != useBtn)
                useBtn.SetOnClickListener(_useBtnDidClick);
        }

        protected override void _OnDestroyEx()
        {
            if (null != useBtn)
                useBtn.RemoveAllListeners();
        }

        protected override void _OnEnableEx()
        {
        }

        protected override void _OnDisableEx()
        {
        }

        public void setUseDelegate(Action<EItemUseStatus> _useDelegate)
        {
            _m_aUseDelegate = _useDelegate;
        }

        public void setData(EItemUseStatus _useStatus)
        {
            _m_eUseStatus = _useStatus;
            _refresh();
        }

        private void _refresh()
        {
            CommonStatusMono<EItemUseStatus>.setStatus(statusMonoList, _m_eUseStatus);
            UGUICommon.setLabelTxt(usingTxt, LocalizationManager.Instance.LocalizationString(eLocalizationText.Used));
            UGUICommon.setLabelTxt(usedTxt, LocalizationManager.Instance.LocalizationString(eLocalizationText.Use));
        }

        private void _useBtnDidClick()
        {
            if (null != _m_aUseDelegate)
                _m_aUseDelegate(_m_eUseStatus);
        }
    }
}