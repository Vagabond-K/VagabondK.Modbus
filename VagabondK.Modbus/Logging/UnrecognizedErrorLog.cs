using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public class UnrecognizedErrorLog : ModbusLog
    {
        public UnrecognizedErrorLog(IModbusChannel channel, byte[] rawMessage) : base(channel)
        {
            RawMessage = rawMessage;
        }

        public IReadOnlyList<byte> RawMessage { get; }

        public override ModbusLogCategory Category { get => ModbusLogCategory.UnrecognizedError; }

        public override string ToString()
            => RawMessage == null && RawMessage.Count > 0 ? base.ToString() : $"Unrecognized: {BitConverter.ToString(RawMessage as byte[])}";
    }
}
