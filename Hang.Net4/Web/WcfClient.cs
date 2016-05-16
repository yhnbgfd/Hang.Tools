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
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TChannel"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        public TChannel GetProxy<TChannel>(string address)
        {
            EndpointAddress endpoint = new EndpointAddress(address);
            Binding binding = new WSHttpBinding(SecurityMode.None)
            {
                MaxReceivedMessageSize = int.MaxValue
            };
            return ChannelFactory<TChannel>.CreateChannel(binding, endpoint);
        }
    }
}
