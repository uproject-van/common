using System;
using UnityEngine;

namespace UTGame
{
    public partial class GCommon
    {
        public static ESelectStatus GetStatus(bool _isSelect)
        {
            return _isSelect == true ? ESelectStatus.IS_SELECTED : ESelectStatus.UN_SELECTED;
        }

        #region Color
        
        //TODO 16进制转颜色
        public static Color HexToColor(string _hex)
        {
            // 编辑器默认颜色
            if (string.IsNullOrEmpty(_hex)) return new Color(1f, 1f, 1f);
        
            // 转换颜色
            _hex = _hex.ToLower();
            if (_hex.IndexOf("#") == 0 && _hex.Length == 7)
            {
                int r = Convert.ToInt32(_hex.Substring(1, 2), 16);
                int g = Convert.ToInt32(_hex.Substring(3, 2), 16);
                int b = Convert.ToInt32(_hex.Substring(5, 2), 16);
                return new Color(r / 255f, g / 255f, b / 255f);
            }

            return new Color(1f, 1f, 1f);
        }
        #endregion
    }
}