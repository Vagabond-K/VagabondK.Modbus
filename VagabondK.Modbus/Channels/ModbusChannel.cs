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
    public abstract class ModbusChannel : IModbusChannel
    {
        public abstract bool IsDisposed { get; protected set; }

        public abstract void Dispose();

        public abstract void Write(byte[] bytes);

        public abstract byte Read(int timeout);

        public abstract IEnumerable<byte> Read(uint count, int timeout);

        public abstract IEnumerable<byte> ReadAllRemain();

        public IModbusLogger Logger { get; set; }

        public abstract string Description { get; protected set; }
    }
}
