using System.Collections.Generic;
using UnityEngine;

/*********************
 * 通用的信息数据接口集合模板对象，在具体功能中可以用通用模板对象进行加载和处理
 **/
namespace UTGame
{
    public class _TUTSOBaseRefSet<T> : ScriptableObject where T : _IUTBaseRefObj
    {
        /** 存储的信息接口对象队列 */
        public List<T> refList;

        /***************
         * 根据id检索对应数据
         **/
        public T searchRef(long _id)
        {
            for (int i = 0; i < refList.Count; i++)
            {
                if (refList[i]._refId == _id)
                    return refList[i];
            }

            return default(T);
        }
    }
}