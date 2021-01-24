using System;
using System.Linq;
using System.Threading;
using VagabondK.Modbus;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Data;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace SimpleModbusMaster
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleModbusLogger();

            var modbusMaster = new ModbusMaster
            {
                Channel = new TcpClientModbusChannel("127.0.0.1", 502, 1000)
                {
                    Logger = logger
                },
                //Serializer = new ModbusRtuSerializer(),
                Serializer = new ModbusTcpSerializer(),
                //Serializer = new ModbusAsciiSerializer(),
                Logger = logger,
            };

            while (true)
            {
                Thread.Sleep(1000);

                try
                {
                    var request = new ModbusReadRequest(1, ModbusObjectType.InputRegister, 100, 2);
                    var resposne = modbusMaster.Request(request, 1000);

                    Console.WriteLine((resposne as ModbusReadRegisterResponse).GetSingle(100));
                }
                catch
                {
                }
            }
        }
    }
}
