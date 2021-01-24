using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace VagabondK.Modbus.Data
{
    abstract class ModbusDataBlock<TData, TRawData> : IModbusDataBlock<TData, TRawData>
    {
        internal TRawData[] rawData;

        public abstract TData this[ushort address] { get; set; }

        public abstract ushort StartAddress { get; set; }
        public abstract ushort EndAddress { get; set; }
        public abstract ushort Count { get; }
        public IReadOnlyList<TRawData> RawData { get => rawData; }
        public abstract int NumberOfUnit { get; }

        public abstract IEnumerator<TData> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
