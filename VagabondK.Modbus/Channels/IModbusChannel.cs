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
    /// Modbus 채널 인터페이스
    /// </summary>
    public interface IModbusChannel : IDisposable
    {
        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Modbus Logger
        /// </summary>
        IModbusLogger Logger { get; set; }

        /// <summary>
        /// 채널 설명
        /// </summary>
        string Description { get; }
    }
}
