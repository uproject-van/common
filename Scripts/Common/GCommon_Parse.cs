using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 处理字符串=》对象相关解析
    /// </summary>
    public partial class GCommon
    {
        #region 解析分隔符

        /// <summary>
        /// 列表解析分隔符
        /// </summary>
        public static char[] listSplitSeparator = new[] {';', ':'};

        #endregion
        
           //替代的转化操作
        public static object EnumParse(Type enumType, string value, bool ignoreCase = true)
        {
            if(null == value || value.Length <= 0)
                return 0;

            //这里这样写是因为，枚举解析错误，这种错误比较严重，所以在Editor下就直接不捕获抛异常
#if !UNITY_EDITOR
            try
            {
#endif
            return Enum.Parse(enumType, value, ignoreCase);
#if !UNITY_EDITOR
            }
            catch (Exception)
            {
                UnityEngine.Debug.LogError("Enum [" + enumType.ToString() + "] Parse err value: " + value);
                return 0;
            }
#endif
        }
        public static bool TryEnumParse<TEnum>(Type enumType, string value, out TEnum res, bool ignoreCase = true) where TEnum : struct
        {
            if(null == value || value.Length <= 0)
            {
                res = default(TEnum);
                return false;
            }

#if !UNITY_EDITOR
            try
            {
#endif
                return Enum.TryParse<TEnum>(value, ignoreCase, out res);
#if! UNITY_EDITOR
            }
            catch (Exception)
            {
                res = default(TEnum);
                UnityEngine.Debug.LogError("Enum [" + enumType.ToString() + "] Parse err value: " + value);
                return false;
            }
#endif
        }
        
        public static float ParseFloat(string _value)
        {
            try
            {
                return float.Parse(_value, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[{_value}] ParseFloat err:{e}");
                return 0;
            }
        }
        
        public static bool TryParseFloat(string _value, out float _result)
        {
            return float.TryParse(_value,NumberStyles.Float,  CultureInfo.InvariantCulture, out _result);
        }

        public static int ParseInt(string _value)
        {
            try
            {
                return int.Parse(_value);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[{_value}] ParseInt err:{e}");
                return 0;
            }
        }
        
        public static long ParseLong(string _value)
        {
            try
            {
                return long.Parse(_value);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[{_value}] ParseLong err:{e}");
                return 0;
            }
        }

        public static byte GetByte(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
                return 0;

            byte tempValue = 0;
            if(!byte.TryParse(_str, out tempValue))
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1},填的不是整数", _errMsg, _str));
            }
            return tempValue;
        }

        public static short GetShort(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
                return 0;

            short tempValue = 0;
            if(!short.TryParse(_str, out tempValue))
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1},填的不是Short", _errMsg, _str));
            }
            return tempValue;
        }

        public static int GetInt(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
                return 0;

            int tempValue = 0;
            if(!int.TryParse(_str, out tempValue))
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1},填的不是整数", _errMsg, _str));
            }
            return tempValue;
        }

        public static long GetLong(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
                return 0;

            long tempValue = 0;
            long.TryParse(_str, out tempValue);
            return tempValue;
        }

        public static float GetFloat(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
            {
                return 0;
            }

            float tempValue = 0;
            if(!GCommon.TryParseFloat(_str, out tempValue))
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1},填的不是浮点数", _errMsg, _str));
            }
            return tempValue;
        }

        public static bool GetBool(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
            {
                return false;
            }

            bool tempValue = false;
            if(!bool.TryParse(_str, out tempValue))
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1}", _errMsg, _str));
            }
            return tempValue;
        }

        public static string GetString(string _str, string _errMsg = "", bool _errorLog = true)
        {
            if(string.IsNullOrEmpty(_str))
            {
                return "";
            }
            return _str;
        }
        public static string GetSolidString(string _str, string _errMsg = "")
        {
            if(string.IsNullOrEmpty(_str))
            {
                return "";
            }
            return _str.Replace(" ", "");
        }

        public static TEnum GetEnum<TEnum>(string _str, string _errMsg = "")
        {
            if(_str == string.Empty || _str == "")
            {
                return (TEnum)Activator.CreateInstance(typeof(TEnum));
            }
            try
            {
                return (TEnum)Enum.Parse(typeof(TEnum), _str.Trim(), true);
            }
            catch
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1}", _errMsg, _str));
                return (TEnum)Activator.CreateInstance(typeof(TEnum));
            }
        }

        public static object GetEnum(string _str, Type _type, string _errMsg = "")
        {
            if(_str == string.Empty || _str == "")
            {
                return Activator.CreateInstance(_type);
            }
            try
            {
                return Enum.Parse(_type, _str.Trim(), true);
            }
            catch
            {
                Debug.LogError(string.Format("{0}数据填写错误: {1}", _errMsg, _str));
                return Activator.CreateInstance(_type);
            }
        }

        #region ParseList
        
        /// <summary>
        /// 解析int列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<int> ParseIntList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<int> _resultList = new List<int>();
                    
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                int value = 0;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (int.TryParse(tempStr, out value))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseIntList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析long列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<long> ParseLongList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<long> _resultList = new List<long>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                long value = 0;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (long.TryParse(tempStr, out value))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseLongList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析string列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<string> ParseStringList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<string> _resultList = new List<string>();
                
                string[] strs = _str.Split(listSplitSeparator);
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    _resultList.Add(strs[i]);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析short列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<short> ParseShortList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<short> _resultList = new List<short>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                short value = 0;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (short.TryParse(tempStr, out value))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseShortList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析byte列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<byte> ParseByteList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<byte> _resultList = new List<byte>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                byte value = 0;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (byte.TryParse(tempStr, out value))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseByteList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }
                
                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析bool列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<bool> ParseBoolList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<bool> _resultList = new List<bool>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                bool value = false;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (bool.TryParse(tempStr, out value))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseBoolList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }
                
                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析float列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<float> ParseFloatList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<float> _resultList = new List<float>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                float value = 0f;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (GCommon.TryParseFloat(tempStr, out value))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseFloatList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析enum列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<TEnum> ParseEnumList<TEnum>(string _str, bool ignoreCase = true, string _errMsg = "") where TEnum : struct
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<TEnum> _resultList = new List<TEnum>();
                    
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Type enumType = typeof(TEnum);
                TEnum value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    if (GCommon.TryEnumParse(enumType, tempStr, out value, ignoreCase))
                    {
                        _resultList.Add(value);
                    }
                    else
                    {
                        Debug.LogError($"ParseEnumList _str:{_str} value:{tempStr} fail: {_errMsg}");
                    }
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析Rect列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Rect> ParseRectList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Rect> _resultList = new List<Rect>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Rect value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetRect(tempStr);
                    _resultList.Add(value);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 解析Color列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Color> ParseColorList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Color> _resultList = new List<Color>();

                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Color value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetColor(tempStr);
                    _resultList.Add(value);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析Color32列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Color32> ParseColor32List(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Color32> _resultList = new List<Color32>();

                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Color32 value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetColor32(tempStr);
                    _resultList.Add(value);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析Vector2列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Vector2> ParseVector2List(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Vector2> _resultList = new List<Vector2>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Vector2 value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetVector2(tempStr);
                    _resultList.Add(value);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析Vector3列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Vector3> ParseVector3List(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Vector3> _resultList = new List<Vector3>();
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Vector3 value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetVector3(tempStr);
                    _resultList.Add(value);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析Vector2Int列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Vector2Int> ParseVector2IntList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Vector2Int> _resultList = new List<Vector2Int>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Vector2Int value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetVector2Int(tempStr);
                    _resultList.Add(value);
                }
                
                return _resultList;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 解析Vector3Int列表字段值
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static List<Vector3Int> ParseVector3IntList(string _str, string _errMsg = "")
        {
            if (!string.IsNullOrWhiteSpace(_str))
            {
                List<Vector3Int> _resultList = new List<Vector3Int>();
                
                string[] strs = _str.Split(listSplitSeparator);
                string tempStr = "";
                Vector3Int value = default;
                for (int i = 0, count = strs.Length; i < count; i++)
                {
                    tempStr = strs[i];
                    if(string.IsNullOrEmpty(tempStr))
                        continue;
                    
                    value = GetVector3Int(tempStr);
                    _resultList.Add(value);
                }

                return _resultList;
            }
            else
            {
                return null;
            }
        }

        #endregion

        public static Vector2 GetVector2(string _str, string _errMsg = "")
        {
            Vector2 v2 = new Vector2();
            if(!_str.Trim().Equals(""))
            {
                string[] strs = _str.Split(new char[] { ';', ':' });
                if(strs.Length < 2)
                {
                    Debug.LogError(string.Format("{0}数据填写错误，格式 [x;y] : {1}", _errMsg, _str));
                    return v2;
                }

                v2.Set(GCommon.ParseFloat(strs[0]), GCommon.ParseFloat(strs[1]));
            }
            return v2;
        }

        public static Vector3 GetVector3(string _str, string _errMsg = "")
        {
            Vector3 v3 = new Vector3();
            if(!_str.Trim().Equals(""))
            {
                string[] strs = _str.Split(new char[] { ';', ':' });
                if(strs.Length < 3)
                {
                    Debug.LogError(string.Format("{0}数据填写错误，格式 [x;y:z] : {1}", _errMsg, _str));
                    return v3;
                }

                v3.Set(GCommon.ParseFloat(strs[0]), GCommon.ParseFloat(strs[1]), GCommon.ParseFloat(strs[2]));
            }
            return v3;
        }
        
        public static Vector4 GetVector4(string _str, string _errMsg = "")
        {
            Vector4 v4 = new Vector4();
            if(!_str.Trim().Equals(""))
            {
                string[] strs = _str.Split(new char[] { ';', ':' });
                if(strs.Length < 4)
                {
                    Debug.LogError(string.Format("{0}数据填写错误，格式 [x:y:z:w] : {1}", _errMsg, _str));
                    return v4;
                }

                v4.Set(GCommon.ParseFloat(strs[0]), GCommon.ParseFloat(strs[1]), GCommon.ParseFloat(strs[2]), GCommon.ParseFloat(strs[3]));
            }
            return v4;
        }

        public static Vector2Int GetVector2Int(string _str, string _errMsg = "")
        {
            Vector2Int v2 = new Vector2Int();
            if(!_str.Trim().Equals(""))
            {
                string[] strs = _str.Split(new char[] { ';', ':' });
                if(strs.Length < 2)
                {
                    Debug.LogError(string.Format("{0}数据填写错误，格式 [x;y] : {1}", _errMsg, _str));
                    return v2;
                }

                v2.Set(GCommon.ParseInt(strs[0]), GCommon.ParseInt(strs[1]));
            }
            return v2;
        }
        
        public static Vector3Int GetVector3Int(string _str, string _errMsg = "")
        {
            Vector3Int v3 = new Vector3Int();
            if(!_str.Trim().Equals(""))
            {
                string[] strs = _str.Split(new char[] { ';', ':' });
                if(strs.Length < 3)
                {
                    Debug.LogError(string.Format("{0}数据填写错误，格式 [x;y:z] : {1}", _errMsg, _str));
                    return v3;
                }

                v3.Set(GCommon.ParseInt(strs[0]), GCommon.ParseInt(strs[1]), GCommon.ParseInt(strs[2]));
            }
            return v3;
        }

        public static Rect GetRect(string _str, string _errMsg = "")
        {
            Rect rect = new Rect();
            if(!_str.Trim().Equals(""))
            {
                string[] strs = _str.Split(new char[] { ';', ':' });
                if(strs.Length < 4)
                {
                    Debug.LogError(string.Format("{0}数据填写错误，格式 [x;y;width;height] : {1}", _errMsg, _str));
                    return rect;
                }

                rect.Set(GCommon.ParseFloat(strs[0]), GCommon.ParseFloat(strs[1]), GCommon.ParseFloat(strs[2]), GCommon.ParseFloat(strs[3]));
            }

            return rect;
        }

        public static Color GetColor(string _str)
        {
            byte br = byte.Parse(_str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(_str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(_str.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte cc = byte.Parse(_str.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            float a = cc / 255f;
            return new Color(r, g, b, a);
        }
        
        public static Color32 GetColor32(string _str, string _errMsg = "")
        {
            if (string.IsNullOrEmpty(_str))
                return Color.clear;
            
            string[] strs = _str.Split(new char[] { ';', ':' });
            if (strs.Length < 4)
            {
                Debug.LogError($"[ALCommonRefTableParse GetColor32] {0}数据填写错误，格式[r;g;b;a] : {_str}");
                return Color.clear;
            }

            Color32 color = new Color32();
            color.r = byte.Parse(strs[0]);
            color.g = byte.Parse(strs[1]);
            color.b = byte.Parse(strs[2]);
            color.a = byte.Parse(strs[3]);

            return color;
        }
    }
}