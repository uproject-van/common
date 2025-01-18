using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using Excel;
using UnityEditor;
using UnityEngine;

namespace UTGame
{
    public class UTExportWnd : UTBaseExportWnd
    {
        private static string EXCEL_LINE_DELIMITER = "\t"; //行内分隔符
        private static string EXCEL_LINE = "\r\n"; //行间分隔符

        private UTInputFieldConfirmItem _m_tiSiftTextItem;
        private UTBaseConfirmItem _m_ciCopyRefdataItem;
        private UTBaseConfirmItem _m_ciExportSelectItem;

        public UTExportWnd()
            : base()
        {
            
            _m_ciCopyRefdataItem = new UTBaseConfirmItem("--- 移除 --- 拷贝refdata数据到__dlserverref,同时复制ai", () =>
            {
                //TODO ==== 导出给服务器使用的所有表数据
                //FireCreater.dlServerRef();

                EditorUtility.DisplayDialog("提示", "复制完成！", "确定");
            });

            _regMenuItem(_m_ciCopyRefdataItem);
            _m_tiSiftTextItem = new UTInputFieldConfirmItem("过滤名称(多个用;分隔开)：", "", "全部导出当前过滤的表", (_inputStr) =>
            {
                UTExportSettingMgr.instance.exeExportCurSelectCanShow();
                AssetDatabase.Refresh();
            }, 150, 30);
            _regMenuItem(_m_tiSiftTextItem);
            _regMenuItem(_m_ciExportSelectItem);
            
            if (_m_eaiExportAllItem != null)
                _m_eaiExportAllItem.regExportFunc(() =>
                {
                    UTExportSettingMgr.instance.exeExportAllSelect();
                    AssetDatabase.Refresh();
                });

            if (_m_saiSelectAllItem != null)
                _m_saiSelectAllItem.regSelectFunc(UTExportSettingMgr.instance.setIsSelectAll);

            _regMenuItem(new UTGeneralRefExportMenu("generalref", _judgeTagCanShow));//常量表

            UTExportSettingMgr.instance.onSelectAllChg += _onSelectAllChg;
            UTExportSettingMgr.instance.onSubExportMenuSelectChg += _onSubExportMenuSelectChg;
        }

        /** 判断标记是否可视 */
        protected bool _judgeTagCanShow(string _tag, string _excelFileName)
        {
            if (null == _m_tiSiftTextItem || string.IsNullOrEmpty(_m_tiSiftTextItem.inputText))
                return true;

            //按照分号分隔不同标记，判断标记或文件名是否包含
            string[] strs = _m_tiSiftTextItem.inputText.Split(new string[] { ";" }, StringSplitOptions.None);

            for (int i = 0; i < strs.Length; i++)
            {
                if (!string.IsNullOrEmpty(strs[i]) && (_tag.ToLower().Contains(strs[i].ToLower()) ||
                                                       _excelFileName.ToLower().Contains(strs[i].ToLower())))
                    return true;
            }

            return false;
        }

        private void _onSelectAllChg(bool _isOn)
        {
            _m_saiSelectAllItem.setToggleValue(_isOn);
        }

        //子导出菜单选择toggle改变事件处理
        private void _onSubExportMenuSelectChg(EUTExportSettingEnum _type, bool _isOn)
        {
        }

        [MenuItem("UTAssets/数据配表导出/打开导出窗口 %#J")]
        static void ResExportWnd()
        {
            //创建窗口
            Rect wr = new Rect(100, 100, 800, 800);
            showExportWnd<UTExportWnd>(wr, "UT Excel资源导出窗口");
            UTExportSettingMgr.instance.resetData();
            UTExportSettingMgr.instance.loadLocalSave();
        }

        /***********
         * excel读取的相关处理文件
         **/
        public class UTExcelReadInfo
        {
            private FileStream _m_fFileStream;
            private string _m_sSrcPath;
            private string _m_sTmpFilePath;
            private IExcelDataReader _m_iExcelReader;

            public UTExcelReadInfo()
            {
                _m_fFileStream = null;
                _m_sSrcPath = string.Empty;
                _m_sTmpFilePath = string.Empty;
                _m_iExcelReader = null;
            }

            public string srcPath { get { return _m_sSrcPath; } }
            public IExcelDataReader excelReader { get { return _m_iExcelReader; } }

            public void init(string _path)
            {
                _m_sSrcPath = _path;
                string exitension = Path.GetExtension(_path);
                //开启文件并进行读取
                _m_sTmpFilePath = _path.Replace(exitension, ".tmpX");
                //拷贝一个文件，避免开启excel时无法读取的问题
                File.Copy(_path, _m_sTmpFilePath, true);
                //开启文件并进行读取
                _m_fFileStream = File.OpenRead(_m_sTmpFilePath);
                _m_iExcelReader = ExcelReaderFactory.CreateOpenXmlReader(_m_fFileStream);
                Debug.Log("path: " + _path);
            }

