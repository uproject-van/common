using System;
using System.Collections.Generic;
using UnityEngine;
using WLDDZ;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的管理类
    /// </summary>
    public class UTObstacleMgr : _IUTBaseTaskInterface
    {
        //是否初始化
        private bool _m_bIsInit;

        //最远的一个障碍物生成位置 这是世界位置
        private float _m_lastObInitY;

        //当前需要生成的障碍物位置 这是世界位置
        private float _m_needObInitY;

        private Transform _m_initParTrans;

        /// <summary>
        /// 所有的碰撞体缓存类
        /// </summary>
        private Dictionary<EObstacleType, UTObstacleCache> _m_obstacleCacheDic;

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

        public bool isInit
        {
            get { return _m_bIsInit; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init(Action _doneAction)
        {
            if (_m_bIsInit)
            {
                if (null != _doneAction)
                    _doneAction();

                return;
            }

            //当前已经生成的位置
            _m_lastObInitY = 0;
            _m_needObInitY = -UTBattleMain.instance.worldPerH * 20;

            _m_showObstacleList = new List<_AUTObstacleBase>();
            _m_obstacleCacheDic = new Dictionary<EObstacleType, UTObstacleCache>();

            int typeLength = Enum.GetValues(typeof(EObstacleType)).Length;
            SimpleStepCounter stepCounter = new SimpleStepCounter();
            stepCounter.chgTotalStepCount(typeLength);
            stepCounter.regAllDoneDelegate(() =>
            {
                _m_bIsInit = true;
                if (null != _doneAction)
                    _doneAction();
            });

            //先把缓存类构建好
            for (int i = 0; i < typeLength; i++)
            {
                EObstacleType type = (EObstacleType)i;
                if (type == EObstacleType.NONE)
                {
                    stepCounter.addDoneStepCount();
                    continue;
                }

                UTObstacleTypeRefObj typeRefObj = GRefdataCoreMgr.instance.obstacleTypeListCore.getRef((int)type);
                if (null == typeRefObj)
                {
                    stepCounter.addDoneStepCount();
                    continue;
                }

                UTYooAssetMgr.instance.LoadAssetAsync(typeRefObj.prefab_name, (_isSuc, _go) =>
                {
                    stepCounter.addDoneStepCount();
                    if (!_isSuc || null == _go)
                        return;

                    GameObject go = _go as GameObject;
                    if (null == go)
                        return;

                    UTObstacleCache cache = new UTObstacleCache(10, 30);
                    cache.init(go);
                    _m_obstacleCacheDic.Add(type, cache);
                });
            }
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
            for (int i = _m_showObstacleList.Count - 1; i >= 0; i--)
            {
                temp = _m_showObstacleList[i];
                if (null == temp)
                {
                    _m_showObstacleList.RemoveAt(i);
                    continue;
                }

                //如果物体超出一个半屏幕则回收 TODO 这里可能会有问题
                if (temp.transform.position.y > _m_needObInitY + UTBattleMain.instance.worldPerH * 30)
                {
                    pushBackObstacel(temp);
                    _m_showObstacleList.RemoveAt(i);
                }
            }

            while (_m_lastObInitY > _m_needObInitY)
            {
                bool isSuc = popObstacle();
                if (!isSuc)
                {
                    stopTick();
                    break;
                }
            }
        }

        public void addNeedObInitY(float _addY)
        {
            _m_needObInitY -= _addY;
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

            //获取障碍物数据
            EObstacleType randomType = stageRefObj.getRandomObstacleType();
            _AUTObstacleBase obstacle = _getObstacle(randomType);
            if (null == obstacle)
                return false;

            //设置数据 先根据类型获取障碍物样式id 再过滤掉当前阶段不可生成的id
            UTObstacleTypeRefObj typeRef = GRefdataCoreMgr.instance.obstacleTypeListCore.getRef((int)randomType);
            List<UTObstacleRefObj> enableObstacleList = new List<UTObstacleRefObj>();
            ;
            for (int i = 0; i < typeRef.obstacle_id_list.Count; i++)
            {
                long obstacleId = typeRef.obstacle_id_list[i];
                UTObstacleRefObj refObj = GRefdataCoreMgr.instance.obstacleListCore.getRef(obstacleId);
                if (null == refObj || refObj.stage_id != stageRefObj.stage_id)
                    continue;

                enableObstacleList.Add(refObj);
            }

            obstacle.setData(GCommon.getRandom(enableObstacleList), stageRefObj.getRandomColor());

            _setObstaclePos(obstacle, stageRefObj);
            _m_showObstacleList.Add(obstacle);
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

            cache.pushBackCacheItem(_obstacle.gameObject);
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

        private _AUTObstacleBase _getObstacle(EObstacleType _type)
        {
            if (!_check())
                return null;

            if (_type == EObstacleType.NONE)
                return null;

            UTObstacleCache cache = null;
            _m_obstacleCacheDic.TryGetValue(_type, out cache);
            if (null == cache)
                return null;

            GameObject go = cache.popItem();
            _AUTObstacleBase wnd = go.GetComponent<_AUTObstacleBase>();
            return wnd;
        }

        private void _setObstaclePos(_AUTObstacleBase _obstacle, UTStageRefObj _stageRefObj)
        {
            if (null == _obstacle || null == _stageRefObj)
                return;

            //计算位置
            _obstacle.transform.SetParent(_m_initParTrans);
            //随机一个y轴距离 UI位置
            float yMargin = 0;
            float xMargin = 0;
            if (null != _stageRefObj)
            {
                yMargin = _stageRefObj.getRandomY() * UTBattleMain.instance.worldPerH;
                xMargin = _stageRefObj.getRandomX() * UTBattleMain.instance.worldPerW;
                float halfSizeX = _obstacle.getSize().x / 2f;
                if (xMargin < halfSizeX)
                    xMargin = halfSizeX;

                if (xMargin + halfSizeX > GCommon.getWorldWidth())
                    xMargin = GCommon.getWorldWidth() - halfSizeX;
            }

            //设置障碍物位置
            _m_lastObInitY -= yMargin;
            _obstacle.setPos(new Vector2(UTBattleMain.instance.startPos.x + xMargin, _m_lastObInitY));

            _m_lastObInitY -= _obstacle.getSize().y / 2f;
        }

        private bool _check()
        {
            if (null == _m_initParTrans || !_m_bIsInit)
                return false;

            return true;
        }
    }
}