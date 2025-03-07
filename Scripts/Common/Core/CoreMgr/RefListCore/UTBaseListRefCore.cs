﻿namespace UTGame
{
    public class UTBaseListRefCore<T> : _ATUTBaseRefListCore<T> where T : _IUTBaseRefObj
    {
        private string _m_sAssetName;

        public UTBaseListRefCore(string _assetName)
        {
            _m_sAssetName = _assetName;
        }

        /** 获取加载资源对象的路径 */
        protected override string _m_assetName{ get { return _m_sAssetName; } }
    }
}

