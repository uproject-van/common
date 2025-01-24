using UnityEngine;
using UnityEngine.UI;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的基类
    /// </summary>
    public abstract class _AUTObstacleBase : _AMonoBase
    {
        [Header("图片")]
        public Image iconImg;

        private EColor _m_eColor;
        private UTObstacleRefObj _m_obstacleRefObj;

        public EObstacleType eObstacleType
        {
            get { return null == _m_obstacleRefObj ? EObstacleType.NONE : _m_obstacleRefObj.type; }
        }

        public EColor eColor
        {
            get { return _m_eColor; }
        }

        public UTObstacleRefObj obstacleRefObj
        {
            get { return _m_obstacleRefObj; }
        }

        /// <summary>
        /// 当前的Y的起始位置
        /// </summary>
        public float curY;

        public void setData(UTObstacleRefObj _refObj, EColor _color)
        {
            _m_obstacleRefObj = _refObj;
            _m_eColor = _color;
        }

        public void setPos(Vector2 _pos)
        {
        }

        /// <summary>
        /// 不同类型的障碍物触发不同的效果
        /// </summary>
        protected abstract void _dealEffect();
    }
}