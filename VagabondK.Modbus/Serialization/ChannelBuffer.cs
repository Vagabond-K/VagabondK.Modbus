using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Serialization
{
    class ChannelBuffer : List<byte>
    {
        internal ChannelBuffer(ModbusChannel channel)
        {
            Channel = channel;
        }

        public ModbusChannel Channel { get; }

        public byte Read(int timeout)
        {
            var result = Channel.Read(timeout);
            Add(result);
            return result;
        }

        public byte Read() => Read(0);

        public byte[] Read(uint count, int timeout)
        {
            var result = Channel.Read(count, timeout).ToArray();
            AddRange(result);
            return result;
        }
    }

    class RequestBuffer : ChannelBuffer
    {
        internal RequestBuffer(ModbusSlaveService modbusSlave, ModbusChannel channel) : base(channel)
        {
            ModbusSlave = modbusSlave;
        }

        public ModbusSlaveService ModbusSlave { get; }
    }

    class ResponseBuffer : ChannelBuffer
    {
        internal ResponseBuffer(ModbusChannel channel) : base(channel)
        {
        }
    }

}
