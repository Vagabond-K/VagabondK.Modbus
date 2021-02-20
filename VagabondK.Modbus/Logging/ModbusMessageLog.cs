using System;
using System.Collections.Generic;
using System.Text;
using VagabondK.Modbus.Channels;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// Modbus 메시지 Log
    /// </summary>
    public class ModbusMessageLog : ModbusLog
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="channel">Modbus 채널</param>
        /// <param name="message">Modbus 메시지</param>
        /// <param name="rawMessage">원본 메시지</param>
        public ModbusMessageLog(IModbusChannel channel, IModbusMessage message, byte[] rawMessage) : base(channel)
        {
            Message = message;
            RawMessage = rawMessage ?? new byte[0];
        }

        /// <summary>
        /// Modbus 메시지
        /// </summary>
        public IModbusMessage Message { get; }
        /// <summary>
        /// 원본 메시지
        /// </summary>
        public byte[] RawMessage { get; }
        /// <summary>
        /// Modbus Log 카테고리
        /// </summary>
        public override ModbusLogCategory Category { get => Message.LogCategory; }

        /// <summary>
        /// 이 인스턴스의 정규화된 형식 이름을 반환합니다.
        /// </summary>
        /// <returns>정규화된 형식 이름입니다.</returns>
        public override string ToString()
        {
            if (Message is ModbusRequest)
                return $"({ChannelDescription}) Request: {BitConverter.ToString(RawMessage)}";
            else if (Message is ModbusResponse)
                return $"({ChannelDescription}) Response: {BitConverter.ToString(RawMessage)}";
            else
                return base.ToString();

        }
    }
}
