using System;
using System.Collections.Generic;
#if NETSTANDARD2_0
using RJCP.IO.Ports;
#else
using System.IO.Ports;
#endif
using System.Threading;
using System.Threading.Tasks;
using VagabondK.Modbus.Channels;
using VagabondK.Modbus.Logging;

namespace VagabondK.Modbus.SerialPortChannel
{
    public class SerialPortModbusChannel : ModbusChannel
    {
#if NETSTANDARD2_0
        public SerialPortModbusChannel(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
        {
            Description = portName;
            SerialPort = new SerialPortStream(portName, baudRate, dataBits, parity, stopBits);
        }
#else
        public SerialPortModbusChannel(string portName, int baudRate, int dataBits, StopBits stopBits, Parity parity)
        {
            Description = portName;
            SerialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        }
#endif

        private readonly object openLock = new object();
        private readonly object writeLock = new object();
        private readonly object readLock = new object();
        private readonly Queue<byte> readBuffer = new Queue<byte>();
        private readonly EventWaitHandle readEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        private bool isRunningReceive = false;

#if NETSTANDARD2_0
        public SerialPortStream SerialPort { get; }
#else
        public SerialPort SerialPort { get; }
#endif

        public bool DtrEnable { get => SerialPort.DtrEnable; set => SerialPort.DtrEnable = value; }
        public bool RtsEnable { get => SerialPort.RtsEnable; set => SerialPort.RtsEnable = value; }

        public override bool IsDisposed { get; protected set; }

        public override string Description { get; protected set; }

        ~SerialPortModbusChannel()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                Close();
            }
        }

        private void Close()
        {
            lock (openLock)
            {
                Logger?.Log(new ChannelCloseEventLog(this));
                SerialPort?.Dispose();
            }
        }

        private void CheckPort()
        {
            lock (openLock)
            {
                if (!IsDisposed)
                {
                    try
                    {
                        SerialPort.Open();
                        ReadAllRemain();
                        Logger?.Log(new ChannelOpenEventLog(this));
                    }
                    catch (Exception ex)
                    {
                        Logger?.Log(new CommErrorLog(this, ex));
                        throw ex;
                    }
                }
            }
        }

        private byte? GetByte(int timeout)
        {
            lock (readBuffer)
            {
                if (readBuffer.Count == 0)
                {
                    readEventWaitHandle.Reset();

                    Task.Run(() =>
                    {
                        if (!isRunningReceive)
                        {
                            isRunningReceive = true;
                            try
                            {
                                CheckPort();
                                if (SerialPort.IsOpen)
                                {
                                    byte[] buffer = new byte[8192];
                                    while (true)
                                    {
                                        int received = SerialPort.Read(buffer, 0, buffer.Length);
                                        lock (readBuffer)
                                        {
                                            for (int i = 0; i < received; i++)
                                                readBuffer.Enqueue(buffer[i]);
                                            readEventWaitHandle.Set();
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Close();
                            }
                            readEventWaitHandle.Set();
                            isRunningReceive = false;
                        }
                    });
                }
                else return readBuffer.Dequeue();
            }

            if (timeout == 0 ? readEventWaitHandle.WaitOne() : readEventWaitHandle.WaitOne(timeout))
                return readBuffer.Count > 0 ? readBuffer.Dequeue() : (byte?)null;
            else
                return null;
        }

        public override void Write(byte[] bytes)
        {
            CheckPort();
            lock (writeLock)
            {
                try
                {
                    if (SerialPort.IsOpen)
                    {
                        SerialPort.Write(bytes, 0, bytes.Length);
#if NETSTANDARD2_0
                        SerialPort.Flush();
#endif
                    }
                }
                catch
                {
                    Close();
                    throw new TimeoutException();
                }
            }
        }

        public override byte Read(int timeout)
        {
            lock (readLock)
            {
                return GetByte(timeout) ?? throw new TimeoutException();
            }
        }

        public override IEnumerable<byte> Read(uint count, int timeout)
        {
            lock (readLock)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return GetByte(timeout) ?? throw new TimeoutException();
                }
            }
        }

        public override IEnumerable<byte> ReadAllRemain()
        {
            lock (readLock)
            {
                while (readBuffer.Count > 0)
                    yield return readBuffer.Dequeue();

                if (!SerialPort.IsOpen)
                    yield break;

                try
                {
                    SerialPort.DiscardInBuffer();
                }
                catch { }
            }
        }

    }
}
