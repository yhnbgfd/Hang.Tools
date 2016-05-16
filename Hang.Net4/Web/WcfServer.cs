using Hang.Net4.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Hang.Net4.Web
{
    public class WcfServer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private ServiceHost _host;

        /// <summary>
        /// 服务地址 Self-Host自承载模式只支持一个地址
        /// </summary>
        public Uri BaseAddresss { get; set; }
        /// <summary>
        /// 实现ContractInterface接口的类
        /// </summary>
        public Type ImplementedContract { get; set; }
        /// <summary>
        /// 拥有ServiceContract特性的接口
        /// </summary>
        public Type ContractInterface { get; set; }

        /// <summary>
        /// WCF Server
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="implementedContract"></param>
        /// <param name="contractInterface"></param>
        /// <param name="name"></param>
        public WcfServer(string ip, int port, Type implementedContract, Type contractInterface, string name = "service")
        {
            try
            {
                BaseAddresss = new Uri(string.Format("http://{0}:{1}/{2}", ip, port, name));
                ImplementedContract = implementedContract;
                ContractInterface = contractInterface;
            }
            catch (Exception ex)
            {
                _host.Abort();
                _logger.ErrorEx(ex.ToString());
            }
        }

        /// <summary>
        /// 打开WCF服务
        /// </summary>
        public void Open()
        {
            try
            {
                Binding binding = new WSHttpBinding(SecurityMode.None)
                {
                    MaxReceivedMessageSize = int.MaxValue
                };

                _host = new ServiceHost(ImplementedContract, BaseAddresss);

                _host.AddServiceEndpoint(ContractInterface, binding, BaseAddresss);

                if (_host.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)//元数据行为
                {
                    _host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true });
                }

                _host.Opened += (s, e) =>
                {
                    _logger.Info("WCF服务启动成功 {0}", BaseAddresss);
                };

                _host.Open();
            }
            catch (Exception ex)
            {
                _logger.ErrorEx("WCF服务启动失败 " + ex.ToString());
            }
        }

        public void Close()
        {
            _host.Close();
        }

    }
}
