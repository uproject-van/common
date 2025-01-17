using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 所以子控件的基类
    /// </summary>
    public abstract class _ABaseMono: MonoBehaviour
    {
        protected void Awake()
        {
            _OnInitEx();
        }
        
        public void OnDestroy()
        {
            _OnDestroyEx();
        }

        public void OnEnable()
        {
            _OnEnableEx();
        }

        public virtual void OnDisable()
        {
            _OnDisableEx();
        }
        
        protected abstract void _OnInitEx();
        protected abstract void _OnDestroyEx();
        protected abstract void _OnEnableEx();
        protected abstract void _OnDisableEx();
    }
}