using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UTGame
{
    /// <summary>
    /// UGUI中的事件监听拖拽对象
    /// </summary>
    public class UTUGUIEventDragListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IMoveHandler
    {
        /** 回调函数对象 */
        public Action<PointerEventData> _m_dOnBeginDrag;

        public Action<PointerEventData> _m_dOnDrag;
        public Action<PointerEventData> _m_dOnEndDrag;

        public Action<Vector2> _m_dOnMove;


        /***************
         * 获取事件监听对象
         **/
        public static UTUGUIEventDragListener Get(GameObject _go)
        {
            if (null == _go)
                return null;

            UTUGUIEventDragListener listener = _go.GetComponent<UTUGUIEventDragListener>();
            if (null == listener)
                listener = _go.AddComponent<UTUGUIEventDragListener>();

            return listener;
        }

        public void regOnBeginDrag(Action<PointerEventData> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnBeginDrag)
                _m_dOnBeginDrag = _delegate;
            else
                _m_dOnBeginDrag += _delegate;
        }

        public void unregOnBeginDrag(Action<PointerEventData> _delegate)
        {
            if (null == _m_dOnBeginDrag)
                return;

            _m_dOnBeginDrag -= _delegate;
        }

        public void regOnDrag(Action<PointerEventData> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnDrag)
                _m_dOnDrag = _delegate;
            else
                _m_dOnDrag += _delegate;
        }

        public void unregOnDrag(Action<PointerEventData> _delegate)
        {
            if (null == _m_dOnDrag)
                return;

            _m_dOnDrag -= _delegate;
        }

        public void regOnEndDrag(Action<PointerEventData> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnEndDrag)
                _m_dOnEndDrag = _delegate;
            else
                _m_dOnEndDrag += _delegate;
        }

        public void unregOnEndDrag(Action<PointerEventData> _delegate)
        {
            if (null == _m_dOnEndDrag)
                return;

            _m_dOnEndDrag -= _delegate;
        }

        public void regOnMove(Action<Vector2> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnMove)
                _m_dOnMove = _delegate;
            else
                _m_dOnMove += _delegate;
        }

        public void unregOnMove(Action<Vector2> _delegate)
        {
            if (null == _m_dOnMove)
                return;

            _m_dOnMove -= _delegate;
        }

        public void OnBeginDrag(PointerEventData _EventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            if (null != _m_dOnBeginDrag)
                _m_dOnBeginDrag(_EventData);
        }

        public void OnDrag(PointerEventData _EventData)
        {
            if (null != _m_dOnDrag)
                _m_dOnDrag(_EventData);
        }

        public void OnEndDrag(PointerEventData _EventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            if (null != _m_dOnEndDrag)
                _m_dOnEndDrag(_EventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (null != _m_dOnMove)
                _m_dOnMove(eventData.moveVector);
        }

        /** 获取是否可操作 */
        protected bool _interactable
        {
            get
            {
                //判断是否可操作
                Selectable selectableMono = GetComponent<Selectable>();
                if (null != selectableMono && !selectableMono.interactable)
                    return false;

                return true;
            }
        }
    }
}