using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTGame
{
	public class UTInputTabData {
	    private static UTInputTabData _g_instance = new UTInputTabData();
	    public static UTInputTabData instance
	    {
	        get
	        {
	            if (null == _g_instance)
	                _g_instance = new UTInputTabData();
	            return _g_instance;
	        }
	    }

	    private List<UTExportData> textList = new List<UTExportData>();

	    protected UTInputTabData () {
	        //读取已有数据
	        UTSOExportData dataObj = AssetDatabase.LoadAssetAtPath("Assets/Resources/Refdata/__DLInputSetting.asset", typeof(UTSOExportData)) as UTSOExportData;
	        if (null != dataObj) {
	            //创建新的存储对象
	            textList = new List<UTExportData>(dataObj.valueList);
	        }
	        else {
	            Debug.LogError("Can not find old setting");
	        }
	    }

	    public void setValue(string _key, string _value)
	    {
	        for (int i = 0; i < textList.Count; i++)
	        {
	            if(textList[i].key == _key)
	            {
	                textList[i] = new UTExportData(_key, _value);
	                return;
	            }
	        }
	        textList.Add(new UTExportData(_key, _value));
	    }

	    public string getValue(string _key)
	    {
	        if (null == textList)
	            return "";

	        for (int i = 0; i < textList.Count; i++)
	        {
	            if (textList[i].key == _key)
	                return textList[i].value;
	        }
	        return "";
	    }

	    public void save () {

	        UTSOExportData dataObj = ScriptableObject.CreateInstance<UTSOExportData>();
	        //设置数据
	        dataObj.valueList = new List<UTExportData>();
	        dataObj.valueList.AddRange(textList);

	        //保存数据
	        AssetDatabase.CreateAsset(dataObj, "Assets/Resources/Refdata/__DLInputSetting.asset");
	    }
	}

	public class UTInputItem : _IUTExportMenuInterface {

	    private string key = "";
	    private string text = "";
	    private int height = 15;
	    private bool hasInit = false;

	    public UTInputItem (string _key, int _height) {
	        key = _key;
	        height = _height;
	    }

	    public virtual bool needShow { get { return true; } }

	    public void onGUI () {
	        if (!hasInit) {
	            hasInit = true;
	            text = UTInputTabData.instance.getValue(key);
	        }
	        string newValue = GUILayout.TextField(text, GUILayout.Height(height));
	        if (!newValue.Equals(text)) {
	            text = newValue;
	            //保存数据
	            UTInputTabData.instance.setValue(key, text);
	        }
	    }


	}
}