using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public abstract class ModbusLog
    {
        protected ModbusLog(IModbusChannel channel)
        {
            TimeStamp = DateTime.Now;
            ChannelDescription = channel?.Description;
        }
        public DateTime TimeStamp { get; }
        public string ChannelDescription { get; }
        public abstract ModbusLogCategory Category { get; }
    }
}
