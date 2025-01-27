namespace UTGame
{
    /// <summary>
    /// UI生成节点node枚举
    /// </summary>
    public enum EUIQueueNode
    {
        NORMAL,
        ADD,
        TOP,
    }

    /// <summary>
    /// 当前战斗状态
    /// </summary>
    public enum EBattleStauts
    {
        READY,//已准备
        GAMING,//游戏中
        END,//游戏结束
    }
    /// <summary>
    /// 颜色枚举
    /// </summary>
    public enum EColor
    {
        NONE,
        YELLOW,
        RED,
        GREEN,
        BLUE,
        ORANGE,
    }
    
    /// <summary>
    /// 碰撞物类型枚举
    /// </summary>
    public enum EObstacleType
    {
        NONE,
        FIXED,
    }
    
    /// <summary>
    /// 碰撞物的大小类型
    /// </summary>
    public enum EObstacleSize
    {
        SMALL,
        MEDIUM,
        LARGE,
    }
    
    //触发效果枚举
    public enum EObstacleEffectType
    {
        NONE,//无效果
        CHG_COLOR,//改变颜色
    }
}