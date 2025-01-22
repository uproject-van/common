using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的基类
    /// </summary>
    public abstract class _AUTObstacleBase : _AMonoBase
    {
        private EObstacleType _m_eObstacleType;
        private EColor _m_eColor;
        
        public EObstacleType eObstacleType{get{return _m_eObstacleType;}}
        public EColor eColor{get{return _m_eColor;}}

        public void setData(EColor _color)
        {
            _m_eColor = _color;
        }

        /// <summary>
        /// 不同类型的障碍物触发不同的效果
        /// </summary>
        protected abstract void _dealEffect();
    }
}