using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UTGame
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
    public class UseBtnMono : _ABaseMono
    {
        [Header("使用状态类型列表")]
        public List<CommonStatusMono<EItemUseStatus>> statusMonoList;

        [Header("使用中文本")]
        public Text usingTxt;

        [Header("使用文本")]
        public Text usedTxt;

        [Header("使用按钮")]
        public GameObject useBtn;

        //当前使用状态
        private EItemUseStatus _m_eUseStatus;

        private Action<EItemUseStatus> _m_aUseDelegate;

        protected override void _OnInitEx()
        {
            UGUICommon.combineBtnClick(useBtn, _useBtnDidClick);
        }

        protected override void _OnDestroyEx()
        {
            UGUICommon.uncombineBtnClick(useBtn, _useBtnDidClick);
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

        //TODO 使用文本修改
        private void _refresh()
        {
            CommonStatusMono<EItemUseStatus>.setStatus(statusMonoList, _m_eUseStatus);
            UGUICommon.setLabelTxt(usingTxt, "useing");
            UGUICommon.setLabelTxt(usedTxt, "used");
        }

        private void _useBtnDidClick(GameObject _go)
        {
            if (null != _m_aUseDelegate)
                _m_aUseDelegate(_m_eUseStatus);
        }
    }
}