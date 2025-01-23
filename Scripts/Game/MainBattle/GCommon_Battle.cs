using UnityEngine;

namespace UTGame
{
    public partial class GCommon
    {
        /// <summary>
        /// 转换成实际显示的位置信息
        /// </summary>
        public static float calBattleRealPosX(int _x)
        {
            float realX = Screen.width * 1.0f / 10 * _x;
            // 转换屏幕坐标为世界坐标
            Vector3 screenPosition = new Vector3(realX, 0, Camera.main.nearClipPlane);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            return worldPosition.x;
        }
        
        public static float calBattleRealPosY(int _y)
        {
            float realY = Screen.height * 1.0f / 20 * _y;
            // 转换屏幕坐标为世界坐标
            Vector3 screenPosition = new Vector3(0, realY, Camera.main.nearClipPlane);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            return worldPosition.y;
        }
        
    }
}