using System;

namespace UTGame
{
    public class UTActionMonoTask : _IUTBaseMonoTask
    {
        private Action _m_action;

        public UTActionMonoTask(Action _action)
        {
            _m_action = _action;
        }

        /// <summary>
        /// 任务具体的执行函数
        /// </summary>
        /// <returns></returns>
        public void deal()
        {
            if (null != _m_action)
                _m_action();

            _m_action = null;
        }
    }
}