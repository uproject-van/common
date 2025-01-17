using System;

namespace UTGame
{
    /// <summary>
    /// 单个TabItem
    /// </summary>
    public class TabItemMono : _ATabItemMono
    {
        protected override void _OnDisableEx()
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