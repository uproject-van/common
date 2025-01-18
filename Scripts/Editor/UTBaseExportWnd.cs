using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UTGame
{
    public class UTBaseExportWnd : EditorWindow
    {
        public static void showExportWnd<T>(Rect _rect, string _title) where T : UTBaseExportWnd
        {
            //创建窗口
            T window = (T)EditorWindow.GetWindowWithRect(typeof(T), _rect, true, _title);
            window.Show();
        }

        private static UTBaseExportWnd _g_instance = null;
        public static UTBaseExportWnd instance { get { return _g_instance; } }

        /***********
         * 注册导出函数
         **/
        public static void regExportFunc(Action _action)
        {
            if (null == _action || null == _g_instance)
                return;

            _g_instance._m_eaiExportAllItem.regExportFunc(_action);
        }

        private List<_IUTExportMenuInterface> _m_lMenuItemList;

        //滚动区域位置
        private Vector2 _m_vScrollViewPos;

        //导出所有的对象
        protected UTExportAllItem _m_eaiExportAllItem;

        //选择所有对象
        protected UTSelectItem _m_saiSelectAllItem;

        //忽略所有对象的差异校验
        protected UTSelectItem _m_saiForceAllItem;

        private UTFolderPathItem _m_fpiFolderPathItem;

        public UTBaseExportWnd()
        {
            _g_instance = this;

            _m_lMenuItemList = new List<_IUTExportMenuInterface>();

            _m_fpiFolderPathItem = new UTFolderPathItem("xml 文 件 导 出 路 径 ：", UTExportDataCore.xmlRootPathKey);

            _m_eaiExportAllItem = new UTExportAllItem();

            _m_saiSelectAllItem = new UTSelectItem("select all");

            _m_saiForceAllItem = new UTSelectItem("force all");

            _regMenuItem(new UTSplitLine());
        }


        /*********
         * gui处理函数
         **/
        void OnGUI()
        {
            _m_vScrollViewPos = GUILayout.BeginScrollView(_m_vScrollViewPos);

            //开始纵向布局
            EditorGUILayout.BeginVertical();

            //显示文本
            EditorGUILayout.LabelField("DL 资源导出窗口");
            _m_fpiFolderPathItem.onGUI();

            EditorGUILayout.BeginHorizontal();
            _m_eaiExportAllItem.onGUI();
            _m_saiSelectAllItem.onGUI();
            _m_saiForceAllItem.onGUI();
            EditorGUILayout.EndHorizontal();

            //开始逐项进行显示
            for (int i = 0; i < _m_lMenuItemList.Count; i++)
            {
                if (!_m_lMenuItemList[i].needShow)
                    continue;

                _m_lMenuItemList[i].onGUI();
            }

            //结束最外围纵向布局
            EditorGUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        /***********************
         * 注册资源导出菜单项
         **/
        protected void _regMenuItem(_IUTExportMenuInterface _item)
        {
            if (null == _item)
                return;

            if (_m_lMenuItemList.Contains(_item))
            {
                UnityEngine.Debug.LogError("reg menu item multi times!");
                return;
            }

            _m_lMenuItemList.Add(_item);
        }
    }
}
