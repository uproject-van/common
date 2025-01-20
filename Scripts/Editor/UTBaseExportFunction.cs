using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace UTGame
{
    /**********************
     * 基本的导出处理函数
     **/
    public class UTBaseExportFunction
    {
        public static void exportXmlFile(XmlDocument _xmlDoc, string _xmlPath)
        {
            if (null == _xmlDoc)
            {
                UnityEngine.Debug.LogError("save a null xml document!");
                return;
            }

            //获取xml导出的根目录
            string xmlRootPath = UTExportDataCore.instance.getXmlRootPath();
            //路径无效则不继续处理
            if (null == xmlRootPath || xmlRootPath.Length <= 0)
                return;

            //总的路径
            string fullPath = xmlRootPath + "/" + _xmlPath;
            //获取文件夹位置
            string folderPath = fullPath.Substring(0, fullPath.LastIndexOf('/'));
            //判断文件夹是否存在，不存在则创建
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //保存文件
            _xmlDoc.Save(fullPath);
        }

        /*****************
         * 根据带入的对象以及导出路径将对象保存到对应路径，并返回最新的对象路径
         * 默认带入的路径格式为xxx/xxxx
         **/
        public static string exportAsset(UnityEngine.Object _assetObj, string _assetPath)
        {
            //获取完整路径，并根据文件夹进行创建操作
            string folderPath = _assetPath.Substring(0, _assetPath.LastIndexOf('/') + 1);

            //检查文件夹是否有效
            checkFolder(folderPath);

            //创建对象
            AssetDatabase.CreateAsset(_assetObj, "Assets/Resources/Refdata/__DLExport/__Client/" + _assetPath + ".asset");

            AssetDatabase.SaveAssets();

            //返回导出对象的asset路径
            return "Assets/Resources/Refdata/__DLExport/__Client/" + _assetPath;
        }
        /*****************
         * 导出对应对象，并设置对应对象的assetbundle路径信息
         **/
        public static void exportAsset(UnityEngine.Object _assetObj, string _assetPath, string _assetBundlePath)
        {
            string assetPath = exportAsset(_assetObj, _assetPath);

            //设置对应的assetbundle路径
            UTAssetBundleCommon.setAssetBundleName(assetPath, _assetBundlePath);
        }
        public static void exportAsset(UnityEngine.Object _assetObj, string _assetPath, string _assetBundlePath, string _varianName)
        {
            string assetPath = exportAsset(_assetObj, _assetPath);

            //设置对应的assetbundle路径
            UTAssetBundleCommon.setAssetBundleName(assetPath, _assetBundlePath.Replace("." + _varianName, ""), _varianName);
        }

        /********************
         * 检查对应文件夹是否存在
         * 默认带入文件夹路径格式为xxxx/xxxx
         **/
        public static void checkFolder(string _folderAssetPath)
        {
            //先检查基本设置
            checkEnv();

            string assetPath = "/Resources/Refdata/__DLExport/__Client/" + _folderAssetPath;
            if (Directory.Exists(Application.dataPath + assetPath))
                return;

            //逐个'/'符号匹配
            int startIndex = 21;

            while (startIndex > 0 && startIndex < assetPath.Length)
            {
                //检测下一个索引
                int nextIndex = assetPath.IndexOf('/', startIndex + 1);
                if (nextIndex < 0)
                    break;

                //获取父目录位置以及文件夹名称
                string fullAssetPath = assetPath.Substring(0, nextIndex);
                string parentPath = assetPath.Substring(0, startIndex);
                string folderName = assetPath.Substring(startIndex + 1, nextIndex - startIndex - 1);

                //检查对应文件夹位置是否创建完成
                if (!Directory.Exists(Application.dataPath + fullAssetPath))
                    AssetDatabase.CreateFolder("Assets" + parentPath, folderName);

                startIndex = nextIndex;
            }
        }

        //将字符串导出到文件
        static void WriteToTxtFile(string _path, System.Text.StringBuilder _str)
        {
            //如文件存在则删除
            if (File.Exists(_path))
                File.Delete(_path);

            //重新创建一个文件读写
            FileStream fs = new FileStream(_path, FileMode.CreateNew, FileAccess.ReadWrite);
            StreamWriter strmWriter = new StreamWriter(fs);    //存入到文本文件中

            strmWriter.Write(_str);

            strmWriter.Close();
            fs.Close();
            strmWriter.Dispose();
            fs.Dispose();
        }

        /********************
         * 检查导出的相关环境配置
         **/
        public static void checkEnv()
        {
            //检查对应文件夹位置是否创建完成
            if (!Directory.Exists(Application.dataPath + "/Resources/Refdata"))
                AssetDatabase.CreateFolder("Assets/Resources", "Refdata");
            if (!Directory.Exists(Application.dataPath + "/Resources/Refdata/__DLExport"))
                AssetDatabase.CreateFolder("Assets/Resources/Refdata", "__DLExport");
            if (!Directory.Exists(Application.dataPath + "/Resources/Refdata/__DLExport/__Client"))
                AssetDatabase.CreateFolder("Assets/Resources/Refdata/__DLExport", "__Client");
            if (!Directory.Exists(Application.dataPath + "/Resources/Refdata/__DLExport/__Server"))
                AssetDatabase.CreateFolder("Assets/Resources/Refdata/__DLExport", "__Server");
        }

        /******************
         * 读取对应的坐标信息
         **/
        public static Vector3 readVector3(string _str)
        {
            string[] strs = _str.Split(':');

            Vector3 vec = new Vector3(GCommon.ParseFloat(strs[0]), GCommon.ParseFloat(strs[1]), GCommon.ParseFloat(strs[2]));

            return vec;
        }

        /******************
         * 读取对应的索引队列信息
         **/
        public static List<T> readIndexList<T>(string _str, Func<T> _createDelegate) where T : UTBaseResIndexInfo
        {
            List<T> list = new List<T>();

            string[] strs = _str.Split(';');

            for (int i = 0; i < strs.Length; i++)
            {
                T tmpObj = _createDelegate();
                if (null == tmpObj)
                    continue;

                tmpObj.readIndex(strs[i]);
                list.Add(tmpObj);
            }

            return list;
        }

        /**********************
         * 将字符串按照位操作转化为对应的长整型数字。按照最高位到最低位的顺序。
         * 不够长度将自动在末尾按照0进行计算
         * 
         * @author alzq.z
         * @time   Aug 20, 2013 12:48:17 AM
         */
        public static long readStringLongValue(String _str)
        {
            long value = 0;

            for (int i = 0; i < 64; i++)
            {
                if (i < _str.Length)
                {
                    if (_str[i] == '0')
                    {
                        value = (value << 1);
                    }
                    else
                    {
                        value = ((value << 1) | 1);
                    }
                }
                else
                {
                    value = (value << 1);
                }
            }

            return value;
        }

        /// <summary>
        /// 数据深拷贝处理
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        public static object deepCopy(object _obj)
        {
            object retval = Activator.CreateInstance(_obj.GetType());
            FieldInfo[] fields = _obj.GetType().GetFields();
            foreach(var field in fields)
            {
                try
                {
                    field.SetValue(retval, _justCopy(field.GetValue(_obj)));
                }
                catch { }
            }

            return retval;
        }
        protected static object _justCopy(object _obj)
        {
            object retval = Activator.CreateInstance(_obj.GetType());
            FieldInfo[] fields = _obj.GetType().GetFields();

            if(fields.Length == 0)
            {
                return _obj;
            }

            foreach(var field in fields)
            {
                try
                {
                    field.SetValue(retval, field.GetValue(_obj));
                }
                catch { }
            }

            return retval;
        }
        public static object deepCopy(object _obj, object _targetObj)
        {
            FieldInfo[] fields = _obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach(var field in fields)
            {
                try
                {
                    field.SetValue(_targetObj, _justCopy(field.GetValue(_obj)));
                }
                catch { }
            }

            return _targetObj;
        }
    }
}

