﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UTGame
{
    public partial class GCommon
    {

        #region 获取随机方法

        public static T getRandom<T>(List<T> _list)
        {
            if (null == _list || _list.Count == 0)
                return default(T);
            
            int randomIdx = Random.Range(0, _list.Count);
            return _list[randomIdx];
        }
        
        // 从枚举类型中随机返回一个值
        public static T getRandom<T>() where T : Enum
        {
            // 获取枚举所有值
            Array values = Enum.GetValues(typeof(T));
            // 使用随机数选择一个值
            int randomIdx = Random.Range(0, values.Length);
            return (T)values.GetValue(randomIdx);
        }

        #endregion

        
        #region 设置选中状态显隐

        public static ESelectStatus GetStatus(bool _isSelect)
        {
            return _isSelect == true ? ESelectStatus.IS_SELECTED : ESelectStatus.UN_SELECTED;
        }

        #endregion

        #region Color相关

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
                catch (Exception)
                {
                    targetId = "null"; //ILRuntime的GetHashCode有时候会报空。。先catch起来吧
                }

                return $"【{_delegate.Target}】  ->  {_delegate.Method.Name}  ->  {targetId}";
            }

            return "null";
        }

        #endregion

        #region 获取UCT时间

        public static int getNowTimeSec()
        {
            return decimal.ToInt32(decimal.Divide(System.DateTime.UtcNow.Ticks - 621355968000000000, 10000000));
        }

        public static long getNowTimeMill()
        {
            return decimal.ToInt64(decimal.Divide(System.DateTime.UtcNow.Ticks - 621355968000000000, 10000));
        }

        #endregion
    }
    
    /// <summary>
    /// 自动导出的时候根据属性判断对应值是否可为空
    /// </summary>
    public class UTAutoExportVariableAttr : Attribute
    {
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool canBeEmpty;
        /// <summary>
        /// 如果数据为空是否继续读取
        /// </summary>
        public bool emptyKeepRead;
        /// <summary>
        /// 是否屏蔽不读取
        /// </summary>
        public bool ignoreReading;

        public UTAutoExportVariableAttr(bool _canBeEmpty = true, bool _emptyKeepRead = true, bool _ignoreReading = false)
        {
            canBeEmpty = _canBeEmpty;
            emptyKeepRead = _emptyKeepRead;
            ignoreReading = _ignoreReading;
        }
    }
}
