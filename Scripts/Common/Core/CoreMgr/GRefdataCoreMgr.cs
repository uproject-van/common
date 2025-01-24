using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UTGame
{
    public partial class GRefdataCoreMgr : _AUTBaseRefdataCoreMgr
    {
        private static GRefdataCoreMgr _g_instance;
        [NotNull] public static GRefdataCoreMgr instance
        {
            get
            {
                if (_g_instance == null)
                {
                    _g_instance = new GRefdataCoreMgr();
                }
                return _g_instance;
            }
        }
        
        //通用配置表
        public UTBaseListRefCore<UTGeneralRefObj> npGeneralMap = new UTBaseListRefCore<UTGeneralRefObj>(
            UTSOGeneralRefSet.assetName);
        public UTGeneralRefObj npGeneral = null;
    
        public UTBaseListRefCore<UTObstacleTypeRefObj> obstacleTypeListCore = new UTBaseListRefCore<UTObstacleTypeRefObj>(UTSOObstacleTypeRefSet.assetName);
        public UTBaseListRefCore<UTObstacleRefObj> obstacleListCore = new UTBaseListRefCore<UTObstacleRefObj>(UTSOObstacleRefSet.assetName);
        public UTBaseListRefCore<UTStageRefObj> stageListCore = new UTBaseListRefCore<UTStageRefObj>(UTSOStageRefSet.assetName);

        
        //是否初始化完成
        private bool _m_bIsInitDone = false;

        public GRefdataCoreMgr()
            : base()
        {
            _m_bIsInitDone = false;
        }

        public bool isInitDone { get { return _m_bIsInitDone; } }

        //初始化加载队列
        protected override void _initLoadList(List<_IUTInitRefObj> _list)
        {
            //通用配置表
            _list.Add(npGeneralMap);
            _list.Add(stageListCore);
        }


        //TODO 提示信息
        /** 加载失败处理 */
        protected override void _onRefInitFail(Type _class, _IUTInitRefObj _obj)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
        // if(null == _class)
        //     NPMesMgr.instance.showOneBtnMes(TextTranslate.instance.getLanguage(TransKeyConst.init_gameres_fail_str, "unkown"), TextTranslate.instance.getLanguage(TransKeyConst.confirm), _obj.finalDelegate);
        // else
        //     NPMesMgr.instance.showOneBtnMes(TextTranslate.instance.getLanguage(TransKeyConst.init_gameres_fail_str, _class.ToString()), TextTranslate.instance.getLanguage(TransKeyConst.confirm), _obj.finalDelegate);
#else
        if (null == _class)
            NPMesMgr.instance.showOneBtnMes(TextTranslate.instance.getLanguage(TransKeyConst.init_gameres_fail_str, "unkown"), TextTranslate.instance.getLanguage(TransKeyConst.confirm), _obj.finalDelegate);
        else
            NPMesMgr.instance.showOneBtnMes(TextTranslate.instance.getLanguage(TransKeyConst.init_gameres_fail_str, _class.ToString()), TextTranslate.instance.getLanguage(TransKeyConst.confirm), _obj.finalDelegate);
#endif

            //设置初始化完成
            _m_bIsInitDone = true;
        }


        protected override void _onInitAllRefCore()
        {
            //通用配置表
            npGeneral = npGeneralMap.getRef(UTSOGeneralRefSet.generalId);

            //设置初始化完成
            _m_bIsInitDone = true;

            //editor下校验一次资源
// #if UNITY_EDITOR
//             UIResPathAssistant.CheckRes();
// #endif
        }
    }
}
