using System;
using Common.Log;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RF_Visitor.Core
{
    /// <summary>
    /// 微光互联串口二维码阅读器
    /// </summary>
    public class SerialQRCodeReader : IDisposable
    {
        private bool _stop = false;
        private SerialPort _serialPort = null;
        private List<char> _barcodeList = new List<char>();
        private Action<string> _callback;

        private const int baudRate = 9600;

        public bool Open(string portName)
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
                _serialPort.Open();

                ThreadPool.QueueUserWorkItem(ReadComm);
                return true;
            }
            catch (Exception ex)
            {
                Log("二维码串口打开失败->" + ex.Message);
                return false;
            }
        }

        public void SetQRCodeCallback(Action<string> callback)
        {
            _callback = callback;
        }

        public void ReadComm(object obj)
        {
            while (!_stop)
            {
                byte b = 0;
                try
                {
                    while ((b = (byte)_serialPort.ReadByte()) > 0)
                    {
                        if (b == 13)
                        {
                            var barcode = new string(_barcodeList.ToArray());
                            _callback?.Invoke(barcode);
                            _barcodeList.Clear();
                        }
                        else
                        {
                            _barcodeList.Add((char)b);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("关闭串口");
                }
            }
        }

        private void Log(string log, params object[] p)
        {
            LogHelper.Info(string.Format(log, p));
        }

        public void Dispose()
        {
            _stop = true;

            if (_serialPort != null)
                _serialPort.Close();
        }
    }
}
