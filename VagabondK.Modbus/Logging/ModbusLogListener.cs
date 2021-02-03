using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VagabondK.Modbus.Logging
{
    public class ModbusLogListener : IModbusLogger
    {
        public ModbusLogCategory CategoryFilter { get; set; }
        public event EventHandler<ModbusLoggedEventArgs> Logged;

        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) != 0)
                Logged?.Invoke(this, new ModbusLoggedEventArgs(log));
        }
    }

    public class ModbusLoggedEventArgs : EventArgs
    {
        internal ModbusLoggedEventArgs(ModbusLog log)
        {
            Log = log;
        }

        public ModbusLog Log { get; }
    }
}
