using Hang.Net4.Base.Attributes;
using Hang.Net4.Base.Enums;
using NLog;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hang.Tools.Views.Pages
{
    [Plugin(Name = "Socket客户端", Type = PluginType.Page)]
    public partial class Page_SocketClient : Page, INotifyPropertyChanged
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly object _lock = new object();
        public event PropertyChangedEventHandler PropertyChanged;

        private Socket _socket = null;

        private string _ip = "127.0.0.1";
        private int _port = 9191;
        private string _text = "<ROOT><REQUEST>QueueAll</REQUEST><DATAS><DATA><THIRDNU2>0000000001</THIRDNU2><USERCODE>SJKH001</USERCODE><OUTLETSID>1733</OUTLETSID><MOBILENO>18319969200</MOBILENO></DATA></DATAS></ROOT>";
        private string _log;
        private int _parallelTimes = 1;
        private int _packingType = 0;

        public string Ip
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged("Ip");
            }
        }
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }
        public string Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
                OnPropertyChanged("Log");
            }
        }
        public int ParallelTimes
        {
            get
            {
                return _parallelTimes;
            }

            set
            {
                _parallelTimes = value; OnPropertyChanged("ParallelTimes");
            }
        }

        public Page_SocketClient()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 连接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_socket != null && _socket.Connected == true)
                {
                    _socket.Close();
                }
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true,
                    SendTimeout = 1000
                };
                _socket.Connect(new IPEndPoint(IPAddress.Parse(Ip), Port)); //连接到服务器
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        byte[] recvBytes = new byte[10240];
                        int bytes;

                        try
                        {
                            bytes = _socket.Receive(recvBytes, recvBytes.Length, SocketFlags.None);//从服务器端接受返回信息

                            if (bytes == 0)
                            {
                                //https://msdn.microsoft.com/zh-cn/library/8s4y8aff%28v=vs.80%29.aspx
                                //如果远程主机使用 Shutdown 方法关闭了 Socket 连接，并且所有可用数据均已收到，则 Receive 方法将立即完成并返回零字节
                                _socket.Close();
                                WriteLog("服务器断开了连接");
                                return;
                            }
                            WriteLog("收到数据长度:{0}", bytes.ToString());
                            byte[] Get = new byte[bytes];
                            for (int i = 0; i < Get.Length; i++)
                            {
                                Get[i] = recvBytes[i];
                            }

                            //byte[] temp = new byte[] { recvBytes[0], recvBytes[1], recvBytes[2], recvBytes[3] };
                            //int a = BitConverter.ToInt32(temp, 0);

                            WriteLog("====== {0} ======\n{1}\n", DateTime.Now.ToString(), Encoding.UTF8.GetString(Get));
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.ToString());
                            return;
                        }
                    }
                });
                WriteLog("Connect成功");
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                return;
            }
        }

        /// <summary>
        /// 关闭连接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _socket.Close();
                _socket.Dispose();
                WriteLog("Close成功");
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                return;
            }
        }

        /// <summary>
        /// 发送数据按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            if (_socket == null || _socket.Connected == false)
            {
                WriteLog("Socket未连接");
                return;
            }

            byte[] content = Encoding.UTF8.GetBytes(Text);
            byte[] head = BitConverter.GetBytes(content.Length);//head.Length==4
            byte[] send = new byte[head.Length + content.Length];
            head.CopyTo(send, 0);
            content.CopyTo(send, head.Length);

            try
            {
                switch (_packingType)
                {
                    case 0:
                        _socket.Send(content, content.Length, SocketFlags.None); //发送信息
                        break;
                    case 1:
                        _socket.Send(send, send.Length, SocketFlags.None); //发送信息
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                return;
            }

            WriteLog("发送成功");
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="parm"></param>
        private void WriteLog(string log, params string[] parm)
        {
            log = string.Format(log, parm);
            _logger.Info(log);
            Log += log + "\n";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ComboBox_PackingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _packingType = ((ComboBox)sender).SelectedIndex;
        }

        /// <summary>
        /// 并行发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ParallelSend_Click(object sender, RoutedEventArgs e)
        {
            Log = "";//清空软件显示的日志

            WriteLog("=======================");
            WriteLog("Parallel Send Start >>>");
            WriteLog("=======================");

            Task.Factory.StartNew(() =>
            {
                try
                {
                    Parallel.For(0, ParallelTimes, (pi) =>
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                        {
                            NoDelay = true,
                            //SendTimeout = 3000,
                        };
                        socket.Connect(new IPEndPoint(IPAddress.Parse(Ip), Port)); //连接到服务器

                        Task t = new Task(() =>
                            {
                                try
                                {
                                    byte[] recvBytes = new byte[4024];
                                    int bytes = socket.Receive(recvBytes, recvBytes.Length, SocketFlags.None);//从服务器端接受返回信息
                                    if (bytes == 0)
                                    {
                                        socket.Close();
                                        return;
                                    }
                                    byte[] Get = new byte[bytes];
                                    for (int i = 0; i < Get.Length; i++)
                                    {
                                        Get[i] = recvBytes[i];
                                    }
                                    socket.Close();
                                    WriteLog("# 连接完成 {0}\n{1}", pi.ToString(), Encoding.UTF8.GetString(Get));
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.ToString());
                                    return;
                                }
                            });

                        t.Start();

                        byte[] content = Encoding.UTF8.GetBytes(Text);
                        byte[] head = BitConverter.GetBytes(content.Length);//head.Length==4
                        byte[] send = new byte[head.Length + content.Length];
                        head.CopyTo(send, 0);
                        content.CopyTo(send, head.Length);

                        try
                        {
                            socket.Send(send, send.Length, SocketFlags.None); //发送信息
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.ToString());
                            return;
                        }

                        t.Wait();
                    });
                }
                catch (Exception ex)
                {
                    WriteLog(ex.ToString());
                }
                WriteLog("========================");
                WriteLog("Parallel Send Finish <<<");
                WriteLog("========================");
            });
        }

        private void TextBox_Log_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TextBox_Log.SelectionStart = int.MaxValue;
                TextBox_Log.ScrollToEnd();
            }));
        }
    }
}
