namespace UTGame
{
    public interface _IUTExportMenuInterface
    {
        //具体的gui绘制函数
        void onGUI();

        //是否需要显示
        bool needShow { get; }
    }
}
