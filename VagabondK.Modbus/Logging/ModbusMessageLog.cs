using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public class ModbusMessageLog : ModbusLog
    {
        public ModbusMessageLog(IModbusChannel channel, IModbusMessage message, byte[] rawMessage) : base(channel)
        {
            Message = message;
            RawMessage = rawMessage ?? new byte[0];
        }

        public IModbusMessage Message { get; }
        public byte[] RawMessage { get; }
        public override ModbusLogCategory Category { get => Message.LogCategory; }

        public override string ToString()
        {
            if (Message is ModbusRequest)
                return $"({ChannelDescription}) Request: {BitConverter.ToString(RawMessage)}";
            else if (Message is ModbusResponse)
                return $"({ChannelDescription}) Response: {BitConverter.ToString(RawMessage)}";
            else
                return base.ToString();

        }
    }
}
