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

        public float getRealWidth(EObstacleSize _sizeE)
        {
            switch (_sizeE)
            {
                case EObstacleSize.SMALL:
                    return 0.66f;
                
                case EObstacleSize.MEDIUM:
                    return 1.26f;
                
                case EObstacleSize.LARGE:
                    return 1.92f;
            }
            return 0f;
        }
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