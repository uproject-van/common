using UnityEngine;

namespace UTGame
{
    public partial class GCommon
    {
        /// <summary>
        /// 转换成实际显示的位置信息
        /// </summary>
        public static float getBattleRealUIPosX(int _xIdx)
        {
            float realX = Screen.width * 1.0f / 10 * _xIdx;
            return realX;
        }
        
        public static float getBattleRealUIPosY(int _yIdx)
        {
            float realY = Screen.height * 1.0f / 20 * _yIdx;
            return realY;
        }
        
        #region 将屏幕坐标转换成场景坐标
        
        public static Vector3 chgUIPosToWorldPos(Vector2 _uiPos)
        {
            float zPosition = -Camera.main.transform.position.z;
            Vector3 screenPosition = new Vector3(_uiPos.x, _uiPos.y, zPosition);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        
        #endregion
    }
}