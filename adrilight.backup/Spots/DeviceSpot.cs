using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Windows.Media.Color;

namespace adrilight.Spots
{
    [DebuggerDisplay("Spot: Rectangle={Rectangle}, Color={Red},{Green},{Blue}")]
    sealed class DeviceSpot : ViewModelBase, IDisposable, IDeviceSpot
    {

        public DeviceSpot(int top, int left, int width, int height, int x, int y)
        {
            Rectangle = new Rectangle(top, left, width, height);
            RadiusX = width / 4;
            RadiusY = height / 4;
        }

        public Rectangle Rectangle { get; private set; }

        private bool _isFirst;
        public bool IsFirst {
            get => _isFirst;
            set { Set(() => IsFirst, ref _isFirst, value); }
        }

        public Color OnDemandColor => Color.FromRgb(Red, Green, Blue);
        public Color OnDemandColorTransparent => Color.FromArgb(255, Red, Green, Blue);
        public int RadiusX { get; private set; }
        public int RadiusY { get; private set; }


        public byte Red { get; private set; }
        public byte Green { get; private set; }
        public byte Blue { get; private set; }

        public void SetColor(byte red, byte green, byte blue, bool raiseEvents)
        {
            Red = red;
            Green = green;
            Blue = blue;
            _lastMissingValueIndication = null;

            if (raiseEvents)
            {
                RaisePropertyChanged(nameof(OnDemandColor));
                RaisePropertyChanged(nameof(OnDemandColorTransparent));
            }
        }

        public void Dispose()
        {
        }

        private DateTime? _lastMissingValueIndication;
        private readonly double _dimToBlackIntervalInMs = TimeSpan.FromMilliseconds(10000).TotalMilliseconds;

        private float _dimR, _dimG, _dimB;

        public void IndicateMissingValue()
        {
            //this method might be called while another thread is calling setcolor() and we need the local copy to have a fixed value
            var localCopyLastMissingValueIndication = _lastMissingValueIndication;

            if (!localCopyLastMissingValueIndication.HasValue)
            {
                //a new period of missing values starts, copy last values
                _dimR = Red;
                _dimG = Green;
                _dimB = Blue;
                localCopyLastMissingValueIndication = _lastMissingValueIndication = DateTime.UtcNow;
            }

            var dimFactor = (float)(1 - (DateTime.UtcNow - localCopyLastMissingValueIndication.Value).TotalMilliseconds / _dimToBlackIntervalInMs);
            dimFactor = Math.Max(0, Math.Min(1, dimFactor));

            SetColor((byte)(dimFactor * _dimR), (byte)(dimFactor * _dimG), (byte)(dimFactor * _dimB), true);
        }
    }
}

