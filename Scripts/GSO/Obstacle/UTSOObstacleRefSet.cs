using System;
using System.Collections.Generic;

namespace UTGame
{
    /// <summary>
    /// 障碍物表
    /// </summary>
    [Serializable]
    public class UTObstacleRefObj : _IUTBaseRefObj
    {
        public long _refId
        {
            get { return id; }
        }
        public long id; //主id
        public long type_id; //类型id
        public string res_path_id;//资源路径

    }

    public class UTSOObstacleRefSet : _TUTSOBaseRefSet<UTObstacleRefObj>
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