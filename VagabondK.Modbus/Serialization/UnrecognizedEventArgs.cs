using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Serialization
{
    class UnrecognizedEventArgs : EventArgs
    {
        public UnrecognizedEventArgs(IModbusChannel channel, IReadOnlyList<byte> unrecognizedMessage)
        {
            Channel = channel;
            UnrecognizedMessage = unrecognizedMessage;
        }

        public IModbusChannel Channel { get; }
        public IReadOnlyList<byte> UnrecognizedMessage { get; }
    }
}
