using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
            get { return floor; }
        }

        public long floor; //层数，大于等于这个层数表示达到当前阶段
        public List<UTIntRange> x_init_pos_list; //x可生成的区间 1-10
        public UTIntRange y_init_range; //y可生成的区间范围 1-20
        public List<EColor> color_list; //可生成的颜色类型
        public List<EObstacleType> type_list; //可生成的障碍物类型

#if IN_GAME
        //当前阶段可以生成的障碍物列表
        private List<UTObstacleRefObj> _m_obstacleRefList = new List<UTObstacleRefObj>();

        public void addObstacleRef(UTObstacleRefObj _refObj)
        {
            if (null == _refObj || _m_obstacleRefList.Contains(_refObj))
                return;

            _m_obstacleRefList.Add(_refObj);
        }

        public UTObstacleRefObj getRandomObstacleRefObj()
        {
            return GCommon.getRandom(_m_obstacleRefList);
        }
#endif

        public EObstacleType getRandomObstacleType()
        {
            return GCommon.getRandom(type_list);
        }
        
        public EColor getRandomColor()
        {
            return GCommon.getRandom(color_list);
        }

        public int getRandomX()
        {
            UTIntRange range = GCommon.getRandom(x_init_pos_list);
            return range.getRandomValue();
        }

        public int getRandomY()
        {
            return y_init_range.getRandomValue();
        }
    }

    public class UTSOStageRefSet : _TUTSOBaseRefSet<UTStageRefObj>
    {
        /************
         * 资源加载路径
         **/
        public static string assetName
        {
            get { return "stage"; }
        }
    }
}