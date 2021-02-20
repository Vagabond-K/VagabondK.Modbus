using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// 통신 오류 Log
    /// </summary>
    public class CommErrorLog : ModbusLog
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="channel">Modbus 채널</param>
        /// <param name="exception">통신 오류 예외</param>
        public CommErrorLog(IModbusChannel channel, Exception exception) : base(channel)
        {
            Exception = exception;
        }

        /// <summary>
        /// 통신 오류 예외
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Modbus Log 카테고리
        /// </summary>
        public override ModbusLogCategory Category { get => ModbusLogCategory.CommError; }

        /// <summary>
        /// 이 인스턴스의 정규화된 형식 이름을 반환합니다.
        /// </summary>
        /// <returns>정규화된 형식 이름입니다.</returns>
        public override string ToString()
            => $"({ChannelDescription}) Comm Error: {Exception?.Message ?? base.ToString()}";
    }
}
