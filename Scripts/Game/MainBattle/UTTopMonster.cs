using System;
using UnityEngine;

namespace UTGame
{
    /// <summary>
    /// 顶部的怪物对象
    /// </summary>
    public class UTTopMonster : MonoBehaviour
    {
        private void Start()
        {
            //设置顶部怪物的宽度
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if(null == spriteRenderer)
                return;
            
            spriteRenderer.size = new Vector2(GCommon.getWorldWidth(), 1);
        }
    }
}