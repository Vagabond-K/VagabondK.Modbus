using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VagabondK.Modbus.Data
{
    public class ModbusRegisters : ModbusDataSet<ushort, byte>
    {
        public override IEnumerator<KeyValuePair<ushort, ushort>> GetEnumerator()
        {
            foreach (ModbusRegisterDataBlock dataBlock in DataBlocks)
            {
                ushort address = dataBlock.StartAddress;
                foreach (var value in dataBlock)
                    yield return new KeyValuePair<ushort, ushort>(address++, value);
            }
        }

        public void Allocate(ushort startAddress, byte[] bytes)
        {
            AllocateCore(new ModbusRegisterDataBlock(startAddress, bytes));
        }
        public void Allocate(ushort startAddress, ushort[] values)
        {
            AllocateCore(new ModbusRegisterDataBlock(startAddress, values));
        }

        public IEnumerable<byte> GetRawData(ushort address, int rawDataCount)
        {
            return GetRawDataCore(address, rawDataCount);
        }

        public void SetRawData(ushort startAddress, byte[] bytes)
        {
            SetDataBlock(new ModbusRegisterDataBlock(startAddress, bytes));
        }

        public void Allocate(ushort address, short value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, ushort value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, int value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, uint value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, long value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, ulong value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, float value) => Allocate(address, value, new ModbusEndian(true));
        public void Allocate(ushort address, double value) => Allocate(address, value, new ModbusEndian(true));

        public short GetInt16(ushort address) => GetInt16(address, new ModbusEndian(true));
        public ushort GetUInt16(ushort address) => GetUInt16(address, new ModbusEndian(true));
        public int GetInt32(ushort address) => GetInt32(address, new ModbusEndian(true));
        public uint GetUInt32(ushort address) => GetUInt32(address, new ModbusEndian(true));
        public long GetInt64(ushort address) => GetInt64(address, new ModbusEndian(true));
        public ulong GetUInt64(ushort address) => GetUInt64(address, new ModbusEndian(true));
        public float GetSingle(ushort address) => GetSingle(address, new ModbusEndian(true));
        public double GetDouble(ushort address) => GetDouble(address, new ModbusEndian(true));

        public void SetValue(ushort address, short value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, ushort value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, int value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, uint value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, long value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, ulong value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, float value) => SetValue(address, value, new ModbusEndian(true));
        public void SetValue(ushort address, double value) => SetValue(address, value, new ModbusEndian(true));

        public void Allocate(ushort address, short value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, ushort value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, int value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, uint value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, long value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, ulong value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, float value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));
        public void Allocate(ushort address, double value, ModbusEndian endian) => Allocate(address, endian.Sort(BitConverter.GetBytes(value)));

        public short GetInt16(ushort address, ModbusEndian endian) => BitConverter.ToInt16(endian.Sort(GetRawData(address, 2).ToArray()), 0);
        public ushort GetUInt16(ushort address, ModbusEndian endian) => BitConverter.ToUInt16(endian.Sort(GetRawData(address, 2).ToArray()), 0);
        public int GetInt32(ushort address, ModbusEndian endian) => BitConverter.ToInt32(endian.Sort(GetRawData(address, 4).ToArray()), 0);
        public uint GetUInt32(ushort address, ModbusEndian endian) => BitConverter.ToUInt32(endian.Sort(GetRawData(address, 4).ToArray()), 0);
        public long GetInt64(ushort address, ModbusEndian endian) => BitConverter.ToInt64(endian.Sort(GetRawData(address, 8).ToArray()), 0);
        public ulong GetUInt64(ushort address, ModbusEndian endian) => BitConverter.ToUInt64(endian.Sort(GetRawData(address, 8).ToArray()), 0);
        public float GetSingle(ushort address, ModbusEndian endian) => BitConverter.ToSingle(endian.Sort(GetRawData(address, 4).ToArray()), 0);
        public double GetDouble(ushort address, ModbusEndian endian) => BitConverter.ToDouble(endian.Sort(GetRawData(address, 8).ToArray()), 0);

        public void SetValue(ushort address, short value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, ushort value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, int value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, uint value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, long value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, ulong value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, float value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));
        public void SetValue(ushort address, double value, ModbusEndian endian) => SetRawData(address, endian.Sort(BitConverter.GetBytes(value)));

        internal override ModbusDataBlock<ushort, byte> CreateDataBlock(ushort startAddress, ushort[] values)
            => new ModbusRegisterDataBlock(startAddress, values);


        class ModbusRegisterDataBlock : ModbusDataBlock<ushort, byte>
        {
            public ModbusRegisterDataBlock(ushort startAddress, byte[] bytes)
            {
                this.startAddress = startAddress;
                rawData = bytes;
            }

            public ModbusRegisterDataBlock(ushort startAddress, ushort[] values)
            {
                this.startAddress = startAddress;
                rawData = values.SelectMany(value => new[] { (byte)(value >> 8), (byte)(value & 0xff) }).ToArray();
            }

            private ushort startAddress = 0;

            public override ushort StartAddress
            {
                get => startAddress;
                set
                {
                    if (value > EndAddress)
                        value = EndAddress;

                    if (startAddress != value)
                    {
                        if (startAddress > value)
                            rawData = Enumerable.Repeat((byte)0, (startAddress - value) * NumberOfUnit).Concat(rawData).ToArray();
                        else
                            rawData = rawData.Skip((value - startAddress) * NumberOfUnit).ToArray();

                        startAddress = value;
                    }
                }
            }
            public override ushort EndAddress { get => (ushort)(StartAddress + Count - 1); set => Array.Resize(ref rawData, Math.Max(value - StartAddress + 1, 0) * NumberOfUnit); }
            public override ushort Count { get => (ushort)Math.Ceiling((double)rawData.Length / NumberOfUnit); }
            public override int NumberOfUnit { get => 2; }

            public override ushort this[ushort address]
            {
                get
                {
                    if (address >= StartAddress && address <= EndAddress)
                    {
                        var index = (address - StartAddress) * NumberOfUnit;
                        if (index + 1 < rawData.Length)
                            return (ushort)((rawData[index] << 8) | rawData[index + 1]);
                        else
                            return (ushort)(rawData[index] << 8);
                    }
                    else
                    {
                        throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);
                    }
                }
                set
                {
                    var index = (address - StartAddress) * NumberOfUnit;
                    rawData[index] = (byte)(value >> 8);
                    if (index + 1 >= rawData.Length)
                        Array.Resize(ref rawData, rawData.Length + 1);
                    rawData[index + 1] = (byte)(value & 0xff);
                }
            }

            public override IEnumerator<ushort> GetEnumerator()
            {
                int count = Count;
                for (int i = 0; i < count; i++)
                {
                    if (i * 2 + 1 < rawData.Length)
                    {
                        yield return (ushort)((rawData[i * 2] << 8) | rawData[i * 2 + 1]);
                    }
                    else
                    {
                        yield return (ushort)(rawData[i * 2] << 8);
                    }
                }
            }
        }
    }
}
