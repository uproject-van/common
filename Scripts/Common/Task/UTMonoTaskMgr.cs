using System.Collections.Generic;
using JetBrains.Annotations;

namespace UTGame
{
    /// <summary>
    /// 全局的任务管理器
    /// </summary>
    public class UTMonoTaskMgr
    {
        private static UTMonoTaskMgr _g_instance = new UTMonoTaskMgr();
        [NotNull]
        public static UTMonoTaskMgr instance
        {
            get
            {
                if (ReferenceEquals(null, _g_instance))
                    _g_instance = new UTMonoTaskMgr();

                return _g_instance;
            }
        }

        /** 需要在Update后放入处理任务队列的所有任务 */
        private List<_IUTBaseMonoTask> _m_lNextFrameMonoTaskList;
        private List<_IUTBaseMonoTask> _m_lNextFrameLaterMonoTaskList;
        private List<_IUTBaseMonoTask> _m_lNextFixedUpdateMonoTaskList;


        
        protected UTMonoTaskMgr()
        {
            _m_lNextFrameMonoTaskList = new List<_IUTBaseMonoTask>();
            _m_lNextFrameLaterMonoTaskList = new List<_IUTBaseMonoTask>();
            _m_lNextFixedUpdateMonoTaskList = new List<_IUTBaseMonoTask>();
        }

     

        /****************
         * 添加一个需要执行Coroutine的对象
         **/
        public void addNextFrameTask(_IUTBaseMonoTask _task)
        {
            if(ReferenceEquals(null, _task))
                return;

            _m_lNextFrameMonoTaskList
                .Add(_task);
        }

        public void getAllNextFrameTask(List<_IUTBaseMonoTask> _taskList)
        {
            _taskList.AddRange(_m_lNextFrameMonoTaskList);
        }
      
    }
}
