using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 战斗的相机跟随
    /// </summary>
    public class UTBattleCameraFollow : _AMonoBase
    {
        [Header("需要跟随的物体")]
        public Transform target;

        [Header("相机与物体的偏移")]
        public Vector3 offset; // 

        [Header("平滑速度")]
        public float smoothSpeed = 0.125f; // 

        [Header("相机的最小范围")]
        public Vector2 minBounds; // 

        [Header("相机的最大范围")]
        public Vector2 maxBounds;

        void LateUpdate()
        {
            if (target != null)
            {
                Vector3 desiredPosition = target.position + offset;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

                // 限制相机在指定范围内
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);

                transform.position = smoothedPosition;
            }
        }
    }
}