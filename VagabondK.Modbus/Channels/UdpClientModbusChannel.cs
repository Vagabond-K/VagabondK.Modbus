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
    public class UdpClientModbusChannel : ModbusChannel
    {
        public UdpClientModbusChannel(string host, int remotePort)
        {
            Host = host;
            RemotePort = remotePort;
        }

        public UdpClientModbusChannel(string host, int remotePort, int localPort)
        {
            Host = host;
            RemotePort = remotePort;
            LocalPort = localPort;
        }

        public string Host { get; }
        public int RemotePort { get; }
        public int? LocalPort { get; }

        private UdpClient udpClient = null;
        private readonly object connectLock = new object();
        private readonly object writeLock = new object();
        private readonly object readLock = new object();
        private readonly Queue<byte> readBuffer = new Queue<byte>();
        private readonly EventWaitHandle readEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool isRunningReceive = false;

        public override bool IsDisposed { get; protected set; }

        public override string Description { get; protected set; }

        ~UdpClientModbusChannel()
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
                if (udpClient != null)
                {
                    Logger?.Log(new ChannelCloseEventLog(this));
                    udpClient?.Close();
                    udpClient = null;
                }
            }
        }

        private void CheckConnection()
        {
            lock (connectLock)
            {
                if (!IsDisposed && udpClient == null)
                {
                    if (LocalPort != null)
                        udpClient = new UdpClient(LocalPort.Value);
                    else
                        udpClient = new UdpClient();

                    udpClient.Connect(Host ?? string.Empty, RemotePort);
                    Description = udpClient.Client.RemoteEndPoint.ToString();
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
                                if (udpClient != null)
                                {
                                    byte[] buffer = new byte[8192];
                                    while (true)
                                    {
                                        int received = udpClient.Client.Receive(buffer);
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
                    if (udpClient?.Client?.Connected == true)
                        udpClient?.Client?.Send(bytes);
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

                if (udpClient == null)
                    yield break;

                byte[] receivedBuffer = new byte[4096];
                int available = 0;

                try
                {
                    available = udpClient.Client.Available;
                }
                catch { }

                while (available > 0)
                {
                    int received = 0;
                    try
                    {
                        received = udpClient.Client.Receive(receivedBuffer);
                    }
                    catch { }
                    for (int i = 0; i < received; i++)
                        yield return receivedBuffer[i];

                try
                {
                    available = udpClient.Client.Available;
                }
                catch { }
                }
            }
        }
    }
}
