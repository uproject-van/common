
namespace UTGame
{
    /// <summary>
    /// 用于定时器tick
    /// </summary>
    public interface _IUTBaseTaskInterface
    {
        /** 显示序列号接口对象 */
        long showOpSerialize { get; }

        /** 刷新函数 */
        void tick();

        virtual void fixedTick()
        {
            
        }
    }
}
