using System;
using System.Collections.Generic;
using System.Linq;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Serialization
{
    /// <summary>
    /// Modbus RTU Serializer
    /// </summary>
    public sealed class ModbusRtuSerializer : ModbusSerializer
    {
        private readonly List<byte> errorBuffer = new List<byte>();

        private static readonly ushort[] crcTable = {
            0X0000, 0XC0C1, 0XC181, 0X0140, 0XC301, 0X03C0, 0X0280, 0XC241,
            0XC601, 0X06C0, 0X0780, 0XC741, 0X0500, 0XC5C1, 0XC481, 0X0440,
            0XCC01, 0X0CC0, 0X0D80, 0XCD41, 0X0F00, 0XCFC1, 0XCE81, 0X0E40,
            0X0A00, 0XCAC1, 0XCB81, 0X0B40, 0XC901, 0X09C0, 0X0880, 0XC841,
            0XD801, 0X18C0, 0X1980, 0XD941, 0X1B00, 0XDBC1, 0XDA81, 0X1A40,
            0X1E00, 0XDEC1, 0XDF81, 0X1F40, 0XDD01, 0X1DC0, 0X1C80, 0XDC41,
            0X1400, 0XD4C1, 0XD581, 0X1540, 0XD701, 0X17C0, 0X1680, 0XD641,
            0XD201, 0X12C0, 0X1380, 0XD341, 0X1100, 0XD1C1, 0XD081, 0X1040,
            0XF001, 0X30C0, 0X3180, 0XF141, 0X3300, 0XF3C1, 0XF281, 0X3240,
            0X3600, 0XF6C1, 0XF781, 0X3740, 0XF501, 0X35C0, 0X3480, 0XF441,
            0X3C00, 0XFCC1, 0XFD81, 0X3D40, 0XFF01, 0X3FC0, 0X3E80, 0XFE41,
            0XFA01, 0X3AC0, 0X3B80, 0XFB41, 0X3900, 0XF9C1, 0XF881, 0X3840,
            0X2800, 0XE8C1, 0XE981, 0X2940, 0XEB01, 0X2BC0, 0X2A80, 0XEA41,
            0XEE01, 0X2EC0, 0X2F80, 0XEF41, 0X2D00, 0XEDC1, 0XEC81, 0X2C40,
            0XE401, 0X24C0, 0X2580, 0XE541, 0X2700, 0XE7C1, 0XE681, 0X2640,
            0X2200, 0XE2C1, 0XE381, 0X2340, 0XE101, 0X21C0, 0X2080, 0XE041,
            0XA001, 0X60C0, 0X6180, 0XA141, 0X6300, 0XA3C1, 0XA281, 0X6240,
            0X6600, 0XA6C1, 0XA781, 0X6740, 0XA501, 0X65C0, 0X6480, 0XA441,
            0X6C00, 0XACC1, 0XAD81, 0X6D40, 0XAF01, 0X6FC0, 0X6E80, 0XAE41,
            0XAA01, 0X6AC0, 0X6B80, 0XAB41, 0X6900, 0XA9C1, 0XA881, 0X6840,
            0X7800, 0XB8C1, 0XB981, 0X7940, 0XBB01, 0X7BC0, 0X7A80, 0XBA41,
            0XBE01, 0X7EC0, 0X7F80, 0XBF41, 0X7D00, 0XBDC1, 0XBC81, 0X7C40,
            0XB401, 0X74C0, 0X7580, 0XB541, 0X7700, 0XB7C1, 0XB681, 0X7640,
            0X7200, 0XB2C1, 0XB381, 0X7340, 0XB101, 0X71C0, 0X7080, 0XB041,
            0X5000, 0X90C1, 0X9181, 0X5140, 0X9301, 0X53C0, 0X5280, 0X9241,
            0X9601, 0X56C0, 0X5780, 0X9741, 0X5500, 0X95C1, 0X9481, 0X5440,
            0X9C01, 0X5CC0, 0X5D80, 0X9D41, 0X5F00, 0X9FC1, 0X9E81, 0X5E40,
            0X5A00, 0X9AC1, 0X9B81, 0X5B40, 0X9901, 0X59C0, 0X5880, 0X9841,
            0X8801, 0X48C0, 0X4980, 0X8941, 0X4B00, 0X8BC1, 0X8A81, 0X4A40,
            0X4E00, 0X8EC1, 0X8F81, 0X4F40, 0X8D01, 0X4DC0, 0X4C80, 0X8C41,
            0X4400, 0X84C1, 0X8581, 0X4540, 0X8701, 0X47C0, 0X4680, 0X8641,
            0X8201, 0X42C0, 0X4380, 0X8341, 0X4100, 0X81C1, 0X8081, 0X4040
        };

        internal override IEnumerable<byte> OnSerialize(IModbusMessage message)
        {
            ushort crc = ushort.MaxValue;

            foreach (var b in message.Serialize())
            {
                byte tableIndex = (byte)(crc ^ b);
                crc >>= 8;
                crc ^= crcTable[tableIndex];
                yield return b;
            }

            foreach (var b in BitConverter.GetBytes(crc))
                yield return b;
        }

        private bool IsException(ResponseBuffer buffer, ModbusRequest request, int timeout, out ModbusResponse responseMessage)
        {
            if ((Read(buffer, 1, timeout) & 0x80) == 0x80)
            {
                var codeValue = Read(buffer, 2, timeout);

                if (IsErrorCRC(buffer, 3, request, timeout))
                    throw new ModbusCommException(ModbusCommErrorCode.ErrorCRC, buffer, request);

                ModbusExceptionCode exceptionCode = ModbusExceptionCode.NotDefined;
                if (Enum.IsDefined(typeof(ModbusExceptionCode), codeValue))
                    exceptionCode = (ModbusExceptionCode)codeValue;

                responseMessage = new ModbusExceptionResponse(exceptionCode, request);
                return true;
            }
            else
            {
                responseMessage = null;
                return false;
            }
        }

        private bool IsErrorCRC(ResponseBuffer buffer, int messageLength, ModbusRequest request, int timeout)
        {
            var crc = Read(buffer, messageLength, 2, timeout).ToArray();

            return !CalculateCrc(buffer.Take(messageLength)).SequenceEqual(crc);
        }


        internal override ModbusResponse DeserializeResponse(ResponseBuffer buffer, ModbusRequest request, int timeout)
        {
            ModbusResponse result = base.DeserializeResponse(buffer, request, timeout);

            while (result is ModbusCommErrorResponse responseCommErrorMessage
                && responseCommErrorMessage.ErrorCode != ModbusCommErrorCode.ResponseTimeout)
            {
                errorBuffer.Add(buffer[0]);
                buffer.RemoveAt(0);
                result = base.DeserializeResponse(buffer, request, timeout);
            }

            if (result is ModbusCommErrorResponse responseCommError)
            {
                result = new ModbusCommErrorResponse(responseCommError.ErrorCode, errorBuffer.Concat(responseCommError.ReceivedBytes), request);
            }
            else if (errorBuffer.Count > 0)
            {
                RaiseUnrecognized(buffer.Channel, errorBuffer.ToArray());
                errorBuffer.Clear();
            }

            return result;
        }

        internal override ModbusResponse DeserializeReadBooleanResponse(ResponseBuffer buffer, ModbusReadRequest request, int timeout)
        {
            if (IsException(buffer, request, timeout, out var responseMessage))
                return responseMessage;

            byte byteLength = Read(buffer, 2, timeout);

            if (IsErrorCRC(buffer, 3 + byteLength, request, timeout))
                throw new ModbusCommException(ModbusCommErrorCode.ErrorCRC, buffer, request);

            if (Read(buffer, 0, timeout) != request.SlaveAddress)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseSlaveAddressDoNotMatch, buffer, request);
            if ((Read(buffer, 1, timeout) & 0x7f) != (byte)request.Function)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseFunctionDoNotMatch, buffer, request);
            if (byteLength != (byte)Math.Ceiling(request.Length / 8d))
                throw new ModbusCommException(ModbusCommErrorCode.ResponseLengthDoNotMatch, buffer, request);

            return new ModbusReadBooleanResponse(Read(buffer, 3, byteLength, timeout).SelectMany(b => ByteToBooleanArray(b)).Take(request.Length).ToArray(), request);
        }

        internal override ModbusResponse DeserializeReadRegisterResponse(ResponseBuffer buffer, ModbusReadRequest request, int timeout)
        {
            if (IsException(buffer, request, timeout, out var responseMessage))
                return responseMessage;

            byte byteLength = Read(buffer, 2, timeout);

            if (IsErrorCRC(buffer, 3 + byteLength, request, timeout))
                throw new ModbusCommException(ModbusCommErrorCode.ErrorCRC, buffer, request);

            if (Read(buffer, 0, timeout) != request.SlaveAddress)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseSlaveAddressDoNotMatch, buffer, request);
            if ((Read(buffer, 1, timeout) & 0x7f) != (byte)request.Function)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseFunctionDoNotMatch, buffer, request);
            if (byteLength != (byte)(request.Length * 2))
                throw new ModbusCommException(ModbusCommErrorCode.ResponseLengthDoNotMatch, buffer, request);

            return new ModbusReadRegisterResponse(Read(buffer, 3, byteLength, timeout).ToArray(), request);
        }

        internal override ModbusResponse DeserializeWriteResponse(ResponseBuffer buffer, ModbusWriteCoilRequest request, int timeout)
        {
            if (IsException(buffer, request, timeout, out var responseMessage))
                return responseMessage;

            if (IsErrorCRC(buffer, 6, request, timeout))
                throw new ModbusCommException(ModbusCommErrorCode.ErrorCRC, buffer, request);

            if (Read(buffer, 0, timeout) != request.SlaveAddress)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseSlaveAddressDoNotMatch, buffer, request);
            if ((Read(buffer, 1, timeout) & 0x7f) != (byte)request.Function)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseFunctionDoNotMatch, buffer, request);
            if (ToUInt16(buffer, 2) != request.Address)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseAddressDoNotMatch, buffer, request);

            switch (request.Function)
            {
                case ModbusFunction.WriteSingleCoil:
                    if (Read(buffer, 4, timeout) != (request.SingleBooleanValue ? 0xff : 0x00)
                        || Read(buffer, 5, timeout) != 0x00)
                        throw new ModbusCommException(ModbusCommErrorCode.ResponseWritedValueDoNotMatch, buffer, request);
                    break;
                case ModbusFunction.WriteMultipleCoils:
                    if (ToUInt16(buffer, 4) != request.Length)
                        throw new ModbusCommException(ModbusCommErrorCode.ResponseWritedLengthDoNotMatch, buffer, request);
                    break;
            }

            return new ModbusWriteResponse(request);
        }

        internal override ModbusResponse DeserializeWriteResponse(ResponseBuffer buffer, ModbusWriteHoldingRegisterRequest request, int timeout)
        {
            if (IsException(buffer, request, timeout, out var responseMessage))
                return responseMessage;

            if (IsErrorCRC(buffer, 6, request, timeout))
                throw new ModbusCommException(ModbusCommErrorCode.ErrorCRC, buffer, request);

            if (Read(buffer, 0, timeout) != request.SlaveAddress)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseSlaveAddressDoNotMatch, buffer, request);
            if ((Read(buffer, 1, timeout) & 0x7f) != (byte)request.Function)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseFunctionDoNotMatch, buffer, request);
            if (ToUInt16(buffer, 2) != request.Address)
                throw new ModbusCommException(ModbusCommErrorCode.ResponseAddressDoNotMatch, buffer, request);

            ushort value = ToUInt16(buffer, 4);

            switch (request.Function)
            {
                case ModbusFunction.WriteSingleHoldingRegister:
                    if (value != request.SingleRegisterValue)
                        throw new ModbusCommException(ModbusCommErrorCode.ResponseWritedValueDoNotMatch, buffer, request);
                    break;
                case ModbusFunction.WriteMultipleHoldingRegisters:
                    if (value != request.Length)
                        throw new ModbusCommException(ModbusCommErrorCode.ResponseWritedLengthDoNotMatch, buffer, request);
                    break;
            }

            return new ModbusWriteResponse(request);
        }


        internal override ModbusRequest DeserializeRequest(RequestBuffer buffer)
        {
            ModbusRequest result = null;
            while (!buffer.Channel.IsDisposed)
            {
                if (errorBuffer.Count >= 256)
                {
                    RaiseUnrecognized(buffer.Channel, errorBuffer.ToArray());
                    errorBuffer.Clear();
                }

                while (buffer.Count < 8 && !buffer.Channel.IsDisposed)
                    buffer.Read();

                if (buffer.Channel.IsDisposed) break;

                var slaveAddress = buffer[0];
                int messageLength = 0;

                if (buffer.ModbusSlave.IsValidSlaveAddress(slaveAddress, buffer.Channel)
                    && Enum.IsDefined(typeof(ModbusFunction), buffer[1]))
                {
                    ModbusFunction function = (ModbusFunction)buffer[1];
                    var address = ToUInt16(buffer, 2);
                    var valueOrLength = ToUInt16(buffer, 4);

                    switch (function)
                    {
                        case ModbusFunction.ReadCoils:
                        case ModbusFunction.ReadDiscreteInputs:
                        case ModbusFunction.ReadHoldingRegisters:
                        case ModbusFunction.ReadInputRegisters:
                        case ModbusFunction.WriteSingleCoil:
                        case ModbusFunction.WriteSingleHoldingRegister:
                            if (CalculateCrc(buffer.Take(6)).SequenceEqual(buffer.Skip(6).Take(2)))
                            {
                                messageLength = 8;
                                switch (function)
                                {
                                    case ModbusFunction.ReadCoils:
                                    case ModbusFunction.ReadDiscreteInputs:
                                    case ModbusFunction.ReadHoldingRegisters:
                                    case ModbusFunction.ReadInputRegisters:
                                        result = new ModbusReadRequest(slaveAddress, (ModbusObjectType)(byte)function, address, valueOrLength);
                                        break;
                                    case ModbusFunction.WriteSingleCoil:
                                        if (valueOrLength != 0xff00 && valueOrLength != 0)
                                            result = new ModbusWriteCoilRequest(slaveAddress, address);
                                        else
                                            result = new ModbusWriteCoilRequest(slaveAddress, address, valueOrLength == 0xff00);
                                        break;
                                    case ModbusFunction.WriteSingleHoldingRegister:
                                        result = new ModbusWriteHoldingRegisterRequest(slaveAddress, address, valueOrLength);
                                        break;
                                }
                            }
                            break;
                        case ModbusFunction.WriteMultipleCoils:
                        case ModbusFunction.WriteMultipleHoldingRegisters:
                            if (buffer.Count < 7 && !buffer.Channel.IsDisposed)
                                buffer.Read();

                            if (buffer.Channel.IsDisposed) break;

                            var byteLength = buffer[6];
                            messageLength = byteLength + 9;

                            if (function == ModbusFunction.WriteMultipleCoils && byteLength == Math.Ceiling(valueOrLength / 8d)
                                || function == ModbusFunction.WriteMultipleHoldingRegisters && byteLength == valueOrLength * 2)
                            {
                                while (buffer.Count < messageLength && !buffer.Channel.IsDisposed)
                                    buffer.Read();

                                if (buffer.Channel.IsDisposed) break;

                                if (CalculateCrc(buffer.Take(byteLength + 7)).SequenceEqual(buffer.Skip(byteLength + 7).Take(2)))
                                {
                                    switch (function)
                                    {
                                        case ModbusFunction.WriteMultipleCoils:
                                            result = new ModbusWriteCoilRequest(slaveAddress, address, buffer.Skip(7).Take(byteLength).SelectMany(b => ByteToBooleanArray(b)).Take(valueOrLength).ToArray());
                                            break;
                                        case ModbusFunction.WriteMultipleHoldingRegisters:
                                            result = new ModbusWriteHoldingRegisterRequest(slaveAddress, address, buffer.Skip(7).Take(byteLength).ToArray());
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }

                if (result != null)
                {
                    if (errorBuffer.Count > 0)
                    {
                        RaiseUnrecognized(buffer.Channel, errorBuffer.ToArray());
                        errorBuffer.Clear();
                    }
                    return result;
                }
                else
                {
                    errorBuffer.Add(buffer[0]);
                    buffer.RemoveAt(0);
                    continue;
                }
            }
            return null;
        }


        private static byte[] CalculateCrc(IEnumerable<byte> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            ushort crc = ushort.MaxValue;

            foreach (byte b in data)
            {
                byte tableIndex = (byte)(crc ^ b);
                crc >>= 8;
                crc ^= crcTable[tableIndex];
            }

            return BitConverter.GetBytes(crc);
        }
    }
}
