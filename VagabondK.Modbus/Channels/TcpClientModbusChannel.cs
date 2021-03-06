﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Channels
{
    /// <summary>
    /// TCP 클라이언트 기반 Modbus 채널
    /// </summary>
    public class TcpClientModbusChannel : ModbusChannel
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="host">호스트</param>
        public TcpClientModbusChannel(string host) : this(host, 502, 10000) { }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="host">호스트</param>
        /// <param name="port">포트</param>
        public TcpClientModbusChannel(string host, int port) : this(host, port, 10000) { }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="host">호스트</param>
        /// <param name="port">포트</param>
        /// <param name="connectTimeout">연결 제한시간(밀리초)</param>
        public TcpClientModbusChannel(string host, int port, int connectTimeout)
        {
            Host = host;
            Port = port;
            ConnectTimeout = connectTimeout;
        }

        internal TcpClientModbusChannel(TcpServerModbusChannelProvider provider, TcpClient tcpClient)
        {
            Guid = Guid.NewGuid();

            this.provider = provider;
            this.tcpClient = tcpClient;
            Description = tcpClient.Client.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// 호스트
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// 포트
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// 연결 제한시간(밀리초)
        /// </summary>
        public int ConnectTimeout { get; }

        internal Guid Guid { get; }
        private readonly TcpServerModbusChannelProvider provider;

        private TcpClient tcpClient = null;
        private readonly object connectLock = new object();
        private readonly object writeLock = new object();
        private readonly object readLock = new object();
        private readonly Queue<byte> readBuffer = new Queue<byte>();
        private readonly EventWaitHandle readEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool isRunningReceive = false;

        /// <summary>
        /// 리소스 헤제 여부
        /// </summary>
        public override bool IsDisposed { get; protected set; }

        /// <summary>
        /// 채널 설명
        /// </summary>
        public override string Description { get; protected set; }

        /// <summary>
        /// 소멸자
        /// </summary>
        ~TcpClientModbusChannel()
        {
            Dispose();
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed)
            {
                provider?.channels?.Remove(Guid);
                IsDisposed = true;

                Close();
            }
        }

        private void Close()
        {
            lock (connectLock)
            {
                if (tcpClient != null)
                {
                    Logger?.Log(new ChannelCloseEventLog(this));
                    tcpClient.Close();
                    tcpClient = null;
                }
            }
        }

        private void CheckConnection()
        {
            if (provider != null) return;

            lock (connectLock)
            {
                if (!IsDisposed && tcpClient == null)
                {
                    tcpClient = new TcpClient();
                    try
                    {
                        Task task = tcpClient.ConnectAsync(Host ?? string.Empty, Port);
                        if (!task.Wait(ConnectTimeout))
                            throw new SocketException(10060);

                        Description = tcpClient.Client.RemoteEndPoint.ToString();
                        Logger?.Log(new ChannelOpenEventLog(this));
                    }
                    catch (Exception ex)
                    {
                        tcpClient?.Client?.Dispose();
                        tcpClient = null;
                        Logger?.Log(new CommErrorLog(this, ex));
                        throw ex;
                    }
                }
            }
        }

        private byte? GetByte(int timeout)
        {
            lock (readBuffer)
            {
                if (readBuffer.Count == 0)
                {
                    readEventWaitHandle.Reset();

                    Task.Run(() =>
                    {
                        if (!isRunningReceive)
                        {
                            isRunningReceive = true;
                            try
                            {
                                CheckConnection();
                                if (tcpClient != null)
                                {
                                    byte[] buffer = new byte[8192];
                                    while (true)
                                    {
                                        int received = tcpClient.Client.Receive(buffer);
                                        lock (readBuffer)
                                        {
                                            for (int i = 0; i < received; i++)
                                                readBuffer.Enqueue(buffer[i]);
                                            readEventWaitHandle.Set();
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Close();
                            }
                            readEventWaitHandle.Set();
                            isRunningReceive = false;
                        }
                    });
                }
                else return readBuffer.Dequeue();
            }

            if (timeout == 0 ? readEventWaitHandle.WaitOne() : readEventWaitHandle.WaitOne(timeout))
                return readBuffer.Count > 0 ? readBuffer.Dequeue() : (byte?)null;
            else
                return null;
        }

        /// <summary>
        /// 바이트 배열 쓰기
        /// </summary>
        /// <param name="bytes">바이트 배열</param>
        public override void Write(byte[] bytes)
        {
            CheckConnection();
            lock (writeLock)
            {
                try
                {
                    if (tcpClient?.Client?.Connected == true)
                        tcpClient?.Client?.Send(bytes);
                }
                catch
                {
                    Close();
                    throw new TimeoutException();
                }
            }
        }

        /// <summary>
        /// 1 바이트 읽기
        /// </summary>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>읽은 바이트</returns>
        public override byte Read(int timeout)
        {
            lock (readLock)
            {
                return GetByte(timeout) ?? throw new TimeoutException();
            }
        }

        /// <summary>
        /// 여러 개의 바이트 읽기
        /// </summary>
        /// <param name="count">읽을 개수</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>읽은 바이트 열거</returns>
        public override IEnumerable<byte> Read(uint count, int timeout)
        {
            lock (readLock)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return GetByte(timeout) ?? throw new TimeoutException();
                }
            }
        }

        /// <summary>
        /// 채널에 남아있는 모든 바이트 읽기
        /// </summary>
        /// <returns>읽은 바이트 열거</returns>
        public override IEnumerable<byte> ReadAllRemain()
        {
            lock (readLock)
            {
                while (readBuffer.Count > 0)
                    yield return readBuffer.Dequeue();

                if (tcpClient == null)
                    yield break;

                byte[] receivedBuffer = new byte[4096];
                int available = 0;

                try
                {
                    available = tcpClient.Client.Available;
                }
                catch { }

                while (available > 0)
                {
                    int received = 0;
                    try
                    {
                        received = tcpClient.Client.Receive(receivedBuffer);
                    }
                    catch { }
                    for (int i = 0; i < received; i++)
                        yield return receivedBuffer[i];

                try
                {
                    available = tcpClient.Client.Available;
                }
                catch { }
                }
            }
        }
    }
}
