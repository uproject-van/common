using System.IO;
using UnityEditor;
using UnityEngine;

namespace UTGame
{
    public abstract class _AUTExcelSelectItem : _IUTExportMenuInterface
    {
        private string _m_sPreValue;
        private bool _m_bInit;

        public _AUTExcelSelectItem()
        {
            _m_bInit = false;
        }

        public virtual bool needShow { get { return true; } }

        //具体的gui绘制函数
        public void onGUI()
        {
            if (!_m_bInit)
            {
                _m_bInit = true;
                _m_sPreValue = UTExportDataCore.instance.getValue(_dataKey);
            }

            //输出文本信息
            GUILayout.Label(_text, GUILayout.Height(20));

            //开始横向排布
            GUILayout.BeginHorizontal();

            //输出浏览按钮
            if (GUILayout.Button("Browser", GUILayout.Width(60), GUILayout.Height(25)))
            {
                //获取原文件路径
                string prePath = _defaultFolderPath;
                if (null != _m_sPreValue && File.Exists(_m_sPreValue))
                {
                    FileInfo fileInfo = new FileInfo(_m_sPreValue);
                    prePath = fileInfo.DirectoryName;
                }
                
                //弹出窗口选择excel文件
                string selectExcel = EditorUtility.OpenFilePanel("Select Excel Files", prePath, "xlsx");
                if (null != selectExcel && selectExcel.Length > 0)
                {
                    _m_sPreValue = selectExcel;
                    //保存数据
                    UTExportDataCore.instance.setValue(_dataKey, _m_sPreValue);
                }
            }
            //输出文件夹路径
            string newValue = GUILayout.TextField(_m_sPreValue, GUILayout.Height(25));
            if (!newValue.Equals(_m_sPreValue))
            {
                _m_sPreValue = newValue;
                //保存数据
                UTExportDataCore.instance.setValue(_dataKey, _m_sPreValue);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 返回默认选择位置
        /// </summary>
        /// <returns></returns>
        protected virtual string _defaultFolderPath
        {
            get
            {
                return string.Empty;
            }
        }

        /****************
         * 显示的说明
         **/
        protected abstract string _text { get; }
        protected abstract string _dataKey { get; }
    }
}