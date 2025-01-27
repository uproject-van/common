using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 一个管理界面的基类node
    /// </summary>
    public abstract class _AUIQueueBaseNode
    {
        public abstract EUIQueueNodeType eNodeType { get; }
        
        //退出是否要销毁
        public virtual bool isExitDiscard
        {
            get { return true; }
        }

        public abstract EUIQueueNode eQueueNode { get; }
        
        public abstract string monoGoName { get; }

        protected GameObject _m_monoGo;
        
        /// <summary>
        /// 进入node
        /// </summary>
        public void enter()
        {
            //重新设置一下父节点
            if (null != _m_monoGo)
            {
                _showGo();
                return;
            }
            
            UTYooAssetMgr.instance.LoadAssetAsync(monoGoName, (_isSuc, _obj) =>
            {
                if(!_isSuc || null == _obj)
                    return;

                _m_monoGo = UGUICommon.cloneGameObj(_obj as GameObject);
                _showGo();
            });
        }

        public void hide()
        {
            _hideGo();
        }
        
        /// <summary>
        /// 退出node
        /// </summary>
        public void exit()
        {
            //如果退出不销毁go
            if (!isExitDiscard)
            {
                _hideGo();
                return;;
            }
            
            UGUICommon.releaseGameObj(_m_monoGo);
            _m_monoGo = null;
        }
        
        private void _showGo()
        {
            if (null == _m_monoGo)
                return;
            
            RectTransform parTrans = _getCanvasNode(eQueueNode);
            if (null == parTrans)
                return;
            
            _m_monoGo.transform.SetParent(parTrans,false);
            UGUICommon.setGameObjEnable(_m_monoGo);
            _m_monoGo.transform.SetAsLastSibling();
        }
        
        private void _hideGo()
        {
            UGUICommon.setGameObjEnable(_m_monoGo,false);
        }
        
        private RectTransform _getCanvasNode(EUIQueueNode _nodeE)
        {
            if (GameMain.instance.canvasGolist.Count < 3)
            {
                UTLog.Error("!!! Lunch的画布配置不正确 清检查 !!!");
                return null;
            }
            
            return GameMain.instance.canvasGolist[(int)_nodeE];
        }
    }
}