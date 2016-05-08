using Hang.Net4.Base.Enums;
using System;

namespace Hang.Net4.Base.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        Tuple<PluginType, object> Register();
        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        Tuple<bool, object> UnRegister();

        //event 
    }
}
