﻿using Hang.Net4.Base.Attributes;
using Hang.Net4.Base.Enums;
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
    [Plugin(Name = "Socket客户端", Type = PluginType.Page)]
    public partial class Page_SocketClient : Page
    {
        public Page_SocketClient()
        {
            InitializeComponent();
        }
    }
}
