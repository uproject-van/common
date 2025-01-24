using System;
using System.Collections.Generic;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的管理类
    /// </summary>
    public class UTObstacleMgr : _IUTBaseTaskInterface
    {
        //是否初始化
        private bool _m_bIsInit;

        //最远的一个障碍物生成位置
        private float _m_lastObInitY;

        //当前需要生成的障碍物位置
        private float _m_needObInitY;

        private Transform _m_initParTrans;

        /// <summary>
        /// 所有的碰撞体缓存类
        /// </summary>
        private Dictionary<long, UTObstacleCache> _m_obstacleCacheDic;

        /// <summary>
        /// 当前取出的所有障碍物
        /// </summary>
        private List<_AUTObstacleBase> _m_showObstacleList;

        /// <summary>
        /// 设置数据的序列号用于区分是否需要tick
        /// </summary>
        private long _m_showOpSerialize;
        
        public long showOpSerialize
        {
            get { return _m_showOpSerialize; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            if (_m_bIsInit)
                return;

            _m_bIsInit = true;
            //需要生成到的位置
            _m_needObInitY = Screen.height + Screen.height / 2.0f;
            //当前已经生成的位置
            _m_lastObInitY = Screen.height / 2.0f;
            _m_showObstacleList = new List<_AUTObstacleBase>();
            _m_obstacleCacheDic = new Dictionary<long, UTObstacleCache>();
        }

        //开始生成障碍物
        public void start(Transform _parTrans)
        {
            _m_initParTrans = _parTrans;
            if (!_check())
                return;

            stopTick();
            _m_showOpSerialize = UTSerializeOpMgr.next();
            UTMonoTaskMgr.instance.addMonoTask(new UTCycleMonoTask(this));
        }

                
        public void stopTick()
        {
            _m_showOpSerialize = UTSerializeOpMgr.next();
        }
        
        /// <summary>
        /// 定时任务
        /// </summary>
        public void tick()
        {
            //先回收不需要的障碍物
            _AUTObstacleBase temp = null;
            for (int i = 0; i < _m_showObstacleList.Count; i++)
            {
                temp = _m_showObstacleList[i];
                if(null == temp)
                    continue;
                
                //如果物体超出一个半屏幕则回收 TODO 这里可能会有问题
                if (temp.curY < _m_needObInitY - (Screen.height * 2.0f / 3))
                {
                    pushBackObstacel(temp);
                }
            }
            //生成需要的障碍物
            while (_m_lastObInitY < _m_needObInitY)
            {
                bool isSuc = popObstacle();
                if (!isSuc)
                {
                    stopTick();
                    break;
                }
            }
        }

        /// <summary>
        /// 取出单个障碍物 并设置数据
        /// </summary>
        /// <param name="_parTrans"></param>
        /// <param name="_type"></param>
        /// <param name="_color"></param>
        public bool popObstacle()
        {
            if (!_check())
                return false;

            //当前阶段信息
            UTStageRefObj stageRefObj = UTBattleMain.instance.getCurStage();
            if (null == stageRefObj)
                return false;

            //获取障碍物
            _AUTObstacleBase obstacle = _getObstacle(stageRefObj.getRandomObstacleType());
            if (null == obstacle)
                return false;

            obstacle.transform.SetParent(_m_initParTrans);
            //随机一个y轴距离
            float yMargin = 0;
            float xStartX = 0;
            if (null != stageRefObj)
            {
                yMargin = GCommon.calBattleRealPosY(stageRefObj.getRandomY());
                xStartX = GCommon.calBattleRealPosX(stageRefObj.getRandomX());
            }

            //设置障碍物位置
            _m_lastObInitY += yMargin;
            obstacle.setPos(new Vector2(_m_lastObInitY, xStartX));
            //设置数据
            obstacle.setData(stageRefObj.getRandomObstacleRefObj(), stageRefObj.getRandomColor());
            return true;
        }

        /// <summary>
        /// 回收单个障碍物
        /// </summary>
        public void pushBackObstacel(_AUTObstacleBase _obstacle)
        {
            if (!_check())
                return;

            if (null == _obstacle)
                return;


            UTObstacleCache cache = null;
            _m_obstacleCacheDic.TryGetValue(_obstacle.eObstacleType, out cache);
            if (null == cache)
                return;

            cache.pushBackCacheItem(_obstacle);
        }


        public void reset()
        {
            if (!_check())
                return;

            stopTick();
            foreach (KeyValuePair<EObstacleType, UTObstacleCache> valuePair in _m_obstacleCacheDic)
            {
                if (valuePair.Value == null)
                    continue;

                valuePair.Value.pushBackAllCacheItems();
            }

            _m_showObstacleList.Clear();
        }

        private _AUTObstacleBase _getObstacle(EObstacleType _type,long _stageId)
        {
            if (!_check())
                return null;

            if (_type == EObstacleType.NONE)
                return null;

            //获取障碍物类型配表
            UTObstacleTypeRefObj typeRef = GRefdataCoreMgr.instance.obstacleTypeListCore.getRef((long)_type);
            //获取可生成的障碍物 需要过滤当前阶段
            if(null == typeRef)
                return null;

            //当前可生成的障碍物列表
            List<UTObstacleRefObj> enableObstacleList = new List<UTObstacleRefObj>();;
            for (int i = 0; i < typeRef.obstacle_id_list.Count; i++)
            {
                long obstacleId = typeRef.obstacle_id_list[i];
                UTObstacleRefObj refObj = GRefdataCoreMgr.instance.obstacleListCore.getRef(obstacleId);
                if(null == refObj)
                    continue;
                
                if(refObj.stage_id == _stageId)
                    enableObstacleList.Add(refObj);
            }

            UTObstacleRefObj randomObstacleRefObj = GCommon.getRandom(enableObstacleList);
            UTObstacleCache cache = null;
            _m_obstacleCacheDic.TryGetValue(randomObstacleRefObj.id, out cache);
            if (null == cache)
            {
                //获取加载的prefab
                GameObject tempObj = UTYooAssetMgr.instance.loadRefdataObjAsset().in
                cache = new UTObstacleCache(10, 20);
                cache.init();
                _m_obstacleCacheDic.Add(randomObstacleRefObj.id, cache);
            }
                return null;

            return cache.popItem();
        }

        private bool _check()
        {
            if (null == _m_initParTrans || !_m_bIsInit)
                return false;

            return true;
        }
    }
}