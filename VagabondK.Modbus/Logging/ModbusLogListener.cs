using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// Modbus Log 수신기
    /// </summary>
    public class ModbusLogListener : IModbusLogger
    {
        /// <summary>
        /// Modbus Log 카테고리 필터
        /// </summary>
        public ModbusLogCategory CategoryFilter { get; set; }

        /// <summary>
        /// Modbus Log 발생 이벤트
        /// </summary>
        public event EventHandler<ModbusLoggedEventArgs> Logged;

        /// <summary>
        /// Log 기록
        /// </summary>
        /// <param name="log">Modbus Log</param>
        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) != 0)
                Logged?.Invoke(this, new ModbusLoggedEventArgs(log));
        }
    }

    /// <summary>
    /// Modbus Log 발생 이벤트 매개변수
    /// </summary>
    public class ModbusLoggedEventArgs : EventArgs
    {
        internal ModbusLoggedEventArgs(ModbusLog log)
        {
            Log = log;
        }

        /// <summary>
        /// Modbus Log
        /// </summary>
        public ModbusLog Log { get; }
    }
}
