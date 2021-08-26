using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace adrilight.Util
{
    public interface IShaderEffect : INotifyPropertyChanged
    {

        bool IsRunning { get; }

        void Run(CancellationToken token);
        //WriteableBitmap MatrixBitmap { get; set; }
        Pixel[] Frame { get; set; }

    }
}
