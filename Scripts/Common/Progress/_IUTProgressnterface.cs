using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 进度对象对外的接口类，重载此接口类可以作为父节点的子节点
    /// 根节点通过遍历子节点进行汇总
    /// </summary>
    public interface _IUTProgressnterface
    {
        /// <summary>
        /// 获取当前进度
        /// </summary>
        float curProcess { get; }
    }
}

