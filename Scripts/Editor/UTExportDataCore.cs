using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/******************
 * excel文件读取通用函数对象
 **/
namespace UTGame
{
    /***************
     * 导出数据中心
     **/
    public class UTExportDataCore
    {
        public const string xmlRootPathKey = "XmlRootPath"; //xml根目录存储key

        private static UTExportDataCore _g_instance = new UTExportDataCore();

        public static UTExportDataCore instance
        {
            get
            {
                if (null == _g_instance)
                    _g_instance = new UTExportDataCore();
                return _g_instance;
            }
        }

        /** 对应值的存储对象 */
        private List<UTExportData> _m_lDataList;

        protected UTExportDataCore()
        {
            _m_lDataList = new List<UTExportData>();

            //读取已有数据
            UTSOExportData dataObj =
                AssetDatabase.LoadAssetAtPath("Assets/Resources/Refdata/__DLExportSetting.asset",
                    typeof(UTSOExportData)) as UTSOExportData;
            if (null != dataObj)
            {
                //创建新的存储对象
                _m_lDataList.AddRange(dataObj.valueList);
            }
            else
            {
                Debug.LogError("Can not find old setting");
            }
        }

        /// <summary>
        /// 重新加载__DLExportSetting到内存
        /// </summary>
        public void reload()
        {
            if (null == _m_lDataList)
                return;

            _m_lDataList.Clear();

            //读取已有数据
            UTSOExportData dataObj =
                AssetDatabase.LoadAssetAtPath("Assets/Resources/Refdata/__DLExportSetting.asset",
                    typeof(UTSOExportData)) as UTSOExportData;
            if (null != dataObj)
            {
                //创建新的存储对象
                _m_lDataList.AddRange(dataObj.valueList);
            }
            else
            {
                Debug.LogError("Can not find old setting");
            }
        }

        /// <summary>
        /// 获取根目录
        /// </summary>
        /// <returns></returns>
        public string getXmlRootPath()
        {
            return getValue(xmlRootPathKey);
        }

        /*****************
         * 获取对应的值
         **/
        public string getValue(string _key)
        {
            if (null == _m_lDataList)
                return "";

            for (int i = 0; i < _m_lDataList.Count; i++)
            {
                if (_m_lDataList[i].key == _key)
                    return _m_lDataList[i].value.Replace("#root#", Application.dataPath);
            }

            return "";
        }

        public List<string> getValueList(string _key)
        {
            if (null == _m_lDataList)
                return new List<string>();

            for (int i = 0; i < _m_lDataList.Count; i++)
            {
                if (_m_lDataList[i].key == _key)
                {
                    //拆分数据
                    string[] strs = _m_lDataList[i].value.Replace("#root#", Application.dataPath).Split('#');
                    return new List<string>(strs);
                }
            }

            return new List<string>();
        }

        /*****************
         * 设置对应的值
         **/
        public void setValue(string _key, string _value)
        {
            if (null == _m_lDataList)
                _m_lDataList = new List<UTExportData>();

            //设置对应的值
            for (int i = 0; i < _m_lDataList.Count; i++)
            {
                if (_m_lDataList[i].key == _key)
                {
                    _m_lDataList[i] = new UTExportData(_key, _value.Replace(Application.dataPath, "#root#"));
                    return;
                }
            }

            //找不到就新增
            _m_lDataList.Add(new UTExportData(_key, _value.Replace(Application.dataPath, "#root#")));
        }

        /*************
         * 保存相关设置
         **/
        public void save()
        {
            UTSOExportData dataObj = ScriptableObject.CreateInstance<UTSOExportData>();
            //设置数据
            dataObj.valueList = new List<UTExportData>();
            dataObj.valueList.AddRange(_m_lDataList);
            //保存数据
            AssetDatabase.CreateAsset(dataObj, "Assets/Resources/Refdata/__DLExportSetting.asset");
        }
    }
}
