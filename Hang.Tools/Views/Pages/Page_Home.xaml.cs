using Hang.Net4.Base.Attributes;
using Hang.Net4.Base.Enums;
using Hang.Net4.Base.Interfaces;
using Hang.Net4.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    public partial class Page_Home : Page
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Page_Home()
        {
            InitializeComponent();

            try
            {
                var files = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.*", SearchOption.AllDirectories).Where(s => s.ToLower().EndsWith(".exe") || s.ToLower().EndsWith(".dll"));
                foreach (var file in files)
                {
                    Assembly ab = Assembly.LoadFrom(file);
                    foreach (Type t in ab.GetTypes())
                    {
                        var attrs = t.GetCustomAttributes(typeof(PluginAttribute), true);
                        foreach (PluginAttribute pa in attrs)
                        {
                            if (pa.Type == PluginType.Page)
                            {
                                Button b = new Button
                                {
                                    Content = pa.Name,
                                    Tag = new Tuple<string, Assembly>(t.FullName, ab),
                                    Margin = new Thickness(5),
                                    Padding = new Thickness(3, 2, 3, 2),
                                };
                                b.Click += (s, e) =>
                                {
                                    var btn = s as Button;
                                    var tag = btn.Tag as Tuple<string, Assembly>;
                                    MainWindow.OnShowPage(btn.Content.ToString(), tag.Item2.CreateInstance(tag.Item1));
                                };

                                WrapPanel_PluginList.Children.Add(b);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
