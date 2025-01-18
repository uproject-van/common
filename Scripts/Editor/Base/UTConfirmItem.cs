using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace UTGame
{
    public class UTConfirmItem : UTBaseConfirmItem
    {
        public UTConfirmItem(string _text, Action _delegate)
            : base(_text, _delegate)
        {
        }

        /**************
         * 具体的处理函数
         */
        protected override void _dealConfirm()
        {
            UTExportDataCore.instance.save();
            UTInputTabData.instance.save();

            base._dealConfirm();

            AssetDatabase.Refresh(); //导出后刷新一次
        }
    }
}
