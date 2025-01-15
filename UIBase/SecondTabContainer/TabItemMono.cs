using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using WLDDZ;

namespace NGame
{
    /// <summary>
    /// 单个TabItem
    /// </summary>
    public class TabItemMono : _ATabItemMono
    {
        
        protected override void AwakeEx()
        {

        }

        protected override void OnDestroyEx()
        {
        }

        protected override void _doSelectStatusChgAnimation(Action _doneAction)
        {
            if (null != _doneAction)
                _doneAction();
        }

        protected override void resetEx()
        {
            
        }
    }
}