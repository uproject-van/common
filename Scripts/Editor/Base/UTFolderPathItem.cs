namespace UTGame
{
    /***************
     * xml导出路径选择对象
     **/
    public class UTFolderPathItem : _AUTFolderSelectItem
    {
        private string _m_sText;
        private string _m_iDataKey;

        public UTFolderPathItem(string _text, string _dataKey)
        {
            _m_sText = _text;
            _m_iDataKey = _dataKey;
        }

        /****************
         * 显示的说明
         **/
        protected override string _text { get { return _m_sText; } }
        protected override string _dataKey { get { return _m_iDataKey; } }
    }
}
