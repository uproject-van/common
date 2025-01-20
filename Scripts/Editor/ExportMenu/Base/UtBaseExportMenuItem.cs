using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTGame
{
    //第一个类型是模板类,  第二个类型是实际游戏中运用的类,  第三个类型是表的集合类
    public abstract class UtBaseExportMenuItem<TTemp, Tobj, TMap> : UtBaseExportMenuItemEx where TTemp : new()
        where Tobj : _IUTBaseRefObj, new()
        where TMap : _TUTSOBaseRefSet<Tobj>, new()
    {
        private string _m_assetName = "";

        public UtBaseExportMenuItem(string _exportName,
            EUTExportSettingEnum _exportEnum,
            string _assetName,
            string _tag,
            Func<string, string, bool> _judgeCanShowFunc)
            : base(_tag, _exportEnum, _judgeCanShowFunc)
        {
            _m_assetName = _assetName;
            _regSubItem(new UTTextButtonItem(_exportName, 15, "copy",
                () => { GUIUtility.systemCopyBuffer = _exportName; }));
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

            if (string.IsNullOrEmpty(_m_assetName))
            {
                Debug.LogError($"{tabName}:Ref Set 导出失败，包名为空，assetName:{_m_assetName}");
                return;
            }

            //开始读取excel文件
            List<TTemp> tempList = UTExportWnd.readXls<TTemp>(
                excelPath
                , tabName
                , exportEnum
                , _readRefInfo);

            if (tempList.Count == 0)
            {
                Debug.LogError(exportEnum + "数据为空,请注意.");
            }

            TMap refSet = ScriptableObject.CreateInstance<TMap>();
            refSet.refList = new List<Tobj>(_exchangeTemplate(tempList));
            UTBaseExportFunction.exportAsset(refSet, _m_assetName);
            AssetDatabase.Refresh();

            tempList.Clear();
            tempList = null;
            //输出导出完成
            Debug.LogWarning(string.Format("{0} : Ref Set 导出完成!! {1}", tabName, _m_assetName));
        }

        protected TTemp obj;

        public void _readRefInfo(TTemp _tmpInfo, int _line, Dictionary<string, string> _lineData)
        {
            lineValue = _lineData;
            line = _line;
            obj = _tmpInfo;
            _readRefInfo();
            lineValue = null;
        }

        public abstract void _readRefInfo();

        public abstract List<Tobj> _exchangeTemplate(List<TTemp> _tempList);


        protected override void _exExport()
        {
            exportGeneralRefSet();
        }
    }
}
