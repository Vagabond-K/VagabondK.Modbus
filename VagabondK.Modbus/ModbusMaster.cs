using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus
{
    public class ModbusMaster : IDisposable
    {
        public void Dispose()
        {
            if (serializer != null)
                serializer.Unrecognized -= OnReceivedUnrecognizedMessage;
            Channel?.Dispose();
        }

        private ModbusSerializer serializer;
        private IModbusChannel channel;

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

        public bool ThrowsModbusExceptions { get; set; } = true;

        public IModbusLogger Logger { get; set; }

        private void OnModbusChannelCreated(object sender, ModbusChannelCreatedEventArgs e)
        {
            lock (this)
            {
                Channel?.Dispose();
                Channel = e.Channel;
                (Channel as ModbusChannel)?.ReadAllRemain();
            }
        }

        private void OnReceivedUnrecognizedMessage(object sender, UnrecognizedEventArgs e)
        {
            Logger?.Log(new UnrecognizedErrorLog(e.Channel, e.UnrecognizedMessage.ToArray()));
        }

        public ModbusResponse Request(ModbusRequest request, int timeout)
        {
            ModbusChannel channel = null;

            lock (this)
            {
                channel = Channel as ModbusChannel;
                if (channel == null)
                {
                    channel = (Channel as IModbusChannelProvider)?.Channels?.LastOrDefault();
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
