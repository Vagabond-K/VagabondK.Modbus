using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VagabondK.Modbus
{
    public class ModbusException : Exception
    {
        public ModbusException(ModbusExceptionCode code)
        {
            Code = code;
        }

        public ModbusExceptionCode Code { get; }

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
