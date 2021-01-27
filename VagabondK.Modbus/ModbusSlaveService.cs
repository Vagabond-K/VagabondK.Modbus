using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus
{
    public class ModbusSlaveService : IDisposable, IEnumerable<KeyValuePair<ushort, ModbusSlave>>
    {
        public ModbusSlaveService()
        {
        }

        public ModbusSlaveService(IModbusChannel channel) : this(new IModbusChannel[] { channel })
        {
        }

        public ModbusSlaveService(IEnumerable<IModbusChannel> channels)
        {
            foreach (var channel in channels)
                AddChannel(channel);
        }

        private ModbusSerializer serializer;
        private readonly Dictionary<ushort, ModbusSlave> modbusSlaves = new Dictionary<ushort, ModbusSlave>();

        public ModbusSlave this[ushort slaveAddress]
        {
            get => modbusSlaves[slaveAddress];
            set
            {
                lock (this)
                {
                    modbusSlaves[slaveAddress] = value;
                }
            }
        }

        public ICollection<ushort> SlaveAddresses { get => modbusSlaves.Keys; }
        public ICollection<ModbusSlave> ModbusSlaves { get => modbusSlaves.Values; }
        public bool ContainsSlaveAddress(ushort slaveAddress) => modbusSlaves.ContainsKey(slaveAddress);
        public bool TryGetValue(ushort slaveAddress, out ModbusSlave modbusSlave) => modbusSlaves.TryGetValue(slaveAddress, out modbusSlave);

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
                if (value == null)
                    throw new ArgumentNullException();

                if (serializer != value)
                {
                    if (serializer != null)
                        serializer.Unrecognized -= OnReceivedUnrecognizedMessage;

                    serializer = value;
                    serializer.Unrecognized += OnReceivedUnrecognizedMessage;
                }
            }
        }

        public IModbusLogger Logger { get; set; }

        private void OnReceivedUnrecognizedMessage(object sender, UnrecognizedEventArgs e)
        {
            Logger?.Log(new UnrecognizedErrorLog(e.Channel, e.UnrecognizedMessage.ToArray()));
        }

        private readonly Dictionary<ModbusChannel, ChannelTask> channelTasks = new Dictionary<ModbusChannel, ChannelTask>();
        private readonly List<IModbusChannel> channels = new List<IModbusChannel>();

        public IReadOnlyList<IModbusChannel> Channels { get => channelTasks.Keys.ToList(); }


        public void AddChannel(IModbusChannel channel)
        {
            lock (channels)
            {
                if (channel is ModbusChannel modbusChannel)
                {
                    var channelTask = new ChannelTask(this, modbusChannel);
                    channelTasks[modbusChannel] = channelTask;
                    channelTask.Start();
                }
                else if (channel is ModbusChannelProvider channelProvider)
                {
                    channelProvider.Created += OnModbusChannelCreated;
                }
                channels.Add(channel);
            }
        }

        public void RemoveChannel(IModbusChannel channel)
        {
            lock (channels)
            {
                if (channel is ModbusChannel modbusChannel)
                {
                    if (channelTasks.TryGetValue(modbusChannel, out var channelTask))
                    {
                        channelTask.Stop();
                        channelTasks.Remove(modbusChannel);
                    }
                }
                else if (channel is ModbusChannelProvider channelProvider
                    && channels.Contains(channelProvider))
                {
                    channelProvider.Created -= OnModbusChannelCreated;
                    channels.Remove(channelProvider);
                }
                channels.Remove(channel);
            }
        }

        private void OnModbusChannelCreated(object sender, ModbusChannelCreatedEventArgs e)
        {
            lock (channels)
            {
                var channelTask = new ChannelTask(this, e.Channel);
                channelTasks[e.Channel] = channelTask;
                channelTask.Start();
            }
        }

        public void Dispose()
        {
            lock (channels)
            {
                foreach (var channel in channels)
                {
                    if (channel is ModbusChannelProvider channelProvider
                        && channels.Contains(channelProvider))
                    {
                        channelProvider.Created -= OnModbusChannelCreated;
                    }
                }
                foreach (var task in channelTasks.Values)
                {
                    task.Stop();
                }
                foreach (var channel in channels)
                {
                    channel.Dispose();
                }
            }
        }

        private void OnReceivedModbusRequest(ModbusChannel channel, ModbusRequest request)
        {
            ModbusResponse response = null;

            try
            {
                if (request.Address + request.Length > 0xffff)
                    throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);

                switch (request.Function)
                {
                    case ModbusFunction.ReadCoils:
                        if (request.Length > 2008)
                            throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);
                        response = new ModbusReadBooleanResponse(OnRequestReadCoils((ModbusReadRequest)request, channel).Take(request.Length).ToArray(), (ModbusReadRequest)request);
                        break;
                    case ModbusFunction.ReadDiscreteInputs:
                        if (request.Length > 2008)
                            throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);
                        response = new ModbusReadBooleanResponse(OnRequestReadDiscreteInputs((ModbusReadRequest)request, channel).Take(request.Length).ToArray(), (ModbusReadRequest)request);
                        break;
                    case ModbusFunction.ReadHoldingRegisters:
                        if (request.Length > 125)
                            throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);
                        response = new ModbusReadRegisterResponse(OnRequestReadHoldingRegisters((ModbusReadRequest)request, channel).Take(request.Length * 2).ToArray(), (ModbusReadRequest)request);
                        break;
                    case ModbusFunction.ReadInputRegisters:
                        if (request.Length > 125)
                            throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);
                        response = new ModbusReadRegisterResponse(OnRequestReadInputRegisters((ModbusReadRequest)request, channel).Take(request.Length * 2).ToArray(), (ModbusReadRequest)request);
                        break;
                    case ModbusFunction.WriteSingleCoil:
                    case ModbusFunction.WriteMultipleCoils:
                        OnRequestWriteCoil((ModbusWriteCoilRequest)request, channel);
                        response = new ModbusWriteResponse((ModbusWriteCoilRequest)request);
                        break;
                    case ModbusFunction.WriteSingleHoldingRegister:
                    case ModbusFunction.WriteMultipleHoldingRegisters:
                        OnRequestWriteHoldingRegister((ModbusWriteHoldingRegisterRequest)request, channel);
                        response = new ModbusWriteResponse((ModbusWriteCoilRequest)request);
                        break;
                }
            }
            catch (ModbusException modbusException)
            {
                response = new ModbusExceptionResponse(modbusException.Code, request);
            }
            catch
            {
                response = new ModbusExceptionResponse(ModbusExceptionCode.SlaveDeviceFailure, request);
            }

            if (response != null)
            {
                var responseMessage = Serializer.Serialize(response).ToArray();
                channel.Write(responseMessage);
                Logger?.Log(new ModbusMessageLog(channel, response, responseMessage));
            }
        }

        public event EventHandler<ValidatingSlaveAddressEventArgs> ValidatingSlaveAddress;
        public event EventHandler<RequestReadBooleanEventArgs> RequestReadCoils;
        public event EventHandler<RequestReadBooleanEventArgs> RequestReadDiscreteInputs;
        public event EventHandler<RequestReadRegisterEventArgs> RequestReadHoldingRegisters;
        public event EventHandler<RequestReadRegisterEventArgs> RequestReadInputRegisters;
        public event EventHandler<RequestWriteCoilEventArgs> RequestWriteCoil;
        public event EventHandler<RequestWriteHoldingRegisterEventArgs> RequestWriteHoldingRegister;

        protected virtual void OnValidatingSlaveAddress(ValidatingSlaveAddressEventArgs e)
            => e.IsValid = modbusSlaves.Count == 0 && e.SlaveAddress == 1 || modbusSlaves.ContainsKey(e.SlaveAddress);
        protected virtual void OnRequestReadCoils(RequestReadBooleanEventArgs e) 
            => e.Values = modbusSlaves.TryGetValue(e.SlaveAddress, out var modbusSlave) ? modbusSlave.Coils.GetData(e.Address, e.Length) : throw new ModbusException(ModbusExceptionCode.IllegalFunction);
        protected virtual void OnRequestReadDiscreteInputs(RequestReadBooleanEventArgs e)
            => e.Values = modbusSlaves.TryGetValue(e.SlaveAddress, out var modbusSlave) ? modbusSlave.DiscreteInputs.GetData(e.Address, e.Length) : throw new ModbusException(ModbusExceptionCode.IllegalFunction);
        protected virtual void OnRequestReadHoldingRegisters(RequestReadRegisterEventArgs e)
            => e.Bytes = modbusSlaves.TryGetValue(e.SlaveAddress, out var modbusSlave) ? modbusSlave.HoldingRegisters.GetRawData(e.Address, e.Length * 2) : throw new ModbusException(ModbusExceptionCode.IllegalFunction);
        protected virtual void OnRequestReadInputRegisters(RequestReadRegisterEventArgs e)
            => e.Bytes = modbusSlaves.TryGetValue(e.SlaveAddress, out var modbusSlave) ? modbusSlave.InputRegisters.GetRawData(e.Address, e.Length * 2) : throw new ModbusException(ModbusExceptionCode.IllegalFunction);
        protected virtual void OnRequestWriteCoil(RequestWriteCoilEventArgs e)
        {
            if (modbusSlaves.TryGetValue(e.SlaveAddress, out var modbusSlave))
                modbusSlave.Coils.SetData(e.Address, e.Values.ToArray());
            else
                throw new ModbusException(ModbusExceptionCode.IllegalFunction);
        }
        protected virtual void OnRequestWriteHoldingRegister(RequestWriteHoldingRegisterEventArgs e)
        {
            if (modbusSlaves.TryGetValue(e.SlaveAddress, out var modbusSlave))
                modbusSlave.HoldingRegisters.SetRawData(e.Address, e.Bytes.ToArray());
            else
                throw new ModbusException(ModbusExceptionCode.IllegalFunction);
        }


        internal bool IsValidSlaveAddress(byte slaveAddress, ModbusChannel channel)
        {
            var eventArgs = new ValidatingSlaveAddressEventArgs(slaveAddress, channel);
            OnValidatingSlaveAddress(eventArgs);
            ValidatingSlaveAddress?.Invoke(this, eventArgs);

            return eventArgs.IsValid;
        }
        private IEnumerable<bool> OnRequestReadCoils(ModbusReadRequest request, ModbusChannel channel)
            => InvokeOverrideMethodAndEvent(
                new RequestReadBooleanEventArgs(request, channel),
                eventArgs => OnRequestReadCoils(eventArgs),
                RequestReadCoils).Values;
        private IEnumerable<bool> OnRequestReadDiscreteInputs(ModbusReadRequest request, ModbusChannel channel)
            => InvokeOverrideMethodAndEvent(
                new RequestReadBooleanEventArgs(request, channel),
                eventArgs => OnRequestReadDiscreteInputs(eventArgs),
                RequestReadDiscreteInputs).Values;
        private IEnumerable<byte> OnRequestReadHoldingRegisters(ModbusReadRequest request, ModbusChannel channel)
            => InvokeOverrideMethodAndEvent(
                new RequestReadRegisterEventArgs(request, channel),
                eventArgs => OnRequestReadHoldingRegisters(eventArgs),
                RequestReadHoldingRegisters).Bytes;
        private IEnumerable<byte> OnRequestReadInputRegisters(ModbusReadRequest request, ModbusChannel channel)
            => InvokeOverrideMethodAndEvent(
                new RequestReadRegisterEventArgs(request, channel),
                eventArgs => OnRequestReadInputRegisters(eventArgs),
                RequestReadInputRegisters).Bytes;
        private void OnRequestWriteCoil(ModbusWriteCoilRequest request, ModbusChannel channel)
            => InvokeOverrideMethodAndEvent(
                new RequestWriteCoilEventArgs(request, channel),
                eventArgs => OnRequestWriteCoil(eventArgs),
                RequestWriteCoil);
        private void OnRequestWriteHoldingRegister(ModbusWriteHoldingRegisterRequest request, ModbusChannel channel)
            => InvokeOverrideMethodAndEvent(
                new RequestWriteHoldingRegisterEventArgs(request, channel),
                eventArgs => OnRequestWriteHoldingRegister(eventArgs),
                RequestWriteHoldingRegister);
        
        private TEventArgs InvokeOverrideMethodAndEvent<TEventArgs>(TEventArgs eventArgs, Action<TEventArgs> action, EventHandler<TEventArgs> eventHandler)
        {
            try
            {
                action(eventArgs);
            }
            catch (Exception ex)
            {
                if (eventHandler == null)
                    throw ex;
            }

            eventHandler?.Invoke(this, eventArgs);

            return eventArgs;
        }

        public IEnumerator<KeyValuePair<ushort, ModbusSlave>> GetEnumerator() => modbusSlaves.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        class ChannelTask
        {
            public ChannelTask(ModbusSlaveService modbusSlave, ModbusChannel channel)
            {
                this.modbusSlave = modbusSlave;
                this.channel = channel;
            }

            private readonly ModbusSlaveService modbusSlave;
            private readonly ModbusChannel channel;
            private bool isRunning = false;

            public void Start()
            {
                if (!channel.IsDisposed)
                {
                    isRunning = true;
                    Task.Run(() =>
                    {
                        while (isRunning && !channel.IsDisposed)
                        {
                            RequestBuffer buffer = new RequestBuffer(modbusSlave, channel);
                            var request = modbusSlave.Serializer.Deserialize(buffer);
                            if (request != null)
                            {
                                modbusSlave.Logger?.Log(new ModbusMessageLog(channel, request, buffer.ToArray()));
                                modbusSlave.OnReceivedModbusRequest(channel, request);
                            }
                            if (channel.IsDisposed)
                            {
                                modbusSlave.RemoveChannel(channel);
                            }
                        }
                        if (!channel.IsDisposed)
                            channel.Dispose();
                    });
                }
            }

            public void Stop()
            {
                isRunning = false;
            }
        }
    }

    public sealed class ValidatingSlaveAddressEventArgs : EventArgs
    {
        internal ValidatingSlaveAddressEventArgs(ushort slaveAddress, ModbusChannel channel)
        {
            SlaveAddress = slaveAddress;
            Channel = channel;
        }

        public ushort SlaveAddress { get; }
        public ModbusChannel Channel { get; }
        public bool IsValid { get; set; }
    }

    public abstract class ModbusRequestEventArgs : EventArgs
    {
        internal ModbusRequestEventArgs(ModbusRequest request, ModbusChannel channel)
        {
            this.request = request;
            Channel = channel;
        }

        internal ModbusRequest request;

        public byte SlaveAddress { get => request.SlaveAddress; }
        public ushort Address { get => request.Address; }
        public ModbusChannel Channel { get; }
    }

    public sealed class RequestReadBooleanEventArgs : ModbusRequestEventArgs
    {
        internal RequestReadBooleanEventArgs(ModbusReadRequest request, ModbusChannel channel)
            : base(request, channel) { }

        public ushort Length { get => request.Length; }
        public IEnumerable<bool> Values { get; set; }
    }

    public sealed class RequestReadRegisterEventArgs : ModbusRequestEventArgs
    {
        internal RequestReadRegisterEventArgs(ModbusReadRequest request, ModbusChannel channel)
            : base(request, channel) { }

        public ushort Length { get => request.Length; }
        public IEnumerable<byte> Bytes { get; set; }
    }

    public sealed class RequestWriteCoilEventArgs : ModbusRequestEventArgs
    {
        internal RequestWriteCoilEventArgs(ModbusWriteCoilRequest request, ModbusChannel channel)
            : base(request, channel)
        {
            Values = request.Values;
        }

        public IReadOnlyList<bool> Values { get; }
    }

    public sealed class RequestWriteHoldingRegisterEventArgs : ModbusRequestEventArgs
    {
        internal RequestWriteHoldingRegisterEventArgs(ModbusWriteHoldingRegisterRequest request, ModbusChannel channel)
            : base(request, channel)
        {
            Bytes = request.Bytes;
        }

        private IReadOnlyList<ushort> registers;

        public IReadOnlyList<byte> Bytes { get; }

        public IReadOnlyList<ushort> Registers
        {
            get
            {
                if (registers == null)
                {
                    var bytes = Bytes;
                    registers = Enumerable.Range(0, bytes.Count / 2).Select(i => (ushort)(bytes[i * 2] << 8 | bytes[i * 2 + 1])).ToArray();
                }
                return registers;
            }
        }
    }
}
