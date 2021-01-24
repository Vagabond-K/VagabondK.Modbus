using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VagabondK.Modbus.Data
{
    public class ModbusBooleans : ModbusDataSet<bool, bool>
    {
        public override IEnumerator<KeyValuePair<ushort, bool>> GetEnumerator()
        {
            foreach (ModbusBooleanDataBlock dataBlock in DataBlocks)
            {
                ushort address = dataBlock.StartAddress;
                foreach (var value in dataBlock)
                    yield return new KeyValuePair<ushort, bool>(address++, value);
            }
        }

        public void Allocate(ushort startAddress, bool[] data)
        {
            AllocateCore(new ModbusBooleanDataBlock(startAddress, data));
        }

        internal override ModbusDataBlock<bool, bool> CreateDataBlock(ushort startAddress, bool[] values)
            => new ModbusBooleanDataBlock(startAddress, values);

        class ModbusBooleanDataBlock : ModbusDataBlock<bool, bool>
        {
            public ModbusBooleanDataBlock(ushort startAddress, bool[] values)
            {
                this.startAddress = startAddress;
                rawData = values;
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
                            rawData = Enumerable.Repeat(false, startAddress - value).Concat(rawData).ToArray();
                        else
                            rawData = rawData.Skip(value - startAddress).ToArray();

                        startAddress = value;
                    }
                }
            }
            public override ushort EndAddress { get => (ushort)(StartAddress + Count - 1); set => Array.Resize(ref rawData, Math.Max(value - StartAddress + 1, 0)); }
            public override ushort Count { get => (ushort)rawData.Length; }
            public override int NumberOfUnit { get => 1; }

            public override bool this[ushort address]
            {
                get
                {
                    if (address >= StartAddress && address <= EndAddress)
                    {
                        return rawData[(address - StartAddress) * NumberOfUnit];
                    }
                    else
                    {
                        throw new ModbusException(ModbusExceptionCode.IllegalDataAddress);
                    }
                }
                set
                {
                    rawData[(address - StartAddress) * NumberOfUnit] = value;
                }
            }

            public override IEnumerator<bool> GetEnumerator()
            {
                foreach (var value in rawData)
                    yield return value;
            }
        }
    }
}
