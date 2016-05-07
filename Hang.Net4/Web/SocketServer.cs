using Hang.Net4.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Hang.Net4.Web
{
    public class SocketServer
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly object _lock = new object();

        /// <summary>
        /// 当前客户端连接数量
        /// </summary>
        private long _connCount = 0;

        private Socket _socket;

        private string _ip;
        private int _port;
        /// <summary>
        /// 数据接收后处理程序
        /// </summary>
        private Func<List<byte>, byte[]> _dataReceiveHandle;
        /// <summary>
        /// 数据接收时验证是否接收完毕
        /// </summary>
        private Func<List<byte>, bool> _dataCompleteVerify;
        /// <summary>
        /// 每次接收的长度
        /// </summary>
        private int _capacity;
        /// <summary>
        /// 接收超时
        /// </summary>
        private int _receiveTimeout;
        /// <summary>
        /// 发送超时
        /// </summary>
        private int _sendTimeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="dataReceiveHandle"></param>
        /// <param name="dataCompleteVerify"></param>
        /// <param name="capacity"></param>
        /// <param name="receiveTimeout"></param>
        /// <param name="sendTimeout"></param>
        public SocketServer(string ip, int port, Func<List<byte>, byte[]> dataReceiveHandle, Func<List<byte>, bool> dataCompleteVerify = null, int capacity = 1024, int receiveTimeout = 0, int sendTimeout = 0)
        {
            if (dataReceiveHandle == null)
            {
                _logger.ErrorEx("未设置数据处理委托");
            }
            if (dataCompleteVerify == null)
            {
                _logger.Warn("未设置数据接收完整性验证委托,将在接收{0}字节后完成数据接收", _capacity);
            }

            _ip = ip;
            _port = port;
            _dataReceiveHandle = dataReceiveHandle;
            _dataCompleteVerify = dataCompleteVerify;
            _capacity = capacity;
            _receiveTimeout = receiveTimeout;
            _sendTimeout = sendTimeout;
        }

        /// <summary>
        /// 启动Socket Server
        /// </summary>
        public void Run()
        {
            //创建Socket并开始监听
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket对象，如果用UDP协议，则要用SocketTyype.Dgram类型的套接字
            _socket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));//绑定EndPoint对象
            _socket.Listen(0);    //开始监听

            _logger.Info("Socket服务启动成功 {0}:{1}", _ip, _port);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        Socket client = _socket.Accept();
                        Task.Factory.StartNew(() =>
                        {
                            NewClientHandle(client);
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn(ex.ToString());
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        private void NewClientHandle(Socket client)
        {
            client.ReceiveTimeout = _receiveTimeout;
            client.SendTimeout = _sendTimeout;

            lock (_lock)
            {
                _connCount++;
            }
            _logger.Info("新客户端连接:{0},当前客户端数量:{1}", ((IPEndPoint)client.RemoteEndPoint).Address.ToString(), _connCount);

            try
            {
                while (true)
                {
                    List<byte> data = new List<byte>();
                    while (true)
                    {
                        byte[] recvBytes = new byte[_capacity];
                        int receiveLength = client.Receive(recvBytes, _capacity, SocketFlags.None);
                        if (receiveLength == 0)
                        {
                            if (data.Count() == 0)
                            {
                                _logger.WarnEx("receiveLength==0 远程连接断开");
                            }
                            else//有数据则跳到数据处理程序
                            {
                                break;
                            }
                        }
                        data.AddRange(recvBytes.Take(receiveLength));
                        if (_dataCompleteVerify == null || _dataCompleteVerify(data))
                        {
                            break;
                        }
                    }
                    byte[] ret = _dataReceiveHandle(data);
                    client.Send(ret, ret.Length, 0);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex);
                throw ex;
            }
            finally
            {
                client.Close();
                lock (_lock)
                {
                    _connCount--;
                }
            }
        }

    }
}
