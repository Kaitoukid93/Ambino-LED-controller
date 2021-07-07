using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace adrilight
{
    public interface IGeneralSettings : INotifyPropertyChanged
    {

        int BorderDistanceX { get; set; }
        int BorderDistanceY { get; set; }
        bool Autostart { get; set; }
        bool MirrorX { get; set; }

        bool MirrorY { get; set; }

        int OffsetLed { get; set; }

        int SpotHeight { get; set; }

        int SpotWidth { get; set; }

        int SpotsX { get; set; }

        int SpotsY { get; set; }
        int SentryMode { get; set; }

        int SelectedDisplay { get; set; }
        int SelectedAdapter { get; set; }
        byte SaturationTreshold { get; set; }
        bool ShouldbeRunning { get; set; }
        int UseLinearLighting { get; set; }
        int LimitFps { get; set; }
        bool StartMinimized { get; set; }


    }
}
