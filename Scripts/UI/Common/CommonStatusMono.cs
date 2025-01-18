using System;
using System.Collections.Generic;
using UnityEngine;

namespace UTGame
{
    public enum ESelectStatus
    {
        IS_SELECTED, //选中
        UN_SELECTED, //未选中
    }

    [System.Serializable]
    public class CommonStatusMono<E> where E : Enum
    {
        [Header("选中状态")]
        public E status;

        [Header("该状态需要显示的GoList")]
        public List<GameObject> showGoList;

        [Header("该状态需要隐藏的GoList")]
        public List<GameObject> hideGoList;

        [Header("该状态需要设置的文本颜色 - 16进制")]
        public Color chgColor;

    }
}