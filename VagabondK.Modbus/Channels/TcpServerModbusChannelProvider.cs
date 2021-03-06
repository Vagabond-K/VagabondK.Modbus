﻿using System;
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
    /// <summary>
    /// TCP 서버 기반 Modbus 채널 공급자
    /// </summary>
    public class TcpServerModbusChannelProvider : ModbusChannelProvider
    {
        /// <summary>
        /// 생성자
        /// </summary>
        public TcpServerModbusChannelProvider() : this(502) { }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="port">TCP 연결 수신 포트</param>
        public TcpServerModbusChannelProvider(int port)
        {
            Port = port;
            tcpListener = new TcpListener(IPAddress.Any, Port);
        }

        /// <summary>
        /// TCP 연결 수신 포트
        /// </summary>
        public int Port { get; }

        private readonly TcpListener tcpListener;
        internal readonly Dictionary<Guid, WeakReference<TcpClientModbusChannel>> channels = new Dictionary<Guid, WeakReference<TcpClientModbusChannel>>();
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// 연결 요청 들어온 TCP 클라이언트 채널 목록
        /// </summary>
        public override IReadOnlyList<ModbusChannel> Channels { get => channels.Values.Select(w => w.TryGetTarget(out var channel) ? channel : null).Where(c => c != null).ToList(); }

        /// <summary>
        /// 채널 공급자 설명
        /// </summary>
        public override string Description { get => tcpListener?.LocalEndpoint?.ToString(); }

        /// <summary>
        /// 리소스 해제
        /// </summary>
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

        /// <summary>
        /// TCP 서버 수신 시작
        /// </summary>
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

        /// <summary>
        /// TCP 서버 수신 정지
        /// </summary>
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
