using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NGame
{
    /// <summary>
    /// 一个简单的计数器
    /// </summary>
    public class CommonSlider : Slider
    {
        /// <summary>
        /// slider开始变动
        /// </summary>
        public UnityAction onValueChgStart;
        public UnityAction<float> onValueChgEnd;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if(null != onValueChgStart)
            {
                onValueChgStart();
            }
            base.OnPointerDown(eventData);
        }

        /// <summary>
        /// slider变化结束
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if(null != onValueChgEnd)
            {
                onValueChgEnd(m_Value);
            }
        }
    }
}