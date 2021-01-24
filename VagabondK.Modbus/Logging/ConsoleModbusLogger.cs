using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Logging
{
    public class ConsoleModbusLogger : IModbusLogger
    {
        public ModbusLogCategory CategoryFilter { get; set; } = ModbusLogCategory.All;

        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) == 0) return;

            //if (log is ModbusMessageLog messageLog
            //    && messageLog.RawMessage != null && messageLog.RawMessage.Length > 0)
            //{
            //    if (messageLog.Message is ModbusRequest)
            //        Console.WriteLine($"Request: {BitConverter.ToString(messageLog.RawMessage)}");
            //    else if (messageLog.Message is ModbusResponse)
            //        Console.WriteLine($"Response: {BitConverter.ToString(messageLog.RawMessage)}");
            //    else
            //        Console.WriteLine(log);
            //}
            //else
                Console.WriteLine(log);
        }
    }
}
