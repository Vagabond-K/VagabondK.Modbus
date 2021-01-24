using System;
using System.Collections.Generic;
using System.Text;

namespace VagabondK.Modbus.Logging
{
    public enum ModbusLogCategory : int
    {
        None = 0,
        All = 0b_0011_0111_1111_1111_1111_1111,

        ModbusMessage = 0b_0000_0000_1111_1111_1111_1111,
        Response = 0b_0000_0000_0000_0000_1111_1111,
        ResponseRead = 0b_0000_0000_0000_0000_0000_1111,
        ResponseWrite = 0b_0000_0000_0000_0000_1111_0000,
        Request = 0b_0000_0000_1111_1111_0000_0000,
        RequestRead = 0b_0000_0000_0000_1111_0000_0000,
        RequestWrite = 0b_0000_0000_1111_0000_0000_0000,
        Error = 0b_0000_0111_0000_0000_0000_0000,
        ChannelEvent = 0b_0011_0000_0000_0000_0000_0000,


        ResponseReadCoil = 0b_0000_0000_0000_0000_0000_0001,
        ResponseReadDiscreteInput = 0b_0000_0000_0000_0000_0000_0010,
        ResponseReadInputRegister = 0b_0000_0000_0000_0000_0000_0100,
        ResponseReadHoldingRegister = 0b_0000_0000_0000_0000_0000_1000,
        ResponseWriteCoil = 0b_0000_0000_0000_0000_1100_0000,
        ResponseWriteSingleCoil = 0b_0000_0000_0000_0000_1000_0000,
        ResponseWriteMultiCoil = 0b_0000_0000_0000_0000_0100_0000,
        ResponseWriteHoldingRegister = 0b_0000_0000_0000_0000_0011_0000,
        ResponseWriteSingleHoldingRegister = 0b_0000_0000_0000_0000_0010_0000,
        ResponseWriteMultiHoldingRegister = 0b_0000_0000_0000_0000_0001_0000,

        RequestReadCoil = 0b_0000_0000_0000_0001_0000_0000,
        RequestReadDiscreteInput = 0b_0000_0000_0000_0010_0000_0000,
        RequestReadInputRegister = 0b_0000_0000_0000_0100_0000_0000,
        RequestReadHoldingRegister = 0b_0000_0000_0000_1000_0000_0000,
        RequestWriteCoil = 0b_0000_0000_1100_0000_0000_0000,
        RequestWriteSingleCoil = 0b_0000_0000_1000_0000_0000_0000,
        RequestWriteMultiCoil = 0b_0000_0000_0100_0000_0000_0000,
        RequestWriteHoldingRegister = 0b_0000_0000_0011_0000_0000_0000,
        RequestWriteSingleHoldingRegister = 0b_0000_0000_0010_0000_0000_0000,
        RequestWriteMultiHoldingRegister = 0b_0000_0000_0001_0000_0000_0000,

        CommError = 0b_0000_0100_0000_0000_0000_0000,
        UnrecognizedError = 0b_0000_0010_0000_0000_0000_0000,
        ResponseException = 0b_0000_0001_0000_0000_0000_0000,

        ChannelOpenEvent = 0b_0001_0000_0000_0000_0000_0000,
        ChannelCloseEvent = 0b_0010_0000_0000_0000_0000_0000,
    }
}
