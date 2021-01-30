using System;
using System.Collections.Generic;
using System.Linq;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Data;
using VagabondK.Modbus.Logging;

namespace VagabondK.Modbus
{
    public abstract class ModbusResponse : IModbusMessage
    {
        protected ModbusResponse(ModbusRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public ModbusRequest Request { get; private set; }
        public abstract IEnumerable<byte> Serialize();

        public ushort TransactionID { get => Request.TransactionID; set => Request.TransactionID = value; }

        public abstract ModbusLogCategory LogCategory { get; }
    }

    public abstract class ModbusOkResponse : ModbusResponse
    {
        protected ModbusOkResponse(ModbusRequest request) : base(request) { }
    }

    public class ModbusExceptionResponse : ModbusOkResponse
    {
        public ModbusExceptionResponse(ModbusExceptionCode exceptionCode, ModbusRequest request) : base(request)
        {
            ExceptionCode = exceptionCode;
        }

        public ModbusExceptionCode ExceptionCode { get; }

        public override IEnumerable<byte> Serialize()
        {
            yield return Request.SlaveAddress;
            yield return (byte)((int)Request.Function | 0x80);
            yield return (byte)ExceptionCode;
        }

        public override ModbusLogCategory LogCategory { get => ModbusLogCategory.ResponseException; }
    }

    public abstract class ModbusOkResponse<TRequest> : ModbusOkResponse where TRequest : ModbusRequest
    {
        protected ModbusOkResponse(TRequest request) : base(request) { }
    }

    public abstract class ModbusReadResponse : ModbusOkResponse<ModbusReadRequest>
    {
        protected ModbusReadResponse(ModbusReadRequest request) : base(request) { }
    }

    public class ModbusReadBooleanResponse : ModbusReadResponse
    {
        public ModbusReadBooleanResponse(bool[] values, ModbusReadRequest request) : base(request)
        {
            switch (request.Function)
            {
                case ModbusFunction.ReadCoils:
                case ModbusFunction.ReadDiscreteInputs:
                    break;
                default:
                    throw new ArgumentException("The Function in the request does not match.", nameof(request));
            }

            Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public IReadOnlyList<bool> Values { get; }

        public override IEnumerable<byte> Serialize()
        {
            yield return Request.SlaveAddress;
            yield return (byte)Request.Function;
            yield return (byte)Math.Ceiling(Values.Count / 8d);

            int value = 0;

            for (int i = 0; i < Values.Count; i++)
            {
                int bitIndex = i % 8;
                value |= (Values[i] ? 1 : 0) << bitIndex;

                if (bitIndex == 7 || i == Values.Count - 1)
                    yield return (byte)value;
            }
        }

        public override ModbusLogCategory LogCategory
        {
            get => Request.ObjectType == ModbusObjectType.Coil
                ? ModbusLogCategory.ResponseReadCoil
                : ModbusLogCategory.ResponseReadDiscreteInput;
        }
    }

    public class ModbusReadRegisterResponse : ModbusReadResponse
    {
        public ModbusReadRegisterResponse(byte[] bytes, ModbusReadRequest request) : base(request)
        {
            switch (request.Function)
            {
                case ModbusFunction.ReadHoldingRegisters:
                case ModbusFunction.ReadInputRegisters:
                    break;
                default:
                    throw new ArgumentException("The Function in the request does not match.", nameof(request));
            }

            Bytes = bytes ?? throw new ArgumentException(nameof(bytes));
        }

        private IReadOnlyList<ushort> values;

        public IReadOnlyList<byte> Bytes { get; }

        public IReadOnlyList<ushort> Values
        {
            get
            {
                if (values == null)
                {
                    var bytes = Bytes;
                    values = Enumerable.Range(0, bytes.Count / 2).Select(i => (ushort)(bytes[i * 2] << 8 | bytes[i * 2 + 1])).ToArray();
                }
                return values;
            }
        }

        public override IEnumerable<byte> Serialize()
        {
            yield return Request.SlaveAddress;
            yield return (byte)Request.Function;
            yield return (byte)(Request.Length * 2);

            for (int i = 0; i < Request.Length * 2; i++)
            {
                if (i < Bytes.Count)
                    yield return Bytes[i];
                else
                    yield return 0;
            }
        }

        public override ModbusLogCategory LogCategory
        {
            get => Request.ObjectType == ModbusObjectType.HoldingRegister
                ? ModbusLogCategory.ResponseReadHoldingRegister
                : ModbusLogCategory.ResponseReadInputRegister;
        }

        private IEnumerable<byte> GetRawData(ushort address, int rawDataCount)
        {
            return Bytes.Skip((address - Request.Address) * 2).Take(rawDataCount);
        }

