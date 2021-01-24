using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VagabondK.Modbus.Logging;

namespace VagabondK.Modbus.Channels
{
    public class TcpServerModbusChannel : IModbusChannelProvider
    {
        public TcpServerModbusChannel(int port)
        {
            Port = port;
            tcpListener = new TcpListener(IPAddress.Any, Port);
        }

        public int Port { get; }

        private readonly TcpListener tcpListener;
        private readonly Dictionary<Guid, WeakReference<TcpClientModbusChannel>> channels = new Dictionary<Guid, WeakReference<TcpClientModbusChannel>>();
        private CancellationTokenSource cancellationTokenSource;

        public bool IsDisposed { get; protected set; }
        public IReadOnlyList<ModbusChannel> Channels { get => channels.Values.Select(w => w.TryGetTarget(out var channel) ? channel : null).Where(c => c != null).ToList(); }

        public event EventHandler<ModbusChannelCreatedEventArgs> Created;
        public IModbusLogger Logger { get; set; }

        public string Description { get => tcpListener?.LocalEndpoint?.ToString(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            lock (this)
            {
                cancellationTokenSource = new CancellationTokenSource();
                tcpListener.Start();
                Task.Run(() =>
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        var tcpClient = tcpListener.AcceptTcpClient();
                        var remote = tcpClient.Client.RemoteEndPoint;
                        var local = tcpClient.Client.LocalEndPoint;
                        var channel = new TcpClientModbusChannel(this, tcpClient)
                        {
                            Logger = Logger
                        };
                        Logger?.Log(new ChannelOpenEventLog(channel));
                        channels[channel.Guid] = new WeakReference<TcpClientModbusChannel>(channel);
                        Created?.Invoke(this, new ModbusChannelCreatedEventArgs(channel));
                    }
                }, cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            lock (this)
            {
                cancellationTokenSource?.Cancel();
                tcpListener?.Stop();
            }
        }


        class TcpClientModbusChannel : ModbusChannel
        {
            public TcpClientModbusChannel(TcpServerModbusChannel provider, TcpClient tcpClient)
            {
                Guid = Guid.NewGuid();

                this.provider = provider;
                this.tcpClient = tcpClient;
                Description = tcpClient.Client.RemoteEndPoint.ToString();
            }

            public Guid Guid { get; }

            private readonly TcpServerModbusChannel provider;
            private TcpClient tcpClient = null;
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
                    provider.channels.Remove(Guid);
                    IsDisposed = true;
                    Close();
                }
            }

            private void Close()
            {
                Logger?.Log(new ChannelCloseEventLog(this));
                tcpClient?.Client?.Dispose();
                tcpClient = null;
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
                                    Dispose();
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
                lock (writeLock)
                {
                    try
                    {
                        if (tcpClient?.Client?.Connected == true)
                            tcpClient?.Client?.Send(bytes);
                    }
                    catch
                    {
                        Dispose();
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
}
