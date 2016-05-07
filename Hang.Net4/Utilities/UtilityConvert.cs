using NLog;
using System;

namespace Hang.Net4.Utilities
{
    /// <summary>
    /// 实用类型转换
    /// </summary>
    public static class UtilityConvert
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 对象转换
        /// </summary>
        /// <typeparam name="T">要转换成的类型</typeparam>
        /// <param name="obj">要转换的对象</param>
        /// <param name="defaultValue">转换失败时的默认值</param>
        /// <returns></returns>
        public static T To<T>(this object obj, T defaultValue = default(T))
        {
            T ret = defaultValue;
            try
            {
                ret = (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.ToString());
            }
            return ret;
        }

    }
}