        public short GetInt16(ushort address) => GetInt16(address, true);
        public ushort GetUInt16(ushort address) => GetUInt16(address, true);
        public int GetInt32(ushort address) => GetInt32(address, new ModbusEndian(true));
        public uint GetUInt32(ushort address) => GetUInt32(address, new ModbusEndian(true));
        public long GetInt64(ushort address) => GetInt64(address, new ModbusEndian(true));
        public ulong GetUInt64(ushort address) => GetUInt64(address, new ModbusEndian(true));
        public float GetSingle(ushort address) => GetSingle(address, new ModbusEndian(true));
        public double GetDouble(ushort address) => GetDouble(address, new ModbusEndian(true));

        public short GetInt16(ushort address, bool isBigEndian) => BitConverter.ToInt16(new ModbusEndian(isBigEndian).Sort(GetRawData(address, 2).ToArray()), 0);
        public ushort GetUInt16(ushort address, bool isBigEndian) => BitConverter.ToUInt16(new ModbusEndian(isBigEndian).Sort(GetRawData(address, 2).ToArray()), 0);
        public int GetInt32(ushort address, ModbusEndian endian) => BitConverter.ToInt32(endian.Sort(GetRawData(address, 4).ToArray()), 0);
        public uint GetUInt32(ushort address, ModbusEndian endian) => BitConverter.ToUInt32(endian.Sort(GetRawData(address, 4).ToArray()), 0);
        public long GetInt64(ushort address, ModbusEndian endian) => BitConverter.ToInt64(endian.Sort(GetRawData(address, 8).ToArray()), 0);
        public ulong GetUInt64(ushort address, ModbusEndian endian) => BitConverter.ToUInt64(endian.Sort(GetRawData(address, 8).ToArray()), 0);
        public float GetSingle(ushort address, ModbusEndian endian) => BitConverter.ToSingle(endian.Sort(GetRawData(address, 4).ToArray()), 0);
        public double GetDouble(ushort address, ModbusEndian endian) => BitConverter.ToDouble(endian.Sort(GetRawData(address, 8).ToArray()), 0);
    }

    public class ModbusWriteResponse : ModbusOkResponse<ModbusWriteRequest>
    {
        public ModbusWriteResponse(ModbusWriteRequest request) : base(request)
        {
            switch (request.Function)
            {
                case ModbusFunction.WriteMultipleCoils:
                case ModbusFunction.WriteSingleCoil:
                case ModbusFunction.WriteMultipleHoldingRegisters:
                case ModbusFunction.WriteSingleHoldingRegister:
                    break;
                default:
                    throw new ArgumentException("The Function in the request does not match.", nameof(request));
            }
        }

        public override IEnumerable<byte> Serialize()
        {
            yield return Request.SlaveAddress;
            yield return (byte)Request.Function;
            yield return (byte)((Request.Address >> 8) & 0xff);
            yield return (byte)(Request.Address & 0xff);

            switch (Request.Function)
            {
                case ModbusFunction.WriteMultipleCoils:
                case ModbusFunction.WriteMultipleHoldingRegisters:
                    yield return (byte)((Request.Length >> 8) & 0xff);
                    yield return (byte)(Request.Length & 0xff);
                    break;
                case ModbusFunction.WriteSingleCoil:
                    ModbusWriteCoilRequest writeCoilRequest = Request as ModbusWriteCoilRequest;
                    yield return writeCoilRequest.SingleBooleanValue ? (byte)0xff : (byte)0x00;
                    yield return 0x00;
                    break;
                case ModbusFunction.WriteSingleHoldingRegister:
                    ModbusWriteHoldingRegisterRequest writeHoldingRegisterRequest = Request as ModbusWriteHoldingRegisterRequest;
                    yield return (byte)((writeHoldingRegisterRequest.SingleRegisterValue >> 8) & 0xff);
                    yield return (byte)(writeHoldingRegisterRequest.SingleRegisterValue  & 0xff);
                    break;
            }
        }

        public override ModbusLogCategory LogCategory
        {
            get
            {
                switch (Request.Function)
                {
                    case ModbusFunction.WriteMultipleCoils:
                        return ModbusLogCategory.ResponseWriteMultiCoil;
                    case ModbusFunction.WriteSingleCoil:
                        return ModbusLogCategory.ResponseWriteSingleCoil;
                    case ModbusFunction.WriteMultipleHoldingRegisters:
                        return ModbusLogCategory.ResponseWriteMultiHoldingRegister;
                    case ModbusFunction.WriteSingleHoldingRegister:
                        return ModbusLogCategory.ResponseWriteSingleHoldingRegister;
                    default:
                        return ModbusLogCategory.None;
                }
            }
        }
    }


}
