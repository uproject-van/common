using System;
using UnityEngine;

namespace UTGame
{
	public class UTTextButtonItem : _IUTExportMenuInterface
	{
	    private string _m_sText;
	    private int _m_iHeight;
	    private string _m_sBtnText;
	    private Action _m_dDelegate;

	    public UTTextButtonItem(string _text, string _textBtn, Action _delegate)
	    {
	        _m_sText = _text;
	        _m_iHeight = 10;
	        _m_sBtnText = _textBtn;
	        _m_dDelegate = _delegate;
	    }

	    public UTTextButtonItem(string _text, int _height, string _textBtn, Action _delegate)
	    {
	        _m_sText = _text;
	        _m_iHeight = _height;
	        _m_sBtnText = _textBtn;
	        _m_dDelegate = _delegate;
	    }

	    public virtual bool needShow { get { return true; } }

	    //具体的gui绘制函数
	    public void onGUI()
	    {
	        //开始横向排布
	        GUILayout.BeginHorizontal();

	        //输出文本信息
	        GUILayout.Label(_m_sText, GUILayout.Height(_m_iHeight));

	        //按钮
	        if (GUILayout.Button(_m_sBtnText, GUILayout.Height(_m_iHeight + 10), GUILayout.Width(40)))
	        {
	            if (null != _m_dDelegate)
	                _m_dDelegate();
	        }

	        GUILayout.FlexibleSpace();

	        //结束横向排布
	        GUILayout.EndHorizontal();
	    }
	}
}