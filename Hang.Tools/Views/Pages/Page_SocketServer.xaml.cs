using Hang.Net4.Base.Attributes;
using Hang.Net4.Base.Enums;
using Hang.Net4.Base.Interfaces;
using Hang.Net4.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hang.Tools.Views.Pages
{
    [Plugin(Name = "Socket服务器", Type = PluginType.Page)]
    public partial class Page_SocketServer : Page
    {
        public Page_SocketServer()
        {
            InitializeComponent();

            try
            {
                SocketServer ss = new SocketServer("0.0.0.0", 9191, handle);
                ss.Start();
                //Thread.Sleep(100);
                //ss.Dispose();
                //ss = new SocketServer("0.0.0.0", 9191, handle);
                //ss.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private byte[] handle(List<byte> arg)
        {
            return Encoding.UTF8.GetBytes("handle");
        }

    }
}
