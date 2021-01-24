using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VagabondK.Modbus.Logging
{
    public class CollectionModbusLogger : IModbusLogger
    {
        public CollectionModbusLogger(ICollection<ModbusLog> collection)
        {
            Collection = collection;
        }

        public ModbusLogCategory CategoryFilter { get; set; }
        public ICollection<ModbusLog> Collection { get; set; }

        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) != 0)
                Collection?.Add(log);
        }
    }
}
