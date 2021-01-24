using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public class ChannelCloseEventLog : ModbusLog
    {
        public ChannelCloseEventLog(IModbusChannel channel) : base(channel) { }
        public override ModbusLogCategory Category { get => ModbusLogCategory.ChannelCloseEvent; }

        public override string ToString()
            => $"({ChannelDescription}) Closed Channel";
    }
}
