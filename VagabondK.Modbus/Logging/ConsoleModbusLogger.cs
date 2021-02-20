using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// 콘솔 기반 Modgus Logger
    /// </summary>
    public class ConsoleModbusLogger : IModbusLogger
    {
        /// <summary>
        /// Modbus Log 카테고리 필터
        /// </summary>
        public ModbusLogCategory CategoryFilter { get; set; } = ModbusLogCategory.All;

        /// <summary>
        /// Log 기록
        /// </summary>
        /// <param name="log">Modbus Log</param>
        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) != 0)
                Console.WriteLine(log);
        }
    }
}
