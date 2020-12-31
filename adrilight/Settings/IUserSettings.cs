using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace adrilight
{
    public interface IUserSettings : INotifyPropertyChanged
    {

        bool Autostart { get; set; }
        int BorderDistanceX { get; set; }
        int BorderDistanceY { get; set; }
        string ComPort { get; set; }
        string ComPort2 { get; set; }
        string ComPort3 { get; set; }
        //string ComPort4 { get; set; }
        string ComPort5 { get; set; }

        DateTime? LastUpdateCheck { get; set; }
        [Obsolete]
        int LedsPerSpot { get; set; }
        bool MirrorX { get; set; }
        bool MirrorY { get; set; }
        int OffsetLed { get; set; }
        int OffsetLed3 { get; set; }
        int OffsetLed2 { get; set; }
        [Obsolete]
        int OffsetX { get; set; }
        [Obsolete]
        int OffsetY { get; set; }
        bool IsPreviewEnabled { get; set; }
        byte SaturationTreshold { get; set; }
        int SpotHeight { get; set; }
        int SpotsX { get; set; }
        int SpotsY { get; set; }
        int SpotsX2 { get; set; }
        int SpotsY2 { get; set; }
        int SpotsX3 { get; set; }
        int SpotsY3 { get; set; }
        int SpotWidth { get; set; }
        bool StartMinimized { get; set; }
        bool TransferActive { get; set; }
        bool CaptureActive { get; set; }
        bool Shuffle { get; set; }
        bool shuffle { get; set; }
        bool ComportOpen { get; set; }
        bool Comport2Open { get; set; }
        bool Comport3Open { get; set; }
        //bool Comport4Open { get; set; }
        bool Comport5Open { get; set; }
        bool Advancesettings { get; set; }
        bool UseLinearLighting { get; set; }


        // Add new
        bool screenOne { get; set; }
        bool hasUSB { get; set; }
        bool hasUSBSecond { get; set; }
        bool hasUSBTwo { get; set; }
        bool hasScreenTwo { get; set; }
        bool hasPCI { get; set; }
        bool hasRainpow { get; set; }
        bool hasNode { get; set; }
        bool hasPCISecond { get; set; }
        bool Pro11 { get; set; }
        bool Pro12 { get; set; }
        bool Pro13 { get; set; }
        bool Pro14 { get; set; }
        bool Pro21 { get; set; }
        bool Pro22 { get; set; }
        bool Pro31 { get; set; }


        Guid InstallationId { get; set; }

        byte WhitebalanceRed { get; set; }

        byte brightnesscounter { get; set; }
        byte edgebrightnesscounter { get; set; }
        byte huecounter { get; set; }
        byte edgehuecounter { get; set; }
        byte fanbrightnesscounter { get; set; }
        byte fanspeedcounter { get; set; }
        byte sincounter { get; set; }
        byte speedcounter { get; set; }
        string filemau { get; set; }
        string filemauchip { get; set; }
        byte methodcounter { get; set; }
        int musiccounter { get; set; }
        byte zoecounter { get; set; }
        byte Faneffectcounter{get; set;}
        int devindex { get; set; }
        byte resetcounter { get; set; }
        byte effectcounter { get; set; }
        byte fanmodecounter { get; set; }
        byte LEDfanmodecounter { get; set; }
        byte zone1speedcounter { get; set; }
        byte zone2speedcounter { get; set; }
        byte zone3speedcounter { get; set; }
        byte buteffectcounter { get; set; }
        byte visualcounter { get; set; }
        byte screeneffectcounter { get; set; }
        byte screencounter{ get; set; }
        bool nodevice { get; set; }
        byte Port4Config { get; set; }
        byte Port3Config { get; set; }
        byte Port2Config { get; set; }
        byte Port1Config { get; set; }
        byte genre { get; set; }
        byte tabindex { get; set; }
        bool caseenable { get; set; }
        byte deskdirrection { get; set; }
        byte holdtimecounter { get; set; }
        byte buttoneffectcounter { get; set; }

         int order_data0 { get; set; } 
         int order_data1 { get; set; } 
         int order_data2 { get; set; } 
         int order_data3 { get; set; }
        int order_data4 { get; set; } 
         int order_data5 { get; set; }
         int order_data6 { get; set; } 
        int order_data7 { get; set; } 
         int order_data8 { get; set; } 
         int order_data9 { get; set; } 
         int order_data10 { get; set; } 
         int order_data11 { get; set; } 
         int order_data12 { get; set; } 
         int order_data13 { get; set; } 
        int order_data14 { get; set; } 
         int order_data15 { get; set; } 

        byte holdeffectcounter { get; set; }
        byte screensizecounter { get; set; }
        byte screen2sizecounter { get; set; }
        byte screen3sizecounter { get; set; }
        byte audiodevice { get; set; }
        byte devicecounter { get; set; }
        byte WhitebalanceGreen { get; set; }
        byte WhitebalanceBlue { get; set; }
        bool SendRandomColors { get; set; }
        bool fixedcolor { get; set; }
        byte lightstatus { get; set; }
        bool method1 { get; set; }
        bool method2 { get; set; }
        bool method3 { get; set; }
        bool method4 { get; set; }
        bool zoe1 { get; set; }
        bool zoe2 { get; set; }
        bool zoe3 { get; set; }
        bool zoe4 { get; set; }
        bool zoe5 { get; set; }
        bool zoe6 { get; set; }
        bool zoe7 { get; set; }
        bool zoe8 { get; set; }
        byte color1R { get; set; }
        byte color1G { get; set; }
        byte color1B { get; set; }

        byte color2R { get; set; }
        byte color2G { get; set; }
        byte color2B { get; set; }

        byte color3R { get; set; }
        byte color3G { get; set; }
        byte color3B { get; set; }

        byte color4R { get; set; }
        byte color4G { get; set; }
        byte color4B { get; set; }

        byte color5R { get; set; }
        byte color5G { get; set; }
        byte color5B { get; set; }

        byte color6R { get; set; }
        byte color6G { get; set; }
        byte color6B { get; set; }

        byte color7R { get; set; }
        byte color7G { get; set; }
        byte color7B { get; set; }

        byte color8R { get; set; }
        byte color8G { get; set; }
        byte color8B { get; set; }

        byte color9R { get; set; }
        byte color9G { get; set; }
        byte color9B { get; set; }
        byte color10R { get; set; }
        byte color10G { get; set; }
        byte color10B { get; set; }
        Color color1 { get; set; }
        Color color2 { get; set; }

        Color color3 { get; set; }

        Color color4 { get; set; }

        Color color5 { get; set; }

        Color color6 { get; set; }

        Color color7 { get; set; }

        Color color8 { get; set; }

        Color color10 { get; set; }
        Color CaseStatic { get; set; }
        Color ScreenStatic { get; set; }
        Color DeskStatic { get; set; }

        bool effect { get; set; }
        bool music { get; set; }
        bool musichue { get; set; }
        int LimitFps { get; set; }
    }
}