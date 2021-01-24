using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Channels
{
    public interface IModbusChannelProvider : IModbusChannel
    {
        event EventHandler<ModbusChannelCreatedEventArgs> Created;

        IReadOnlyList<ModbusChannel> Channels { get; }

        void Start();
        void Stop();
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
