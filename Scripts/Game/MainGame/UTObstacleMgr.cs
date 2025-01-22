using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的管理类
    /// </summary>
    public class UTObstacleMgr
    {
        private static UTObstacleMgr _g_instance;

        private bool _m_bIsInit;
        
        /// <summary>
        /// 所有的碰撞体缓存类
        /// </summary>
        private Dictionary<EObstacleType, UTObstacleCache> _m_obstacleCacheDic;
        
        [NotNull] public static UTObstacleMgr instance
        {
            get
            {
                if (_g_instance == null)
                {
                    _g_instance = new UTObstacleMgr();
                }
                return _g_instance;
            }
        }

        public void init()
        {
            if(_m_bIsInit)
                return;

            _m_bIsInit = true;
            _m_obstacleCacheDic = new Dictionary<EObstacleType, UTObstacleCache>();
            //先把缓存类构建好
            for (int i = 0; i < Enum.GetValues(typeof(EObstacleType)).Length; i++)
            {
                EObstacleType type= (EObstacleType) i;
                if(type == EObstacleType.NONE)
                    continue;

                UTObstacleCache cache = new UTObstacleCache(10, 50);
                _m_obstacleCacheDic.Add(type,cache);
            }
        }

        //开始生成障碍物
        public void start(Transform _parTrans)
        {
            if (null == _parTrans) 
                return;
            
            //生成规则通过阶段配表
            
            //TODO 当前阶段允许生成的障碍物类型
            EObstacleType type = EObstacleType.FIXED;
            //TODO 当前阶段允许生成的障碍物颜色
            EColor color = EColor.RED;
            //获取障碍物
            _AUTObstacleBase obstacle = _getObstacle(type);
            obstacle.setData(color);
            //TODO根据当前阶段获取生成的位置信息
            
            //TODO设置障碍物位置
        }
        
        public void reset()
        {
            foreach (KeyValuePair<EObstacleType,UTObstacleCache> valuePair in _m_obstacleCacheDic)
            {
                if(valuePair.Value == null)
                    continue;
                
                valuePair.Value.pushBackAllCacheItems();
            }
        }
        
        private _AUTObstacleBase _getObstacle(EObstacleType _type)
        {
            if(_m_bIsInit || _type == EObstacleType.NONE)
                return null;

            UTObstacleCache cache = null;
            _m_obstacleCacheDic.TryGetValue(_type,out cache);
            if(null == cache)
                return null;

            return cache.popItem();
        }
    }
}