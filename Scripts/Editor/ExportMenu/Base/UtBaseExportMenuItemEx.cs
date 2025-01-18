using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UTGame;
using Excel;


namespace UTGame
{
	public abstract class UtBaseExportMenuItemEx : _AUTBaseExportMenuItem {

	    protected EUTExportSettingEnum exportEnum;
	    protected int line;
	    protected Dictionary<string, string> lineValue;

	    //字符串标记
	    private string _m_sTag;
	    //判断是否可以显示的处理函数
	    private Func<string,string, bool> _m_fJudgeCanShowFunc;

	    public UtBaseExportMenuItemEx(string _tag, EUTExportSettingEnum _exportEnum, Func<string,string, bool> _judgeCanShowFunc)
	    {
	        _m_sTag = _tag;
	        _m_fJudgeCanShowFunc = _judgeCanShowFunc;
	        exportEnum = _exportEnum;

	        UTExportSettingMgr.instance.onSelectAllChg += _onSelectAllChg;
	        UTExportSettingMgr.instance.onSubExportMenuSelectChg += _onSubExportMenuSelectChg;

	        if(_m_siSelectItem != null)
	            _m_siSelectItem.regSelectFunc((isOn) => UTExportSettingMgr.instance.setSubExportIsSelect(exportEnum, isOn));
	    }

	    //是否需要显示
	    public override bool needShow
	    {
	        get
	        {
	            if (null == _m_fJudgeCanShowFunc)
	                return true;

	            return _m_fJudgeCanShowFunc(_m_sTag, _getExcelName());
	        }
	    }

	    //父菜单的toggle改变事件
	    public void _onSelectAllChg (bool _isOn) {
	        _m_siSelectItem.setToggleValue(_isOn);
	    }

	    //子菜单的toggle改变事件
	    public virtual void _onSubExportMenuSelectChg (EUTExportSettingEnum _type, bool _isOn) {
	        if (_type != exportEnum)
	            return;
	        _m_siSelectItem.setToggleValue(_isOn);
	    }

	    private string _getExcelName()
	    {
	        string path = UTExportDataCore.instance.getValue(exportEnum.ToString());
	        if (!string.IsNullOrEmpty(path))
	        {
	            string[] strs = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
	            if (strs.Length > 0)
	                return strs[strs.Length - 1];
	        }

	        return "";
	    }

	    protected byte GetByte (string key) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return 0;
	        }

	        if (string.IsNullOrEmpty(strValue))
	            return 0;

