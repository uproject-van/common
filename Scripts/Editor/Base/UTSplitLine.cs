using UnityEngine;

namespace UTGame
{
    public class UTSplitLine : _IUTExportMenuInterface
    {
        public UTSplitLine()
        {
        }

        public virtual bool needShow { get { return true; } }

        //具体的gui绘制函数
        public void onGUI()
        {
            //输出文本信息
            GUILayout.Label("-----------------------------------", GUILayout.Height(10));
        }
    }
}
