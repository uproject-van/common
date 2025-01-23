using System;

namespace UTGame
{
	//第一个类型是模板类,  第二个类型是实际游戏中运用的类,  第三个类型是表的集合类
	public class _TUTAutoExportRefMenu<Tobj, TMap> : UTBaseAutoExportMenuItem<Tobj, TMap> where Tobj : _IUTBaseRefObj, new() where TMap : _TUTSOBaseRefSet<Tobj>, new()
	{
	    private string _m_sMenuStr;

	    public _TUTAutoExportRefMenu(EUTExportSettingEnum _exprotEnum, string _assetName, string _tag, string
			    _menuText,
		    Func<string, string, bool> _judgeCanShowFunc)
		    : base(_exprotEnum, _assetName, _tag, _judgeCanShowFunc)
	    {
	        _m_sMenuStr = _menuText;
	    }

	    /****************
	     * 显示的菜单文字
	     **/
	    protected override string _menuText { get { return _m_sMenuStr; } }
	}

}