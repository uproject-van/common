using System;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using WLDDZ;

namespace NGame
{
    public enum ECommonBtnStatus
    {
        NONE,
        ENABLE,
        UNABLE
    }

    /// <summary>
    /// 通用展示按钮
    /// </summary>
    public class CommonBtnMono : _ABaseUIEntity
    {
        [Header("使用状态类型列表")]
        public List<CommonStatusMono<ECommonBtnStatus>> statusMonoList;

        [Header("按钮文本")]
        public RTLTextMeshPro btnTxt;

        [Header("按钮点击事件")]
        public ClickButton clickBtn;

        [Header("图片")]
        public Image iconImg;

        private ECommonBtnStatus _m_status;
        
        private Action<ECommonBtnStatus>_m_clickBtnDidClick;
        
        protected override void _OnInitEx()
        {
            if (null != clickBtn)
                clickBtn.SetOnClickListener(_clickBtnDidClick);
        }

        protected override void _OnDestroyEx()
        {
            if (null != clickBtn)
                clickBtn.RemoveAllListeners();

            _m_clickBtnDidClick = null;
        }

        protected override void _OnEnableEx()
        {
        }

        protected override void _OnDisableEx()
        {
        }

        public void setClickDelegate(Action<ECommonBtnStatus> _clickBtnDidClick)
        {
            _m_clickBtnDidClick = _clickBtnDidClick;
        }

        public void setStatus(ECommonBtnStatus _status)
        {
            _m_status = _status;
            CommonStatusMono<ECommonBtnStatus>.setStatus(statusMonoList, _status);
        }
        
        public void setTxt(string _txt)
        {
            UGUICommon.setLabelTxt(btnTxt, _txt);
        }

        public void setTxt(int _code,bool _isTrans = false)
        {
            string str = _code.ToString();
            if (_isTrans)
                str = LocalizationManager.Instance.LocalizationString(_code);
            
            UGUICommon.setLabelTxt(btnTxt, str);
        }

        public void setType(BuyType _type)
        {
            switch (_type)
            {
                case BuyType.Diamond:
                {
                    Sprite sp = PlayerComponentMgr.instance.resLoader.Load<Sprite>(Res.hall.hall_common.sprites
                        .RES_COMMON_ICON_COMMON_GOLD);

                    UGUICommon.setSprite(iconImg,sp);
                    
                }
                    break;
            }
        }
        
        private void _clickBtnDidClick()
        {
            if (null != _m_clickBtnDidClick)
                _m_clickBtnDidClick(_m_status);
        }
    }
}