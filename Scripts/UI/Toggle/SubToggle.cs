using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;

namespace NGame
{
    /// <summary>
    /// 自定义toggle 直接控制go显影，不使用ugui的toggle组件
    /// </summary>
    public class SubToggle : MonoBehaviour
    {
        [Header("默认的选中状态")]
        public ESelectStatus defaultSelectStatus;
        [Header("点击按钮")] 
        public Button clickBtn;
        [Header("状态列表")] 
        public List<CommonStatusMono<ESelectStatus>> statusMonoList;
        [Header("需要改变颜色的文本")] 
        public RTLTextMeshPro chgColorTxt; 
        
        
        protected Action<SubToggle> _m_dClickDelegate;
        protected ESelectStatus _m_selectStatus;//当前选中状态

        public Action<SubToggle> clickDelegate { get { return _m_dClickDelegate; } }
        public ESelectStatus selectStatus { get { return _m_selectStatus; } }

        public void Awake()
        {
            clickBtn.SetOnClickListener(_clickBtnDidClick);
            setSelected(defaultSelectStatus,true);
        }

        public void OnDestroy()
        {
            _m_dClickDelegate = null;
            clickBtn.RemoveAllListeners();
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
            CommonStatusMono<ESelectStatus>.setStatus(statusMonoList,_m_selectStatus, (_statusMono) =>
            {
                chgColorTxt.color = ColorHelper.ColorFrom16(_statusMono.chgColor);
            });
        }
        private void _clickBtnDidClick()
        {
            if(null != _m_dClickDelegate)
                _m_dClickDelegate(this);
        }
    }
}

