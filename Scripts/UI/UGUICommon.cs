using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UTGame
{
    public class UGUICommon
    {
        #region 为GO对象增加点击响应函数

        public static void combineBtnClick(GameObject _go, Action<GameObject> _delegate)
        {
            combineBtnClick(_go, _delegate, false, true);
        }

        public static void combineBtnClick(GameObject _go, Action<GameObject> _delegate,
            bool _triggerEventAgain = false, bool _oneClickPerFrame = true)
        {
            if (null == _go)
                return;

            //设置操作对象
            UTUGUIEventTriggerListener listener =
                UTUGUIEventTriggerListener.Get(_go, _triggerEventAgain, _oneClickPerFrame ? 1 : 0);
            listener.regOnClick(_delegate);
        }

        public static void combineBtnClick(GameObject _go, Action<GameObject> _delegate,
            bool _triggerEventAgain = false, int _oneClickPerFrame = UTConst.CLICK_INTERVAL_FRAME)
        {
            if (null == _go)
                return;

            //设置操作对象
            UTUGUIEventTriggerListener listener =
                UTUGUIEventTriggerListener.Get(_go, _triggerEventAgain, _oneClickPerFrame);
            listener.regOnClick(_delegate);
        }

        public static void uncombineBtnClick(GameObject _go, Action<GameObject> _delegate)
        {
            if (null == _go)
                return;

            //设置操作对象
            UTUGUIEventTriggerListener.Get(_go).unregOnClick(_delegate);
        }

        #endregion

        #region 为GO对象增加Press响应函数

        public static void combineBtnPress(GameObject _go, Action<bool, PointerEventData> _delegate,
            bool _triggerEventAgain = false)
        {
            if (null == _go)
                return;

            UTUGUIEventTriggerListener listener = UTUGUIEventTriggerListener.Get(_go, _triggerEventAgain);
            listener.regOnPress(_delegate);
        }

        public static void uncombineBtnPress(GameObject _go, Action<bool, PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventTriggerListener.Get(_go).unregOnPress(_delegate);
        }

        public static void combineBtnLongPress(GameObject _go, Action _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventTriggerListener.Get(_go).regOnLongPress(_delegate);
        }

        public static void uncombineBtnLongPress(GameObject _go, Action _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventTriggerListener.Get(_go).unregOnLongPress(_delegate);
        }

        #endregion

        #region 为GO对象增加Drag响应函数

        public static void combineBeginDrag(GameObject _go, Action<PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).regOnBeginDrag(_delegate);
        }

        public static void combineDrag(GameObject _go, Action<PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).regOnDrag(_delegate);
        }

        public static void combineEndDrag(GameObject _go, Action<PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).regOnEndDrag(_delegate);
        }

        public static void combineMove(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).regOnMove(_delegate);
        }

        public static void uncombineBeginDrag(GameObject _go, Action<PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).unregOnBeginDrag(_delegate);
        }

        public static void uncombineDrag(GameObject _go, Action<PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).unregOnDrag(_delegate);
        }

        public static void uncombineEndDrag(GameObject _go, Action<PointerEventData> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).unregOnEndDrag(_delegate);
        }

        public static void uncombineMove(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventDragListener.Get(_go).unregOnMove(_delegate);
        }

        #endregion

        #region 为GO对象增加Pointer响应函数

        public static void combinePointerEnter(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).regOnPointerEnter(_delegate);
        }

        public static void combinePointerExit(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).regOnPointerExit(_delegate);
        }

        public static void combinePointerDown(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).regOnPointerDown(_delegate);
        }

        public static void combinePointerUp(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).regOnPointerUp(_delegate);
        }

        public static void uncombinePointerEnter(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).unregOnPointerEnter(_delegate);
        }

        public static void uncombinePointerExit(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).unregOnPointerExit(_delegate);
        }

        public static void uncombinePointerDown(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).unregOnPointerDown(_delegate);
        }

        public static void uncombinePointerUp(GameObject _go, Action<Vector2> _delegate)
        {
            if (null == _go)
                return;

            UTUGUIEventPointerListener.Get(_go).unregOnPointerUp(_delegate);
        }

        #endregion

        #region 设置游戏物件有效性

        public static void setGameObjEnable(MaskableGraphic _uiObj, bool _enable)
        {
            if (null == _uiObj || null == _uiObj.gameObject || _enable == _uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(_enable);
        }

        public static void setGameObjEnable(Transform _uiObj, bool _enable)
        {
            if (null == _uiObj || null == _uiObj.gameObject || _enable == _uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(_enable);
        }

        public static void setGameObjEnable(MonoBehaviour _uiObj, bool _enable)
        {
            if (null == _uiObj || null == _uiObj.gameObject || _enable == _uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(_enable);
        }

        public static void setGameObjEnable(GameObject _go, bool _enable)
        {
            if (_enable)
                setGameObjEnable(_go);
            else
                setGameObjDisable(_go);
        }

        public static void setGameObjEnable(List<GameObject> _list, bool _enable)
        {
            if (null == _list)
                return;

            for (int i = 0; i < _list.Count; i++)
            {
                setGameObjEnable(_list[i], _enable);
            }
        }

        public static void setGameObjEnable(GameObject[] _array, bool _enable)
        {
            if (null == _array)
                return;

            for (int i = 0; i < _array.Length; i++)
            {
                setGameObjEnable(_array[i], _enable);
            }
        }

        public static void setGameObjDisable(GameObject _go)
        {
            if (null == _go || !_go.activeSelf)
                return;

            _go.SetActive(false);
        }

        public static void setGameObjDisable(MaskableGraphic _uiObj)
        {
            if (null == _uiObj || null == _uiObj.gameObject || !_uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(false);
        }

        public static void setGameObjDisable(RectTransform _uiObj)
        {
            if (null == _uiObj || null == _uiObj.gameObject || !_uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(false);
        }

        public static void setGameObjEnable(GameObject _go)
        {
            if (null == _go || _go.activeSelf)
                return;

            _go.SetActive(true);
        }

        public static void setGameObjEnable(MaskableGraphic _uiObj)
        {
            if (null == _uiObj || null == _uiObj.gameObject || _uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(true);
        }

        public static void setGameObjEnable(RectTransform _uiObj)
        {
            if (null == _uiObj || null == _uiObj.gameObject || _uiObj.gameObject.activeSelf)
                return;

            _uiObj.gameObject.SetActive(true);
        }

        #endregion

        #region 设置透明度和颜色

        public static void setUIObjColor(Graphic _graphic, Color _color)
        {
            if (null == _graphic)
                return;

            if (_graphic.color == _color)
                return;

            _color.a = _graphic.color.a;
            _graphic.color = _color;
        }

        public static void setUIObjColor(List<Graphic> _graphic, Color _color)
        {
            if (null == _graphic)
                return;

            for (int i = 0; i < _graphic.Count; i++)
                setUIObjColor(_graphic[i], _color);
        }

        public static void setUIObjColor(Graphic[] _graphic, Color _color)
        {
            if (null == _graphic)
                return;

            for (int i = 0; i < _graphic.Length; i++)
                setUIObjColor(_graphic[i], _color);
        }

        public static void setUIObjColor(SpriteRenderer _render, Color _color)
        {
            if (null == _render)
                return;

            if (_render.color == _color)
                return;

            _color.a = _render.color.a;
            _render.color = _color;
        }

        public static void setUIObjFullColor(SpriteRenderer _render, Color _color)
        {
            if (null == _render)
                return;

            if (_render.color == _color)
                return;

            _render.color = _color;
        }

        //设置完整颜色，包括透明度
        public static void setUIObjFullColor(Graphic _render, Color _color)
        {
            if (null == _render)
                return;

            if (_render.color == _color)
                return;

            _render.color = _color;
        }

        public static void setUIObjFullColor(List<Graphic> _graphic, Color _color)
        {
            if (null == _graphic)
                return;

            for (int i = 0; i < _graphic.Count; i++)
                setUIObjFullColor(_graphic[i], _color);
        }

        public static void setUIObjFullColor(Graphic[] _graphic, Color _color)
        {
            if (null == _graphic)
                return;

            for (int i = 0; i < _graphic.Length; i++)
                setUIObjFullColor(_graphic[i], _color);
        }

        public static void setUIObjAlpha(Graphic _graphic, float _alpha)
        {
            if (null == _graphic)
                return;

            if (_graphic.color.a == _alpha)
                return;

            Color newC = _graphic.color;
            newC.a = _alpha;
            _graphic.color = newC;
        }

        public static void setUIObjAlpha(SpriteRenderer _render, float _alpha)
        {
            if (null == _render)
                return;

            if (_render.color.a == _alpha)
                return;

            Color newC = _render.color;
            newC.a = _alpha;
            _render.color = newC;
        }

        public static void setUIObjAlpha(List<Graphic> _graphic, float _alpha)
        {
            if (null == _graphic)
                return;

            for (int i = 0; i < _graphic.Count; i++)
                setUIObjAlpha(_graphic[i], _alpha);
        }

        public static void setUIObjAlpha(CanvasGroup _graphic, float _alpha)
        {
            if (null == _graphic)
                return;

            _graphic.alpha = _alpha;
        }

        #endregion

        #region 设置图片

        public static void setSprite(Image _img, Sprite _sprite)
        {
            if (null == _img)
                return;

            //设置默认缺省图片 TODO 这里可以开个全局配置
            // if (null == _sprite)
            //     _sprite = Sprite. HallManager.GetSprite("bg_empty_01");

            if (_img.sprite == _sprite)
                return;

            _img.sprite = _sprite;
        }

        /// <summary>
        /// 用统一的读取图片方式
        /// </summary>
        /// <param name="_img"></param>
        /// <param name="_spriteStr"></param>
        public static void setSprite(Image _img, string _spriteStr)
        {
            if (string.IsNullOrEmpty(_spriteStr) || null == _img)
                return;

            Sprite spr = null;

            //设置默认缺省图片
            if (null == spr)
                spr = GameMain.instance.globeEmptySpr;

            if (_img.sprite == spr)
                return;

            _img.sprite = spr;
        }

        #endregion

        #region 设置文本

        public static void setLabelTxt(Text _label, string _txt)
        {
            if (null == _label)
                return;

            _label.text = _txt;
        }

        public static void setLabelTxt(Text _label, long _num)
        {
            if (null == _label)
                return;

            _label.text = _num.ToString();
        }

        #endregion

        #region 设置UI对象缩放

        public static void setUIObjScale(GameObject _go, float _scale)
        {
            if (null == _go || null == _go.transform)
                return;

            if (Math.Abs(_go.transform.localScale.x - _scale) < 0.001f)
                return;

            _go.transform.localScale = new Vector3(_scale, _scale, _scale);
        }

        public static void setUIObjScale(List<GameObject> _go, float _scale)
        {
            for (int i = 0; i < _go.Count; i++)
            {
                setUIObjScale(_go[i], _scale);
            }
        }

        public static void setUIObjScale(MonoBehaviour _mono, float _scale)
        {
            if (null == _mono || null == _mono.transform)
                return;

            if (Math.Abs(_mono.transform.localScale.x - _scale) < 0.001f)
                return;

            _mono.transform.localScale = new Vector3(_scale, _scale, _scale);
        }

        public static void setUIObjScale(MonoBehaviour _mono, Vector2 _scale)
        {
            if (null == _mono || null == _mono.transform)
                return;

            _mono.transform.localScale = new Vector3(_scale.x, _scale.y, 1);
        }

        public static void setUIObjScale(RectTransform _rectTrans, float _scale)
        {
            if (null == _rectTrans)
                return;

            if (Math.Abs(_rectTrans.localScale.x - _scale) < 0.001f)
                return;

            _rectTrans.localScale = new Vector3(_scale, _scale, _scale);
        }

        public static void setUIObjScale(RectTransform _rectTrans, Vector2 _scale)
        {
            if (null == _rectTrans)
                return;

            _rectTrans.localScale = new Vector3(_scale.x, _scale.y, 1);
        }

        public static void setUIObjScale(Transform _rectTrans, float _scale)
        {
            if (null == _rectTrans)
                return;

            if (Math.Abs(_rectTrans.localScale.x - _scale) < 0.001f)
                return;

            _rectTrans.localScale = new Vector3(_scale, _scale, _scale);
        }

        public static void setUIObjScale(Transform _rectTrans, Vector2 _scale)
        {
            if (null == _rectTrans)
                return;

            _rectTrans.localScale = new Vector3(_scale.x, _scale.y, 1);
        }

        #endregion

        #region 设置UI对象位置

        public static void setUIPos(GameObject _go, Vector2 _pos)
        {
            if (null == _go)
                return;

            //设置操作对象
            setUIPos(_go.transform as RectTransform, _pos);
        }

        public static void setUIPos(Transform _transform, Vector2 _pos)
        {
            if (null == _transform)
                return;

            //设置操作对象
            setUIPos(_transform as RectTransform, _pos);
        }

        public static void setUIPos(RectTransform _rectTrans, Vector2 _pos)
        {
            if (null == _rectTrans)
                return;

            //比对并判断是否需要调整
            if (Mathf.Abs(_rectTrans.anchoredPosition.x - _pos.x) < 0.5f
                && Mathf.Abs(_rectTrans.anchoredPosition.y - _pos.y) < 0.5f)
                return;

            //设置位置
            _rectTrans.anchoredPosition = _pos;
        }

        public static void setUIPos(GameObject _go, float _x, float _y)
        {
            if (null == _go)
                return;

            //设置操作对象
            setUIPos(_go.transform as RectTransform, _x, _y);
        }

        public static void setUIPos(Transform _transform, float _x, float _y)
        {
            if (null == _transform)
                return;

            //设置操作对象
            setUIPos(_transform as RectTransform, _x, _y);
        }

        public static void setUIPos(RectTransform _rectTrans, float _x, float _y)
        {
            if (null == _rectTrans)
                return;

            //比对并判断是否需要调整
            if (Mathf.Abs(_rectTrans.anchoredPosition.x - _x) < 0.5f
                && Mathf.Abs(_rectTrans.anchoredPosition.y - _y) < 0.5f)
                return;

            //设置位置
            _rectTrans.anchoredPosition = new Vector2(_x, _y);
        }

        #endregion

        //克隆一个go
        public static T cloneGameObj<T>(GameObject _temp) where T : MonoBehaviour
        {
            if (null == _temp)
            {
                Debug.LogError("when clone game object temp is null !!!");
                return null;
            }

            T wnd = _temp.GetComponent<T>();
            if (null == wnd)
                wnd = _temp.AddComponent<T>();

            return cloneGameObj(wnd);
        }

        public static T cloneGameObj<T>(T _temp) where T : MonoBehaviour
        {
            if (null == _temp)
            {
                Debug.LogError("when clone game object temp is null !!!");
                return null;
            }

            T addGo = GameObject.Instantiate<T>(_temp);
            if (addGo != null)
            {
                addGo.transform.localPosition = Vector3.zero;
                addGo.transform.localScale = Vector3.one;
                addGo.transform.localRotation = Quaternion.identity;
                return addGo;
            }

            return null;
        }

        /// <summary>
        /// 统一释放资源函数
        /// </summary>
        /// <param name="_obj"></param>
        public static void releaseGameObj(MonoBehaviour _obj)
        {
            if (null == _obj || null == _obj.gameObject)
                return;

            GameObject.Destroy(_obj.gameObject);
        }
    }
}