	        byte tempValue = 0;
	        if (!byte.TryParse(strValue, out tempValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3},填的不是整数", GetType().FullName, line, key, strValue));
	        }
	        return tempValue;
	    }

	    protected short GetShort(string key)
	    {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue))
	        {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return 0;
	        }

	        if (string.IsNullOrEmpty(strValue))
	            return 0;

	        short tempValue = 0;
	        if (!short.TryParse(strValue, out tempValue))
	        {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3},填的不是Short", GetType().FullName, line, key, strValue));
	        }
	        return tempValue;
	    }

	    protected int GetInt (string key) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return 0;
	        }

	        if (string.IsNullOrEmpty(strValue))
	            return 0;

	        int tempValue = 0;
	        if (!int.TryParse(strValue, out tempValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3},填的不是整数", GetType().FullName, line, key, strValue));
	        }
	        return tempValue;
	    }
	    
	    protected long GetLong (string key) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return 0;
	        }

	        if (string.IsNullOrEmpty(strValue))
	            return 0;

	        long tempValue = 0;
	        long.TryParse(strValue, out tempValue);
	        return tempValue;
	    }

	    protected float GetFloat (string key) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return 0f;
	        }

	        if (string.IsNullOrEmpty(strValue)) {
	            return 0;
	        }

	        float tempValue = 0;
	        if (!float.TryParse(strValue, out tempValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3},填的不是浮点数", GetType().FullName, line, key, strValue));
	        }
	        return tempValue;
	    }

	    protected bool GetBool (string key) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return false;
	        }

	        if (string.IsNullOrEmpty(strValue)) {
	            return false;
	        }

	        bool tempValue = false;
	        if (!bool.TryParse(strValue, out tempValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	        }
	        return tempValue;
	    }

	    protected string GetString (string key, bool _errorLog = true) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            if(_errorLog)
	                Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return "";
	        }

	        if (string.IsNullOrEmpty(strValue)) {
	            return "";
	        }
	        return strValue;
	    }
	    protected string GetSolidString(string key)
	    {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue))
	        {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return "";
	        }

	        if (string.IsNullOrEmpty(strValue))
	        {
	            return "";
	        }
	        return strValue.Replace(" ", "");
	    }

	    protected object GetEnum (string key, Type _type) {
	        string strValue = "";
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return Activator.CreateInstance(_type);
	        }
	        if (strValue == string.Empty || strValue == "") {
	            return Activator.CreateInstance(_type);
	        }
	        try {
	            return Enum.Parse(_type, strValue.Trim(), true);
	        }
	        catch {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return Activator.CreateInstance(_type);
	        }
	    }

	    protected List<T> GetList<T> (string key) {
	        string strValue = "";
	        List<T> list = new List<T>();
	        if (!lineValue.TryGetValue(key.ToLower(), out strValue)) {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	            return list;
	        }

	        if (!strValue.Trim().Equals("")) {
	            string[] strs = strValue.Split(new char[] { ';', ':' });
	            foreach (string tempStr in strs) {
	                object value = ParseValue(tempStr, typeof(T));
	                if (value == null) {
	                    Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误: {3}", GetType().FullName, line, key, strValue));
	                }
	                else {
	                    list.Add((T)value);
	                }
	            }
	        }

	        return list;
	    }

	    protected Rect GetRect(string key)
	    {
	        Rect rect = new Rect();
	        string strValue = "";
	        if(!lineValue.TryGetValue(key.ToLower(), out strValue))
	        {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误，格式 [x;y;width;height] : {3}", GetType().FullName, line, key, strValue));
	            return rect;
	        }

	        if(!strValue.Trim().Equals(""))
	        {
	            string[] strs = strValue.Split(new char[] { ';', ':' });
	            if(strs.Length < 4)
	            {
	                Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误，格式 [x;y;width;height] : {3}", GetType().FullName, line, key, strValue));
	                return rect;
	            }

	            rect.Set(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]), float.Parse(strs[3]));
	        }

	        return rect;
	    }
    
	    protected Vector2 GetVector2(string key)
	    {
	        Vector2 vector2 = new Vector2();
	        string strValue = "";
	        if(!lineValue.TryGetValue(key.ToLower(), out strValue))
	        {
	            Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误，格式 [width:height] : {3}", GetType().FullName, line, key, strValue));
	            return vector2;
	        }

	        if(!strValue.Trim().Equals(""))
	        {
	            string[] strs = strValue.Split(new char[] {':',';' });
	            if(strs.Length < 2)
	            {
	                Debug.LogError(string.Format("表\"{0}\"：第{1}行第{2}列数据填写错误，格式 [width:height] : {3}", GetType().FullName, line, key, strValue));
	                return vector2;
	            }

	            vector2.Set(float.Parse(strs[0]), float.Parse(strs[1]));
	        }

	        return vector2;
	    }
    

	    // 解析字段值
	    protected static object ParseValue (string _value, Type _type) {
	        try {
	            if (_value.Equals(string.Empty)) {
	                if (_type == typeof(string)) {
	                    return "";
	                }
	                return Activator.CreateInstance(_type);
	            }
	            else {
	                _value = _value.Trim();

	                // 枚举 暂不支持
	                if (_type.IsEnum) {
	                    return Enum.Parse(_type, _value, true);
	                }

	                // 字符串
	                else if (_type == typeof(string)) {
	                    return _value;
	                }

	                // 浮点型
	                else if (_type == typeof(float)) {
	                    if (_value == "0" || _value == "" || _value == string.Empty)
	                        return 0f;

	                    return float.Parse(_value);
	                }

	                // 整形
	                else if (_type == typeof(int)) {
	                    if (_value == "")
	                        return 0;

	                    return int.Parse(_value);
	                }

	                else if (_type == typeof(bool)) {
	                    return bool.Parse(_value);
	                }

	                else if (_type == typeof(long)) {
	                    return long.Parse(_value);
	                }
	            }
	        }
	        catch (System.Exception ex) {
	            Debug.LogError(string.Format("ParseValue type:{0}, value:{1}, failed: {2}", _type.ToString(), _value, ex.Message));
	        }
	        return null;
	    }
	}

}