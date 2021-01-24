using System;
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
    public class TcpClientModbusChannel : ModbusChannel
    {
        public TcpClientModbusChannel(string host, int port) : this(host, port, 10000) { }

        public TcpClientModbusChannel(string host, int port, int connectTimeout)
        {
            Host = host;
            Port = port;
            ConnectTimeout = connectTimeout;
        }

        public string Host { get; }
        public int Port { get; }
        public int ConnectTimeout { get; }

        private TcpClient tcpClient = null;
        private readonly object connectLock = new object();
        private readonly object writeLock = new object();
        private readonly object readLock = new object();
        private readonly Queue<byte> readBuffer = new Queue<byte>();
        private readonly EventWaitHandle readEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool isRunningReceive = false;

        public override bool IsDisposed { get; protected set; }

        public override string Description { get; protected set; }

        ~TcpClientModbusChannel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                Close();
            }
        }

        private void Close()
        {
            lock (connectLock)
            {
                Logger?.Log(new ChannelCloseEventLog(this));
                tcpClient?.Client?.Dispose();
                tcpClient = null;
            }
        }

        private void CheckConnection()
        {
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

        public override byte Read(int timeout)
        {
            lock (readLock)
            {
                return GetByte(timeout) ?? throw new TimeoutException();
            }
        }

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
