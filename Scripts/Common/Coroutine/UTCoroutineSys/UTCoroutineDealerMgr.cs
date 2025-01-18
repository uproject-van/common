using System;
using System.Collections.Generic;

namespace UTGame
{
    class UTCoroutineDealerMgr
    {
        private static UTCoroutineDealerMgr _g_instance = new UTCoroutineDealerMgr();
        public static UTCoroutineDealerMgr instance
        {
            get
            {
                if (null == _g_instance)
                    _g_instance = new UTCoroutineDealerMgr();

                return _g_instance;
            }
        }

        /** 需要开启Coroutine的对象列表 */
        private LinkedList<_IALCoroutineDealer> _m_lCoroutineDealerList;

        protected UTCoroutineDealerMgr()
        {
            _m_lCoroutineDealerList = new LinkedList<_IALCoroutineDealer>();
        }

        /****************
         * 添加一个需要执行Coroutine的对象
         **/
        public void addCoroutine(_IALCoroutineDealer _obj)
        {
            if (null == _obj)
                return;

            lock (_m_lCoroutineDealerList)
            {
                _m_lCoroutineDealerList.AddLast(_obj);
            }
        }

        /****************
         * 取出一个需要执行的Coroutine对象
         **/
        public _IALCoroutineDealer popCoroutineObj()
        {
            lock (_m_lCoroutineDealerList)
            {
                if (_m_lCoroutineDealerList.Count <= 0)
                    return null;

                _IALCoroutineDealer obj = _m_lCoroutineDealerList.First.Value;
                _m_lCoroutineDealerList.RemoveFirst();

                return obj;
            }
        }

        /****************
         * 清除所有Coroutine
         **/
        public void clearAllCoroutineObj()
        {
            lock (_m_lCoroutineDealerList)
            {
                _m_lCoroutineDealerList.Clear();
            }
        }
    }
}
