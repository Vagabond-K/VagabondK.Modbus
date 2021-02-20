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
    /// Modbus 채널 공급자
    /// </summary>
    public abstract class ModbusChannelProvider : IModbusChannel
    {
        /// <summary>
        /// 채널 생성 이벤트
        /// </summary>
        public event EventHandler<ModbusChannelCreatedEventArgs> Created;

        /// <summary>
        /// 생성된 채널 목록
        /// </summary>
        public abstract IReadOnlyList<ModbusChannel> Channels { get; }

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Modbus Logger
        /// </summary>
        public IModbusLogger Logger { get; set; }

        /// <summary>
        /// 채널 공급자 설명
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// 채널 생성 시작
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 채널 생성 정지
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 채널 생성 이벤트 호출
        /// </summary>
        /// <param name="eventArgs">이벤트 매개변수</param>
        protected void RaiseCreatedEvent(ModbusChannelCreatedEventArgs eventArgs) => Created?.Invoke(this, eventArgs);
    }

    /// <summary>
    /// Modbus 채널 생성 이벤트 매개변수
    /// </summary>
    public class ModbusChannelCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="channel">Modbus 채널</param>
        public ModbusChannelCreatedEventArgs(ModbusChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Modbus 채널
        /// </summary>
        public ModbusChannel Channel { get; }
    }
}
