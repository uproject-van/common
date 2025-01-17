using System.Collections.Generic;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 通用二级弹窗的数据
    /// </summary>
    public interface _ITabItemData
    {
        public abstract string name { get; }
        public abstract Sprite selectImg { get; }
        public abstract Sprite unSelectImg { get; }
        public abstract string selectIconColor { get; }
        public abstract string unSelectIconColor { get; }
        public abstract string selectFontColor { get; }
        public abstract string unSelectFontColor { get; }
        public abstract Sprite selectBgImg { get; }
        public abstract Sprite unSelectBgImg { get; }
        public List<_ITabItemData> subData { get; }
    }
}