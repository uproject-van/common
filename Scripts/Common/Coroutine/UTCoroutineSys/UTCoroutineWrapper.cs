using System.Collections;

namespace UTGame
{
    public class UTCoroutineWrapper : _IALCoroutineDealer
    {
        private IEnumerator _m_coroutine;

        public UTCoroutineWrapper(IEnumerator _coroutine)
        {
            _m_coroutine = _coroutine;
        }

        /*******************
         * Coroutine的执行函数体
         **/
        public IEnumerator dealCoroutine()
        {
            yield return _m_coroutine;
        }
    }
}