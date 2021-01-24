using System;
using System.Collections.Generic;
using System.Text;

namespace VagabondK.Modbus.Data
{
    public interface IModbusDataBlock<TData, TRawData> : IEnumerable<TData>
    {
        ushort StartAddress { get; }
        ushort EndAddress { get; }
        ushort Count { get; }
        IReadOnlyList<TRawData> RawData { get; }
        int NumberOfUnit { get; }
        TData this[ushort address] { get; }
    }
}
