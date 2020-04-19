using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UI
{
    public class LoggerHelper
    {
        private static ILog log;
        static LoggerHelper()
        {
            if (log == null)
            {
                ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                log = LogManager.GetLogger(repository.Name, "NETCorelog4net");
            }
        }

        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception = null)
        {
            if (exception == null)
                log.Info(message);
            else
                log.Info(message, exception);
        }

        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception = null)
        {
            if (exception == null)
                log.Warn(message);
            else
                log.Warn(message, exception);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></cannot be null.param>
        public static void Error(string message, Exception exception = null)
        {
            if (exception == null)
                log.Error(message);
            else
                log.Error(message, exception);
        }
    }
}
