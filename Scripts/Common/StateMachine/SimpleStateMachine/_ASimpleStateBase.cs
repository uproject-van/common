using System;

namespace UTGame
{
    /// <summary>
    /// 一个状态通用的基类
    /// </summary>
    /// <remarks>
    /// 外部实现的时候不要直接继承这个类，继承<see cref="_ASimpleState{T}"/>系列的类
    /// </remarks>
    public abstract class _ASimpleStateBase<T> where T : Enum
    {
        //状态机
        protected SimpleStateMachine<T> _m_machine;
        
        // enter 的序列号，序列号还是一样的说明还处于同一次 enter 之中
        private int _m_enterSerialize;
        /// <summary>
        /// enter 的序列号，序列号还是一样的说明还处于同一次 enter 之中
        /// </summary>
        public int enterSerialize { get { return _m_enterSerialize; } }
        /// <summary>
        /// 这个状态是什么状态
        /// </summary>
        public abstract T state { get; }
        
        protected _ASimpleStateBase(SimpleStateMachine<T> _machine)
        {
            // 先自增一次序列号，给下一次 enter 使用
            _m_enterSerialize = UTSerializeOpMgr.next();
            
            _m_machine = _machine;
        }

        internal void exit()
        {
            _onExit();
            // 增加序列号，给下一次 enter 使用
            _m_enterSerialize = UTSerializeOpMgr.next();
        }

        public virtual void discard()
        {
            _m_machine = null;
        }
        
        /// <summary>
        /// 当退出这个状态时的处理
        /// </summary>
        protected abstract void _onExit();
        /// <summary>
        /// 判断是否可以进入新状态
        /// </summary>
        public abstract bool canEnterState(T _newState);
    }
    /// <summary>
    /// 一个状态机使用的状态
    /// </summary>
    /// <remarks>
    /// 切换到这个状态不需要参数
    /// </remarks>
    public abstract class _ASimpleState<T> : _ASimpleStateBase<T> where T : Enum
    {
        protected _ASimpleState(SimpleStateMachine<T> _machine):base(_machine)
        {
        }

        internal void enter(params object[] _args)
        {
            _onEnter(_args);
        }

        protected abstract void _onEnter(params object[] _args);
    }
}