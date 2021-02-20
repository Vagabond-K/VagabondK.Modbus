using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VagabondK.Modbus.Logging
{
    /// <summary>
    /// 컬렉션 기반 Modgus Logger
    /// </summary>
    public class CollectionModbusLogger : IModbusLogger
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection">Modbus Log 컬렉션</param>
        public CollectionModbusLogger(ICollection<ModbusLog> collection)
        {
            Collection = collection;
        }

        /// <summary>
        /// Modbus Log 카테고리 필터
        /// </summary>
        public ModbusLogCategory CategoryFilter { get; set; }

        /// <summary>
        /// Modbus Log 컬렉션
        /// </summary>
        public ICollection<ModbusLog> Collection { get; set; }

        /// <summary>
        /// Log 기록
        /// </summary>
        /// <param name="log">Modbus Log</param>
        public void Log(ModbusLog log)
        {
            if ((CategoryFilter & log.Category) != 0)
                Collection?.Add(log);
        }
    }
}
