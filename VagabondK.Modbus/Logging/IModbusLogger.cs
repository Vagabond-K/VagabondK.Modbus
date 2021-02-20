using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// Modgus Logger 인터페이스
    /// </summary>
    public interface IModbusLogger
    {
        /// <summary>
        /// Modbus Log 카테고리 필터
        /// </summary>
        ModbusLogCategory CategoryFilter { get; set; }

        /// <summary>
        /// Log 기록
        /// </summary>
        /// <param name="log">Modbus Log</param>
        void Log(ModbusLog log);
    }
}
