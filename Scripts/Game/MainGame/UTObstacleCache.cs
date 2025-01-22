using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 碰撞物的缓存类
    /// </summary>
    public class UTObstacleCache:_ACacheControllerBase<_AUTObstacleBase,GameObject>
    {
        public UTObstacleCache(int _minCount, int _maxCount) : base(_minCount, _maxCount)
        {
        }

        public UTObstacleCache(int _minCount, int _maxCount, int _addUnit) : base(_minCount, _maxCount, _addUnit)
        {
        }

        protected override string _warningTxt
        {
            get { return "UTObstacleCache warning"; }
        }
        protected override void _onInit(GameObject _template)
        {
        }

        protected override _AUTObstacleBase _createItem(GameObject _template)
        {
            return UGUICommon.cloneGameObj<_AUTObstacleBase>(_template);
        }

        protected override void _discardItem(_AUTObstacleBase _item)
        {
            UGUICommon.releaseGameObj(_item);
        }

        protected override void _resetItem(_AUTObstacleBase _item)
        {
        }
    }
}