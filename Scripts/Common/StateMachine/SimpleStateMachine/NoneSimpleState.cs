
using System;

namespace UTGame
{
    /// <summary>
    /// 一个空状态，什么都不做
    /// </summary>
    public class NoneSimpleState<T> : _ASimpleState<T> where T : Enum
    {
        public NoneSimpleState(SimpleStateMachine<T> _machine) : base(_machine)
        {
        }

        public override T state { get { return default; } }

        public override bool canEnterState(T _newState)
        {
            return true;
        }

        protected override void _onEnter(params object[] _args)
        {
        }
        protected override void _onExit()
        {
        }


    }
}