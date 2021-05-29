using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Util
{
    class WrappedSerialPort : ISerialPortWrapper
    {
        public WrappedSerialPort(SerialPort serialPort)
        {
            SerialPort = serialPort ?? throw new ArgumentNullException(nameof(serialPort));
        }

        private SerialPort SerialPort { get; }

        public bool IsOpen => SerialPort.IsOpen;
        public bool DtrEnabled => SerialPort.DtrEnable;
        public bool RtsEnabled => SerialPort.RtsEnable;
        public void DisableDtr() => SerialPort.DtrEnable = false;
        public void DisableRts() => SerialPort.RtsEnable = false;

        public void Close() => SerialPort.Close();

        public void Open() => SerialPort.Open();

        public void Write(byte[] outputBuffer, int v, int streamLength) => SerialPort.Write(outputBuffer, v, streamLength);
        public void Print(string outputBuffer) => SerialPort.Write(outputBuffer);
        public void Read(byte[] inputBuffer, int v, int streamLength) => SerialPort.Read(inputBuffer, v, streamLength);

        public int ReadByte()

        {
            Int32 touchvalue = SerialPort.ReadByte();
            return touchvalue;
        }
        public int BytesToRead => SerialPort.BytesToRead;

        public void Dispose() => SerialPort.Dispose();
        public void Zoe() => SerialPort.RtsEnable = true;
       
    }
}
