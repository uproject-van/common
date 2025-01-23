using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTGame
{
    //第一个类型是模板类,  第二个类型是实际游戏中运用的类,  第三个类型是表的集合类
    public abstract class UTBaseAutoExportMenuItem<Tobj, TMap> : UTBaseExportMenuItemEx
        where Tobj : _IUTBaseRefObj, new() where TMap : _TUTSOBaseRefSet<Tobj>, new()
    {
        private string assetName = "";
        
        public UTBaseAutoExportMenuItem(EUTExportSettingEnum _exportEnum,
            string _assetName,
            string _tag,
            Func<string, string, bool> _judgeCanShowFunc)
            : base(_tag, _exportEnum, _judgeCanShowFunc)
        {
            assetName = _assetName;
            _regSubItem(new UTTextButtonItem(_tag, 15, "copy", () => { GUIUtility.systemCopyBuffer = _tag; }));
            _regSubItem(new UTSplitLine());
            _regSubItem(new UTExcelPathItem("数 据 信 息 Excel 文 件：", _exportEnum.ToString()));
            _regSubItem(new UTTextItem("选择的页签名字。", 15));
            _regSubItem(new UTInputItem(_exportEnum.ToString(), 25));

            _regSubItem(new UTConfirmItem("导 出", exportGeneralRefSet));

            UTExportSettingMgr.instance.regSubExportSetting(_exportEnum, exportGeneralRefSet, isSelect, _tag,
                _judgeCanShowFunc);
        }
        
        /******************
         * 具体的导出操作
         **/
        private void exportGeneralRefSet()
        {
            //读取对应的excel文件
            string excelPath = UTExportDataCore.instance.getValue(exportEnum.ToString());
            if (string.IsNullOrEmpty(excelPath))
            {
                Debug.LogError("ExcelPath is null or empty!!");
                return;
            }

            string tabName = UTInputTabData.instance.getValue(exportEnum.ToString());

            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError($"{tabName}:Ref Set 导出失败，包名为空，assetName:{assetName}");
                return;
            }

            //开始读取excel文件
            List<Tobj> tempList = UTExportWnd.autoReadXls<Tobj>(excelPath, tabName, exportEnum);
            if (tempList.Count == 0)
            {
                Debug.LogError(exportEnum + "数据为空,请注意.");
            }

            TMap refSet = ScriptableObject.CreateInstance<TMap>();
            refSet.refList = new List<Tobj>(tempList);
            UTBaseExportFunction.exportAsset(refSet, assetName, "refdata", "unity3d");
            AssetDatabase.Refresh();

            tempList.Clear();
            tempList = null;
            //输出导出完成
            Debug.LogWarning(string.Format("{0} : Ref Set 导出完成!! {1}", tabName, assetName));
        }

        protected override void _exExport()
        {
            exportGeneralRefSet();
        }
    }
}
