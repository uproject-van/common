namespace UTGame
{
    //输出日志等级
    public enum UTLogLevel
    {
        VERBOSE = 10,
        DEBUG = 20,
        SYS_OUT = 30,
        WARNING = 40,
        ERROR = 50,
        CRUSH = 60,
    }

    public class UTLog
    {
        private static UTLog g_instance = new UTLog();

        public static UTLog Instance
        {
            get
            {
                if (null == g_instance)
                    g_instance = new UTLog();

                return g_instance;
            }
        }

        private void showLog(UTLogLevel _logLvl, string _str)
        {
            if (_logLvl >= GameMain.instance.logLevel)
            {
                if (_logLvl == UTLogLevel.CRUSH || _logLvl == UTLogLevel.ERROR)
                    //输出到异常信息输出窗口
                    UnityEngine.Debug.LogError("ALLog - [ " + _logLvl.ToString() + " ] " + _str);
                else if (_logLvl == UTLogLevel.WARNING)
                    UnityEngine.Debug.LogWarning("ALLog - [ " + _logLvl.ToString() + " ] " + _str);
                else
                    UnityEngine.Debug.Log("ALLog - [ " + _logLvl.ToString() + " ] " + _str);
            }
        }

        public static void Verbose(string _str)
        {
            Instance.showLog(UTLogLevel.VERBOSE, _str);
        }

        public static void Debug(string _str)
        {
            Instance.showLog(UTLogLevel.DEBUG, _str);
        }

        public static void Warning(string _str)
        {
            Instance.showLog(UTLogLevel.WARNING, _str);
        }

        public static void Error(string _str)
        {
            Instance.showLog(UTLogLevel.ERROR, _str);
        }

        public static void Crush(string _str)
        {
            Instance.showLog(UTLogLevel.CRUSH, _str);
        }

        public static void Sys(string _str)
        {
            Instance.showLog(UTLogLevel.SYS_OUT, _str);
        }
    }
}