using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEditor;

using UnityEngine.Networking;

/******************
 * 资源管理对象的通用函数处理类
 **/
namespace UTGame
{
    public class UTAssetBundleCommon
    {
        private static BuildTarget[] _g_arrAllBuildTarget = { BuildTarget.Android, BuildTarget.iOS, BuildTarget.StandaloneWindows };

        /*****************
         * 检测相关黄静配置是否创建
         **/
        public static void checkEnv()
        {
            for(int i = 0; i < _g_arrAllBuildTarget.Length; i++)
                checkEnv(_g_arrAllBuildTarget[i]);
        }
        public static void checkEnv(BuildTarget _buildTarget)
        {
            //检查对应路径的目录是否存在，不存在则创建目录
            if(!Directory.Exists(Application.dataPath + "/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if(!Directory.Exists(Application.dataPath + "/Resources/__ALAssetBundle"))
                AssetDatabase.CreateFolder("Assets/Resources", "__ALAssetBundle");
            if(!Directory.Exists(Application.dataPath + "/Resources/__ALAssetBundle/" + _buildTarget.ToString()))
                AssetDatabase.CreateFolder("Assets/Resources/__ALAssetBundle", _buildTarget.ToString());

            //还需要初始化相关导出资源的版本信息控制对象
        }

        /**************
         * 获取导出的相关路径信息
         **/
        public static string getAssetExportFolderFullPath(BuildTarget _buildTarget)
        {
            return "Assets/Resources/__ALAssetBundle/" + _buildTarget.ToString();
        }
        public static string getAssetExportFullPath(BuildTarget _buildTarget, string _assetName)
        {
            if(_assetName.StartsWith("/"))
                return "Assets/Resources/__ALAssetBundle/" + _buildTarget.ToString() + _assetName;
            else
                return "Assets/Resources/__ALAssetBundle/" + _buildTarget.ToString() + "/" + _assetName;
        }
        public static string getAssetExportFolderAssetPath(BuildTarget _buildTarget)
        {
            return "Resources/__ALAssetBundle/" + _buildTarget.ToString();
        }
        public static string getAssetExportAssetPath(BuildTarget _buildTarget, string _assetName)
        {
            if(_assetName.StartsWith("/"))
                return "Resources/__ALAssetBundle/" + _buildTarget.ToString() + _assetName;
            else
                return "Resources/__ALAssetBundle/" + _buildTarget.ToString() + "/" + _assetName;
        }

        /**************
         * 获取导出的相关压缩路径信息
         **/
        public static string getAssetExportCompressFolderFullPath(BuildTarget _buildTarget)
        {
            return "Assets/Resources/__ALAssetBundleCompress/" + _buildTarget.ToString();
        }
        public static string getAssetExportCompressFullPath(BuildTarget _buildTarget, string _assetName)
        {
            if(_assetName.StartsWith("/"))
                return "Assets/Resources/__ALAssetBundleCompress/" + _buildTarget.ToString() + _assetName;
            else
                return "Assets/Resources/__ALAssetBundleCompress/" + _buildTarget.ToString() + "/" + _assetName;
        }
        public static string getAssetExportCompressFolderAssetPath(BuildTarget _buildTarget)
        {
            return "Resources/__ALAssetBundleCompress/" + _buildTarget.ToString();
        }
        public static string getAssetExportCompressAssetPath(BuildTarget _buildTarget, string _assetName)
        {
            if(_assetName.StartsWith("/"))
                return "Resources/__ALAssetBundleCompress/" + _buildTarget.ToString() + _assetName;
            else
                return "Resources/__ALAssetBundleCompress/" + _buildTarget.ToString() + "/" + _assetName;
        }

        /*******************
         * 根据带入的资源名称，删除对应的资源导出文件
         **/
        public static void removeExportFile(string _assetName)
        {
            if(null == _assetName || _assetName.Length <= 0)
                return;

            //删除资源文件和manifest文件
            for(int i = 0; i < _g_arrAllBuildTarget.Length; i++)
                removeExportFile(_g_arrAllBuildTarget[i], _assetName);
        }
        public static void removeExportFile(BuildTarget _buildTarget, string _assetName)
        {
            if(null == _assetName || _assetName.Length <= 0)
                return;

            //删除资源文件和manifest文件
            AssetDatabase.DeleteAsset(getAssetExportFullPath(_buildTarget, _assetName));
            AssetDatabase.DeleteAsset(getAssetExportFullPath(_buildTarget, _assetName) + ".manifest");
        }

        /********************
         * 输出一个unity文件中的包含对象列表
         **/
        public static void printUnity3DFileList()
        {
            //选择需要输出的文件对象
            string objPath = EditorUtility.OpenFilePanel("Select Unity File Path", "", "");

            if(null == objPath || objPath.Length <= 0)
                return;

            AssetBundle assetBundle = AssetBundle.LoadFromFile(objPath);

            UnityEngine.Object[] objsList = assetBundle.LoadAllAssets();
            if(null != objsList)
            {
                //输出日志
                UnityEngine.Debug.LogError("Loaded Asset: " + objPath + " contain total: " + objsList.Length + " objects.");

                StringBuilder strBuilder = new StringBuilder();
                for(int i = 0; i < objsList.Length; i++)
                {
                    //输出对象名
                    strBuilder.Append(objsList[i].name + " - " + objsList[i].GetType())
                        .Append("\n");
                }

                //输出日志
                UnityEngine.Debug.LogError("Asset: " + objPath + " objects' name: " + strBuilder.ToString());
            }

            //卸载资源
            assetBundle.Unload(true);
        }

        /**************
         * 设置对应路径的资源对象的导出资源信息
         **/
        public static void setAssetBundleName(string _assetPath, string _assetBundleName)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath + ".asset");
            if(null == assetImporter)
            {
                UnityEngine.Debug.LogError("get AssetImporter is null: " + _assetPath);
                return;
            }

            assetImporter.assetBundleName = _assetBundleName;
        }
        public static void setAssetBundleName(string _assetPath, string _assetBundleName, string _variantName)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath + ".asset");
            if(null == assetImporter)
            {
                UnityEngine.Debug.LogError("get AssetImporter is null: " + _assetPath);
                return;
            }

            assetImporter.assetBundleName = _assetBundleName;
            assetImporter.assetBundleVariant = _variantName;
        }

        public static void setFileAssetBundleName(string _fileAssetPath, string _assetBundleName)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(_fileAssetPath);
            if (null == assetImporter)
            {
                UnityEngine.Debug.LogError("get AssetImporter is null: " + _fileAssetPath);
                return;
            }

            assetImporter.assetBundleName = _assetBundleName;
        }
        public static void setFileAssetBundleName(string _fileAssetPath, string _assetBundleName, string _variantName)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(_fileAssetPath);
            if (null == assetImporter)
            {
                UnityEngine.Debug.LogError("get AssetImporter is null: " + _fileAssetPath);
                return;
            }

            assetImporter.assetBundleName = _assetBundleName;
            assetImporter.assetBundleVariant = _variantName;
        }

        /**************
         * 资源版本号另存为 version_num.txt到_ALAssetBundle目录下
         **/
        public static void CreateVersionNumTxtFile(long _versionNum)
        {
            string path = Application.dataPath + "/Resources/__ALAssetBundle/version_num.txt";
            if(File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, _versionNum.ToString());
        }
    }
}
