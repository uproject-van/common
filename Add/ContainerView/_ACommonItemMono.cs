using System;
using System.Collections.Generic;
using LBConfig;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using WLDDZ;

namespace NGame
{
    /// <summary>
    /// 支持额外展示的通用mono
    /// </summary>
    public abstract class _ACommonItemMono<T> : _ABaseItemMono where T:_ICommonItem
    {
        protected virtual void _refreshEx(T _item)
        {
            
        }
        
        public void setItem(T _item)
        {
            if (null == _item)
                return;
            
            ResItemAllConfig config = LubanCoreMgr.instance.resAllItemCore.getRef(_item.itemId);
            if (null == config)
                return;
            
            setData(config.icon, config.name, config.descr);
            setCount(_item.count);
            setEnableDay(_item.enableDay);
            _refreshEx(_item);
        }
     
        public void setItemId(int _itemId,int _count = -1)
        {
            ResItemAllConfig config = LubanCoreMgr.instance.resAllItemCore.getRef(_itemId);
            setData(config.icon, config.name, config.descr);
            if (_count != -1)
                setCount(_count);
        }
    }
}