namespace UTGame
{
    /*********************
     * 延迟的任务信息
     **/
    public struct UTDelayMonoTaskInfo
    {
        //任务对象
        public _IUTBaseMonoTask task;
        //延迟时间
        public float delayTime;
        //是否Late处理对象
        public bool isLateTask;

        public UTDelayMonoTaskInfo(_IUTBaseMonoTask _task, float _delayTime, bool _isLateTask)
        {
            task = _task;
            delayTime = _delayTime;
            isLateTask = _isLateTask;
        }
    }
}
