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
        private ServiceHost _host;

        public WcfServer(string address, Type implementedContract)
        {
            try
            {
                _host = new ServiceHost(implementedContract);

                Binding binding = new WSHttpBinding(SecurityMode.None)
                {
                    MaxReceivedMessageSize = long.MaxValue
                };
                ServiceMetadataBehavior behavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri(address + "/metadata")
                };

                _host.AddServiceEndpoint(implementedContract, binding, address);
                _host.Description.Behaviors.Add(behavior);
            }
            catch (CommunicationException)
            {
                _host.Abort();
                _host = null;
            }
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
    }
}
