using System;
using System.Collections.Generic;
using System.Linq;
using VagabondK.Modbus.Logging;

namespace VagabondK.Modbus
{
    public abstract class ModbusRequest : IModbusMessage
    {
        protected ModbusRequest(byte slaveAddress, ModbusFunction function, ushort address)
        {
            SlaveAddress = slaveAddress;
            Function = function;
            Address = address;
        }

        public abstract ModbusObjectType ObjectType { get; }
        public byte SlaveAddress { get; }
        public ModbusFunction Function { get; }
        public ushort Address { get; }
        public abstract ushort Length { get; }

        public abstract IEnumerable<byte> Serialize();

        public ushort TransactionID { get; set; }

        public abstract ModbusLogCategory LogCategory { get; }
    }

    public class ModbusReadRequest : ModbusRequest
    {
        public ModbusReadRequest(byte slaveAddress, ModbusObjectType objectType, ushort address, ushort length)
            : base(slaveAddress, (ModbusFunction)objectType, address)
        {
            Length = length;
        }

        public override ModbusObjectType ObjectType { get => (ModbusObjectType)Function; }
        public override ushort Length { get; }

        public override IEnumerable<byte> Serialize()
        {
            yield return SlaveAddress;
            yield return (byte)Function;
            yield return (byte)((Address >> 8) & 0xff);
            yield return (byte)(Address & 0xff);
            yield return (byte)((Length >> 8) & 0xff);
            yield return (byte)(Length & 0xff);
        }

        public override ModbusLogCategory LogCategory
        {
            get
            {
                switch (ObjectType)
                {
                    case ModbusObjectType.Coil:
                        return ModbusLogCategory.RequestReadCoil;
                    case ModbusObjectType.DiscreteInput:
                        return ModbusLogCategory.RequestReadDiscreteInput;
                    case ModbusObjectType.HoldingRegister:
                        return ModbusLogCategory.RequestReadHoldingRegister;
                    case ModbusObjectType.InputRegister:
                        return ModbusLogCategory.RequestReadInputRegister;
                    default:
                        return ModbusLogCategory.None;
                }
            }
        }
    }

    public abstract class ModbusWriteRequest : ModbusRequest
    {
        protected ModbusWriteRequest(byte slaveAddress, ModbusFunction function, ushort address) : base(slaveAddress, function, address) { }
    }

    public class ModbusWriteCoilRequest : ModbusWriteRequest
    {
        public ModbusWriteCoilRequest(byte slaveAddress, ushort address)
            : base(slaveAddress, ModbusFunction.WriteSingleCoil, address)
        {
        }

        public ModbusWriteCoilRequest(byte slaveAddress, ushort address, bool value)
            : base(slaveAddress, ModbusFunction.WriteSingleCoil, address)
        {
            Values = new List<bool> { value };
        }

        public ModbusWriteCoilRequest(byte slaveAddress, ushort address, IEnumerable<bool> values)
            : base(slaveAddress, ModbusFunction.WriteMultipleCoils, address)
        {
            Values = values as List<bool> ?? values.ToList();
            byteLength = (byte)Math.Ceiling(Length / 8d);
        }

        public bool SingleBooleanValue => Values != null && Values.Count> 0 ? Values[0] : throw new ModbusException(ModbusExceptionCode.IllegalDataValue);
        public List<bool> Values { get; }
        public override ushort Length => (ushort)(Values?.Count ?? throw new ModbusException(ModbusExceptionCode.IllegalDataValue));
        private readonly byte byteLength = 0;

        public override ModbusObjectType ObjectType { get => ModbusObjectType.Coil; }

        public override IEnumerable<byte> Serialize()
        {
            yield return SlaveAddress;
            yield return (byte)Function;
            yield return (byte)((Address >> 8) & 0xff);
            yield return (byte)(Address & 0xff);

            switch (Function)
            {
                case ModbusFunction.WriteSingleCoil:
                    yield return SingleBooleanValue ? (byte)0xff : (byte)0x00;
                    yield return 0x00;
                    break;
                case ModbusFunction.WriteMultipleCoils:
                    yield return (byte)((Length >> 8) & 0xff);
                    yield return (byte)(Length & 0xff);
                    yield return byteLength;

                    int i = 0;
                    int byteValue = 0;
                    foreach (var bit in Values)
                    {
                        if (bit)
                            byteValue |= 1 << i;
                        i++;
                        if (i >= 8)
                        {
                            i = 0;
                            yield return (byte)byteValue;
                        }
                    }

                    if (i < 8)
                        yield return (byte)byteValue;
                    break;
            }
        }

        public override ModbusLogCategory LogCategory
        {
            get => Function == ModbusFunction.WriteMultipleCoils 
                ? ModbusLogCategory.RequestWriteMultiCoil 
                : ModbusLogCategory.RequestWriteSingleCoil;
        }
    }

    public class ModbusWriteHoldingRegisterRequest : ModbusWriteRequest
    {
        public ModbusWriteHoldingRegisterRequest(byte slaveAddress, ushort address, ushort register)
            : base(slaveAddress, ModbusFunction.WriteSingleHoldingRegister, address)
        {
            Bytes = new List<byte> { (byte)((register >> 8) & 0xff), (byte)(register & 0xff) };
        }

        public ModbusWriteHoldingRegisterRequest(byte slaveAddress, ushort address, IEnumerable<byte> bytes)
            : base(slaveAddress, ModbusFunction.WriteMultipleHoldingRegisters, address)
        {
            Bytes = bytes as List<byte> ?? bytes.ToList();
        }

        public ModbusWriteHoldingRegisterRequest(byte slaveAddress, ushort address, IEnumerable<ushort> registers)
            : base(slaveAddress, ModbusFunction.WriteMultipleHoldingRegisters, address)
        {
            Bytes = registers.SelectMany(register => new byte[] { (byte)((register >> 8) & 0xff), (byte)(register & 0xff) }).ToList();
        }

        public ushort SingleRegisterValue => Bytes.Count >= 2 ?
            (ushort)(Bytes[0] << 8 | Bytes[1]) : throw new ModbusException(ModbusExceptionCode.IllegalDataValue);
        public List<byte> Bytes { get; }
        public override ushort Length => (ushort)Math.Ceiling(Bytes.Count / 2d);

        public override ModbusObjectType ObjectType { get => ModbusObjectType.HoldingRegister; }

        public override IEnumerable<byte> Serialize()
        {
            if (Bytes.Count < 2)
                throw new ModbusException(ModbusExceptionCode.IllegalDataValue);

            yield return SlaveAddress;
            yield return (byte)Function;
            yield return (byte)((Address >> 8) & 0xff);
            yield return (byte)(Address & 0xff);

            byte byteLength = (byte)(Math.Ceiling(Bytes.Count / 2d) * 2);

            switch (Function)
            {
                case ModbusFunction.WriteSingleHoldingRegister:
                    yield return Bytes[0];
                    yield return Bytes[1];
                    break;
                case ModbusFunction.WriteMultipleHoldingRegisters:
                    yield return (byte)((Length >> 8) & 0xff);
                    yield return (byte)(Length & 0xff);
                    yield return byteLength;

                    int i = 0;
                    foreach (var b in Bytes)
                    {
                        yield return b;
                        i++;
                    }

                    if (i % 2 == 1)
                        yield return 0;
                    break;
            }
        }

        public override ModbusLogCategory LogCategory
        {
            get => Function == ModbusFunction.WriteMultipleHoldingRegisters
                ? ModbusLogCategory.RequestWriteMultiHoldingRegister
                : ModbusLogCategory.RequestWriteSingleHoldingRegister;
        }
    }

}