            /*********
             * 释放相关资源
             **/
            public void discard()
            {
                if (null == _m_fFileStream)
                    return;

                //释放文件资源
                _m_fFileStream.Close();
                _m_fFileStream.Dispose();
                _m_iExcelReader.Dispose();
                _m_fFileStream = null;
                _m_iExcelReader = null;

                //删除拷贝的文件
                File.Delete(_m_sTmpFilePath);
                _m_sSrcPath = string.Empty;
                _m_sTmpFilePath = string.Empty;
            }
        }

        /******************
         * 读取excel文件的具体函数，带入的回调为对每个格子进行处理的处理函数
         * 回调带入的参数为 定义结构体对象，格子列名称，格子内容
         **/
        public static UTExcelReadInfo readExcel(string _path)
        {
            UTExcelReadInfo excelInfo = new UTExcelReadInfo();
            excelInfo.init(_path);

            return excelInfo;
        }

        /**************
         * 读取操作类
         **/
        public class UTExcelReadingFuncInfo<T> : _AUTExcelReadingBasicClass
        {
            public List<T> dataList;
            public Func<T> createDelegate;
            public Action<T, int, Dictionary<string, string>> readDelegate;

            public UTExcelReadingFuncInfo(string _sheetName,
                EUTExportSettingEnum _exportEnum,
                Func<T> _createDelegate,
                Action<T, int, Dictionary<string, string>> _readDelegate,
                bool _exportTxt = true)
                : base(_sheetName, _exportEnum, _exportTxt, new List<T>())
            {
                //将基类数据转化过来
                dataList = (List<T>)recList;
                exportEnum = _exportEnum;
                createDelegate = _createDelegate;
                readDelegate = _readDelegate;
                exportTxt = _exportTxt;
            }

            protected override object _createObj()
            {
                if (null == createDelegate)
                    return null;

                return createDelegate();
            }

            protected override void _readFunc(object _obj, int _lineIdx, Dictionary<string, string> _lineInfo)
            {
                if (null == readDelegate)
                    return;

                readDelegate((T)_obj, _lineIdx, _lineInfo);
            }
        }

        public abstract class _AUTExcelReadingBasicClass
        {
            public string sheetName;
            public EUTExportSettingEnum exportEnum;
            public bool exportTxt;
            public IList recList;

            public _AUTExcelReadingBasicClass(string _sheetName,
                EUTExportSettingEnum _exportEnum,
                bool _exportTxt,
                IList _recList)
            {
                sheetName = _sheetName;
                exportEnum = _exportEnum;
                exportTxt = _exportTxt;
                recList = _recList;
            }

            /*********
             * 处理读取操作
             **/
            public object dealReadingFunc(int _lineIdx, Dictionary<string, string> _lineInfo)
            {
                object newObj = _createObj();
                _readFunc(newObj, _lineIdx, _lineInfo);

                return newObj;
            }

            protected abstract object _createObj();
            protected abstract void _readFunc(object _obj, int _lineIdx, Dictionary<string, string> _lineInfo);
        }

        public class UTExcelReadingDealObj
        {
            private Dictionary<string, _AUTExcelReadingBasicClass> _m_dicDealDic;

            public UTExcelReadingDealObj()
            {
                _m_dicDealDic = new Dictionary<string, _AUTExcelReadingBasicClass>();
            }

            //添加处理对象
            public void addDealObj(_AUTExcelReadingBasicClass _dealObj)
            {
                if (null == _dealObj)
                    return;

                if (_m_dicDealDic.ContainsKey(_dealObj.sheetName.ToLower()))
                {
                    Debug.LogError("Multiplie deal sheet: " + _dealObj.sheetName);
                    return;
                }

                _m_dicDealDic.Add(_dealObj.sheetName.ToLower(), _dealObj);
            }

            /**********
             * 获取对应页签的处理对象
             **/
            public _AUTExcelReadingBasicClass getDealObj(string _sheetName)
            {
                if (!_m_dicDealDic.ContainsKey(_sheetName.ToLower()))
                    return null;

                return _m_dicDealDic[_sheetName.ToLower()];
            }
        }

