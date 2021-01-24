using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Channels
{
    public interface IModbusChannel : IDisposable
    {
        bool IsDisposed { get; }

        IModbusLogger Logger { get; set; }

        string Description { get; }
    }
}
