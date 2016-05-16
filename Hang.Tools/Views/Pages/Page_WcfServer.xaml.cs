using Hang.Net4.Base.Attributes;
using Hang.Net4.Base.Enums;
using Hang.Net4.Base.Interfaces;
using Hang.Net4.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [Plugin(Name = "Wcf服务器", Type = PluginType.Page)]
    public partial class Page_WcfServer : Page
    {
        public Page_WcfServer()
        {
            InitializeComponent();

            WcfServer ws = new WcfServer("127.0.0.1", 8888, typeof(Test), typeof(IBaseServiceContract));
            ws.Open();
        }
    }

    public class Test : IBaseServiceContract
    {
        public string Action(string parameter)
        {
            return "~" + parameter;
        }
    }
}
