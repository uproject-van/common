using System.Collections.Generic;

using UnityEngine;

/******************
 * excel文件读取通用函数对象
 **/
namespace UTGame
{
    [System.Serializable]
    public struct UTExportData
    {
        public string key;
        public string value;
        public UTExportData(string _key,string _value)
        {
            key = _key;
            value = _value;
        }
    }
    /********************
     * 导出部分数据存储对象
     **/
    [System.Serializable]
    public class UTSOExportData : ScriptableObject
    {
        /** 对应值的存储队列 */
        public List<UTExportData> valueList;
    }
}
