using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Logging
{
    public interface IModbusLogger
    {
        ModbusLogCategory CategoryFilter { get; set; }
        void Log(ModbusLog log);
    }
}
