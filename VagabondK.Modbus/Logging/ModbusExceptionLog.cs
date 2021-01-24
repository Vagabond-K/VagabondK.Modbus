using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public class ModbusExceptionLog : ModbusLog
    {
        public ModbusExceptionLog(IModbusChannel channel, ModbusExceptionCode exceptionCode, byte[] rawMessage) : base(channel)
        {
            ExceptionCode = exceptionCode;
            RawMessage = rawMessage;
        }

        public ModbusExceptionCode ExceptionCode { get; }
        public byte[] RawMessage { get; }
        public override ModbusLogCategory Category { get => ModbusLogCategory.CommError; }

        public override string ToString()
        {
            var codeName = ExceptionCode.ToString();
            return $"({ChannelDescription}) Exception: {(typeof(ModbusExceptionCode).GetMember(codeName, BindingFlags.Static | BindingFlags.Public)?.FirstOrDefault()?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? codeName}";
        }
    }
}
