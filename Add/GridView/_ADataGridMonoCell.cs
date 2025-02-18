namespace NGame
{
    /// <summary>
    /// 带数据的cell基类
    /// </summary>
    public abstract class _ADataGridMonoCell<T_DATA> : _AGridMonoCellBase where T_DATA : _IBaseItem
    {
        protected T_DATA _m_data;

        protected abstract void _refresh();
        
        protected override void _OnEnableEx()
        {
            _refresh();
        }
        
        public void setData(T_DATA _data)
        {
            if (null == _data)
                return;
            
            _m_data = _data;
            _refresh();
        }
        
    }
}