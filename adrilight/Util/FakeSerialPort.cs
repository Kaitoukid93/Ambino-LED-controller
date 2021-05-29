using NLog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace adrilight.Util
{
    class FakeSerialPort : ISerialPortWrapper
    {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        public FakeSerialPort() => _log.Warn("FakeSerialPort created!");

        public bool IsOpen { get; private set; }
        public bool DtrEnabled { get;  private set; }
        public bool RtsEnabled { get;  private set; }
        public void DisableDtr()
        {

        }
        public void DisableRts()
        {

        }

        public void Open() => IsOpen = true;
        public void Close() => IsOpen = false;

        public void Dispose() {
            fpsLogger?.Dispose();
        }

        private readonly FpsLogger fpsLogger = new FpsLogger("FakeSerialPort");

        public void Write(byte[] outputBuffer, int v, int streamLength)
        {
            //_log.Warn($"Faking writing of {streamLength} bytes to the serial port");
            fpsLogger.TrackSingleFrame();
        }

        public void Print(string outputBuffer)
        {
            //_log.Warn($"Faking writing of {streamLength} bytes to the serial port");
            fpsLogger.TrackSingleFrame();
        }
        public void Read(byte[] inputBuffer, int v, int streamLength)
        {
            //_log.Warn($"Faking writing of {streamLength} bytes to the serial port");
            fpsLogger.TrackSingleFrame();
        }
        public int BytesToRead { get; }

       

        public int ReadByte()
        {

            return 0;

        }
    }
}