        /******************
         * 读取excel文件的具体函数，带入的回调为对每个格子进行处理的处理函数
         * 回调带入的参数为 定义结构体对象，格子列名称，格子内容
         **/
        public static void readXls(string _path, UTExcelReadingDealObj _readingDealObj)
        {
            //判断带入的函数对象是否有效
            if (null == _readingDealObj)
                return;

            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            long s = GCommon.getNowTimeMill();
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("read file spend ms: " + (GCommon.getNowTimeMill() - s));
            s = GCommon.getNowTimeMill();
            Debug.Log("path: " + _path);

            do
            {
                //判断sheet name是否有读取对象
                //获取处理对象
                _AUTExcelReadingBasicClass readingObj = _readingDealObj.getDealObj(excelReader.Name);
                if (null == readingObj)
                    continue;

                //存储列名称,用于分析数据并进行读取
                List<string> columnNameList = new List<string>();
                int lineIdx = 0;
                //对表进行处理
                while (excelReader.Read())
                {
                    if (1 == lineIdx)
                    {
                        //取列名
                        for (int i = 0; i < excelReader.FieldCount; i++)
                        {
                            //添加列名称, 第一行不能有空列，空列意味着隔断
                            if (excelReader.IsDBNull(i))
                            {
                                break;
                            }
                            else
                            {
                                columnNameList.Add(excelReader.GetString(i));
                            }
                        }
                    }
                    else if (lineIdx > 1)
                    {
                        //进行行处理
                        Dictionary<string, string> lineData = new Dictionary<string, string>();
                        int keyCount = columnNameList.Count;

                        //记录本行是否有效
                        bool isLineEnable = false;

                        //当前行中的列数目，每列值进行处理
                        for (int i = 0; i < excelReader.FieldCount; i++)
                        {
                            if (i >= keyCount)
                                continue;

                            if (excelReader.IsDBNull(i))
                            {
                                lineData[columnNameList[i].ToLower()] = "";
                                continue;
                            }

                            //获取对应值，进行后续处理
                            string value = excelReader.GetString(i);
                            if (i == 1 && (null != value && value.StartsWith("--")))
                            {
                                //设置本行无效
                                isLineEnable = false;
                                break;
                            }

                            try
                            {
                                //处理读取操作
                                lineData[columnNameList[i].ToLower()] = value;
                                //设置本行有效
                                isLineEnable = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError(ex.Message + " Error Position:" + "[" + (i + 1) + "," + (lineIdx + 1) +
                                               "]" + "----name:" + columnNameList[i] + "   value:" + value);
                            }
                        }

                        //添加到结果集合
                        if (isLineEnable)
                        {
                            //创建新的接受数据节点
                            object dataObj = readingObj.dealReadingFunc(lineIdx, lineData);
                            readingObj.recList.Add(dataObj);
                        }
                    }

                    //增加行号
                    lineIdx++;
                }

                //导出Txt
                if (readingObj.exportTxt)
                    WriteToTxtFile(_path, excelReader.Name.ToLower(), readingObj.exportEnum);

                if (GCommon.getNowTimeMill() - s > 1000)
                    Debug.LogWarning("read sheet: " + excelReader.Name + " spend ms: " +
                                     (GCommon.getNowTimeMill() - s));
                else
                    Debug.Log("read sheet: " + excelReader.Name + " spend ms: " + (GCommon.getNowTimeMill() - s));
                s = GCommon.getNowTimeMill();
            } while (excelReader.NextResult());

            //释放文件资源
            stream.Close();
            stream.Dispose();
            excelReader.Dispose();
            stream = null;
            excelReader = null;

            //删除拷贝的文件
            File.Delete(tmpPath);
        }

        //获取页面列表
        public static List<string> getAllSheetNameList(string _path)
        {
            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            //创建结果队列
            List<string> resList = new List<string>();

            do
            {
                resList.Add(excelReader.Name);
            } while (excelReader.NextResult());

            return resList;
        }

        /******************
         * 读取excel文件的具体函数，带入的回调为对每个格子进行处理的处理函数
         * 回调带入的参数为 定义结构体对象，格子列名称，格子内容
         **/
        public static List<T> readXls<T>(string _path,
            string _sheetName,
            EUTExportSettingEnum _exportEnum,
            Action<T, int, Dictionary<string, string>> _readDelegate,
            bool _exportTxt = true)
        {
            //判断带入的函数对象是否有效
            if (null == _readDelegate)
                return null;
            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("path: " + _path + "\t\t_sheetName:  " + _sheetName);
            //创建结果队列
            List<T> resList = new List<T>();

            do
            {
                //判断sheet name是否data
                if (excelReader.Name.Equals(_sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Start Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                }
                            }

                            //判断是否有字段，不存在columnNameList中，有的话说明配表少了列，报错
                            //自动根据字段识别
                            Type TempCls = typeof(T);
                            //识别变量
                            FieldInfo[] fields = TempCls.GetFields();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                FieldInfo field = fields[i];
                                if (!columnNameList.Contains(field.Name))
                                {
                                    Debug.LogError($"表格{_sheetName}缺少列:{field.Name}");
                                }
                            }
                        }
                        else if (lineIdx > 1)
                        {
                            //进行行处理
                            //创建新的接受数据节点
                            T dataObj = Activator.CreateInstance<T>();

                            Dictionary<string, string> lineData = new Dictionary<string, string>();
                            int keyCount = columnNameList.Count;

                            //记录本行是否有效
                            bool isLineEnable = false;

                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                if (excelReader.IsDBNull(i))
                                {
                                    lineData[columnNameList[i].ToLower()] = "";
                                    continue;
                                }

                                //获取对应值，进行后续处理
                                string value = excelReader.GetString(i);
                                if (i == 1 && (null != value && value.StartsWith("--")))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                try
                                {
                                    //处理读取操作
                                    lineData[columnNameList[i].ToLower()] = value;
                                    //设置本行有效
                                    isLineEnable = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError(ex.Message + " Error Position:" + "[" + (i + 1) + "," +
                                                   (lineIdx + 1) + "]" + "----name:" + columnNameList[i] + "   value:" +
                                                   value);
                                }
                            }

