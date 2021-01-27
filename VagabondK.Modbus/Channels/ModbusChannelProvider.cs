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
    public abstract class ModbusChannelProvider : IModbusChannel
    {
        public event EventHandler<ModbusChannelCreatedEventArgs> Created;

        public abstract IReadOnlyList<ModbusChannel> Channels { get; }

        public bool IsDisposed { get; protected set; }
        public IModbusLogger Logger { get; set; }

        public abstract string Description { get; }

        public abstract void Start();
        public abstract void Stop();

        public abstract void Dispose();

        protected void RaiseCreatedEvent(ModbusChannelCreatedEventArgs eventArgs) => Created?.Invoke(this, eventArgs);
    }

    public class ModbusChannelCreatedEventArgs : EventArgs
    {
        public ModbusChannelCreatedEventArgs(ModbusChannel channel)
        {
            Channel = channel;
        }

        public ModbusChannel Channel { get; }
    }
}
