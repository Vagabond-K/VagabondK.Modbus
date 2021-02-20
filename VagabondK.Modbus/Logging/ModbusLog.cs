using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// Modbus Log
    /// </summary>
    public abstract class ModbusLog
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="channel">Modbus 채널</param>
        protected ModbusLog(IModbusChannel channel)
        {
            TimeStamp = DateTime.Now;
            ChannelDescription = channel?.Description;
        }

        /// <summary>
        /// 타임 스탬프
        /// </summary>
        public DateTime TimeStamp { get; }

        /// <summary>
        /// 채널 설명
        /// </summary>
        public string ChannelDescription { get; }

        /// <summary>
        /// Modbus Log 카테고리
        /// </summary>
        public abstract ModbusLogCategory Category { get; }
    }
}
