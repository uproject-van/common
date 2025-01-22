using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 范围类型
/// </summary>
[System.Serializable]
public class UTIntRange
{
    public static bool inRange(long _value, long _min, long _max)
    {
        if (-1 == _min && -1 == _max)
            return true;

        if (-1 != _min && _value < _min)
            return false;

        if (-1 != _max && _value > _max)
            return false;

        return true;
    }
    public static bool inRange(int _value, int _min, int _max)
    {
        if (-1 == _min && -1 == _max)
            return true;

        if (-1 != _min && _value < _min)
            return false;

        if (-1 != _max && _value > _max)
            return false;

        return true;
    }

    [SerializeField]
    private int _m_iMin;
    [SerializeField]
    private int _m_iMax;

    public UTIntRange()
    {

    }
    public UTIntRange(int _min, int _max)
    {
        _m_iMin = _min;
        _m_iMax = _max;
    }
    public UTIntRange(string _minStr, string _maxStr)
    {
        _m_iMin = int.Parse(_minStr);
        _m_iMax = int.Parse(_maxStr);
    }
    public UTIntRange(string _str)
    {
        ParseFromString(_str);
    }
    
    public int min { get { return _m_iMin;} }
    public int max { get { return _m_iMax; } }
    public int margin { get { return _m_iMax - _m_iMin; } }

    //提前判断结果是否正确
    public bool preJudgeAddRes(int _num)
    {
        if (-1 == _m_iMin && -1 == _m_iMax)
            return true;

        if (-1 == _m_iMax && _num >= _m_iMin)
            return true;

        return false;
    }
    public bool preJudgeRedRes(int _num)
    {
        if (-1 == _m_iMin && -1 == _m_iMax)
            return true;

        if (-1 == _m_iMin && _num <= _m_iMax)
            return true;

        return false;
    }

    /***************
     * 判断值是否在范围内
     **/
    public bool inRange(int _value)
    {
        if (-1 == _m_iMin && -1 == _m_iMax)
            return true;

        if (-1 != _m_iMin && _value < _m_iMin)
            return false;

        if (-1 != _m_iMax && _value > _m_iMax)
            return false;

        return true;
    }
    public bool inRange(long _value)
    {
        if (-1 == _m_iMin && -1 == _m_iMax)
            return true;

        if (-1 != _m_iMin && _value < _m_iMin)
            return false;

        if (-1 != _m_iMax && _value > _m_iMax)
            return false;

        return true;
    }
    /// <summary>
    /// 获取随机值
    /// </summary>
    public int getRandomValue()
    {
        return Random.Range(_m_iMin, _m_iMax + 1);
    }
    
    /// <summary>
    /// 为自动导出写的
    /// </summary>
    public void ParseFromString(string _str)
    {
        if (string.IsNullOrEmpty(_str))
            return;
        
        //拆分字符串后进行读取
        string[] strs = _str.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
        if (strs.Length > 0)
            _m_iMin = int.Parse(strs[0]);
        if (strs.Length > 1)
            _m_iMax = int.Parse(strs[1]);
    }
    
    public static UTIntRange readFromStr(string _str)
    {
        //拆分字符串后进行读取
        string[] strs = _str.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

        if (strs.Length < 1)
        {
            UnityEngine.Debug.LogWarning("没有配置 对象名!");
            return null;
        }

        //需要支持物品类型没有配置的情况
        UTIntRange ret = new UTIntRange();
        try
        {
            if (strs.Length > 0)
                ret._m_iMin = int.Parse(strs[0]);

            if (strs.Length > 1)
                ret._m_iMax = int.Parse(strs[1]);
        }
        catch (Exception e)
        {
            Debug.LogError($"解析UTIntRange时发生错误：{_str}\n{e}");
            return null;
        }

        return ret;
    }
    
    /// <summary>
    /// 读取队列
    /// </summary>
    /// <param name="_str"></param>
    /// <returns></returns>
    public static List<UTIntRange> readList(string _str)
    {
        List<UTIntRange> list = new List<UTIntRange>();
        if (null == _str || _str.Length <= 0)
            return list;
        
        string[] strs = _str.Split(new string[] { "|", ";" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < strs.Length; i++)
        {
            UTIntRange newItem = UTIntRange.readFromStr(strs[i]);
            if (null == newItem)
                continue;

            list.Add(newItem);
        }
        return list;
    }
    
    /// <summary>
    /// 为自动导出写的
    /// </summary>
    public static List<UTIntRange> MakeListFromString(string _str)
    {
        return readList(_str);
    }

    /// <summary>
    /// 为自动导出写的
    /// </summary>
    public static UTIntRange[] MakeArrayFromString(string _str)
    {
        return readList(_str).ToArray();
    }

    public override string ToString()
    {
        return "[" + min + "-" + max + "]";
    }
}
