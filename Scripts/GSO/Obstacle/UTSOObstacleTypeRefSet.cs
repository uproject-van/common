using System;
using System.Collections.Generic;

namespace UTGame
{
    /// <summary>
    /// 障碍物类型表
    /// </summary>
    [Serializable]
    public class UTObstacleTypeRefObj : _IUTBaseRefObj
    {
        public long _refId
        {
            get { return (long)type; }
        }

        public EObstacleType type; //主id
        public string prefab_name;//资源名称
        //可生成障碍物id列表
        public List<long> obstacle_id_list;
        public string effect_name;//效果 后面自定义  
    }

    public class UTSOObstacleTypeRefSet : _TUTSOBaseRefSet<UTObstacleTypeRefObj>
    {
        /************
         * 资源加载路径
         **/
        public static string assetName
        {
            get { return "obstacle_type"; }
        }
    }
}