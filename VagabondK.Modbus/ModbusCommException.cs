using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VagabondK.Modbus
{
    public class ModbusCommException : Exception
    {
        public ModbusCommException(ModbusCommErrorCode errorCode, Exception innerException) : base(null, innerException)
        {
            Code = errorCode;
        }

        public ModbusCommException(Exception innerException, ModbusRequest request) : base(null, innerException)
        {
            Code = ModbusCommErrorCode.NotDefined;
            ReceivedBytes = new byte[0];
            Request = request;
        }

        public ModbusCommException(IEnumerable<byte> receivedMessage, Exception innerException, ModbusRequest request) : base(null, innerException)
        {
            Code = ModbusCommErrorCode.NotDefined;
            ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            Request = request;
        }

        public ModbusCommException(ModbusCommErrorCode errorCode, IEnumerable<byte> receivedMessage, ModbusRequest request)
        {
            Code = errorCode;
            ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            Request = request;
        }

        public ModbusCommException(ModbusCommErrorCode errorCode, IEnumerable<byte> receivedMessage, Exception innerException, ModbusRequest request) : base(null, innerException)
        {
            Code = errorCode;
            ReceivedBytes = receivedMessage?.ToArray() ?? new byte[0];
            Request = request;
        }

        public ModbusCommErrorCode Code { get; }
        public IReadOnlyList<byte> ReceivedBytes { get; }
        public ModbusRequest Request { get; }

        public override string Message
        {
            get
            {
                var codeName = Code.ToString();
                return (typeof(ModbusCommErrorCode).GetMember(codeName, BindingFlags.Static | BindingFlags.Public)?.FirstOrDefault()?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? codeName;
            }
        }
    }
}
