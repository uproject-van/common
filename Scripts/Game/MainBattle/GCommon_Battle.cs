using UnityEngine;

namespace UTGame
{
    public partial class GCommon
    {

        public static float getWorldWidth()
        {
            return UTBattleMain.instance.endPos.x - UTBattleMain.instance.startPos.x;
        }
        
        #region 将屏幕坐标转换成场景坐标
        
        public static Vector3 chgUIPosToWorldPos(Vector2 _uiPos)
        {
            float zPosition = -GameMain.instance.mainCamera.transform.position.z;
            Vector3 screenPosition = new Vector3(_uiPos.x, _uiPos.y, zPosition);
            Vector3 worldPosition = GameMain.instance.mainCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
        
        public static float chgUIPosXToWorldPosX(float _uiPosX)
        {
            float zPosition = -GameMain.instance.mainCamera.transform.position.z;
            Vector3 screenPosition = new Vector3(_uiPosX, 0, zPosition);
            Vector3 worldPosition = GameMain.instance.mainCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition.x;
        }
        
        public static float chgUIPosYToWorldPosY(float _uiPosY)
        {
            float zPosition = -GameMain.instance.mainCamera.transform.position.z;
            Vector3 screenPosition = new Vector3(0, _uiPosY, zPosition);
            Vector3 worldPosition = GameMain.instance.mainCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition.y;
        }
        #endregion
    }
}