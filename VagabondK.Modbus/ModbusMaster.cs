﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Data;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus
{
    /// <summary>
    /// Modbus 마스터
    /// </summary>
    public class ModbusMaster : IDisposable
    {
        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Dispose()
        {
            if (serializer != null)
                serializer.Unrecognized -= OnReceivedUnrecognizedMessage;
            Channel?.Dispose();
        }

        private ModbusSerializer serializer;
        private IModbusChannel channel;

        /// <summary>
        /// Modbus Serializer
        /// </summary>
        public ModbusSerializer Serializer
        {
            get
            {
                if (serializer == null)
                    serializer = new ModbusRtuSerializer();
                return serializer;
            }
            set
            {
                if (serializer != value)
                {
                    if (serializer != null)
                        serializer.Unrecognized -= OnReceivedUnrecognizedMessage;

                    serializer = value;

                    if (serializer != null)
                        serializer.Unrecognized += OnReceivedUnrecognizedMessage;
                }
            }
        }

        /// <summary>
        /// Modbus 채널
        /// </summary>
        public IModbusChannel Channel
        {
            get => channel;
            set
            {
                if (channel != value)
                {
                    channel = value;
                }
            }
        }

        /// <summary>
        /// 응답 제한시간(밀리초)
        /// </summary>
        public int Timeout { get; set; } = 1000;

        /// <summary>
        /// Modbus Exception에 대한 예외 발생 여부
        /// </summary>
        public bool ThrowsModbusExceptions { get; set; } = true;

        /// <summary>
        /// Modbus Logger
        /// </summary>
        public IModbusLogger Logger { get; set; }

        private void OnReceivedUnrecognizedMessage(object sender, UnrecognizedEventArgs e)
        {
            Logger?.Log(new UnrecognizedErrorLog(e.Channel, e.UnrecognizedMessage.ToArray()));
        }

        /// <summary>
        /// ModbusChannelProvider로 생성된 Modbus 채널 중 하나를 선택
        /// </summary>
        /// <param name="channels">생성된 Modbus 채널 목록</param>
        /// <returns></returns>
        protected virtual ModbusChannel OnSelectChannel(IReadOnlyList<ModbusChannel> channels) => channels?.LastOrDefault();

        /// <summary>
        /// Modbus 요청하기
        /// </summary>
        /// <param name="request">Modbus 요청</param>
        /// <returns>Modbus 응답</returns>
        public ModbusResponse Request(ModbusRequest request) => Request(request, Timeout);
        /// <summary>
        /// Modbus 요청하기
        /// </summary>
        /// <param name="request">Modbus 요청</param>
        /// <param name="timeout">응답 제한시간(밀리초)</param>
        /// <returns>Modbus 응답</returns>
        public ModbusResponse Request(ModbusRequest request, int timeout)
        {
            ModbusChannel channel = null;

            lock (this)
            {
                channel = Channel as ModbusChannel;
                if (channel == null)
                {
                    channel = OnSelectChannel((Channel as ModbusChannelProvider)?.Channels);
                }
            }

            if (channel == null)
                throw new ModbusCommException(ModbusCommErrorCode.NullChannelError, new byte[0], request);

            if (Serializer == null)
                throw new ModbusCommException(ModbusCommErrorCode.NotDefinedModbusSerializer, new byte[0], request);


            var requestMessage = Serializer.Serialize(request).ToArray();
            var buffer = new ResponseBuffer(channel);

            if (!(Serializer is ModbusTcpSerializer))
            {
                channel.ReadAllRemain().ToArray();
            }

            channel.Write(requestMessage);
            Logger?.Log(new ModbusMessageLog(channel, request, requestMessage));

            ModbusResponse result;
            try
            {
                result = Serializer.Deserialize(buffer, request, timeout);
            }
            catch (ModbusCommException ex)
            {
                Logger?.Log(new CommErrorLog(channel, ex));
                throw ex;
            }

            if (result is ModbusExceptionResponse exceptionResponse)
            {
                Logger?.Log(new ModbusExceptionLog(channel, exceptionResponse.ExceptionCode, buffer.ToArray()));
                if (ThrowsModbusExceptions)
                    throw new ModbusException(exceptionResponse.ExceptionCode);
            }
            else
                Logger?.Log(new ModbusMessageLog(channel, result, result is ModbusCommErrorResponse ? null : buffer.ToArray()));


            return result;
        }
    }
}
