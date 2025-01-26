using System;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 固定的碰撞体
    /// </summary>
    public class UTObstacleFixed : _AUTObstacleBase
    {
        /// <summary>
        /// 碰撞触发效果
        /// </summary>
        protected override void _dealEffect()
        {
        }

        protected override void _refreshEx()
        {
            if (null == _m_loadGo || null == _m_obstacleRefObj)
                return;

            SpriteRenderer spriteRenderer = _m_loadGo.GetComponent<SpriteRenderer>();
            if (null == spriteRenderer)
                return;

            BoxCollider2D boxCollider2D = _m_loadGo.GetComponent<BoxCollider2D>();
            if (null == boxCollider2D)
                return;

            //设置尺寸
            float realWidth = _m_obstacleRefObj.getRealWidth(_m_eObstacleSize);
            spriteRenderer.size.Set(realWidth, spriteRenderer.size.y);
            boxCollider2D.size = new Vector2(realWidth, spriteRenderer.size.y);
            
            //重置一下位置
            if (null != _m_loadGo)
            {
                _m_loadGo.transform.localPosition = Vector3.zero;
            }
        }
    }
}