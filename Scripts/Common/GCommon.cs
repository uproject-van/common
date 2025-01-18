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
        
        #region 打印方便阅读的方法
        
        /// <summary>
        /// 打印方便阅读的String
        /// </summary>
        /// <param name="_delegate"></param>
        public static string ToReadableString(Delegate _delegate)
        {
            if (_delegate != null)
            {
                //string className = _delegate.Method.ReflectedType != null ? _delegate.Method.ReflectedType.FullName : "null";
                string targetId;
                try
                {
                    targetId = _delegate.Target != null ? _delegate.Target.GetHashCode().ToString() : "null";
                }
                catch (Exception )
                {
                    targetId = "null";//ILRuntime的GetHashCode有时候会报空。。先catch起来吧
                }
                return $"【{_delegate.Target}】  ->  {_delegate.Method.Name}  ->  {targetId}";
            }
            return "null";
        }
        #endregion
    }
}