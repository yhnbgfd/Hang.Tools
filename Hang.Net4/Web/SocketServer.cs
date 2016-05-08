﻿using Hang.Net4.Utilities;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Hang.Net4.Web
{
    public class SocketServer : IDisposable
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly object _lock = new object();

        private readonly string _ip;
        private readonly int _port;
        /// <summary>
        /// 数据接收后处理程序
        /// </summary>
        private readonly Func<List<byte>, byte[]> _dataReceiveHandle;
        /// <summary>
        /// 数据接收时验证是否接收完毕
        /// </summary>
        private readonly Func<List<byte>, bool> _dataCompleteVerify;
        /// <summary>
        /// 每次接收的长度
        /// </summary>
        private readonly int _capacity;
        /// <summary>
        /// 接收超时
        /// </summary>
        private readonly int _receiveTimeout;
        /// <summary>
        /// 发送超时
        /// </summary>
        private readonly int _sendTimeout;

        private bool _disposed = false;
        /// <summary>
        /// 当前客户端连接数量
        /// </summary>
        private long _connCount = 0;
        /// <summary>
        /// Socket
        /// </summary>
        private Socket _socket;

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
                _logger.Info("未设置数据接收完整性验证委托,将在接收{0}字节后完成数据接收", capacity);
            }

            _ip = ip;
            _port = port;
            _dataReceiveHandle = dataReceiveHandle;
            _dataCompleteVerify = dataCompleteVerify;
            _capacity = capacity;
            _receiveTimeout = receiveTimeout;
            _sendTimeout = sendTimeout;
        }

        ~SocketServer()
        {
            Dispose(false);
        }

        /// <summary>
        /// 启动Socket Server
        /// </summary>
        public void Start()
        {
            if (_socket != null)
            {
                _logger.WarnEx("SocketServer已经启动");
            }

            //创建Socket并开始监听
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建一个Socket对象，如果用UDP协议，则要用SocketTyype.Dgram类型的套接字
                _socket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));//绑定EndPoint对象
                _socket.Listen(0);//开始监听
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(string.Format("{0}:{1}, {2}", _ip, _port, ex.ToString()));
            }

            Task.Factory.StartNew(() =>
            {
                _logger.Trace("Task Start,ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
                while (true)
                {
                    Socket client = null;
                    try
                    {
                        client = _socket.Accept();
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnEx(string.Format("ThreadId:{0}, {1}", Thread.CurrentThread.ManagedThreadId, ex.ToString()));
                    }

                    Task.Factory.StartNew(() =>
                    {
                        var ep = (IPEndPoint)client.RemoteEndPoint;

                        lock (_lock) _connCount++;
                        _logger.Info("客户端连接({2}) : {0}:{1}", ep.Address, ep.Port, _connCount);

                        SocketAccept(client);

                        lock (_lock) _connCount--;
                        _logger.Info("客户端断开({2}) : {0}:{1}", ep.Address, ep.Port, _connCount);
                    });
                }
            });

            _logger.Info("SocketServer启动成功 {0}:{1}", _ip, _port);
        }

        /// <summary>
        /// 新的Socket Client连接进来
        /// </summary>
        /// <param name="client"></param>
        private void SocketAccept(Socket client)
        {
            client.ReceiveTimeout = _receiveTimeout;
            client.SendTimeout = _sendTimeout;

            try
            {
                while (true)//循环接收
                {
                    List<byte> data = new List<byte>();
                    while (true)//单次接收
                    {
                        byte[] recvBytes = new byte[_capacity];
                        int receiveLength = client.Receive(recvBytes, _capacity, SocketFlags.None);
                        if (receiveLength == 0)
                        {
                            if (data.Count() == 0)
                            {
                                return;
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
                    client.Send(ret, ret.Length, SocketFlags.None);
                }
            }
            catch
            {

            }
            finally
            {
                client.Close();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            //清理托管资源
            if (disposing)
            {
                if (_socket != null)
                {
                    _socket.Dispose();
                }
            }
            //清理非托管资源
            {

            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}