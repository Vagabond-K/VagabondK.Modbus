using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// 채널 열림 이벤트 Log
    /// </summary>
    public class ChannelOpenEventLog : ModbusLog
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="channel">Modbus 채널</param>
        public ChannelOpenEventLog(IModbusChannel channel) : base(channel) { }

        /// <summary>
        /// Modbus Log 카테고리
        /// </summary>
        public override ModbusLogCategory Category { get => ModbusLogCategory.ChannelOpenEvent; }

        /// <summary>
        /// 이 인스턴스의 정규화된 형식 이름을 반환합니다.
        /// </summary>
        /// <returns>정규화된 형식 이름입니다.</returns>
        public override string ToString()
            => $"({ChannelDescription}) Opened Channel";
    }
}
