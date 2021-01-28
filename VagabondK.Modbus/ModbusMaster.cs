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

        public int Timeout { get; set; } = 1000;

        public bool ThrowsModbusExceptions { get; set; } = true;

        public IModbusLogger Logger { get; set; }

        private void OnReceivedUnrecognizedMessage(object sender, UnrecognizedEventArgs e)
        {
            Logger?.Log(new UnrecognizedErrorLog(e.Channel, e.UnrecognizedMessage.ToArray()));
        }

        protected virtual ModbusChannel OnSelectChannel(IReadOnlyList<ModbusChannel> channels) => channels?.LastOrDefault();

        public ModbusResponse Request(ModbusRequest request) => Request(request, Timeout);
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

        public bool[] ReadCoils(byte slaveAddress, ushort address, ushort length) => ReadCoils(slaveAddress, address, length, Timeout);
        public bool[] ReadDiscreteInputs(byte slaveAddress, ushort address, ushort length) => ReadDiscreteInputs(slaveAddress, address, length, Timeout);
        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort address, ushort length) => ReadHoldingRegisters(slaveAddress, address, length, Timeout);
        public ushort[] ReadInputRegisters(byte slaveAddress, ushort address, ushort length) => ReadInputRegisters(slaveAddress, address, length, Timeout);
        public void WriteCoils(byte slaveAddress, ushort address, IEnumerable<bool> values) => WriteCoils(slaveAddress, address, values, Timeout);
        public void WriteHoldingRegisters(byte slaveAddress, ushort address, IEnumerable<ushort> values) => WriteHoldingRegisters(slaveAddress, address, values, Timeout);

        public bool? ReadCoil(byte slaveAddress, ushort address) => ReadCoil(slaveAddress, address, Timeout);
        public bool? ReadDiscreteInput(byte slaveAddress, ushort address) => ReadDiscreteInput(slaveAddress, address, Timeout);
        public ushort? ReadHoldingRegister(byte slaveAddress, ushort address) => ReadHoldingRegister(slaveAddress, address, Timeout);
        public ushort? ReadInputRegister(byte slaveAddress, ushort address) => ReadInputRegister(slaveAddress, address, Timeout);
        public void WriteCoil(byte slaveAddress, ushort address, bool value) => WriteCoil(slaveAddress, address, value, Timeout);
        public void WriteHoldingRegister(byte slaveAddress, ushort address, ushort value) => WriteHoldingRegister(slaveAddress, address, value, Timeout);


        public bool[] ReadCoils(byte slaveAddress, ushort address, ushort length, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.Coil, address, length)) as ModbusReadBooleanResponse)?.Values?.ToArray();

        public bool[] ReadDiscreteInputs(byte slaveAddress, ushort address, ushort length, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.DiscreteInput, address, length)) as ModbusReadBooleanResponse)?.Values?.ToArray();

        public ushort[] ReadHoldingRegisters(byte slaveAddress, ushort address, ushort length, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.HoldingRegister, address, length)) as ModbusReadRegisterResponse)?.Values?.ToArray();

        public ushort[] ReadInputRegisters(byte slaveAddress, ushort address, ushort length, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.InputRegister, address, length)) as ModbusReadRegisterResponse)?.Values?.ToArray();

        public void WriteCoils(byte slaveAddress, ushort address, IEnumerable<bool> values, int timeout)
            => Request(new ModbusWriteCoilRequest(slaveAddress, address, values));

        public void WriteHoldingRegisters(byte slaveAddress, ushort address, IEnumerable<ushort> values, int timeout)
            => Request(new ModbusWriteHoldingRegisterRequest(slaveAddress, address, values));


        public bool? ReadCoil(byte slaveAddress, ushort address, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.Coil, address, 1)) as ModbusReadBooleanResponse)?.Values?.FirstOrDefault();

        public bool? ReadDiscreteInput(byte slaveAddress, ushort address, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.DiscreteInput, address, 1)) as ModbusReadBooleanResponse)?.Values?.FirstOrDefault();

        public ushort? ReadHoldingRegister(byte slaveAddress, ushort address, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.HoldingRegister, address, 1)) as ModbusReadRegisterResponse)?.Values?.FirstOrDefault();

        public ushort? ReadInputRegister(byte slaveAddress, ushort address, int timeout)
            => (Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.InputRegister, address, 1)) as ModbusReadRegisterResponse)?.Values?.FirstOrDefault();

        public void WriteCoil(byte slaveAddress, ushort address, bool value, int timeout)
            => Request(new ModbusWriteCoilRequest(slaveAddress, address, value));

        public void WriteHoldingRegister(byte slaveAddress, ushort address, ushort value, int timeout)
            => Request(new ModbusWriteHoldingRegisterRequest(slaveAddress, address, value));
    }
}
