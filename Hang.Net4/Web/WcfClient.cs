using Hang.Net4.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Hang.Net4.Web
{
    public class WcfClient
    {
        public T GetProxy<T>(string address)
        {
            EndpointAddress endpoint = new EndpointAddress(address);
            Binding binding = new WSHttpBinding(SecurityMode.None)
            {
                MaxReceivedMessageSize = long.MaxValue
            };
            return ChannelFactory<T>.CreateChannel(binding, endpoint);
        }
    }
}
