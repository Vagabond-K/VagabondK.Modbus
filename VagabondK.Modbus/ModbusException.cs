using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VagabondK.Modbus
{
    /// <summary>
    /// Modbus Exception
    /// </summary>
    public class ModbusException : Exception
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="code">Modbus Exception 코드</param>
        public ModbusException(ModbusExceptionCode code)
        {
            Code = code;
        }

        /// <summary>
        /// Modbus Exception 코드
        /// </summary>
        public ModbusExceptionCode Code { get; }

        /// <summary>
        /// 예외 메시지
        /// </summary>
        public override string Message
        {
            get
            {
                var codeName = Code.ToString();
                return (typeof(ModbusExceptionCode).GetMember(codeName, BindingFlags.Static | BindingFlags.Public)
                    ?.FirstOrDefault()?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description ?? codeName;
            }
        }
    }
}
