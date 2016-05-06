using Hang.Net4.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hang.Net4.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Tuple<PluginType, object> Register();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool UnRegister();
    }
}
