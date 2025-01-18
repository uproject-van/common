using UnityEngine;

namespace UTGame
{
    public class UTTextItem : _IUTExportMenuInterface
    {
        private string _m_sText;
        private int _m_iHeight;

        public UTTextItem(string _text)
        {
            _m_sText = _text;
            _m_iHeight = 10;
        }
        public UTTextItem(string _text, int _height)
        {
            _m_sText = _text;
            _m_iHeight = _height;
        }

        public virtual bool needShow { get { return true; } }

        //具体的gui绘制函数
        public void onGUI()
        {
            //输出文本信息
            GUILayout.Label(_m_sText, GUILayout.Height(_m_iHeight));
        }
    }
}
