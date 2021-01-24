using System;
using System.Collections.Generic;
using System.Linq;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Logging;

namespace VagabondK.Modbus
{
    public interface IModbusMessage
    {
        IEnumerable<byte> Serialize();
        ushort TransactionID { get; set; }

        ModbusLogCategory LogCategory { get; }
    }
}