using System;
using System.Collections.Generic;
using System.Text;

namespace VagabondK.Modbus.Data
{
    public struct ModbusEndian
    {
        public ModbusEndian(bool isBigEndian)
        {
            InnerBigEndian = isBigEndian;
            OuterBigEndian = isBigEndian;
        }

        public ModbusEndian(bool innerBigEndian, bool outerBigEndian)
        {
            InnerBigEndian = innerBigEndian;
            OuterBigEndian = outerBigEndian;
        }

        public bool InnerBigEndian { get; }
        public bool OuterBigEndian { get; }

        public override string ToString()
        {
            if (OuterBigEndian)
            {
                if (InnerBigEndian) return "BADC";
                else return "DCBA";
            }
            else
            {
                if (InnerBigEndian) return "ABCD";
                else return "CDAB";
            }
        }

        public static readonly ModbusEndian AllBig = new ModbusEndian(true, true);

        public ModbusEndian Reverse() => new ModbusEndian(!InnerBigEndian, !OuterBigEndian);

        public byte[] Sort(byte[] bytes)
        {
            if (bytes.Length % 2 == 1)
                Array.Resize(ref bytes, bytes.Length / 2 * 2);

            var count = bytes.Length / 2;
            byte temp;

            if (OuterBigEndian == BitConverter.IsLittleEndian)
            {
                if (InnerBigEndian == BitConverter.IsLittleEndian)
                {
                    for (int i = 0; i < count; i++)
                    {
                        temp = bytes[i];
                        bytes[i] = bytes[bytes.Length - 1 - i];
                        bytes[bytes.Length - 1 - i] = temp;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        temp = bytes[i];
                        if (i % 2 == 0)
                        {
                            bytes[i] = bytes[bytes.Length - 2 - i];
                            bytes[bytes.Length - 2 - i] = temp;
                        }
                        else
                        {
                            bytes[i] = bytes[bytes.Length - i];
                            bytes[bytes.Length - i] = temp;
                        }
                    }
                }
            }
            else if (InnerBigEndian == BitConverter.IsLittleEndian)
            {
                for (int i = 0; i < count; i++)
                {
                    temp = bytes[i * 2];
                    bytes[i * 2] = bytes[i * 2 + 1];
                    bytes[i * 2 + 1] = temp;
                }
            }

            return bytes;
        }
    }
}
