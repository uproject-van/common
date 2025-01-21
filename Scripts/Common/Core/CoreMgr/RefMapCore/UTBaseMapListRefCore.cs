using UnityEngine;
using System.Collections;
using System;


namespace UTGame
{
    public class UTBaseMapListRefCore<T> : _ATUTBaseRefMapListCore<T> where T : _IUTBaseRefObj
    {
        private string _m_sAssetName;

        public UTBaseMapListRefCore(string _assetName)
        {
            _m_sAssetName = _assetName;
        }
        
        protected override string _assetName { get{ return _m_sAssetName; }}
    }
}

