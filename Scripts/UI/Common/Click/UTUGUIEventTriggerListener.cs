using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UTGame
{
    /// <summary>
    /// UGUI中的事件监听对象
    /// </summary>
    public class UTUGUIEventTriggerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IMoveHandler
    {
        //上次点击帧数
        private static int _g_lastClickFrame;

        //重置当前点击判断帧数
        public static void g_resetLastClickFrame()
        {
            _g_lastClickFrame = -1;
        }

        /** 回调函数对象 */
        private Action<GameObject> _m_dOnClick;

        private Action<bool, PointerEventData> _m_dOnPress;

        /** 长按事件 */
        private Action _m_dOnLongProess;

        private float _m_fLongPressDuration = 0.5f; //长按事件触发的时间条件

        private bool _m_bIsMove = false; //是否移动了，当移动超出距离则不触发长按
        private int _m_iPointerId = -1; //按下的操作点Id
        private bool _m_bIsPointerDown = false; //是否按下
        private bool _m_bLongPressTriggered = false; //长按事件是否触发
        private float _m_fLongPressTriggerTime; //触发长按的时间标记
        private bool _m_bTriggerPointerEventAgain = false; //是否重新触发PointerDown/Up/Click事件，让被盖住的UI也能监听到
        private int _m_iOneClickPerFrame = UTConst.CLICK_INTERVAL_FRAME; //点击间隔帧数
        private Vector2 _m_vPressPos;
        private Vector2 _m_vTempMovDis;

        private void Update()
        {
            //满足条件触发长按
            if (_m_bIsPointerDown && !_m_bIsMove && !_m_bLongPressTriggered)
            {
                if (Time.unscaledTime >= _m_fLongPressTriggerTime)
                {
                    _m_bLongPressTriggered = true;
                    try
                    {
                        if (null != _m_dOnLongProess)
                            _m_dOnLongProess();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError("UGUIEventTriggerListener OnLongPress has Exception:\n" + e);
                        if (GameMain.instance != null)
                        {
                            GameMain.instance.onUnKnowErrorOccurred(e);
                        }
                    }
                }
            }
        }

        public void OnDisable()
        {
            _m_bIsMove = false;
            _m_iPointerId = -1;
            _m_bIsPointerDown = false;
            _m_bLongPressTriggered = false;
            _m_fLongPressTriggerTime = 0;
        }

        public static UTUGUIEventTriggerListener Get(GameObject _go, bool _triggerEventAgain = false,
            int _oneClickPerFrame = UTConst.CLICK_INTERVAL_FRAME)
        {
            if (null == _go)
                return null;

            UTUGUIEventTriggerListener listener = _go.GetComponent<UTUGUIEventTriggerListener>();
            if (null == listener)
                listener = _go.AddComponent<UTUGUIEventTriggerListener>();
            listener._m_bTriggerPointerEventAgain = _triggerEventAgain;
            listener._m_iOneClickPerFrame = _oneClickPerFrame;

            return listener;
        }

        public void regOnClick(Action<GameObject> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnClick)
                _m_dOnClick = _delegate;
            else
                _m_dOnClick += _delegate;
        }

        public void unregOnClick(Action<GameObject> _delegate)
        {
            if (null == _m_dOnClick)
                return;

            _m_dOnClick -= _delegate;
        }

        public void regOnPress(Action<bool, PointerEventData> _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnPress)
                _m_dOnPress = _delegate;
            else
                _m_dOnPress += _delegate;
        }

        public void unregOnPress(Action<bool, PointerEventData> _delegate)
        {
            if (null == _m_dOnPress)
                return;

            _m_dOnPress -= _delegate;
        }

        public void regOnLongPress(Action _delegate)
        {
            if (null == _delegate)
                return;

            if (null == _m_dOnLongProess)
                _m_dOnLongProess = _delegate;
            else
                _m_dOnLongProess += _delegate;
        }

        public void unregOnLongPress(Action _delegate)
        {
            if (null == _m_dOnLongProess)
                return;

            _m_dOnLongProess -= _delegate;
        }

        public void OnMove(AxisEventData eventData)
        {
            //没有按下且没有触发长按时处理本函数
            if (_m_bIsPointerDown && !_m_bIsMove && !_m_bLongPressTriggered)
            {
                _m_vTempMovDis = eventData.moveVector - _m_vPressPos;
                if (Math.Abs(_m_vTempMovDis.x) > Screen.width / 20
                    || Math.Abs(_m_vTempMovDis.y) > Screen.height / 20)
                {
                    _m_bIsMove = true;
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            //判断操作Id是否一致
            if (_m_iPointerId != eventData.pointerId)
                return;

            //如果已经触发了长按则不继续处理
            if (_m_bLongPressTriggered)
                return;

            //如果每帧只能点击一次，并且这一帧已经点击过了，则不处理
            if (_m_iOneClickPerFrame > 0)
            {
                int curFrameCount = Time.frameCount;
                //不是同一个事件的情况下，点击帧数间隔小于配置间隔才不处理点击
                if (curFrameCount - _g_lastClickFrame < _m_iOneClickPerFrame)
                    return;

                _g_lastClickFrame = curFrameCount; //记录这一次点击的帧数
            }

            try
            {
                //TODO 按钮点击时统一判断服务器版本过低或者热更版本过低的处理
                //长按的状态下不触发点击事件
                if (null != _m_dOnClick)
                    _m_dOnClick(gameObject);

                if (_m_bTriggerPointerEventAgain && transform.parent != null)
                {
                    ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData,
                        ExecuteEvents.pointerClickHandler);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UGUIEventTriggerListener OnPointerClick has Exception:\n" + e);
                if (GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //判断是否可操作
            if (!_interactable)
                return;

            //已经有按键按下则不处理
            if (_m_bIsPointerDown)
                return;

            _m_bIsMove = false;
            _m_iPointerId = eventData.pointerId;
            _m_fLongPressTriggerTime = Time.unscaledTime + _m_fLongPressDuration;
            _m_bIsPointerDown = true;
            _m_bLongPressTriggered = false;
            _m_vPressPos = eventData.pressPosition;

            try
            {
                if (null != _m_dOnPress)
                    _m_dOnPress(true, eventData);

                if (_m_bTriggerPointerEventAgain && transform.parent != null)
                {
                    ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData,
                        ExecuteEvents.pointerDownHandler);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UGUIEventTriggerListener OnPointerDown has Exception:\n" + e);
                if (GameMain.instance != null)
                {
                    GameMain.instance.onUnKnowErrorOccurred(e);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //无按键按下则不处理
            if (!_m_bIsPointerDown)
                return;

            //判断操作Id是否一致
            if (_m_iPointerId != eventData.pointerId)
                return;

            //重置参数
            _m_bIsMove = false;
            //_m_iPointerId = -1;
            _m_bIsPointerDown = false;
            _m_fLongPressTriggerTime = 0;

            try
            {
                if (null != _m_dOnPress)
                    _m_dOnPress(false, eventData);

                if (_m_bTriggerPointerEventAgain && transform.parent != null)
                {
                    ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData,
                        ExecuteEvents.pointerUpHandler);
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("UGUIEventTriggerListener OnPointerUp has Exception:\n" + e);
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