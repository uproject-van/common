using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace UTGame
{
    public abstract class _AUTBaseExportMenuItem : _IUTExportMenuInterface
    {
        /** 显示的子对象 */
        private List<_IUTExportMenuInterface> _m_lChildShowItems;

        protected UTSelectItem _m_siSelectItem;
        protected UTSelectItem _m_siForceItem;
        //是否展开状态
        private bool _m_bIsExpend;


        public bool isSelect { get { return _m_siSelectItem.isOn; } }
        public bool isForce { get { return _m_siForceItem.isOn; } }

        public _AUTBaseExportMenuItem ()
        {
            _m_lChildShowItems = new List<_IUTExportMenuInterface>();
            _m_bIsExpend = false;

            _m_siSelectItem = new UTSelectItem("select");
            _m_siForceItem = new UTSelectItem("force");
        }


        //是否需要显示
        public virtual bool needShow { get { return true; } }

        //具体的gui绘制函数
        public void onGUI()
        {
            //横向排布，空出10宽度
            GUILayout.BeginHorizontal();

            //空出10
            GUILayout.Label(_m_bIsExpend ? "-" : "+", GUILayout.Width(15));

            //继续纵向处理
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button((_m_bIsExpend ? "shrink" : "expand"), GUILayout.Width(140), GUILayout.Height(20))) 
            {
                _m_bIsExpend = !_m_bIsExpend;
            }

            GUILayout.Label(_menuText, GUILayout.Width(220), GUILayout.Height(15));

            if (GUILayout.Button("单项导出", GUILayout.Width(240), GUILayout.Height(20))) {
                _exExport();
            }

            _m_siSelectItem.onGUI();
            _m_siForceItem.onGUI();

            _exGuiFunc();

            GUILayout.EndHorizontal();

            if (_m_bIsExpend) 
            {
                //调用额外的GUI函数
                _exGuiExpendFunc();

                //开始子对象的绘制
                for (int i = 0; i < _m_lChildShowItems.Count; i++) {
                    _m_lChildShowItems[i].onGUI();
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /***********************
         * 注册资源导出菜单项
         **/
        protected void _regSubItem(_IUTExportMenuInterface _item)
        {
            if (null == _item)
                return;

            if (_m_lChildShowItems.Contains(_item))
            {
                UnityEngine.Debug.LogError("reg sub item multi times for menu: " + _menuText);
                return;
            }

            _m_lChildShowItems.Add(_item);
        }

        /****************
         * 显示的菜单文字
         **/
        protected abstract string _menuText { get; }

        /****************
         * 展开时额外的gui展示函数
         **/
        protected virtual void _exGuiExpendFunc() { }

        /****************
         * 额外的gui展示函数
         **/
        protected virtual void _exGuiFunc() { }

        /// <summary>
        /// 单项导出按钮处理
        /// </summary>
        protected virtual void _exExport () { 
        }
    }
}
