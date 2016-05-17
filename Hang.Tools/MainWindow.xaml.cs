using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Hang.Tools
{
    public partial class MainWindow : Window
    {
        public static Action<string, object> ShowPage;

        public MainWindow()
        {
            InitializeComponent();
        }

        public static void OnShowPage(string name, object obj)
        {
            ShowPage?.Invoke(name, obj);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowPage += AddNewTabItem;

            TabItem_Home.Content = new Frame
            {
                Content = new Views.Pages.Page_Home(),
                FocusVisualStyle = null,
                NavigationUIVisibility = NavigationUIVisibility.Hidden,
            };
        }

        private void AddNewTabItem(string name, object obj)
        {
            TabControl_Main.Items.Add(new TabItem
            {
                Header = name,
                Content = new Frame
                {
                    Content = (Page)obj,
                    FocusVisualStyle = null,
                    NavigationUIVisibility = NavigationUIVisibility.Hidden,
                },
                Height = 30,
                Margin = new Thickness(-2, 0, -2, -0),
                IsSelected = true,
            });
        }

    }
}
