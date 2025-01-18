using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UTGame
{
    public class UTBaseExportFunc
    {
        #region 类定义
        public static Type C_T_List = typeof(List<>);
        public static Type C_T_Int = typeof(int);
        public static Type C_T_Long = typeof(long);
        public static Type C_T_String = typeof(string);
        public static Type C_T_Short = typeof(short);
        public static Type C_T_Byte = typeof(byte);
        public static Type C_T_Bool = typeof(bool);
        public static Type C_T_Float = typeof(float);

        public static Type C_T_Rect = typeof(Rect);
        public static Type C_T_Color = typeof(Color);
        public static Type C_T_Vector2 = typeof(Vector2);
        public static Type C_T_Vector3 = typeof(Vector3);
        public static Type C_T_Vector4 = typeof(Vector4);
        public static Type C_T_Vector2Int = typeof(Vector2Int);
        public static Type C_T_Vector3Int = typeof(Vector3Int);

        #endregion
        
        /// <summary>
        /// 自动根据对应类从字典中读取数据
        /// </summary>
        /// <param name="_tmpInfo"></param>
        /// <param name="_line"></param>
        /// <param name="_lineData"></param>
        public static TTemp AutoRead<TTemp>(Dictionary<string, string> _lineData, string _errMsg = "")
        {
            //自动根据字段识别
            Type TempCls = typeof(TTemp);
            //识别变量
            FieldInfo[] fields = TempCls.GetFields();
            if(null == fields)
                return default(TTemp);
            
            //开始读取
            FieldInfo tmpField = null;
            string tmpValue = string.Empty;
            object[] param = new object[1];
            TTemp readObj = Activator.CreateInstance<TTemp>();
            for(int i = 0; i < fields.Length; i++)
            {
                //逐个进行获取
                tmpField = fields[i];
                if(null == tmpField)
                    continue;

                //判断是否有数据
                if(!AutoReadField(tmpField, readObj, _lineData, _errMsg))
                {
                    Debug.LogError("类： " + TempCls.Name + " 的变量： " + tmpField.Name + " 无法读取到数据: " + _errMsg);
                    continue;
                }
            }

            return readObj;
        }

        /// <summary>
        /// 自动读取字段值，返回是否读取成功
        /// </summary>
        /// <param name="_fieldInfo"></param>
        /// <param name="_obj"></param>
        /// <param name="_lineData"></param>
        /// <param name="_errMsg"></param>
        /// <returns></returns>
        public static bool AutoReadField(FieldInfo _fieldInfo, object _obj, Dictionary<string, string> _lineData, string _errMsg = "")
        {
            if(null == _fieldInfo || null == _obj)
                return false;

            //无数据则获取属性判断，如果可为空则不做处理
            object[] objs = _fieldInfo.GetCustomAttributes(typeof(UTAutoExportVariableAttr), false);
            UTAutoExportVariableAttr fieldAtt = null;
            if(objs.Length > 0)
                fieldAtt = (UTAutoExportVariableAttr)objs[0];

            //判断是否屏蔽不读取，是则不读取
            if(null != fieldAtt && fieldAtt.ignoreReading)
            {
                return true;
            }

            string tmpValue = string.Empty;
            //判断是否有数据
            if(!_lineData.TryGetValue(_fieldInfo.Name.ToLowerInvariant(), out tmpValue) || tmpValue.Trim().Length <= 0)
            {
                //有配置属性的时候才需要log
                if(null != fieldAtt)
                {
                    if (!fieldAtt.canBeEmpty)
                    {
                        Debug.LogError("类： " + _obj.GetType().Name + " 的变量： " + _fieldInfo.Name + " 无法查询到数据: " + _errMsg);
                        return false;
                    }
                    else if (!fieldAtt.emptyKeepRead)
                    {
                        //不需要继续读取才返回
                        return true;
                    }
                }
            }

            if (null == tmpValue)
                tmpValue = string.Empty;

            //判断类型
            if(_fieldInfo.FieldType.IsArray || HasImplementedRawGeneric(_fieldInfo.FieldType, C_T_List))
            {
                Type singleType = null;
                if(_fieldInfo.FieldType.IsArray)
                    singleType = _fieldInfo.FieldType.GetElementType();
                else
                    singleType = _fieldInfo.FieldType.GetGenericArguments()[0];

                //尝试读取列表
                object preReadObj;
                if(_fieldInfo.FieldType.IsArray && readCustomFieldArray(singleType, tmpValue, out preReadObj, _errMsg))
                {
                    //如读取数组成功则直接返回
                    _fieldInfo.SetValue(_obj, preReadObj);
                    return true;
                }
                else if(HasImplementedRawGeneric(_fieldInfo.FieldType, C_T_List) && readCustomFieldList(singleType, tmpValue, out preReadObj, _errMsg))
                {
                    //如队列读取成功则直接返回
                    _fieldInfo.SetValue(_obj, preReadObj);
                    return true;
                }

                //队列处理方式
                if(!tmpValue.Trim().Equals(""))
                {
                    string[] strs = tmpValue.Split(new char[] { ';' });

                    //创建数组
                    Array arrObj = Array.CreateInstance(singleType, strs.Length);

                    for(int i = 0; i < strs.Length; i++)
                    {
                        string tempStr = strs[i];
                        if(null == tempStr)
                            continue;

                        object value = null;
                        bool readRes = false;
                        if(!ParseValue(tempStr, singleType, ref value, _errMsg))
                        {
                            value = Activator.CreateInstance(singleType, true);//没有传true的话，没有public构造函数的对象无法反射创建
                            readRes = readCustomSingleFieldValue(singleType, value, tempStr, _errMsg);
                        }
                        else
                        {
                            readRes = true;
                        }

                        if(!readRes)
                        {
                            Debug.LogError("类： " + _obj.GetType().Name + " 的变量： " + _fieldInfo.Name + " 读取失败: " + _errMsg);
                            return false;
                        }

                        //设置值
                        arrObj.SetValue(value, i);
                    }

                    //设置值
                    if(_fieldInfo.FieldType.IsArray)
                    {
                        _fieldInfo.SetValue(_obj, arrObj);
                    }
                    else
                    {
                        //创建队列
                        object listObj = Activator.CreateInstance(_fieldInfo.FieldType, true);//没有传true的话，没有public构造函数的对象无法反射创建
                        MethodInfo method = _fieldInfo.FieldType.GetMethod("AddRange");
                        //调用函数
                        object[] param = new object[1];
                        param[0] = arrObj;
                        method.Invoke(listObj, param);
                        //设置值
                        _fieldInfo.SetValue(_obj, listObj);
                    }
                }

                return true;
            }
            else
            {
                object[] param = new object[1];

                object readRes = null;
                //非队列处理方式，则使用单体读取
                if(ParseValue(tmpValue, _fieldInfo.FieldType, ref readRes, _errMsg))
                {
                    //读取成功直接返回
                    _fieldInfo.SetValue(_obj, readRes);
                    return true;
                }

                //使用自定义读取方式
                object readObj = Activator.CreateInstance(_fieldInfo.FieldType, true);//没有传true的话，没有public构造函数的对象无法反射创建
                if(!readCustomSingleFieldValue(_fieldInfo.FieldType, readObj, tmpValue, _errMsg))
                {
                    Debug.LogError("类： " + _obj.GetType().Name + " 的变量： " + _fieldInfo.Name + " 对应结构体没有实现ParseFromString函数: " + _errMsg);
                    return false;
                }

                //set value
                _fieldInfo.SetValue(_obj, readObj);

                return true;
            }
        }

        private static Type[] _g_arrParseArg = { typeof(string) };
        /// <summary>
        /// 读取单个对象的数据
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_resObj"></param>
        /// <param name="_value"></param>
        /// <param name="_errMsg"></param>
        /// <returns></returns>
        public static bool readCustomFieldList(Type _singleType, string _value, out object _returnList, string _errMsg = "")
        {
            object invokeObj = null;
            MethodInfo readFuncInfo = _singleType.GetMethod("MakeListFromString", 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(null != readFuncInfo)
            {
                //调用函数读取
                invokeObj = Activator.CreateInstance(_singleType, true);//没有传true的话，没有public构造函数的对象无法反射创建
            }
            else
            {
                readFuncInfo = _singleType.GetMethod("MakeListFromString",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            }
            if(null == readFuncInfo)
            {
                _returnList = null;
                return false;
            }

            try
            {
                _returnList = readFuncInfo.Invoke(invokeObj, new object[] {_value});
            }
            catch (Exception e)
            {
                Debug.LogError($"{_errMsg} 读取{_singleType.Name}类型时，readCustomFieldList发生错误：【{_value}】\n{e}");
                _returnList = null;
            }

            return true;
        }
        public static bool readCustomFieldArray(Type _singleType, string _value, out object _returnArray, string _errMsg = "")
        {
            object invokeObj = null;
            MethodInfo readFuncInfo = _singleType.GetMethod("MakeArrayFromString", 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if(null != readFuncInfo)
            {
                //调用函数读取
                invokeObj = Activator.CreateInstance(_singleType, true);//没有传true的话，没有public构造函数的对象无法反射创建
            }
            else
            {
                readFuncInfo = _singleType.GetMethod("MakeArrayFromString",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            }

            if(null == readFuncInfo)
            {
                _returnArray = null;
                return false;
            }
            
            try
            {
                _returnArray = readFuncInfo.Invoke(invokeObj, new object[] {_value});
            }
            catch (Exception e)
            {
                 Debug.LogError($"{_errMsg} 读取{_singleType.Name}类型时，readCustomFieldArray发生错误：【{_value}】\n{e}");
                 _returnArray = null;
            }

            return true;
        }

        public static bool readCustomSingleFieldValue(Type _singleType, object _obj, string _value, string _errMsg = "")
        {
            MethodInfo readFuncInfo = _singleType.GetMethod("ParseFromString", _g_arrParseArg);
            if(null == readFuncInfo)
            {
                Debug.LogError("类： " + _singleType.Name + " 没有实现ParseFromString函数");
                return false;
            }

            try
            {
                readFuncInfo.Invoke(_obj, new object[]{_value});
            }
            catch (Exception e)
            {
                Debug.LogError($"{_errMsg} 读取{_singleType.Name}类型时，readCustomSingleFieldValue发生错误：【{_value}】\n{e}");
            }

            return true;
        }

        /// <summary>
        /// 判断指定的类型 <paramref name="_judgeType"/> 是否是指定泛型类型的子类型，或实现了指定泛型接口。
        /// </summary>
        /// <param name="_judgeType">需要测试的类型。</param>
        /// <param name="_basicType">泛型接口类型，传入 typeof(IXxx&lt;&gt;)</param>
        /// <returns>如果是泛型接口的子类型，则返回 true，否则返回 false。</returns>
        public static bool HasImplementedRawGeneric(Type _judgeType, Type _basicType)
        {
            if(_judgeType == null)
                throw new ArgumentNullException(_judgeType.Name);
            if(_basicType == null)
                throw new ArgumentNullException(_basicType.Name);

            // 测试接口。
            bool isTheRawGenericType = false;
            Type[] all = _judgeType.GetInterfaces();
            for(int i = 0; i < all.Length; i++)
            {
                if(_isTheRawGenericType(_basicType, all[i]))
                {
                    isTheRawGenericType = true;
                    break;
                }
            }
            if(isTheRawGenericType)
                return true;

            // 测试类型。
            while(_judgeType != null && _judgeType != typeof(object))
            {
                isTheRawGenericType = _isTheRawGenericType(_basicType, _judgeType);
                if(isTheRawGenericType)
                    return true;
                _judgeType = _judgeType.BaseType;
            }

            // 没有找到任何匹配的接口或类型。
            return false;
        }
        // 测试某个类型是否是指定的原始接口。
        protected static bool _isTheRawGenericType(Type generic, Type test)
        {
            return generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
        
        /// <summary>
        /// 解析字段值，返回是否读取成功，如失败则需要通过自定义方式读取
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_type"></param>
        /// <param name="_obj"></param>
        /// <returns></returns>
        public static bool ParseValue(string _value, Type _type, ref object _obj, string _errMsg = "")
        {
            try
            {
                //如果策划没有配置，设置默认值
                if(_value.Equals(string.Empty))
                {
                    //枚举没填默认给0
                    if(_type.IsEnum)
                    {
                        _obj = 0;
                        return true;
                    }
                    else if (_type == C_T_String)
                    {
                        _obj = "";
                        return true;
                    }
                    else if (_type == C_T_Float)
                    {
                        _obj = 0f;
                        return true;
                    }
                    else if (_type == C_T_Byte)
                    {
                        _obj = 0;
                        return true;
                    }
                    else if (_type == C_T_Short)
                    {
                        _obj = 0;
                        return true;
                    }
                    else if (_type == C_T_Int)
                    {
                        _obj = 0;
                        return true;
                    }
                    else if (_type == C_T_Bool)
                    {
                        _obj = false;
                        return true;
                    }
                    else if (_type == C_T_Long)
                    {
                        _obj = 0;
                        return true;
                    }
                    else if(_type == C_T_Rect)
                    {
                        _obj = new Rect();

                        return true;
                    }
                    else if(_type == C_T_Vector2)
                    {
                        _obj = Vector2.zero;

                        return true;
                    }
                    else if(_type == C_T_Vector3)
                    {
                        _obj = Vector3.zero;

                        return true;
                    }
                    else if (_type == C_T_Color)
                    {
                        _obj = Color.black;
                        return true;
                    }
                    else if (_type == C_T_Vector4)
                    {
                        _obj = Vector4.zero;
                    }
                    //其他的就先不需要了吧
                    return false;
                }
                else
                {
                    _value = _value.Trim();

                    // 枚举
                    if(_type.IsEnum)
                    {
                        _obj = Enum.Parse(_type, _value, true);
                        return true;
                    }
                    else if(_type == C_T_String)
                    {
                        _obj = _value;
                        return true;
                    }
                    else if(_type == C_T_Float)
                    {
                        if(_value == "0" || _value == "" || _value == string.Empty)
                            _obj = 0f;
                        else
                            _obj = GCommon.ParseFloat(_value);

                        return true;
                    }
                    else if (_type == C_T_Byte)
                    {
                        if (_value == "")
                            _obj = 0;
                        else
                            _obj = byte.Parse(_value);

                        return true;
                    }
                    else if (_type == C_T_Short)
                    {
                        if (_value == "")
                            _obj = 0;
                        else
                            _obj = short.Parse(_value);

                        return true;
                    }
                    else if(_type == C_T_Int)
                    {
                        if(_value == "")
                            _obj = 0;
                        else
                            _obj = int.Parse(_value);

                        return true;
                    }
                    else if(_type == C_T_Bool)
                    {
                        _obj = bool.Parse(_value);

                        return true;
                    }
                    else if(_type == C_T_Long)
                    {
                        _obj = long.Parse(_value);

                        return true;
                    }
                    //以下是扩展
                    else if(_type == C_T_Rect)
                    {
                        _obj = GCommon.GetRect(_value, _errMsg);

                        return true;
                    }
                    else if(_type == C_T_Vector2)
                    {
                        _obj = GCommon.GetVector2(_value, _errMsg);

                        return true;
                    }
                    else if(_type == C_T_Vector3)
                    {
                        _obj = GCommon.GetVector3(_value, _errMsg);

                        return true;
                    }
                    else if(_type == C_T_Vector2Int)
                    {
                        _obj = GCommon.GetVector2Int(_value, _errMsg);

                        return true;
                    }
                    else if(_type == C_T_Vector3Int)
                    {
                        _obj = GCommon.GetVector3Int(_value, _errMsg);

                        return true;
                    }
                    else if(_type == C_T_Color)
                    {
                        _obj = GCommon.GetColor(_value);

                        return true;
                    }
                    else if (_type == C_T_Vector4)
                    {
                        _obj = GCommon.GetVector4(_value, _errMsg);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError(string.Format("ParseValue type:{0}, value:{1}, failed: {2}", _type.ToString(), _value, ex.ToString()));
                return false;
            }
        }

    }
}

