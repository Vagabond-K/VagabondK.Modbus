using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Data;

namespace VagabondK.Modbus
{
    public class ModbusSlave
    {
        public ModbusBooleans Coils { get; set; } = new ModbusBooleans();
        public ModbusBooleans DiscreteInputs { get; set; } = new ModbusBooleans();
        public ModbusRegisters HoldingRegisters { get; set; } = new ModbusRegisters();
        public ModbusRegisters InputRegisters { get; set; } = new ModbusRegisters();
    }
}
