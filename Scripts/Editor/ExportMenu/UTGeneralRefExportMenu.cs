using System;
using System.Collections.Generic;

namespace UTGame
{
    //导出的值结构体
    public class UTGeneralExportValuePair
    {
        public string name;
        public string value;
    }

    public class
        UTGeneralRefExportMenu : UTBaseExportMenuItem<UTGeneralExportValuePair, UTGeneralRefObj, UTSOGeneralRefSet>
    {
        public UTGeneralRefExportMenu(string _tag, Func<string, string, bool> _judgeCanShowFunc)
            : base("general", EUTExportSettingEnum.GENERAL, UTSOGeneralRefSet.assetName,
                _tag, _judgeCanShowFunc)
        {
        }

        protected override string _menuText
        {
            get
            {
                return "general 常量信息(general)";
            }
        }

        public override List<UTGeneralRefObj> _exchangeTemplate(List<UTGeneralExportValuePair> _tempList)
        {
            if (lineValue == null)
                lineValue = new Dictionary<string, string>();

            //将数据放入集合，用于用基本读取方式读取数据
            lineValue.Clear();
            for (int i = 0; i < _tempList.Count; i++)
            {
                lineValue.Add(_tempList[i].name.ToLower(), _tempList[i].value);
            }

            //创建队列
            List<UTGeneralRefObj> list = new List<UTGeneralRefObj>();

            //创建对象
            UTGeneralRefObj realObj = new UTGeneralRefObj();

            realObj.test_id = GetInt("test_id");

            list.Add(realObj); //加入对象
            return list;
        }

        public override void _readRefInfo()
        {
            //读取所有数据集合
            obj.name = GetString("name");
            obj.value = GetString("value");
        }
    }
}