                            //添加到结果集合
                            if (isLineEnable)
                            {
                                _readDelegate(dataObj, lineIdx, lineData);
                                resList.Add(dataObj);
                            }
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            //释放文件资源
            stream.Close();
            stream.Dispose();
            excelReader.Dispose();
            stream = null;
            excelReader = null;

            //删除拷贝的文件
            File.Delete(tmpPath);

            if (_exportTxt)
                WriteToTxtFile(_path, _sheetName, _exportEnum);

            if (_exportEnum == EUTExportSettingEnum.GENERAL)
            {
                exportGeneralText(_path, "general_txt", _exportEnum);
            }

            return resList;
        }

        public static List<T> readXls<T>(string _path,
            string[] _sheetName,
            EUTExportSettingEnum _exportEnum,
            Action<T, int, Dictionary<string, string>> _readDelegate,
            bool _exportTxt = true)
        {
            //判断带入的函数对象是否有效
            if (null == _readDelegate || null == _sheetName || _sheetName.Length <= 0)
                return null;

            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("path: " + _path + "\t\t_sheetName:  " + _sheetName);
            //创建结果队列
            List<T> resList = new List<T>();

            do
            {
                string sheetName = excelReader.Name;

                //判断sheet name是否data
                bool fix = false;
                for (int i = 0; i < _sheetName.Length; i++)
                {
                    string tmpName = _sheetName[i];
                    if (null == tmpName)
                        continue;

                    if (sheetName.Equals(tmpName, StringComparison.OrdinalIgnoreCase))
                    {
                        fix = true;
                        break;
                    }
                }

                //判断是否读取
                if (fix)
                {
                    Debug.Log("Start Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                }
                            }

                            //判断是否有字段，不存在columnNameList中，有的话说明配表少了列，报错
                            //自动根据字段识别
                            Type TempCls = typeof(T);
                            //识别变量
                            FieldInfo[] fields = TempCls.GetFields();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                FieldInfo field = fields[i];
                                if (!columnNameList.Contains(field.Name))
                                {
                                    Debug.LogError($"表格{_sheetName}缺少列:{field.Name}");
                                }
                            }
                        }
                        else if (lineIdx > 1)
                        {
                            //进行行处理
                            //创建新的接受数据节点
                            T dataObj = Activator.CreateInstance<T>();

                            Dictionary<string, string> lineData = new Dictionary<string, string>();
                            int keyCount = columnNameList.Count;

                            //记录本行是否有效
                            bool isLineEnable = false;

                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                if (excelReader.IsDBNull(i))
                                {
                                    lineData[columnNameList[i].ToLower()] = "";
                                    continue;
                                }

                                //获取对应值，进行后续处理
                                string value = excelReader.GetString(i);
                                if (i == 1 && (null != value && value.StartsWith("--")))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                try
                                {
                                    //处理读取操作
                                    lineData[columnNameList[i].ToLower()] = value;
                                    //设置本行有效
                                    isLineEnable = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError(ex.Message + " Error Position:" + "[" + (i + 1) + "," +
                                                   (lineIdx + 1) + "]" + "----name:" + columnNameList[i] + "   value:" +
                                                   value);
                                }
                            }

                            //添加到结果集合
                            if (isLineEnable)
                            {
                                _readDelegate(dataObj, lineIdx, lineData);
                                resList.Add(dataObj);
                            }
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            //释放文件资源
            stream.Close();
            stream.Dispose();
            excelReader.Dispose();
            stream = null;
            excelReader = null;

            //删除拷贝的文件
            File.Delete(tmpPath);

            if (_exportTxt)
                WriteToTxtFile(_path, _sheetName[0], _exportEnum);

            if (_exportEnum == EUTExportSettingEnum.GENERAL)
            {
                exportGeneralText(_path, "general_txt", _exportEnum);
            }

            return resList;
        }

