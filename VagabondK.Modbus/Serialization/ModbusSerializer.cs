﻿using System;
using System.Collections.Generic;
using System.Linq;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Serialization
{
    public abstract class ModbusSerializer
    {
        internal event EventHandler<UnrecognizedEventArgs> Unrecognized;

        internal void RaiseUnrecognized(IModbusChannel channel, IReadOnlyList<byte> errorMessage)
            => Unrecognized?.Invoke(this, new UnrecognizedEventArgs(channel, errorMessage));

        public IEnumerable<byte> Serialize(IModbusMessage message)
        {
            if (message is ModbusCommErrorResponse commErrorResponse)
            {
                return commErrorResponse.Serialize();
            }
            else
                return OnSerialize(message);
        }


        internal abstract IEnumerable<byte> OnSerialize(IModbusMessage message);

        internal virtual byte Read(ResponseBuffer buffer, int index, int timeout)
        {
            if (index >= buffer.Count)
                buffer.Read((uint)(index - buffer.Count + 1), timeout);

            return buffer[index];
        }

        internal virtual IEnumerable<byte> Read(ResponseBuffer buffer, int index, int count, int timeout)
        {
            if (index + count > buffer.Count)
                buffer.Read((uint)(index + count - buffer.Count), timeout);

            return buffer.Skip(index).Take(count);
        }

        internal static ushort ToUInt16(IReadOnlyList<byte> buffer, int index)
        {
            return (ushort)(buffer[index] << 8 | buffer[index + 1]);
        }


        internal ModbusResponse Deserialize(ResponseBuffer buffer, ModbusRequest request, int timeout)
        {
            ModbusResponse result;
            try
            {
                result = DeserializeResponse(buffer, request, timeout);
            }
            catch (TimeoutException ex)
            {
                throw new ModbusCommException(ModbusCommErrorCode.ResponseTimeout, buffer, ex, request);
            }
            catch (ModbusCommException ex)
            {
                throw new ModbusCommException(ex.Code, buffer, ex.InnerException, request);
            }
            catch (Exception ex)
            {
                throw new ModbusCommException(buffer, ex, request);
            }

            if (result is ModbusCommErrorResponse commErrorResponse)
                throw new ModbusCommException(commErrorResponse.ErrorCode, commErrorResponse.ReceivedBytes, commErrorResponse.Request);

            return result;
        }

        internal virtual ModbusResponse DeserializeResponse(ResponseBuffer buffer, ModbusRequest request, int timeout)
        {
            if (request is ModbusReadRequest readMessage)
            {
                switch (readMessage.ObjectType)
                {
                    case ModbusObjectType.DiscreteInput:
                    case ModbusObjectType.Coil:
                        return DeserializeReadBooleanResponse(buffer, readMessage, timeout);
                    case ModbusObjectType.InputRegister:
                    case ModbusObjectType.HoldingRegister:
                        return DeserializeReadRegisterResponse(buffer, readMessage, timeout);
                }
            }
            else if (request is ModbusWriteCoilRequest writeCoilMessage)
            {
                return DeserializeWriteResponse(buffer, writeCoilMessage, timeout);
            }
            else if (request is ModbusWriteHoldingRegisterRequest writeHoldingRegisterMessage)
            {
                return DeserializeWriteResponse(buffer, writeHoldingRegisterMessage, timeout);
            }

            throw new ArgumentOutOfRangeException(nameof(request));
        }

        internal abstract ModbusResponse DeserializeReadBooleanResponse(ResponseBuffer buffer, ModbusReadRequest request, int timeout);
        internal abstract ModbusResponse DeserializeReadRegisterResponse(ResponseBuffer buffer, ModbusReadRequest request, int timeout);
        internal abstract ModbusResponse DeserializeWriteResponse(ResponseBuffer buffer, ModbusWriteCoilRequest request, int timeout);
        internal abstract ModbusResponse DeserializeWriteResponse(ResponseBuffer buffer, ModbusWriteHoldingRegisterRequest request, int timeout);



        internal ModbusRequest Deserialize(RequestBuffer buffer)
        {
            return DeserializeRequest(buffer);
        }

        internal abstract ModbusRequest DeserializeRequest(RequestBuffer buffer);


        internal static IEnumerable<bool> ByteToBooleanArray(byte value)
        {
            yield return (value & 0b00000001) != 0;
            yield return (value & 0b00000010) != 0;
            yield return (value & 0b00000100) != 0;
            yield return (value & 0b00001000) != 0;
            yield return (value & 0b00010000) != 0;
            yield return (value & 0b00100000) != 0;
            yield return (value & 0b01000000) != 0;
            yield return (value & 0b10000000) != 0;
        }
    }
}
