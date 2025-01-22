using System;
using UnityEngine;

namespace UTGame
{
    //战斗管理类
    public class UTBattleMain : MonoBehaviour
    {
        [Header("碰撞物生成父节点")]
        public Transform obstacleInitPar;

        [Header("主游戏对象")]
        public UTCirclePlayer mainPlayer;
        
        [Header("移动控制器")]
        public VariableJoystick variableJoystick;

        //碰撞物管理类
        private UTObstacleMgr _m_ObstacleMgr;

        private Rigidbody2D _m_rigidbody2D;
        private void Start()
        {
            if(null == mainPlayer)
                return;

            _m_rigidbody2D = mainPlayer.GetComponent<Rigidbody2D>();
            _m_ObstacleMgr = new UTObstacleMgr();
            _m_ObstacleMgr.init();
        }
        
        private void OnDisable()
        {
            _m_ObstacleMgr.reset();
        }

        public void FixedUpdate()
        {
            if(null == _m_rigidbody2D || null == _m_rigidbody2D)
                return;
            
            // 移动
            Vector3 currentVelocity = _m_rigidbody2D.velocity;
            Vector3 newVelocity = new Vector3(variableJoystick.Horizontal * mainPlayer.speed * Time.fixedDeltaTime, currentVelocity.y, currentVelocity.z);
            _m_rigidbody2D.velocity = newVelocity;
        }
        
    }
}