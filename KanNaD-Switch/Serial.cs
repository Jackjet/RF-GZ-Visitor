using Common.Log;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonNaDSwitch
{
    /// <summary>
    /// 康奈德485继电器
    /// </summary>
    public class Serial
    {
        private const int baudRate = 9600;
        private SerialPort _serialPort = null;

        public bool Open(string portName)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
                _serialPort.Open();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Info("继电器串口打开失败->" + ex.Message);
                return false;
            }
        }

        public void Write(byte[] data)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Write(data, 0, data.Length);
            }
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}
