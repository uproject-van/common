using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UTGame
{
	/// <summary>
	/// 保存信息在本地
	/// </summary>
	public class UTExportSettingMgr 
	{
	    private static string exportSettingKey = "cache_export_setting_";
	    private static UTExportSettingMgr _g_instance;
	    public static UTExportSettingMgr instance 
	    {
	        get
	        {
	            if (null == _g_instance)
	                _g_instance = new UTExportSettingMgr();
	            return _g_instance;
	        }
	    }

	    private bool _m_bIsSelectAll;//是否全选
	    private List<NPExportSettingData> _m_lExportSettingList;//所有单项导出的配置数据


	    public Action<bool> onSelectAllChg; //全选值改变
	    public Action<EUTExportSettingEnum, bool> onSubExportMenuSelectChg;//单项选择改变

	    //没有意义的临时 变量
	    private NPExportSettingData tmpSettingData = null;

	    public UTExportSettingMgr()
	    {
	        _m_lExportSettingList = new List<NPExportSettingData>();
	        _m_bIsSelectAll = true;
	    }

	    /// <summary>注册子导出菜单功能的设置 </summary>
	    public void regSubExportSetting (EUTExportSettingEnum _type, Action _action, bool _isOn, string _tag, Func<string, string, bool> _judgeCanShowFunc) {
	        tmpSettingData = getExportSettingData(_type);
	        if (tmpSettingData == null) {
	            _m_lExportSettingList.Add(new NPExportSettingData(_type, _action, _isOn, _tag, _judgeCanShowFunc));
	        }
	        else
	        {
	            for (int i = 0; i < _m_lExportSettingList.Count; i++)
	            {
	                if (_type == _m_lExportSettingList[i].type)
	                {
	                    _m_lExportSettingList[i].setInfo(_type, _action, _isOn, _tag, _judgeCanShowFunc);
	                    break;
	                }
	            }
	        }
	    }

	    public void resetData () {
	        _m_bIsSelectAll = false;
	        for (int i = 0; i < _m_lExportSettingList.Count; ++i) {
	            _m_lExportSettingList[i].setIsSelect(false);
	        }
	        _m_bIsSelectAll = false;
	    }

	    //加载缓存的配置信息
	    public void loadLocalSave()
	    {

	        string cacheStr = PlayerPrefs.GetString(exportSettingKey);
	        if (string.IsNullOrEmpty(cacheStr))
	            return;
	        //Debug.LogError("loadLocalSave:  " + cacheStr);
	        //第一个值是 全选的状态值
	        int idx = cacheStr.IndexOf('|');
	        if (idx < 1)
	            return;
	        int selectAll = 0;
	        if (!int.TryParse(cacheStr.Substring(0, idx), out selectAll)) {
	            return;
	        }

	        //设置全选
	        setIsSelectAll(selectAll > 0);

	        //已选的单项的枚举列表
	        string[] strs = cacheStr.Substring(idx + 1).Split(';');
	        for (int i = 0; i < strs.Length; ++i)
	        {
	            int enumValue = 0;
	            if(!int.TryParse(strs[i], out enumValue))
	                continue;

	            //根据枚举获取配置的数据，设置为当前值
	            setSubExportIsSelect((EUTExportSettingEnum)enumValue, true);
	        }
	    }

	    //根据枚举获取对应的导出配置数据
	    public NPExportSettingData getExportSettingData (EUTExportSettingEnum _type) {
	        for (int i = 0; i < _m_lExportSettingList.Count; ++i) {
	            tmpSettingData = _m_lExportSettingList[i];
	            if(tmpSettingData == null || tmpSettingData.type != _type)
	                continue;
	            return tmpSettingData;
	        }
	        return null;
	    }

	    //1|2;3;4
	    //本地缓存，只缓存 全选和已经勾选的单项导出的枚举
	    public void localSave ()
	    {
	        StringBuilder sb = new StringBuilder();
	        sb.Append(_m_bIsSelectAll ? "1" : "0");
	        sb.Append("|");
	        int count = 0;
	        for (int i = 0; i < _m_lExportSettingList.Count; ++i) 
	        {
	            tmpSettingData = _m_lExportSettingList[i];
	            //过滤掉无效数据和没有勾选的数据
	            if(tmpSettingData == null || !tmpSettingData.isSelect)
	                continue;
	            if (count > 0)
	                sb.Append(";");
	            sb.Append((int)tmpSettingData.type);
	            ++count;
	        }
	        //UnityEngine.Debug.LogError(sb.ToString());
	        PlayerPrefs.SetString(exportSettingKey, sb.ToString());
	    }
   
	    //设置 全选
   
	    public void setIsSelectAll (bool _isOn) {
	        if(_m_bIsSelectAll == _isOn)
	            return;

	        _m_bIsSelectAll = _isOn;
	        //Debug.LogError("setIsSelectAll   :  " + _isOn);
	        for (int i = 0; i < _m_lExportSettingList.Count; ++i) {
	            tmpSettingData = _m_lExportSettingList[i];
	            if (tmpSettingData == null)
	                continue;
	            tmpSettingData.setIsSelect(_m_bIsSelectAll);
	        }
        
	        if(onSelectAllChg != null)
	            onSelectAllChg(_m_bIsSelectAll);

	        localSave();
	    }

	    //设置单项导出是否被选中
	    public void setSubExportIsSelect (EUTExportSettingEnum _type, bool _isOn) {
	        tmpSettingData = getExportSettingData(_type);
	        if (tmpSettingData == null)
	            return;
	        if (tmpSettingData.setIsSelect(_isOn)) {
	            onSubExportMenuSelectChg(_type, tmpSettingData.isSelect);
	            localSave();
	        }
	    }

	    //执行 全部导出
	    public void exeExportAllSelect () {
	        for (int i = 0; i < _m_lExportSettingList.Count; ++i) {
	            tmpSettingData = _m_lExportSettingList[i];
	            //过滤掉无效数据和没有勾选的数据
	            if (tmpSettingData == null || !tmpSettingData.isSelect)
	                continue;
	            tmpSettingData.exportAction();
	        }
	    }

	    //执行当前筛选的全部导出
	    public void exeExportCurSelectCanShow () {
	        for (int i = 0; i < _m_lExportSettingList.Count; ++i) {
	            tmpSettingData = _m_lExportSettingList[i];
	            //过滤掉无效数据和没有展示的数据
	            if (tmpSettingData == null || !tmpSettingData.needShow)
	                continue;
	            tmpSettingData.exportAction();
	        }
	    }
	}

	public class NPExportSettingData {
	    private EUTExportSettingEnum _m_eType;
	    private Action _m_dExportAction;
	    private bool _m_bIsSelect;
	    private string _m_sTag;
	    private Func<string, string, bool> _m_fJudgeCanShowFunc;

	    public EUTExportSettingEnum type { get { return _m_eType; } }

	    public bool isSelect { get { return _m_bIsSelect; } }

	    public Action exportAction { get { return _m_dExportAction; } }

	    public bool needShow
	    {
	        get
	        {
	            if (null == _m_fJudgeCanShowFunc)
	                return true;

	            return _m_fJudgeCanShowFunc(_m_sTag, _getExcelName());
	        }
	    }


	    public NPExportSettingData(EUTExportSettingEnum _enumType, Action _action, bool _isSelect, string _tag, Func<string, string, bool> _judgeCanShowFunc)
	    {
	        setInfo(_enumType, _action, _isSelect, _tag, _judgeCanShowFunc);
	    }

	    public void setInfo(EUTExportSettingEnum _enumType, Action _action, bool _isSelect, string _tag, Func<string, string, bool> _judgeCanShowFunc)
	    {
	        _m_eType = _enumType;
	        _m_dExportAction = _action;
	        _m_bIsSelect = _isSelect;
	        _m_sTag = _tag;
	        _m_fJudgeCanShowFunc = _judgeCanShowFunc;
	    }

	    public bool setIsSelect (bool _isOn) {
	        if(_m_bIsSelect == _isOn)
	            return false;
	        _m_bIsSelect = _isOn;
	        return true;
	    }

	    private string _getExcelName()
	    {
	        string path = UTExportDataCore.instance.getValue(_m_eType.ToString());
	        if (!string.IsNullOrEmpty(path))
	        {
	            string[] strs = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
	            if (strs.Length > 0)
	                return strs[strs.Length - 1];
	        }

	        return "";
	    }
	}
}