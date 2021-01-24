using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public class ChannelOpenEventLog : ModbusLog
    {
        public ChannelOpenEventLog(IModbusChannel channel) : base(channel) { }
        public override ModbusLogCategory Category { get => ModbusLogCategory.ChannelOpenEvent; }

        public override string ToString()
            => $"({ChannelDescription}) Opened Channel";
    }
}
