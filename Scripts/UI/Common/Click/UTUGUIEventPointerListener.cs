using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UTGame
{
    /// <summary>
    /// UGUI中的事件监听对象
    /// </summary>
    public class UTUGUIEventPointerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        /** 回调函数对象 */
        public Action<Vector2> _m_dOnPointerEnter;

        public Action<Vector2> _m_dOnPointerExit;
        public Action<Vector2> _m_dOnPointerDown;
        public Action<Vector2> _m_dOnPointerUp;

        /***************
         * 获取事件监听对象
         **/
        public static UTUGUIEventPointerListener Get(GameObject _go)
        {
            if (null == _go)
                return null;

            UTUGUIEventPointerListener listener = _go.GetComponent<UTUGUIEventPointerListener>();
            if (null == listener)
                listener = _go.AddComponent<UTUGUIEventPointerListener>();

            return listener;
        }

        public void regOnPointerEnter(Action<Vector2> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnPointerEnter)
                _m_dOnPointerEnter = _delegate;
            else
                _m_dOnPointerEnter += _delegate;
        }

        public void unregOnPointerEnter(Action<Vector2> _delegate)
        {
            if (null == _m_dOnPointerEnter)
                return;

            _m_dOnPointerEnter -= _delegate;
        }

        public void regOnPointerExit(Action<Vector2> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnPointerExit)
                _m_dOnPointerExit = _delegate;
            else
                _m_dOnPointerExit += _delegate;
        }

        public void unregOnPointerExit(Action<Vector2> _delegate)
        {
            if (null == _m_dOnPointerExit)
                return;

            _m_dOnPointerExit -= _delegate;
        }

        public void regOnPointerDown(Action<Vector2> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnPointerDown)
                _m_dOnPointerDown = _delegate;
            else
                _m_dOnPointerDown += _delegate;
        }

        public void unregOnPointerDown(Action<Vector2> _delegate)
        {
            if (null == _m_dOnPointerDown)
                return;

            _m_dOnPointerDown -= _delegate;
        }

        public void regOnPointerUp(Action<Vector2> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnPointerUp)
                _m_dOnPointerUp = _delegate;
            else
                _m_dOnPointerUp += _delegate;
        }

        public void unregOnPointerUp(Action<Vector2> _delegate)
        {
            if (null == _m_dOnPointerUp)
                return;

            _m_dOnPointerUp -= _delegate;
        }

        public void OnPointerEnter(PointerEventData _EventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            if (null != _m_dOnPointerEnter)
                _m_dOnPointerEnter(_EventData.position);
        }

        public void OnPointerExit(PointerEventData _EventData)
        {
            if (null != _m_dOnPointerExit)
                _m_dOnPointerExit(_EventData.position);
        }

        public void OnPointerDown(PointerEventData _EventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            try
            {
                if (null != _m_dOnPointerDown)
                    _m_dOnPointerDown(_EventData.position);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UGUIEventPointerListener OnPointerDown has Exception:\n" + e);
                if (GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            try
            {
                if (null != _m_dOnPointerUp)
                    _m_dOnPointerUp(eventData.position);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UGUIEventPointerListener OnPointerUp has Exception:\n" + e);
                if (GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
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