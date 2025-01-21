using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UTGame
{
    /// <summary>
    /// 一个极其简单的状态机，只有状态进入和退出调用
    /// </summary>
    public class SimpleStateMachine<T> where T : Enum
    {
        // 当前所处的状态
        [NotNull]
        private _ASimpleState<T> _m_curState;

        [NotNull]
        private List<_ASimpleState<T>> _m_stateList;

        public SimpleStateMachine()
        {
            // 构造一个空状态
            NoneSimpleState<T> noneState = new NoneSimpleState<T>(this);
            _m_stateList = new List<_ASimpleState<T>>();
            _m_curState = noneState;
            _m_stateList.Add(noneState);
        }

        /// <summary>
        /// 当状态机的状态发生变化
        /// </summary>
        public event Action<T, T> onStateChg;

        /// <summary>
        /// 当前所处的状态
        /// </summary>
        [NotNull]
        public _ASimpleStateBase<T> curState
        {
            get { return _m_curState; }
        }
        
        public void clear()
        {
            foreach (var state in _m_stateList)
            {
                state.discard();
            }
            _m_stateList.Clear();
        }
        
        public void addState(_ASimpleState<T> _state)
        {
            _m_stateList.Add(_state);
        }

        public _ASimpleState<T> getState(T _stateE)
        {
            _ASimpleState<T> temp = null;
            for (int i = 0; i < _m_stateList.Count; i++)
            {
                temp = _m_stateList[i];
                if (null == temp)
                    continue;

                if (temp.state.Equals(_stateE))
                    return temp;
            }

            return null;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void changeState(T _stateE,params object[] _args)
        {
            _ASimpleState<T> state = getState(_stateE);
            if (null == state)
                return;

            if (!_m_curState.canEnterState(_stateE))
                return;

            T lastStateType = _m_curState.state;
            _m_curState.exit();
            _m_curState = state;
            state.enter(_args);
            onStateChg?.Invoke(lastStateType, _stateE);
        }
    }
}