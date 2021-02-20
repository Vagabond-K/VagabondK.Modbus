using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// 인식할 수 없는 메시지 수신 오류 Log
    /// </summary>
    public class UnrecognizedErrorLog : ModbusLog
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="channel">Modbus 채널</param>
        /// <param name="rawMessage">원본 메시지</param>
        public UnrecognizedErrorLog(IModbusChannel channel, byte[] rawMessage) : base(channel)
        {
            RawMessage = rawMessage;
        }

        /// <summary>
        /// 원본 메시지
        /// </summary>
        public IReadOnlyList<byte> RawMessage { get; }

        /// <summary>
        /// Modbus Log 카테고리
        /// </summary>
        public override ModbusLogCategory Category { get => ModbusLogCategory.UnrecognizedError; }

        /// <summary>
        /// 이 인스턴스의 정규화된 형식 이름을 반환합니다.
        /// </summary>
        /// <returns>정규화된 형식 이름입니다.</returns>
        public override string ToString()
            => RawMessage == null && RawMessage.Count > 0 ? base.ToString() : $"Unrecognized: {BitConverter.ToString(RawMessage as byte[])}";
    }
}
