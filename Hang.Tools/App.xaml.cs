using System.Windows;
using System.Windows.Threading;

namespace Hang.Tools
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("应用程序出现了未捕获的异常，请联系开发商。\n" + e.Exception.Message);

            e.Handled = true;
        }
    }
}
