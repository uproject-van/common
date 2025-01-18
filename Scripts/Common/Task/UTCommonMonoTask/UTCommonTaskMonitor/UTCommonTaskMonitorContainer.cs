using System.Collections.Generic;
using UnityEngine;

namespace UTGame
{
#if UNITY_EDITOR
    /// <summary>
    /// 用于监控所有任务控制对象，需要在最后释放的时候的管理器。
    /// 可以通过管理器在关键节点进行释放，并打印未释放的对象
    /// </summary>
    public class UTCommonTaskMonitorContainer
    {
        //监控接口对象容器
        private HashSet<_IUTCommonTaskMonitorInterface> _m_hsMonitorInterfaceSet;

        public UTCommonTaskMonitorContainer()
        {
            _m_hsMonitorInterfaceSet = new HashSet<_IUTCommonTaskMonitorInterface>();
        }

        /// <summary>
        /// 增删接口对象
        /// </summary>
        /// <param name="_interface"></param>
        public void addMonitor(_IUTCommonTaskMonitorInterface _interface)
        {
            if (null == _interface)
                return;

            _m_hsMonitorInterfaceSet.Add(_interface);
        }
        public void rmvMonitor(_IUTCommonTaskMonitorInterface _interface)
        {
            if (null == _interface)
                return;

            _m_hsMonitorInterfaceSet.Remove(_interface);
        }

        /// <summary>
        /// 清理所有数据对象
        /// </summary>
        public void clearAll()
        {
            //逐个输出
            foreach(_IUTCommonTaskMonitorInterface interfaceObj in _m_hsMonitorInterfaceSet)
            {
                if (null == interfaceObj)
                    continue;

                //输出错误信息
                Debug.LogError($"Task {interfaceObj} Didn't discard!");

                //调用释放后函数
                interfaceObj.discard();
            }

            _m_hsMonitorInterfaceSet.Clear();
        }
    }
#endif
}
