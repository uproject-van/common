using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace UTGame
{
    public class UTBaseConfirmItem : _AUTConfirmItem
    {
        private string _m_sText;
        private Action _m_dDelegate;

        public UTBaseConfirmItem (string _text, Action _delegate)
        {
            _m_sText = _text;
            _m_dDelegate = _delegate;

            //注册函数
            //ALExportWnd.regExportFunc(_delegate);

        }

        /****************
         * 显示的说明
         **/
        protected override string _text { get { return _m_sText; } }
        /**************
         * 具体的处理函数
         */
        protected override void _dealConfirm()
        {
            if (null != _m_dDelegate)
                _m_dDelegate();
        }
    }
}
