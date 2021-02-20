using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// 스트림 기반 Modgus Logger
    /// </summary>
    public class StreamModbusLogger : IModbusLogger, IDisposable
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="stream">스트림</param>
        /// <param name="encoding">인코딩</param>
        public StreamModbusLogger(Stream stream, Encoding encoding = null)
        {
            InnerStream = stream ?? throw new ArgumentNullException(nameof(stream));
            streamWriter = new StreamWriter(InnerStream, encoding ?? Encoding.UTF8);
        }

        private readonly StreamWriter streamWriter;

        /// <summary>
        /// 내부 스트림
        /// </summary>
        public Stream InnerStream { get; }

        /// <summary>
        /// Modbus Log 카테고리 필터
        /// </summary>
        public ModbusLogCategory CategoryFilter { get; set; } = ModbusLogCategory.All;

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Dispose()
        {
            streamWriter.Dispose();
        }

        /// <summary>
        /// Log 기록
        /// </summary>
        /// <param name="log">Modbus Log</param>
        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) != 0)
                WriteToStream(streamWriter, log);
        }

        /// <summary>
        /// 스트림에 로그 쓰기
        /// </summary>
        /// <param name="writer">StreamWriter</param>
        /// <param name="log">Modbus Log</param>
        protected virtual void WriteToStream(StreamWriter writer, ModbusLog log)
        {
            writer.WriteLine(log);
        }
    }
}
