using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Hang.Net4.Web
{
    public class WcfServer : IDisposable
    {
        private ServiceHost _host;

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
                Uri baseAddress = new Uri(string.Format("http://{0}:{1}/{2}", ip, port, name));

                Binding binding = new WSHttpBinding(SecurityMode.None)
                {
                    MaxReceivedMessageSize = int.MaxValue
                };

                _host = new ServiceHost(implementedContract, baseAddress);

                _host.AddServiceEndpoint(contractInterface, binding, baseAddress);

                if (_host.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
                {
                    _host.Description.Behaviors.Add(new ServiceMetadataBehavior
                    {
                        HttpGetEnabled = true,
                        //HttpGetUrl = new Uri(baseAddress.ToString() + "/metadata"),
                    });
                }
            }
            catch (Exception ex)
            {
                _host.Abort();
                _host = null;
            }
        }

        ~WcfServer()
        {

        }

        public bool Open()
        {
            if (_host != null)
            {
                _host.Open();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Close()
        {

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