        /// <summary>
        /// 自动根据模板类的相关参数反射信息进行智能解析的处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_path"></param>
        /// <param name="_sheetName"></param>
        /// <param name="_exportEnum"></param>
        /// <param name="_readDelegate"></param>
        /// <param name="_exportTxt"></param>
        /// <returns></returns>
        public static List<T> autoReadXls<T>(string _path,
            string _sheetName,
            EUTExportSettingEnum _exportEnum,
            bool _exportTxt = true)
        {
            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("path: " + _path + "\t\t_sheetName:  " + _sheetName);
            //创建结果队列
            List<T> resList = new List<T>();

            do
            {
                //判断sheet name是否data
                if (excelReader.Name.Equals(_sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Start Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                }
                            }

                            //判断是否有字段，不存在columnNameList中，有的话说明配表少了列，报错
                            //自动根据字段识别
                            Type TempCls = typeof(T);
                            //识别变量
                            FieldInfo[] fields = TempCls.GetFields();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                FieldInfo field = fields[i];
                                UTAutoExportVariableAttr attr = null;
                                using (IEnumerator<UTAutoExportVariableAttr> attrIEnumerator =
                                       field.GetCustomAttributes<UTAutoExportVariableAttr>().GetEnumerator())
                                {
                                    if (attrIEnumerator.MoveNext())
                                        attr = attrIEnumerator.Current;
                                }

                                // 如果是可以为空和忽略的不用检查
                                if (attr != null && (attr.canBeEmpty || attr.ignoreReading))
                                    continue;

                                if (!columnNameList.Contains(field.Name))
                                {
                                    Debug.LogError($"表格{_sheetName}缺少列:{field.Name}");
                                }
                            }
                        }
                        else if (lineIdx > 1)
                        {
                            Dictionary<string, string> lineData = new Dictionary<string, string>();
                            int keyCount = columnNameList.Count;

                            //记录本行是否有效
                            bool isLineEnable = false;

                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                if (excelReader.IsDBNull(i))
                                {
                                    //第一列无数据则当做无效数据
                                    if (i == 0)
                                    {
                                        //设置本行无效
                                        isLineEnable = false;
                                        break;
                                    }

                                    lineData[columnNameList[i].ToLower()] = "";
                                    continue;
                                }

                                //获取对应值，进行后续处理,如第一行以--开头则不处理
                                string value = excelReader.GetString(i);
                                if (i == 1 && (null != value && value.StartsWith("--")))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                //如第一行没有数据也不处理
                                if (i == 0 && (null == value || value.Length <= 0))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                try
                                {
                                    //处理读取操作
                                    lineData[columnNameList[i].ToLower()] = value;
                                    //设置本行有效
                                    isLineEnable = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError(ex.Message + " Error Position:" + "[" + (i + 1) + "," +
                                                   (lineIdx + 1) + "]" + "----name:" + columnNameList[i] + "   value:" +
                                                   value);
                                }
                            }

                            //添加到结果集合
                            if (isLineEnable)
                            {
                                //使用自动读取函数
                                T readObj = UTBaseExportFunc.AutoRead<T>(lineData, _sheetName + "第" + lineIdx + "行");
                                if (null != readObj)
                                    resList.Add(readObj);
                                else
                                    Debug.LogError(_sheetName + "第" + lineIdx + "行，读取失败！");
                            }
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            //释放文件资源
            stream.Close();
            stream.Dispose();
            excelReader.Dispose();
            stream = null;
            excelReader = null;

            //删除拷贝的文件
            File.Delete(tmpPath);

            if (_exportTxt)
                WriteToTxtFile(_path, _sheetName, _exportEnum);

            if (_exportEnum == EUTExportSettingEnum.GENERAL)
            {
                exportGeneralText(_path, "general_txt", _exportEnum);
            }

