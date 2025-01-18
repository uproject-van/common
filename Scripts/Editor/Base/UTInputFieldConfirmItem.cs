using System;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 带按钮点击的输入框item
    /// </summary>
    public class UTInputFieldConfirmItem : _IUTExportMenuInterface
    {
        private string _m_sText;    //描述文本
        private string _m_sConfirmStr;  //按钮文字
        private string _m_sInputText;   //输入框文字
        private int _m_iHeight; //显示高度
        private int _m_iBtnHWidth;// 按钮显示宽度
        private Action<string> _m_dealDone;//按钮点击回调
        
        /// <summary>
        /// 带按钮点击的输入框item
        /// </summary>
        /// <param name="_text">描述文本</param>
        /// <param name="_defaultValue">默认输入文本</param>
        /// <param name="_confirmStr">按钮文字</param>
        /// <param name="_dealDone">按钮点击回调</param>
        /// <param name="_btnWidth">按钮显示宽度</param>
        /// <param name="_height">显示高度</param>
        public UTInputFieldConfirmItem(string _text, string _defaultValue,string _confirmStr,Action<string> _dealDone,int _btnWidth, int _height)
        {
            _m_sText = _text;
            _m_sInputText = _defaultValue;
            _m_sConfirmStr = _confirmStr;
            _m_dealDone = _dealDone;
            _m_iHeight = _height;
            _m_iBtnHWidth = _btnWidth;
        }

        public virtual bool needShow { get { return true; } }
        public string inputText { get { return _m_sInputText; } }//当前输入文本

        //具体的gui绘制函数
        public void onGUI()
        {
            //输出文本信息
            GUILayout.Label(_m_sText, GUILayout.Height(_m_iHeight));
            //输入框 + 按钮 并排
            GUILayout.BeginHorizontal();
            _m_sInputText = GUILayout.TextField(_m_sInputText, GUILayout.Height(_m_iHeight));
            if(GUILayout.Button(_m_sConfirmStr, GUILayout.Height(_m_iHeight), GUILayout.Width(_m_iBtnHWidth)))
            {
                //按钮点击触发回调
                if (null != _m_dealDone)
                {
                    _m_dealDone(_m_sInputText);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
