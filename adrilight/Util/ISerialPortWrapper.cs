using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Util
{
    interface ISerialPortWrapper : IDisposable
    {
        bool IsOpen { get; }

        void Close();
        void Open();
        void Write(byte[] outputBuffer, int v, int streamLength);
        void Read(byte[] inputBuffer, int v, int streamLength);
        void Print(string outputBuffer);
        int BytesToRead { get; }
        int ReadByte();
        bool DtrEnabled { get; }
        bool RtsEnabled { get;  }
        void DisableDtr();
        void DisableRts();
        // void Print(byte[] outputBuffer);
    }
}