            return resList;
        }

        public static List<T> autoReadXls<T>(string _path,
            string[] _sheetName,
            EUTExportSettingEnum _exportEnum,
            bool _exportTxt = true)
        {
            //判断带入的函数对象是否有效
            if (null == _sheetName || _sheetName.Length <= 0)
                return null;

            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("path: " + _path + "\t\t_sheetName:  " + _sheetName);
            //创建结果队列
            List<T> resList = new List<T>();

            do
            {
                string sheetName = excelReader.Name;

                //判断sheet name是否data
                bool fix = false;
                for (int i = 0; i < _sheetName.Length; i++)
                {
                    string tmpName = _sheetName[i];
                    if (null == tmpName)
                        continue;

                    if (sheetName.Equals(tmpName, StringComparison.OrdinalIgnoreCase))
                    {
                        fix = true;
                        break;
                    }
                }

                //判断sheet name是否data
                if (fix)
                {
                    Debug.Log("Start Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                }
                            }

                            //判断是否有字段，不存在columnNameList中，有的话说明配表少了列，报错
                            //自动根据字段识别
                            Type TempCls = typeof(T);
                            //识别变量
                            FieldInfo[] fields = TempCls.GetFields();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                FieldInfo field = fields[i];
                                UTAutoExportVariableAttr attr = null;
                                using (IEnumerator<UTAutoExportVariableAttr> attrIEnumerator =
                                       field.GetCustomAttributes<UTAutoExportVariableAttr>().GetEnumerator())
                                {
                                    if (attrIEnumerator.MoveNext())
                                        attr = attrIEnumerator.Current;
                                }

                                // 如果是可以为空和忽略的不用检查
                                if (attr != null && (attr.canBeEmpty || attr.ignoreReading))
                                    continue;

                                if (!columnNameList.Contains(field.Name))
                                {
                                    Debug.LogError($"表格{_sheetName}缺少列:{field.Name}");
                                }
                            }
                        }
                        else if (lineIdx > 1)
                        {
                            Dictionary<string, string> lineData = new Dictionary<string, string>();
                            int keyCount = columnNameList.Count;

                            //记录本行是否有效
                            bool isLineEnable = false;

                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                if (excelReader.IsDBNull(i))
                                {
                                    //第一列无数据则当做无效数据
                                    if (i == 0)
                                    {
                                        //设置本行无效
                                        isLineEnable = false;
                                        break;
                                    }

                                    lineData[columnNameList[i].ToLower()] = "";
                                    continue;
                                }

                                //获取对应值，进行后续处理,如第一行以--开头则不处理
                                string value = excelReader.GetString(i);
                                if (i == 1 && (null != value && value.StartsWith("--")))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                //如第一行没有数据也不处理
                                if (i == 0 && (null == value || value.Length <= 0))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                try
                                {
                                    //处理读取操作
                                    lineData[columnNameList[i].ToLower()] = value;
                                    //设置本行有效
                                    isLineEnable = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError(ex.Message + " Error Position:" + "[" + (i + 1) + "," +
                                                   (lineIdx + 1) + "]" + "----name:" + columnNameList[i] + "   value:" +
                                                   value);
                                }
                            }

                            //添加到结果集合
                            if (isLineEnable)
                            {
                                //使用自动读取函数
                                T readObj = UTBaseExportFunc.AutoRead<T>(lineData, _sheetName + "第" + lineIdx + "行");
                                if (null != readObj)
                                    resList.Add(readObj);
                                else
                                    Debug.LogError(_sheetName + "第" + lineIdx + "行，读取失败！");
                            }
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            //释放文件资源
            stream.Close();
            stream.Dispose();
            excelReader.Dispose();
            stream = null;
            excelReader = null;

            //删除拷贝的文件
            File.Delete(tmpPath);

            if (_exportTxt)
                WriteToTxtFile(_path, _sheetName[0], _exportEnum);

            if (_exportEnum == EUTExportSettingEnum.GENERAL)
            {
                exportGeneralText(_path, "general_txt", _exportEnum);
            }

            return resList;
        }


        /// <summary>
        /// 自动根据模板类的相关参数反射信息进行智能解析的处理，支持返回两组数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_path"></param>
        /// <param name="_sheetName"></param>
        /// <param name="_exportEnum"></param>
        /// <param name="_readDelegate"></param>
        /// <param name="_exportTxt"></param>
        /// <returns></returns>
        public static void autoReadXlsTwoTemp<T, K>(string _path,
            string _sheetName,
            EUTExportSettingEnum _exportEnum,
            List<T> _listT,
            List<K> _listK,
            bool _exportTxt = true)
        {
            string exitension = Path.GetExtension(_path);

            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream stream = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            Debug.Log("path: " + _path + "\t\t_sheetName:  " + _sheetName);
            do
            {
                //判断sheet name是否data
                if (excelReader.Name.Equals(_sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Start Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                }
                            }

                            //判断是否有字段，不存在columnNameList中，有的话说明配表少了列，报错
                            //自动根据字段识别
                            Type TempCls = typeof(T);
                            //识别变量
                            FieldInfo[] fields = TempCls.GetFields();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                FieldInfo field = fields[i];
                                UTAutoExportVariableAttr attr = null;
                                using (IEnumerator<UTAutoExportVariableAttr> attrIEnumerator =
                                       field.GetCustomAttributes<UTAutoExportVariableAttr>().GetEnumerator())
                                {
                                    if (attrIEnumerator.MoveNext())
                                        attr = attrIEnumerator.Current;
                                }

                                // 如果是可以为空和忽略的不用检查
                                if (attr != null && (attr.canBeEmpty || attr.ignoreReading))
                                    continue;

                                if (!columnNameList.Contains(field.Name))
                                {
                                    Debug.LogError($"表格{_sheetName}缺少列:{field.Name}");
                                }
                            }
                        }
                        else if (lineIdx > 1)
                        {
                            Dictionary<string, string> lineData = new Dictionary<string, string>();
                            int keyCount = columnNameList.Count;

                            //记录本行是否有效
                            bool isLineEnable = false;

                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                if (excelReader.IsDBNull(i))
                                {
                                    //第一列无数据则当做无效数据
                                    if (i == 0)
                                    {
                                        //设置本行无效
                                        isLineEnable = false;
                                        break;
                                    }

                                    lineData[columnNameList[i].ToLower()] = "";
                                    continue;
                                }

                                //获取对应值，进行后续处理,如第一行以--开头则不处理
                                string value = excelReader.GetString(i);
                                if (i == 1 && (null != value && value.StartsWith("--")))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                //如第一行没有数据也不处理
                                if (i == 0 && (null == value || value.Length <= 0))
                                {
                                    //设置本行无效
                                    isLineEnable = false;
                                    break;
                                }

                                try
                                {
                                    //处理读取操作
                                    lineData[columnNameList[i].ToLower()] = value;
                                    //设置本行有效
                                    isLineEnable = true;
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError(ex.Message + " Error Position:" + "[" + (i + 1) + "," +
                                                   (lineIdx + 1) + "]" + "----name:" + columnNameList[i] + "   value:" +
                                                   value);
                                }
                            }

                            //添加到结果集合
                            if (isLineEnable)
                            {
                                //使用自动读取函数
                                T readObjT = UTBaseExportFunc.AutoRead<T>(lineData, _sheetName + "第" + lineIdx + "行");
                                K readObjK = UTBaseExportFunc.AutoRead<K>(lineData, _sheetName + "第" + lineIdx + "行");

                                if (null != readObjT)
                                    _listT.Add(readObjT);
                                else
                                    Debug.LogError(_sheetName + "第" + lineIdx + "行，读取失败！");

                                if (null != readObjK)
                                    _listK.Add(readObjK);
                                else
                                    Debug.LogError(_sheetName + "第" + lineIdx + "行，读取失败！");
                            }
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            //释放文件资源
            stream.Close();
            stream.Dispose();
            excelReader.Dispose();
            stream = null;
            excelReader = null;

            //删除拷贝的文件
            File.Delete(tmpPath);

            if (_exportTxt)
                WriteToTxtFile(_path, _sheetName, _exportEnum);

            if (_exportEnum == EUTExportSettingEnum.GENERAL)
            {
                exportGeneralText(_path, "general_txt", _exportEnum);
            }
        }

        public DataSet ExcelToDS(string _path, string _sheetName)
        {
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + _path + ";" +
                             "Extended Properties=Excel 8.0;";
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = "select * from [sheet1$]";
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            myCommand.Fill(ds, "table1");
            return ds;
        }

        /// <summary>  
        /// 读取 Excel 需要添加 Excel; System.Data;  
        /// </summary>  
        /// <param name="sheet"></param>  
        /// <returns></returns>  
        static DataRowCollection ReadExcel(string _path, string _sheetName)
        {
            FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            DataSet result = excelReader.AsDataSet();
            //int columns = result.Tables[0].Columns.Count;  
            //int rows = result.Tables[0].Rows.Count;  
            return result.Tables[_sheetName].Rows;
        }

        //读一个excel里的单个表
        public static void WriteToTxtFile(string _path,
            string _sheetName,
            EUTExportSettingEnum _exportEnum,
            Action<string> _onDone = null)
        {
            //检查对应文件夹位置是否创建完成
            UTFilePathData pathData = UTFilePathData.getSavePathData(_exportEnum);
            if (pathData == null)
            {
                Debug.LogError("save path data is null, ENPExportSettingEnum:  " + _exportEnum);
                return;
            }

            pathData.fileDirectoryCheck();
            string filePath = pathData.getAbsolutePath(_sheetName);
            //当前TXT文件是否存在，存在就先删除
            if (File.Exists(filePath))
            {
                File.Delete(Path.GetFullPath(filePath));
            }

            string exitension = Path.GetExtension(_path);
            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream fs = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);

            //重新创建一个文件读写
            fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
            StreamWriter strmWriter = new StreamWriter(fs); //存入到文本文件中

            do
            {
                if (excelReader.Name.Equals(_sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    //Debug.Log("Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                    strmWriter.Write(excelReader.GetString(i));
                                }

                                strmWriter.Write(EXCEL_LINE_DELIMITER); //同一行的每一列分隔符
                            }

                            strmWriter.Write(EXCEL_LINE);
                        }
                        else if (lineIdx > 1)
                        {
                            int keyCount = columnNameList.Count; //最大有效列
                            string zenoV = string.Empty;
                            bool isSkip = false;
                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                string value = excelReader.GetString(i);
                                if (i == 0)
                                {
                                    zenoV = value;
                                    isSkip = string.IsNullOrEmpty(zenoV);
                                    if (isSkip)
                                        break;
                                }
                                else if (i == 1)
                                {
                                    if (null != value && value.StartsWith("--"))
                                    {
                                        isSkip = true;
                                        break;
                                    }

                                    strmWriter.Write(zenoV);
                                    strmWriter.Write(EXCEL_LINE_DELIMITER); //先输出第一列
                                    strmWriter.Write(value);
                                    strmWriter.Write(EXCEL_LINE_DELIMITER); //同一行的每一列分隔符
                                }
                                else
                                {
                                    strmWriter.Write(value);
                                    strmWriter.Write(EXCEL_LINE_DELIMITER); //同一行的每一列分隔符
                                }
                            }

                            if (isSkip)
                                continue;
                            strmWriter.Write(EXCEL_LINE);
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            strmWriter.Close();
            fs.Close();
            strmWriter.Dispose();
            fs.Dispose();
            //删除拷贝的文件
            File.Delete(tmpPath);

            string log = string.Format("excel文件[{0}]的表[{1}]导出到txt文件{2}成功完成.", _path, _sheetName, filePath);
            Debug.LogWarning(log);
            if (_onDone != null)
                _onDone(filePath);
        }

        //对常量表特殊处理，导出一个文本
        static void exportGeneralText(string _path, string _sheetName, EUTExportSettingEnum _exportEnum)
        {
            //检查对应文件夹位置是否创建完成
            UTFilePathData pathData = UTFilePathData.getSavePathData(_exportEnum);
            if (pathData == null)
            {
                Debug.LogError("save path data is null, ENPExportSettingEnum:  " + _exportEnum);
                return;
            }

            pathData.fileDirectoryCheck();
            string filePath = pathData.getAbsolutePath(_sheetName);
            //当前TXT文件是否存在，存在就先删除
            if (File.Exists(filePath))
            {
                File.Delete(Path.GetFullPath(filePath));
            }

            string exitension = Path.GetExtension(_path);
            //开启文件并进行读取
            string tmpPath = _path.Replace(exitension, ".tmpX");
            //拷贝一个文件，避免开启excel时无法读取的问题
            File.Copy(_path, tmpPath, true);
            //开启文件并进行读取
            FileStream fs = File.OpenRead(tmpPath);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);

            //重新创建一个文件读写
            fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite);
            StreamWriter strmWriter = new StreamWriter(fs); //存入到文本文件中

            do
            {
                if (excelReader.Name.Equals(_sheetName, StringComparison.OrdinalIgnoreCase))
                {
                    //Debug.Log("Read Sheet: " + excelReader.Name);
                    //存储列名称,用于分析数据并进行读取
                    List<string> columnNameList = new List<string>();
                    int lineIdx = 0;
                    //对表进行处理
                    while (excelReader.Read())
                    {
                        if (1 == lineIdx)
                        {
                            //取列名
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                //添加列名称, 第一行不能有空列，空列意味着隔断
                                if (excelReader.IsDBNull(i))
                                {
                                    break;
                                }
                                else
                                {
                                    columnNameList.Add(excelReader.GetString(i));
                                    strmWriter.Write(excelReader.GetString(i));
                                }

                                strmWriter.Write(EXCEL_LINE_DELIMITER); //同一行的每一列分隔符
                            }

                            strmWriter.Write(EXCEL_LINE);
                        }
                        else if (lineIdx > 1)
                        {
                            int keyCount = columnNameList.Count; //最大有效列
                            string zenoV = string.Empty;
                            //当前行中的列数目，每列值进行处理
                            for (int i = 0; i < excelReader.FieldCount; i++)
                            {
                                if (i >= keyCount)
                                    continue;

                                string value = excelReader.GetString(i);
                                if (i == 0)
                                {
                                    zenoV = value;
                                }
                                else if (i == 1)
                                {
                                    if (null != value && value.StartsWith("--"))
                                        break;

                                    strmWriter.Write(zenoV);
                                    strmWriter.Write(EXCEL_LINE_DELIMITER); //先输出第一列
                                    strmWriter.Write(value);
                                    strmWriter.Write(EXCEL_LINE_DELIMITER); //同一行的每一列分隔符
                                }
                                else
                                {
                                    strmWriter.Write(value);
                                    strmWriter.Write(EXCEL_LINE_DELIMITER); //同一行的每一列分隔符
                                }
                            }

                            strmWriter.Write(EXCEL_LINE);
                        }

                        //增加行号
                        lineIdx++;
                    }
                }
            } while (excelReader.NextResult());

            strmWriter.Close();
            fs.Close();
            strmWriter.Dispose();
            fs.Dispose();
            //删除拷贝的文件
            File.Delete(tmpPath);
            string log = string.Format("excel文件[{0}]的表[{1}]导出到txt文件{2}成功完成.", _path, _sheetName, filePath);
            Debug.LogWarning(log);
        }
    }
}
