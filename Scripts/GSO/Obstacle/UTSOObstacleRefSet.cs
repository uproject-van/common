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
        public EObstacleType type;//障碍物类型
        public long stage_id;//障碍物阶段
        public string prefab_name;//资源名称
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