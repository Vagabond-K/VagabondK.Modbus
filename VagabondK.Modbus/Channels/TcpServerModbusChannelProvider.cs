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
    public class TcpServerModbusChannelProvider : ModbusChannelProvider
    {
        public TcpServerModbusChannelProvider(int port)
        {
            Port = port;
            tcpListener = new TcpListener(IPAddress.Any, Port);
        }

        public int Port { get; }

        private readonly TcpListener tcpListener;
        internal readonly Dictionary<Guid, WeakReference<TcpClientModbusChannel>> channels = new Dictionary<Guid, WeakReference<TcpClientModbusChannel>>();
        private CancellationTokenSource cancellationTokenSource;

        public override IReadOnlyList<ModbusChannel> Channels { get => channels.Values.Select(w => w.TryGetTarget(out var channel) ? channel : null).Where(c => c != null).ToList(); }

        public override string Description { get => tcpListener?.LocalEndpoint?.ToString(); }

        public override void Dispose()
        {
            lock (this)
            {
                if (!IsDisposed)
                {
                    IsDisposed = true;
                    Stop();
                }
            }
        }

        public override void Start()
        {
            lock (this)
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(TcpServerModbusChannelProvider));

                cancellationTokenSource = new CancellationTokenSource();
                tcpListener.Start();
                Task.Run(() =>
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        var tcpClient = tcpListener.AcceptTcpClient();
                        var channel = new TcpClientModbusChannel(this, tcpClient)
                        {
                            Logger = Logger
                        };
                        Logger?.Log(new ChannelOpenEventLog(channel));
                        channels[channel.Guid] = new WeakReference<TcpClientModbusChannel>(channel);
                        RaiseCreatedEvent(new ModbusChannelCreatedEventArgs(channel));
                        foreach (var disposed in channels.Where(c => !c.Value.TryGetTarget(out var target)).Select(c => c.Key).ToArray())
                            channels.Remove(disposed);
                    }
                }, cancellationTokenSource.Token);
            }
        }

        public override void Stop()
        {
            lock (this)
            {
                cancellationTokenSource?.Cancel();
                tcpListener?.Stop();
            }
        }
    }
}
