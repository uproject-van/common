using System;
using System.Collections.Generic;

namespace UTGame
{
    /// <summary>
    /// 阶段表
    /// </summary>
    [Serializable]
    public class UTStageRefObj : _IUTBaseRefObj
    {
        public long _refId
        {
            get { return id; }
        }
        public long id; //阶段id
        public UTIntRange range; //阶段区间
        public List<UTIntRange> x_init_pos_list;//x可生成的区间 1-10
        public UTIntRange y_init_range;//y可生成的区间范围 1-20

    }

    public class UTSOStageRefSet : _TUTSOBaseRefSet<UTStageRefObj>
    {
        /************
         * 资源加载路径
         **/
        public static string assetName
        {
            get { return "obstacle"; }
        }
    }
}