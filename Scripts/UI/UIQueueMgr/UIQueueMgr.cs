using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// UI视图管理类
    /// </summary>
    public class UIQueueMgr
    {
        private static UIQueueMgr _g_instance;

        [NotNull]
        public static UIQueueMgr instance
        {
            get
            {
                if (_g_instance == null)
                {
                    _g_instance = new UIQueueMgr();
                }

                return _g_instance;
            }
        }

        /// <summary>
        /// 不销毁的node列表
        /// </summary>
        private Dictionary<EUIQueueNodeType, _AUIQueueBaseNode> _m_nodeCacheDic;

        /// <summary>
        /// 当前打开的node列表
        /// </summary>
        private List<_AUIQueueBaseNode> _m_nodeOpenList;

        public void init()
        {
            _m_nodeCacheDic = new Dictionary<EUIQueueNodeType, _AUIQueueBaseNode>();
            _m_nodeOpenList = new List<_AUIQueueBaseNode>();
        }

        //打开一个窗口
        public void addNode(_AUIQueueBaseNode _node)
        {
            if (null == _node)
                return;

            //如果这个node在之前打开过则直接show
            _AUIQueueBaseNode hasNode = _getOpenNode(_node.eNodeType);
            //如果这个node在缓存中 则从缓存中直接取
            if (null == hasNode)
                hasNode = _getCacheNode(_node.eNodeType);

            //如果都没有则用传入的node
            if (null == hasNode)
                hasNode = _node;

            //把上一个node隐藏掉
            if (_m_nodeOpenList.Count > 0)
            {
                _AUIQueueBaseNode lastNode = _m_nodeOpenList[_m_nodeOpenList.Count - 1];
                lastNode.hide();
            }
            
            //添加到当前的node打开列表中
            _m_nodeOpenList.Add(hasNode);
            hasNode.enter();
        }

        //退出一个窗口
        public void removeNode(EUIQueueNodeType _nodeType)
        {
            //从当前打开的node中寻找
            _AUIQueueBaseNode node = _getOpenNode(_nodeType);
            if(null == node)
                return;

            if(!node.isExitDiscard && null == _getCacheNode(_nodeType))
                _m_nodeCacheDic.Add(_nodeType,node);
            
            _m_nodeOpenList.Remove(node);

            node.exit();
        }

        private _AUIQueueBaseNode _getOpenNode(EUIQueueNodeType _nodeType)
        {
            _AUIQueueBaseNode node = null;
            for (int i = _m_nodeOpenList.Count - 1; i >= 0; i--)
            {
                node = _m_nodeOpenList[i];
                if (null == node)
                {
                    _m_nodeOpenList.RemoveAt(i);
                    continue;
                }

                if (node.eNodeType == _nodeType)
                    return node;
            }

            return null;
        }

        private _AUIQueueBaseNode _getCacheNode(EUIQueueNodeType _nodeType)
        {
            _AUIQueueBaseNode node = null;
            _m_nodeCacheDic.TryGetValue(_nodeType, out node);
            return node;
        }
    }
}