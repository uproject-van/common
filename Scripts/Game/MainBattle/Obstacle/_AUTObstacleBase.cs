using UnityEngine;
using UnityEngine.UI;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的基类
    /// </summary>
    public abstract class _AUTObstacleBase : _AMonoBase
    {
        [Header("加载父节点")]
        public Transform parTrans;
        
        //随机出来的尺寸大小
        protected EObstacleSize _m_eObstacleSize;
        
        //加载出来的Go
        protected GameObject _m_loadGo;
        
        protected EColor _m_eColor;
        protected UTObstacleRefObj _m_obstacleRefObj;

        protected abstract void _refreshEx();
        public EObstacleType eObstacleType
        {
            get { return null == _m_obstacleRefObj ? EObstacleType.NONE : _m_obstacleRefObj.type; }
        }

        public void setData(UTObstacleRefObj _refObj, EColor _color)
        {
            _m_obstacleRefObj = _refObj;
            _m_eColor = _color;
            _m_eObstacleSize = GCommon.getRandom<EObstacleSize>();
            _refresh();
        }

        /// <summary>
        /// 设置自身的位置
        /// </summary>
        /// <param name="_pos"></param>
        public void setPos(Vector2 _pos)
        {
            if(null == transform)
                return;
            
            transform.position = _pos;
            //重置一下位置
            if (null != _m_loadGo)
            {
                _m_loadGo.transform.localPosition = Vector3.zero;
            }
        }

        private void _refresh()
        {
            if(null == _m_obstacleRefObj || null == parTrans)
                return;
            
            if (null != _m_loadGo)
            {
                _refreshEx();
                return;
            }
            
            UTYooAssetMgr.instance.LoadAssetAsync(_m_obstacleRefObj.prefab_name, (_isSuc, _go) =>
            {
                if (!_isSuc || null == _go)
                    return;
                
                GameObject go = _go as GameObject;
                GameObject cloneGo =  UGUICommon.cloneGameObj(go);
                if(null == cloneGo)
                    return;
                
                cloneGo.transform.SetParent(parTrans);
                _m_loadGo = cloneGo;
                _refreshEx();
            });
        }
        
        /// <summary>
        /// 不同类型的障碍物触发不同的效果
        /// </summary>
        protected abstract void _dealEffect();
        
        public abstract Vector2 getSize();
    }
}