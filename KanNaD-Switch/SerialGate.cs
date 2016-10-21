using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KonNaDSwitch
{
    public class SerialGate : IGate
    {
        private Serial _serial = null;

        public void Connect(string ip)
        {
            _serial = new Serial();
            var open = _serial.Open(ip);
            if (open)
            {
                LogHelper.Info("485继电器打开->" + ip);
            }
        }

        public void Disconnect()
        {
            _serial.Close();
        }

        public void OpenIn(int delay)
        {
            var data = SerialData(1);
            _serial.Write(data);

            Thread.Sleep(delay);
            data = SerialData(0);
            _serial.Write(data);
        }

        public void OpenOut(int delay)
        {
            var data = SerialData(2);
            _serial.Write(data);

            Thread.Sleep(delay);
            data = SerialData(0);
            _serial.Write(data);
        }

        private byte[] SerialData(byte data)
        {
            List<byte> list = new List<byte>();
            //地址
            list.Add(01);
            //功能码
            list.Add(0x0F);
            //起始地址
            list.Add(00);
            list.Add(0x64);
            //寄存器个数
            list.Add(00);
            list.Add(02);
            //数据长度
            list.Add(01);
            //数据
            list.Add(data);

            list.Add(0);
            list.Add(0);

            byte[] crc = new byte[2];
            GetCRC(list.ToArray(), ref crc);

            list[list.Count - 2] = crc[0];
            list[list.Count - 1] = crc[1];

            var buffer = list.ToArray();
            return buffer;
        }

        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }
    }
}
