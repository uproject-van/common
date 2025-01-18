using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UTGame
{
    [Serializable]
    public class UTGeneralRefObj : _IUTBaseRefObj
    {
        public long _refId
        {
            get
            {
                return UTSOGeneralRefSet.generalId;
            }
        }

        public int test_id; //测试id
        
    }

    public class UTSOGeneralRefSet : _TUTSOBaseRefSet<UTGeneralRefObj>
    {
        public const long generalId = 1000;

        /************
         * 资源加载路径
         **/
        public static string assetPath { get { return "refdata/game_refdata.unity3d"; } }
        public static string objName { get { return "general"; } }
    }
}


