using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VagabondK.Modbus.Logging;
using VagabondK.Modbus.Serialization;

namespace VagabondK.Modbus.Channels
{
    /// <summary>
    /// Modbus 채널
    /// </summary>
    public abstract class ModbusChannel : IModbusChannel
    {
        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public abstract bool IsDisposed { get; protected set; }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 바이트 배열 쓰기
        /// </summary>
        /// <param name="bytes">바이트 배열</param>
        public abstract void Write(byte[] bytes);

        /// <summary>
        /// 1 바이트 읽기
        /// </summary>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>읽은 바이트</returns>
        public abstract byte Read(int timeout);

        /// <summary>
        /// 여러 개의 바이트 읽기
        /// </summary>
        /// <param name="count">읽을 개수</param>
        /// <param name="timeout">제한시간(밀리초)</param>
        /// <returns>읽은 바이트 열거</returns>
        public abstract IEnumerable<byte> Read(uint count, int timeout);

        /// <summary>
        /// 채널에 남아있는 모든 바이트 읽기
        /// </summary>
        /// <returns>읽은 바이트 열거</returns>
        public abstract IEnumerable<byte> ReadAllRemain();

        /// <summary>
        /// Modbus Logger
        /// </summary>
        public IModbusLogger Logger { get; set; }

        /// <summary>
        /// 채널 설명
        /// </summary>
        public abstract string Description { get; protected set; }
    }
}
