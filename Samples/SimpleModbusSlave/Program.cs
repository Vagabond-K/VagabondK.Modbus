using System;
using System.Collections.Generic;
using System.Linq;
using VagabondK.Modbus;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace SimpleModbusSlave
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleModbusLogger();

            var channelProvider = new TcpServerModbusChannelProvider(502)
            //var channelProvider = new UdpServerModbusChannelProvider(502)
            {
                Logger = logger
            };

            var modbusSlaveService = new ModbusSlaveService(channelProvider)
            {
                //Serializer = new ModbusRtuSerializer(),
                Serializer = new ModbusTcpSerializer(),
                //Serializer = new ModbusAsciiSerializer(),
                Logger = logger,
                [1] = new ModbusSlave()
            };

            modbusSlaveService[1].InputRegisters.Allocate(100, 1.23f);

            channelProvider.Start();

            Console.ReadKey();
        }
    }
}
