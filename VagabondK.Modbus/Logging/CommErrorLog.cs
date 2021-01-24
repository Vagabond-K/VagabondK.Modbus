using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    public class CommErrorLog : ModbusLog
    {
        public CommErrorLog(IModbusChannel channel, Exception exception) : base(channel)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public override ModbusLogCategory Category { get => ModbusLogCategory.CommError; }

        public override string ToString()
            => $"({ChannelDescription}) Comm Error: {Exception?.Message ?? base.ToString()}";
    }
}
