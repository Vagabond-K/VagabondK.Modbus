using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Data;

namespace VagabondK.Modbus
{
    /// <summary>
    /// Modbus 슬레이브
    /// </summary>
    public class ModbusSlave
    {
        /// <summary>
        /// Coils
        /// </summary>
        public ModbusBooleans Coils { get; set; } = new ModbusBooleans();

        /// <summary>
        /// Discrete Inputs
        /// </summary>
        public ModbusBooleans DiscreteInputs { get; set; } = new ModbusBooleans();

        /// <summary>
        /// Holding Registers
        /// </summary>
        public ModbusRegisters HoldingRegisters { get; set; } = new ModbusRegisters();

        /// <summary>
        /// Input Registers
        /// </summary>
        public ModbusRegisters InputRegisters { get; set; } = new ModbusRegisters();
    }
}
