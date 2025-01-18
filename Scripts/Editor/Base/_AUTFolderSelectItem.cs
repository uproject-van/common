using UnityEngine;
using UnityEditor;

namespace UTGame
{
    public abstract class _AUTFolderSelectItem : _IUTExportMenuInterface
    {
        private string _m_sPreValue;
        private bool _m_bInit;

        public _AUTFolderSelectItem()
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
                //弹出窗口选择文件夹
                string selectFolder = EditorUtility.OpenFolderPanel("Select Folder", _m_sPreValue, "");
                if (null != selectFolder && selectFolder.Length > 0)
                {
                    _m_sPreValue = selectFolder;
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

        /****************
         * 显示的说明
         **/
        protected abstract string _text { get; }
        protected abstract string _dataKey { get; }
    }
}

