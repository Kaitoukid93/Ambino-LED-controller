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
        int OffsetLed2 { get; set; }

        int SpotHeight { get; set; }

        int SpotWidth { get; set; }

        int SpotsX { get; set; }
        int SpotsX2 { get; set; }

        int SpotsY { get; set; }
        int SpotsY2 { get; set; }
        int SentryMode { get; set; }
        int ScreenSize { get; set; }
        byte WhitebalanceRed { get; set; }
        byte WhitebalanceGreen { get; set; }
        byte WhitebalanceBlue { get; set; }
        int ScreenSizeSecondary { get; set; }
        //smooth choice
        int SmoothFactor { get; set; }
        int SelectedDisplay { get; set; }
        int SelectedAdapter { get; set; }
        byte SaturationTreshold { get; set; }
        bool ShouldbeRunning { get; set; }
        bool ShouldbeRunningSecondary { get; set; }
        int UseLinearLighting { get; set; }
        int LimitFps { get; set; }
        bool StartMinimized { get; set; }


    }
}
