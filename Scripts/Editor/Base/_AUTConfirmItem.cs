
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 按钮基类
    /// </summary>
    public abstract class _AUTConfirmItem
    {
        public void onGUI()
        {
            //创建一个按钮对象
            if (GUILayout.Button(_text, GUILayout.Height(30), GUILayout.Width(600)))
            {
                _dealComfirm();
            }
        }

        /// <summary>
        /// 显示的说明
        /// </summary>
        protected abstract string _text { get; }

        /// <summary>
        /// 具体的处理函数
        /// </summary>
        protected abstract void _dealComfirm();
    }

    /// <summary>
    /// Toggle 基类
    /// </summary>
    public abstract class _AUTToggleItem 
    {
        protected bool _m_bIsOn;
        private bool tempResult;

        public bool isOn { get { return _m_bIsOn; } }

        public _AUTToggleItem()
        {
            _m_bIsOn = false;
        }

        public void onGUI()
        {
            tempResult = GUILayout.Toggle(_m_bIsOn, _text, GUILayout.Height(30));
            if (tempResult != _m_bIsOn)
            {
                _m_bIsOn = tempResult;
                _dealTgeChg(_m_bIsOn);
            }
        }

        protected abstract string _text { get; }


        protected abstract void _dealTgeChg(bool _isOn);

        public void setToggleValue(bool _isOn)
        {
            if (_m_bIsOn == _isOn)
                return;

            _m_bIsOn = _isOn;
        }

    }
}

