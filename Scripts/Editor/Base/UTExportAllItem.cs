using System;


namespace UTGame
{
    /// <summary>
    /// 全部导出按钮
    /// </summary>
    public class UTExportAllItem : _AUTConfirmItem
    {
        //导出所有的操作函数
        private Action _m_dExportAllAction = default(Action);

        protected override string _text { get { return "全 部 导 出"; } }

        //处理点击
        protected override void _dealConfirm()
        {
            if (_m_dExportAllAction != null) {
                _m_dExportAllAction();
            }
        }

        //注册点击回调
        public void regExportFunc(Action _action)
        {
            if (null == _action)
                return;

            _m_dExportAllAction += _action;
        }
    }

    //选择按钮
    public class UTSelectItem : _AUTToggleItem
    {
        private string _m_sText;

        private Action<bool> _m_dSelected = default(Action<bool>);

        public UTSelectItem(string _text) {
             _m_sText = _text;
        }

        public UTSelectItem(string _text, Action<bool> _action) {
            _m_sText = _text;
            _m_dSelected = _action;
        }

        protected override string _text { get { return _m_sText; } }

        protected override void _dealTgeChg (bool _isOn) {

           if(_m_dSelected != null)
               _m_dSelected(_isOn);
        }

        //注册选择回调
        public void regSelectFunc(Action<bool> _action)
        {
            if (null == _action)
                return;

            _m_dSelected += _action;
        }
    }
}

