using System.Collections.Generic;
using UnityEngine;
using System;

namespace UTGame
{
    /// <summary>
    /// 自定义toggle
    /// </summary>
    public class CommonToggle : _ABaseMono
    {
        [Header("所有的toggle")]
        public List<SubToggle> subToggles;

        [Header("默认选中的下标")]
        public int defaultIdx;

        private Action<int> _m_dClickDelegate;

        protected override void _OnInitEx()
        {
            if (null != subToggles)
            {
                SubToggle temp = null;
                for (int i = 0; i < subToggles.Count; i++)
                {
                    temp = subToggles[i];
                    if (null == temp)
                        continue;

                    temp.setClickDelegate(_subToggleClickDelegate);
                }
            }
        }

        protected override void _OnDestroyEx()
        {
            _m_dClickDelegate = null;
        }

        protected override void _OnEnableEx()
        {
            //选中配置的下标
            if (null != subToggles)
            {
                SubToggle temp = null;
                for (int i = 0; i < subToggles.Count; i++)
                {
                    temp = subToggles[i];
                    if (null == temp)
                        continue;

                    ESelectStatus status = i == defaultIdx ? ESelectStatus.IS_SELECTED : ESelectStatus.UN_SELECTED;
                    temp.setSelected(status);
                }
            }

            if (null != _m_dClickDelegate)
                _m_dClickDelegate(defaultIdx);
        }

        protected override void _OnDisableEx()
        {
        }

        public void setClickDelegate(Action<int> _clickDelegate)
        {
            _m_dClickDelegate = _clickDelegate;
        }

        private void _subToggleClickDelegate(SubToggle _subToggle)
        {
            if (null != subToggles)
            {
                int selectIdx = -1;
                SubToggle temp = null;
                for (int i = 0; i < subToggles.Count; i++)
                {
                    temp = subToggles[i];
                    if (null == temp)
                        continue;

                    if (_subToggle == temp)
                    {
                        selectIdx = i;
                        temp.setSelected(ESelectStatus.IS_SELECTED);
                    }
                    else
                        temp.setSelected(ESelectStatus.UN_SELECTED);
                }

                if (null != _m_dClickDelegate)
                    _m_dClickDelegate(selectIdx);
            }
        }
    }
}