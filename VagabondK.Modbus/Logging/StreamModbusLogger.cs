using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Logging
{
    public class StreamModbusLogger : IModbusLogger, IDisposable
    {
        public StreamModbusLogger(Stream stream, Encoding encoding = null)
        {
            InnerStream = stream ?? throw new ArgumentNullException(nameof(stream));
            streamWriter = new StreamWriter(InnerStream, encoding ?? Encoding.UTF8);
        }

        private StreamWriter streamWriter;

        public Stream InnerStream { get; }
        public ModbusLogCategory CategoryFilter { get; set; } = ModbusLogCategory.All;

        public void Dispose()
        {
            streamWriter.Dispose();
        }

        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) == 0) return;

            if (log is ModbusMessageLog messageLog
            && messageLog.RawMessage != null && messageLog.RawMessage.Length > 0)
            {
                if (messageLog.Message is ModbusRequest)
                    streamWriter.WriteLine($"Request: {BitConverter.ToString(messageLog.RawMessage)}");
                else if (messageLog.Message is ModbusResponse)
                    streamWriter.WriteLine($"Response: {BitConverter.ToString(messageLog.RawMessage)}");
                else
                    streamWriter.WriteLine(log);
            }
            else
                streamWriter.WriteLine(log);
        }
    }
}
