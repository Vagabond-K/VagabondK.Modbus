using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Data;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus
{
    /// <summary>
    /// Modbus 마스터 확장 메서드 모음
    /// </summary>
    public static class ModbusMasterExtensions
    {
        /// <summary>
        /// 다중 Coil 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <returns>Coil 값 목록</returns>
        public static bool[] ReadCoils(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length) => ReadCoils(modbusMaster, slaveAddress, address, length, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Discrete Input 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <returns>Discrete Input 값 목록</returns>
        public static bool[] ReadDiscreteInputs(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length) => ReadDiscreteInputs(modbusMaster, slaveAddress, address, length, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Holding Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <returns>Holding Register 값 목록</returns>
        public static ushort[] ReadHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length) => ReadHoldingRegisters(modbusMaster, slaveAddress, address, length, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Holding Register를 Raw 바이트 배열로 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <returns>Holding Register들의 Raw 바이트 배열</returns>
        public static byte[] ReadHoldingRegisterBytes(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length) => ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, length, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Input Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <returns>Input Register 값 목록</returns>
        public static ushort[] ReadInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length) => ReadInputRegisters(modbusMaster, slaveAddress, address, length, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Input Register를 Raw 바이트 배열로 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <returns>Input Register들의 Raw 바이트 배열</returns>
        public static byte[] ReadInputRegisterBytes(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length) => ReadInputRegisterBytes(modbusMaster, slaveAddress, address, length, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Coil 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="values">보낼 Coil 값 목록</param>
        public static void WriteCoils(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, IEnumerable<bool> values) => WriteCoils(modbusMaster, slaveAddress, address, values, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Holding Register 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="values">보낼 Holding Register 값 목록</param>
        public static void WriteHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, IEnumerable<ushort> values) => WriteHoldingRegisters(modbusMaster, slaveAddress, address, values, modbusMaster.Timeout);
        /// <summary>
        /// 다중 Holding Register를 Raw 바이트 배열로 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="bytes">보낼 Holding Register 값들의 Raw 바이트 목록</param>
        public static void WriteHoldingRegisterBytes(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, IEnumerable<byte> bytes) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, bytes, modbusMaster.Timeout);

        /// <summary>
        /// 단일 Coil 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <returns>Coil 값</returns>
        public static bool? ReadCoil(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadCoil(modbusMaster, slaveAddress, address, modbusMaster.Timeout);
        /// <summary>
        /// 단일 Discrete Input 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <returns>Discrete Input 값</returns>
        public static bool? ReadDiscreteInput(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadDiscreteInput(modbusMaster, slaveAddress, address, modbusMaster.Timeout);
        /// <summary>
        /// 단일 Holding Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <returns>Holding Register 값</returns>
        public static ushort? ReadHoldingRegister(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadHoldingRegister(modbusMaster, slaveAddress, address, modbusMaster.Timeout);
        /// <summary>
        /// 단일 Input Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <returns>Input Register 값</returns>
        public static ushort? ReadInputRegister(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInputRegister(modbusMaster, slaveAddress, address, modbusMaster.Timeout);
        /// <summary>
        /// 단일 Coil 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">보낼 Coil 값</param>
        public static void WriteCoil(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool value) => WriteCoil(modbusMaster, slaveAddress, address, value, modbusMaster.Timeout);
        /// <summary>
        /// 단일 Holding Register 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">보낼 Holding Register 값</param>
        public static void WriteHoldingRegister(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort value) => WriteHoldingRegister(modbusMaster, slaveAddress, address, value, modbusMaster.Timeout);

        /// <summary>
        /// Input Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        public static short ReadInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian) => ReadInt16FromInputRegisters(modbusMaster, slaveAddress, address, isBigEndian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        public static ushort ReadUInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian) => ReadUInt16FromInputRegisters(modbusMaster, slaveAddress, address, isBigEndian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static int ReadInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadInt32FromInputRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static uint ReadUInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadUInt32FromInputRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static long ReadInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadInt64FromInputRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static ulong ReadUInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadUInt64FromInputRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static float ReadSingleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadSingleFromInputRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static double ReadDoubleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadDoubleFromInputRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);

        /// <summary>
        /// Input Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static short ReadInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInt16FromInputRegisters(modbusMaster, slaveAddress, address, true, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static ushort ReadUInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadUInt16FromInputRegisters(modbusMaster, slaveAddress, address, true, modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static int ReadInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInt32FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static uint ReadUInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadUInt32FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static long ReadInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInt64FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static ulong ReadUInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadUInt64FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static float ReadSingleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadSingleFromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Input Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static double ReadDoubleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadDoubleFromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);




        /// <summary>
        /// Holding Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        public static short ReadInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian) => ReadInt16FromHoldingRegisters(modbusMaster, slaveAddress, address, isBigEndian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        public static ushort ReadUInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian) => ReadUInt16FromHoldingRegisters(modbusMaster, slaveAddress, address, isBigEndian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static int ReadInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadInt32FromHoldingRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static uint ReadUInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadUInt32FromHoldingRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static long ReadInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadInt64FromHoldingRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static ulong ReadUInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadUInt64FromHoldingRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static float ReadSingleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadSingleFromHoldingRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        public static double ReadDoubleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian) => ReadDoubleFromHoldingRegisters(modbusMaster, slaveAddress, address, endian, modbusMaster.Timeout);

        /// <summary>
        /// Holding Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static short ReadInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInt16FromHoldingRegisters(modbusMaster, slaveAddress, address, true, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static ushort ReadUInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadUInt16FromHoldingRegisters(modbusMaster, slaveAddress, address, true, modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static int ReadInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInt32FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static uint ReadUInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadUInt32FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static long ReadInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadInt64FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static ulong ReadUInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadUInt64FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static float ReadSingleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadSingleFromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// Holding Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        public static double ReadDoubleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address) => ReadDoubleFromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), modbusMaster.Timeout);

        /// <summary>
        /// 부호 있는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, short value, bool isBigEndian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, new ModbusEndian(isBigEndian).Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 부호 없는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort value, bool isBigEndian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, new ModbusEndian(isBigEndian).Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 부호 있는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int value, ModbusEndian endian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 부호 없는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, uint value, ModbusEndian endian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 부호 있는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, long value, ModbusEndian endian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 부호 없는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ulong value, ModbusEndian endian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 4바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, float value, ModbusEndian endian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);
        /// <summary>
        /// 8바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, double value, ModbusEndian endian) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), modbusMaster.Timeout);

        /// <summary>
        /// 부호 있는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, short value) => Write(modbusMaster, slaveAddress, address, value, true, modbusMaster.Timeout);
        /// <summary>
        /// 부호 없는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort value) => Write(modbusMaster, slaveAddress, address, value, true, modbusMaster.Timeout);
        /// <summary>
        /// 부호 있는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int value) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// 부호 없는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, uint value) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// 부호 있는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, long value) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// 부호 없는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ulong value) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// 4바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, float value) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), modbusMaster.Timeout);
        /// <summary>
        /// 8바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, double value) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), modbusMaster.Timeout);

        /// <summary>
        /// 다중 Coil 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Coil 값 목록</returns>
        public static bool[] ReadCoils(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.Coil, address, length), timeout) as ModbusReadBooleanResponse)?.Values?.ToArray();
        /// <summary>
        /// 다중 Discrete Input 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Discrete Input 값 목록</returns>
        public static bool[] ReadDiscreteInputs(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.DiscreteInput, address, length), timeout) as ModbusReadBooleanResponse)?.Values?.ToArray();
        /// <summary>
        /// 다중 Holding Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Holding Register 값 목록</returns>
        public static ushort[] ReadHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.HoldingRegister, address, length), timeout) as ModbusReadRegisterResponse)?.Values?.ToArray();
        /// <summary>
        /// 다중 Holding Register를 Raw 바이트 배열로 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Holding Register들의 Raw 바이트 배열</returns>
        public static byte[] ReadHoldingRegisterBytes(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.HoldingRegister, address, length), timeout) as ModbusReadRegisterResponse)?.Bytes?.ToArray();
        /// <summary>
        /// 다중 Input Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Input Register 값 목록</returns>
        public static ushort[] ReadInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.InputRegister, address, length), timeout) as ModbusReadRegisterResponse)?.Values?.ToArray();
        /// <summary>
        /// 다중 Input Register를 Raw 바이트 배열로 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="length">길이</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Input Register들의 Raw 바이트 배열</returns>
        public static byte[] ReadInputRegisterBytes(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort length, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.InputRegister, address, length), timeout) as ModbusReadRegisterResponse)?.Bytes?.ToArray();
        /// <summary>
        /// 다중 Coil 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="values">보낼 Coil 값 목록</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void WriteCoils(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, IEnumerable<bool> values, int timeout) => modbusMaster.Request(new ModbusWriteCoilRequest(slaveAddress, address, values), timeout);
        /// <summary>
        /// 다중 Holding Register 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="values">보낼 Holding Register 값 목록</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void WriteHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, IEnumerable<ushort> values, int timeout) => modbusMaster.Request(new ModbusWriteHoldingRegisterRequest(slaveAddress, address, values), timeout);
        /// <summary>
        /// 다중 Holding Register를 Raw 바이트 배열로 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="bytes">보낼 Holding Register 값들의 Raw 바이트 목록</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void WriteHoldingRegisterBytes(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, IEnumerable<byte> bytes, int timeout) => modbusMaster.Request(new ModbusWriteHoldingRegisterRequest(slaveAddress, address, bytes), timeout);

        /// <summary>
        /// 단일 Coil 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Coil 값</returns>
        public static bool? ReadCoil(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.Coil, address, 1), timeout) as ModbusReadBooleanResponse)?.Values?.FirstOrDefault();
        /// <summary>
        /// 단일 Discrete Input 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Discrete Input 값</returns>
        public static bool? ReadDiscreteInput(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.DiscreteInput, address, 1), timeout) as ModbusReadBooleanResponse)?.Values?.FirstOrDefault();
        /// <summary>
        /// 단일 Holding Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Holding Register 값</returns>
        public static ushort? ReadHoldingRegister(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.HoldingRegister, address, 1), timeout) as ModbusReadRegisterResponse)?.Values?.FirstOrDefault();
        /// <summary>
        /// 단일 Input Register 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>Input Register 값</returns>
        public static ushort? ReadInputRegister(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => (modbusMaster.Request(new ModbusReadRequest(slaveAddress, ModbusObjectType.InputRegister, address, 1), timeout) as ModbusReadRegisterResponse)?.Values?.FirstOrDefault();
        /// <summary>
        /// 단일 Coil 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">보낼 Coil 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void WriteCoil(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool value, int timeout) => modbusMaster.Request(new ModbusWriteCoilRequest(slaveAddress, address, value), timeout);
        /// <summary>
        /// 단일 Holding Register 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">보낼 Holding Register 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void WriteHoldingRegister(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort value, int timeout) => modbusMaster.Request(new ModbusWriteHoldingRegisterRequest(slaveAddress, address, value), timeout);

        /// <summary>
        /// Input Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static short ReadInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian, int timeout) => BitConverter.ToInt16(new ModbusEndian(isBigEndian).Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 1, timeout)), 0);
        /// <summary>
        /// Input Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ushort ReadUInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian, int timeout) => BitConverter.ToUInt16(new ModbusEndian(isBigEndian).Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 1, timeout)), 0);
        /// <summary>
        /// Input Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static int ReadInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout)  => BitConverter.ToInt32(endian.Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 2, timeout)), 0);
        /// <summary>
        /// Input Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static uint ReadUInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToUInt32(endian.Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 2, timeout)), 0);
        /// <summary>
        /// Input Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static long ReadInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToInt64(endian.Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 4, timeout)), 0);
        /// <summary>
        /// Input Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ulong ReadUInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToUInt64(endian.Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 4, timeout)), 0);
        /// <summary>
        /// Input Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static float ReadSingleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToSingle(endian.Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 2, timeout)), 0);
        /// <summary>
        /// Input Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static double ReadDoubleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToDouble(endian.Sort(ReadInputRegisterBytes(modbusMaster, slaveAddress, address, 4, timeout)), 0);

        /// <summary>
        /// Input Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static short ReadInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadInt16FromInputRegisters(modbusMaster, slaveAddress, address, true, timeout);
        /// <summary>
        /// Input Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ushort ReadUInt16FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadUInt16FromInputRegisters(modbusMaster, slaveAddress, address, true, timeout);
        /// <summary>
        /// Input Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static int ReadInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadInt32FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Input Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static uint ReadUInt32FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadUInt32FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Input Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static long ReadInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadInt64FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Input Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ulong ReadUInt64FromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadUInt64FromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Input Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static float ReadSingleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadSingleFromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Input Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static double ReadDoubleFromInputRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadDoubleFromInputRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);

        /// <summary>
        /// Holding Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static short ReadInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian, int timeout) => BitConverter.ToInt16(new ModbusEndian(isBigEndian).Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 1, timeout)), 0);
        /// <summary>
        /// Holding Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ushort ReadUInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, bool isBigEndian, int timeout) => BitConverter.ToUInt16(new ModbusEndian(isBigEndian).Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 1, timeout)), 0);
        /// <summary>
        /// Holding Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static int ReadInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToInt32(endian.Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 2, timeout)), 0);
        /// <summary>
        /// Holding Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static uint ReadUInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToUInt32(endian.Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 2, timeout)), 0);
        /// <summary>
        /// Holding Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static long ReadInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToInt64(endian.Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 4, timeout)), 0);
        /// <summary>
        /// Holding Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ulong ReadUInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToUInt64(endian.Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 4, timeout)), 0);
        /// <summary>
        /// Holding Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static float ReadSingleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToSingle(endian.Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 2, timeout)), 0);
        /// <summary>
        /// Holding Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static double ReadDoubleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ModbusEndian endian, int timeout) => BitConverter.ToDouble(endian.Sort(ReadHoldingRegisterBytes(modbusMaster, slaveAddress, address, 4, timeout)), 0);

        /// <summary>
        /// Holding Register에서 부호 있는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static short ReadInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadInt16FromHoldingRegisters(modbusMaster, slaveAddress, address, true, timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 2바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ushort ReadUInt16FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadUInt16FromHoldingRegisters(modbusMaster, slaveAddress, address, true, timeout);
        /// <summary>
        /// Holding Register에서 부호 있는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static int ReadInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadInt32FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 4바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static uint ReadUInt32FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadUInt32FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Holding Register에서 부호 있는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static long ReadInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadInt64FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Holding Register에서 부호 없는 8바이트 정수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static ulong ReadUInt64FromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadUInt64FromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Holding Register에서 4바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static float ReadSingleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadSingleFromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);
        /// <summary>
        /// Holding Register에서 8바이트 실수 값 읽기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static double ReadDoubleFromHoldingRegisters(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int timeout) => ReadDoubleFromHoldingRegisters(modbusMaster, slaveAddress, address, new ModbusEndian(true), timeout);

        /// <summary>
        /// 부호 있는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, short value, bool isBigEndian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, new ModbusEndian(isBigEndian).Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 부호 없는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="isBigEndian">빅 엔디안 여부</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort value, bool isBigEndian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, new ModbusEndian(isBigEndian).Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 부호 있는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int value, ModbusEndian endian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 부호 없는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, uint value, ModbusEndian endian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 부호 있는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, long value, ModbusEndian endian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 부호 없는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ulong value, ModbusEndian endian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 4바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, float value, ModbusEndian endian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), timeout);
        /// <summary>
        /// 8바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="endian">엔디안</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, double value, ModbusEndian endian, int timeout) => WriteHoldingRegisterBytes(modbusMaster, slaveAddress, address, endian.Sort(BitConverter.GetBytes(value)), timeout);

        /// <summary>
        /// 부호 있는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, short value, int timeout) => Write(modbusMaster, slaveAddress, address, value, true, timeout);
        /// <summary>
        /// 부호 없는 2바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ushort value, int timeout) => Write(modbusMaster, slaveAddress, address, value, true, timeout);
        /// <summary>
        /// 부호 있는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, int value, int timeout) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), timeout);
        /// <summary>
        /// 부호 없는 4바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, uint value, int timeout) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), timeout);
        /// <summary>
        /// 부호 있는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, long value, int timeout) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), timeout);
        /// <summary>
        /// 부호 없는 8바이트 정수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, ulong value, int timeout) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), timeout);
        /// <summary>
        /// 4바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, float value, int timeout) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), timeout);
        /// <summary>
        /// 8바이트 실수 값 쓰기
        /// </summary>
        /// <param name="modbusMaster">Modbus 마스터</param>
        /// <param name="slaveAddress">슬레이브 주소</param>
        /// <param name="address">데이터 주소</param>
        /// <param name="value">쓸 값</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        public static void Write(this ModbusMaster modbusMaster, byte slaveAddress, ushort address, double value, int timeout) => Write(modbusMaster, slaveAddress, address, value, new ModbusEndian(true), timeout);
    }
}
