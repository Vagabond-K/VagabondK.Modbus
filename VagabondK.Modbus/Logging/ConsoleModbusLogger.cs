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
            if ((CategoryFilter & log.Category) != 0)
                Console.WriteLine(log);
        }
    }
}
