using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UTGame
{
    //战斗管理类
    public class UTBattleMain : MonoBehaviour
    {
        [Header("障碍物生成父节点")]
        public Transform obstacleInitPar;

        [Header("主游戏对象")]
        public UTCirclePlayer mainPlayer;

        [Header("移动控制器")]
        public VariableJoystick variableJoystick;

        [Header("当前的层数文本")]
        public Text floorTxt;

        private static UTBattleMain _g_instance = null;

        public static UTBattleMain instance
        {
            get { return _g_instance; }
        }

        //碰撞物管理类
        private UTObstacleMgr _m_obstacleMgr;
        
        private Rigidbody2D _m_rigidbody2D;

        private void Start()
        {
            //设置处理对象
            if (null == _g_instance)
                _g_instance = this;
            else
            {
                UTLog.Error("Multiple BattleMain Mono!!!");
                return;
            }
            
            if (!_check())
                return;

            _m_rigidbody2D = mainPlayer.GetComponent<Rigidbody2D>();
            _m_obstacleMgr = new UTObstacleMgr();
            _m_obstacleMgr.init();
            _m_obstacleMgr.start(obstacleInitPar);
        }

        private void OnDisable()
        {
            if (!_check())
                return;
            
            _m_obstacleMgr.reset();
        }

        public void Update()
        {
            
        }

        public void FixedUpdate()
        {
            if (!_check())
                return;
            
            if (null == _m_rigidbody2D)
                return;

            // 移动
            Vector3 currentVelocity = _m_rigidbody2D.velocity;
            Vector3 newVelocity = new Vector3(variableJoystick.Horizontal * mainPlayer.speed * Time.fixedDeltaTime,
                currentVelocity.y, currentVelocity.z);
            _m_rigidbody2D.velocity = newVelocity;
            
            //更新当前层数
            UGUICommon.setLabelTxt(floorTxt,_calCurFloor());
        }
        
        #region 工具类

        /// <summary>
        /// 计算当前的层数 1000米表示一层
        /// </summary>
        private int _calCurFloor()
        {
            if (!_check())
                return 0;

            float moveY = mainPlayer.transform.localPosition.y;
            int curFloor = (int)(moveY / 1000);
            Debug.LogError($"moveY = {moveY} curFloor = {curFloor}");
            return curFloor;
        }

        /// <summary>
        /// 获取当前阶段
        /// </summary>
        /// <returns></returns>
        public UTStageRefObj getCurStage()
        {
            if (!_check())
                return null;
            
            int curFloor = _calCurFloor();
            //TODO 从配表取
            List<UTStageRefObj> stageList = new List<UTStageRefObj>();
            UTStageRefObj temp = null;
            for (int i = stageList.Count - 1; i >= 0; i--)
            {
                temp = stageList[i];
                if (null == temp)
                    continue;
                if (curFloor >= temp.floor)
                {
                    return temp;
                }
            }

            return null;
        }

        private bool _check()
        {
            if (null == mainPlayer || null == obstacleInitPar)
                return false;

            return true;
        }
        
        #endregion
    }
}