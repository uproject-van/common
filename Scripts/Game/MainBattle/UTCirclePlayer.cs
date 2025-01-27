using System;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 小球的控制器
    /// </summary>
    public class UTCirclePlayer : MonoBehaviour
    {
        [Header("移动速度")]
        public float speed;

        private void OnCollisionEnter2D(Collision2D _other)
        {
            if(null == _other || null == _other.gameObject)
                return;
            
            //如果碰到顶部怪兽
            UTTopMonster monster = _other.gameObject.GetComponent<UTTopMonster>();
            if (null != monster)
            {
                UIQueueMgr.instance.addNode(new UTBattlePauseNode());
            }
        }
    }
}