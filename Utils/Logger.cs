using log4net;
using System;

namespace MMC.Utils
{
    public static class Logger
    {
        private static readonly ILog _Log;

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure();
            _Log = LogManager.GetLogger("Results");
        }
        public static void LogError(object msg)
        {
            _Log.Error(msg);
        }
        public static void LogError(object msg, Exception ex)
        {
            _Log.Error(msg, ex);
        }
        public static void LogError(Exception ex)
        {
            _Log.Error(ex.Message, ex);
        }
        public static void LogInfo(object msg)
        {
            _Log.Info(msg);
        }
    }
}
