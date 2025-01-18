using System;
using UnityEngine;

/********************
 * 基本的索引信息对象
 **/
[Serializable]
public class UTBaseResIndexInfo
{
    public int mainId = 0;
    public int subId = 0;

    public UTBaseResIndexInfo()
    {
        
    }

    public UTBaseResIndexInfo(int _mainId, int _subId)
    {
        mainId = _mainId;
        subId = _subId;
    }

    public UTBaseResIndexInfo(string _str, string _columnName = "")
    {
        readIndex(_str, _columnName);
    }

    /*******************
     * 从字符串初始化数据
     **/
    public void readIndex(string _str, string _columnName = "")
    {
        if(string.IsNullOrEmpty(_str))
        {
            //使用_str来初始化ResIndex，基本只会在excel表里。如果_str不能为空，excel会校验，导表代码也可以加判断，所以这里面不要强行对空值进行报错
//            Debug.LogError($"{_columnName}是空");
            return;
        }
        //拆分字符串后进行读取
        string[] strs = _str.Split(new string[] { "||", ":", "_" }, StringSplitOptions.RemoveEmptyEntries);

        if (strs.Length < 1)
        {
            //Debug.LogWarning("没有配置 main id! columnName: " + _columnName);
            return;
        }
        try
        {
            mainId = int.Parse(strs[0]);
        }
        catch (Exception)
        {
            Debug.LogError(string.Format("index main id格式错误：{0} columnName: {1}", strs[0], _columnName));
            return;
        }

        if (strs.Length < 2)
        {
            Debug.LogError("没有配置 sub id! columnName: " + _columnName);
            return;
        }
        try
        {
            subId = int.Parse(strs[1]);
        }
        catch (Exception)
        {
            Debug.LogError(string.Format("index sub id格式错误：{0} columnName: {1}", strs[1], _columnName));
            return;
        }
    }

    public override string ToString () {
        return string.Format("{0}:{1}", mainId, subId);
    }
    
    public override int GetHashCode() { return mainId * 100000 + subId; }

    public override bool Equals(object _other)
    {
        if(_other == null) return false;

        if(this.GetType() != _other.GetType()) return false;

        return this.GetHashCode() == _other.GetHashCode();
    }


    //重载运算符 == 的任何类型还应重载运算符 !=,否则会产生编译错误
    public static bool operator ==(UTBaseResIndexInfo _a, UTBaseResIndexInfo _b)
    {
        // If both are null, or both are same instance, return true.
        if(ReferenceEquals(_a, _b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if(((object)_a == null) || ((object)_b == null))
        {
            return false;
        }

        // Return true if the fields match:
        if(_a.mainId != _b.mainId)
            return false;
        if(_a.subId != _b.subId)
            return false;

        return true;
    }

    public static bool operator !=(UTBaseResIndexInfo _a, UTBaseResIndexInfo _b)
    {
        return !(_a == _b);
    }
}

public static class ALBasicResIndexInfoExtension
{
    public static string ToStringExt(this UTBaseResIndexInfo _self)
    {
        if(_self == null)
        {
            return "Null";
        }
        else
        {
            return _self.ToString();
        }
    }
}