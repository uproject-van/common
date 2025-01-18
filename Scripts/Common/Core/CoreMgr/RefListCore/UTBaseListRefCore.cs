namespace UTGame
{
    public class UTBaseListRefCore<T> : _ATUTBaseRefListCore<T> where T : _IUTBaseRefObj
    {
        private string _m_sAssetPath;
        private string _m_sObjName;

        public UTBaseListRefCore(string _assetPath, string _objName)
        {
            _m_sAssetPath = _assetPath;
            _m_sObjName = _objName;
        }

        /** 获取加载资源对象的路径 */
        protected override string _assetPath { get { return _m_sAssetPath; } }
        protected override string _objName { get { return _m_sObjName; } }
    }
}

