using Hang.Net4.Base.Enums;
using System;

namespace Hang.Net4.Base.Attributes
{
    /// <summary>
    /// 插件特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PluginAttribute : Attribute
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public PluginType Type { get; set; } = PluginType.UnKnow;
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Tag { get; set; }
    }
}
