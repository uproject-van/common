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
    public class CommonStatusMono<T> where T : Enum
    {
        [Header("选中状态")]
        public T status;

        [Header("该状态需要显示的GoList")]
        public List<GameObject> showGoList;

        [Header("该状态需要隐藏的GoList")]
        public List<GameObject> hideGoList;

        [Header("该状态需要设置的文本颜色 - 16进制")]
        public Color chgColor;

        public static void setStatus<T, E>(List<E> _statusMonoList, T _status,
            Action<E> _perAction = null)
            where T : Enum
            where E : CommonStatusMono<T>
        {
            if (null == _statusMonoList || _statusMonoList.Count == 0)
                return;

            E temp = null;
            for (int i = 0; i < _statusMonoList.Count; i++)
            {
                temp = _statusMonoList[i];
                if (null == temp)
                    continue;

                if (temp.status.Equals(_status))
                {
                    UGUICommon.setGameObjEnable(temp.showGoList, true);
                    UGUICommon.setGameObjEnable(temp.hideGoList, false);
                    if (null != _perAction)
                        _perAction(temp);
                    break;
                }
            }
        }
    }
}