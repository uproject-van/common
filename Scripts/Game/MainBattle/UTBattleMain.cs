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

        [Header("相机移动速度")]
        public float cameraMoveSpeed = 5.0f;

        [Header("顶部的追逐对象")]
        public UTTopMonster topMonster;

        [Header("移动控制器")]
        public VariableJoystick variableJoystick;

        [Header("当前的层数文本")]
        public Text floorTxt;

        public float worldPerW
        {
            get { return _m_worldPerW; }
        }

        public float worldPerH
        {
            get { return _m_worldPerH; }
        }

        public Vector2 startPos
        {
            get { return _m_startPos; }
        }

        public Vector2 endPos
        {
            get { return _m_endPos; }
        }

        private static UTBattleMain _g_instance = null;

        public static UTBattleMain instance
        {
            get { return _g_instance; }
        }

        private float _m_worldPerW;
        private float _m_worldPerH;

        private Vector2 _m_startPos;

        private Vector2 _m_endPos;

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

            //算一下屏幕的宽度高度
            Vector3 startPos = GCommon.chgUIPosToWorldPos(Vector2.zero);
            Vector3 endPos = GCommon.chgUIPosToWorldPos(new Vector2(Screen.width, Screen.height));
            _m_worldPerW = (endPos.x - startPos.x) / 10.0f;
            _m_worldPerH = (endPos.y - startPos.y) / 20.0f;
            _m_startPos = startPos;
            _m_endPos = endPos;

            _m_rigidbody2D = mainPlayer.GetComponent<Rigidbody2D>();
            _m_obstacleMgr = new UTObstacleMgr();
            _m_obstacleMgr.init(() => { _m_obstacleMgr.start(obstacleInitPar); });
        }

        private void OnDisable()
        {
            if (!_check())
                return;

            _m_obstacleMgr.reset();
        }

        public void Update()
        {
            if (null == _m_obstacleMgr || !_m_obstacleMgr.isInit || null == GameMain.instance.mainCamera)
                return;

            // 相机以一定速度向下移动
            GameMain.instance.mainCamera.transform.position += Vector3.down * (cameraMoveSpeed * Time.deltaTime);

            _m_obstacleMgr.addNeedObInitY(cameraMoveSpeed * Time.deltaTime);
            // 将尖尖固定在相机顶部
            topMonster.transform.position = new Vector3(GameMain.instance.mainCamera.transform.position.x,
                GameMain.instance.mainCamera.transform.position.y + UTBattleMain.instance.worldPerH * 10, 0);
        }

        public void FixedUpdate()
        {
            if (null == _m_obstacleMgr || !_m_obstacleMgr.isInit || null == GameMain.instance.mainCamera)
                return;
            
            if (!_check())
                return;

            if (null == _m_rigidbody2D)
                return;

            // 移动
            Vector3 newVelocity = new Vector3(variableJoystick.Horizontal, 0, 0) *
                                  (mainPlayer.speed * Time.fixedDeltaTime);
            _m_rigidbody2D.transform.Translate(newVelocity);

            // 限制角色移动范围
            _m_rigidbody2D.transform.position =
                new Vector3(
                    Mathf.Clamp(_m_rigidbody2D.transform.position.x, UTBattleMain.instance.startPos.x + 0.2f,
                        UTBattleMain.instance.endPos.x - 0.2f),
                    _m_rigidbody2D.transform.position.y, _m_rigidbody2D.transform.position.z);

            //更新当前层数
            UGUICommon.setLabelTxt(floorTxt, _calCurFloor());
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
            //Debug.LogError($"moveY = {moveY} curFloor = {curFloor}");
            return curFloor + 1;
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
            //从配表取
            List<UTStageRefObj> stageList = GRefdataCoreMgr.instance.stageListCore.RefList;
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