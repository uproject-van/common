namespace UTGame
{
    /// <summary>
    /// 用于监控和处理任务关联的接口对象
    /// </summary>
    public interface _IUTCommonTaskMonitorInterface
    {
        /// <summary>
        /// 释放本任务对象的相关资源或者关联
        /// </summary>
        void discard();
    }
}
