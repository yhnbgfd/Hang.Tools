using NLog;
using System;

namespace Hang.Net4.Utilities
{
    public static class NLogExtend
    {
        /// <summary>
        /// 写日志并抛异常
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void WarnEx(this Logger logger, string message)
        {
            logger.Warn(message);
            throw new Exception(message);
        }

        /// <summary>
        /// 写日志并抛异常
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void ErrorEx(this Logger logger, string message)
        {
            logger.Error(message);
            throw new Exception(message);
        }

        /// <summary>
        /// 写日志并抛异常
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        public static void FatalEx(this Logger logger, string message)
        {
            logger.Fatal(message);
            throw new Exception(message);
        }

    }
}
