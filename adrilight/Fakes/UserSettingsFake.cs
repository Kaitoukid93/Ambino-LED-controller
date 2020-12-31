using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace adrilight.Fakes
{
    class
        UserSettingsFake : IUserSettings
    {
        public static System.Windows.Media.Color AliceBlue { get; }
        public bool Autostart { get; set; } = true;
        public int BorderDistanceX { get; set; } = 33;
        public int BorderDistanceY { get; set; } = 44;
        public string ComPort { get; set; } = "Không có";
        public string ComPort2 { get; set; } = "Không có";
        public string ComPort3 { get; set; } = "Không có";
        //public string ComPort4 { get; set; } = "Không có";
        public string ComPort5 { get; set; } = "Không có";
        public DateTime? LastUpdateCheck { get; set; } = DateTime.Now;
        public int LedsPerSpot { get; set; } = 1;
        public bool MirrorX { get; set; } = true;
        public bool MirrorY { get; set; } = false;
        public int OffsetLed { get; set; } = 10;
        public int OffsetLed2 { get; set; } = 10;
        public int OffsetLed3 { get; set; } = 10;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public bool IsPreviewEnabled { get; set; } = false;
        public byte SaturationTreshold { get; set; } = 4;
        public int SpotsX { get; set; } = 11;
        public int SpotsY { get; set; } = 6;
        public int SpotsX2 { get; set; } = 11;
        public int SpotsY2 { get; set; } = 6;
        public int SpotsX3 { get; set; } = 11;
        public int SpotsY3 { get; set; } = 6;
        public int SpotHeight { get; set; } = 40;
        public int SpotWidth { get; set; } = 40;
        public bool StartMinimized { get; set; } = true;
        public bool TransferActive { get; set; } = true;
        public bool CaptureActive { get; set; } = true;
        public bool Shuffle { get; set; } = false;
        public bool shuffle { get; set; } = false;
        public bool ComportOpen { get; set; } = true;
        public bool Comport2Open { get; set; } = true;
        public bool Comport3Open { get; set; } = true;
        //public bool Comport4Open { get; set; } = true;
        public bool Comport5Open { get; set; } = true;
        public bool Advancesettings { get; set; } = false;
        public bool UseLinearLighting { get; set; } = true;


        // Add new
        public bool screenOne { get; set; } = false;
        public bool hasPCI { get; set; } = false;
        public bool hasRainpow { get; set; } = false;
        public bool hasNode{ get; set; } = false;
        public bool hasUSB { get; set; } = false;
        public bool hasPCISecond { get; set; } = false;
        public bool hasUSBSecond { get; set; } = false;
        public bool hasUSBTwo { get; set; } = false;
        public bool hasScreenTwo { get; set; } = false;
        public bool Pro11 { get; set; } = false;
        public bool Pro12 { get; set; } = false;
        public bool Pro13 { get; set; } = false;
        public bool Pro14 { get; set; } = false;
        public bool Pro21 { get; set; } = false;
        public bool Pro22 { get; set; } = false;
        public bool Pro31 { get; set; } = false;



        public byte WhitebalanceRed { get; set; } = 100;
        public byte WhitebalanceGreen { get; set; } = 100;
        public byte WhitebalanceBlue { get; set; } = 100;
        public byte zoecounter { get; set; } = 200;
        public byte Faneffectcounter { get; set; } = 0;
        public int devindex { get; set; } = 0;
        public byte resetcounter { get; set; } = 1;
        public byte screensizecounter { get; set; } = 2;
        public byte screen2sizecounter { get; set; } = 2;
        public byte screen3sizecounter { get; set; } = 2;
        public byte audiodevice { get; set; } = 0;
        public byte devicecounter { get; set; } = 0;
        public byte screeneffectcounter { get; set; } = 0;
        public byte screencounter { get; set; } = 0;
        public bool nodevice { get; set; } = false;
        public byte Port4Config { get; set; } = 0;
        public byte Port3Config { get; set; } = 0;
        public byte Port2Config { get; set; } = 0;
        public byte Port1Config { get; set; } = 0;
        public byte genre { get; set; } = 0;
        public bool caseenable { get; set; } = false;
        public byte tabindex { get; set; } = 0;
        

        public byte deskdirrection { get; set; } = 0;
        public byte holdtimecounter { get; set; } = 0;
        public byte buttoneffectcounter { get; set; } = 0;

        public int order_data0 { get; set; } = 0;
        public int order_data1 { get; set; } = 1;
        public int order_data2 { get; set; } = 2;
        public int order_data3 { get; set; } = 3;
        public int order_data4 { get; set; } = 4;
        public int order_data5 { get; set; } = 5;
        public int order_data6 { get; set; } = 6;
        public int order_data7 { get; set; } = 7;
        public int order_data8 { get; set; } = 8;
        public int order_data9 { get; set; } = 9;
        public int order_data10 { get; set; } = 10;
        public int order_data11 { get; set; } = 11;
        public int order_data12 { get; set; } = 12;
        public int order_data13 { get; set; } = 13;
        public int order_data14 { get; set; } = 14;
        public int order_data15 { get; set; } = 15;

        public byte holdeffectcounter { get; set; } = 0;
        public byte effectcounter { get; set; } = 0;

        public byte fanmodecounter { get; set; } = 0;
        public byte zone1speedcounter { get; set; } = 200;
        public byte zone2speedcounter { get; set; } = 200;
        public byte zone3speedcounter { get; set; } = 200;
        public byte LEDfanmodecounter { get; set; } = 0;
        public byte buteffectcounter { get; set; } = 0;
        public byte visualcounter { get; set; } = 1;
        public byte brightnesscounter { get; set; } = 200;
        public byte edgebrightnesscounter { get; set; } = 200;
        public byte huecounter { get; set; } = 200;
        public byte edgehuecounter { get; set; } = 200;
        public byte fanbrightnesscounter { get; set; } = 200;
        public byte fanspeedcounter { get; set; } = 200;
        public byte sincounter { get; set; } = 47;
        public byte speedcounter { get; set; } = 1;
        public string filemau { get; set; } = "Blackout.txt";
        public string filemauchip { get; set; } = "Blackout.txt";
        public byte methodcounter { get; set; } = 0;
        public int musiccounter { get; set; } = 0;
        public byte color1R { get; set; } = 255;
        public byte color1G { get; set; } = 0;
        public byte color1B { get; set; } = 0;
        public Color color1 { get; set; } = AliceBlue;
        public Color color2 { get; set; } = AliceBlue;
        public Color color3 { get; set; } = AliceBlue;
        public Color color4 { get; set; } = AliceBlue;
        public Color color5 { get; set; } = AliceBlue;
        public Color color6 { get; set; } = AliceBlue;
        public Color color7 { get; set; } = AliceBlue;
        public Color color8 { get; set; } = AliceBlue;
        public Color color10 { get; set; } = AliceBlue;
        public Color CaseStatic { get; set; } = AliceBlue;
        public Color ScreenStatic { get; set; } = AliceBlue;
        public Color DeskStatic { get; set; } = AliceBlue;
        

        public byte color2R { get; set; } = 255;
        public byte color2G { get; set; } = 0;
        public byte color2B { get; set; } = 0;

        public byte color3R { get; set; } = 255;
        public byte color3G { get; set; } = 0;
        public byte color3B { get; set; } = 0;

        public byte color4R { get; set; } = 255;
        public byte color4G { get; set; } = 0;
        public byte color4B { get; set; } = 0;

        public byte color5R { get; set; } = 255;
        public byte color5G { get; set; } = 0;
        public byte color5B { get; set; } = 0;

        public byte color6R { get; set; } = 255;
        public byte color6G { get; set; } = 0;
        public byte color6B { get; set; } = 0;

        public byte color7R { get; set; } = 255;
        public byte color7G { get; set; } = 0;
        public byte color7B { get; set; } = 0;

        public byte color8R { get; set; } = 255;
        public byte color8G { get; set; } = 0;
        public byte color8B { get; set; } = 0;

        public byte color9R { get; set; } = 255;
        public byte color9G { get; set; } = 0;
        public byte color9B { get; set; } = 0;
        public byte color10R { get; set; } = 255;
        public byte color10G { get; set; } = 255;
        public byte color10B { get; set; } = 255;

        public byte lightstatus { get; set; } = 0;



        public Guid InstallationId { get; set; } = Guid.NewGuid();

        public bool SendRandomColors { get; set; }
        public bool fixedcolor { get; set; }
        
        public bool music { get; set; }
        public bool musichue { get; set; }
        public bool zoe1 { get; set; }
        
        public bool zoe2 { get; set; }
        public bool zoe3 { get; set; }
        public bool zoe4 { get; set; }
        public bool zoe5 { get; set; }
        public bool zoe6 { get; set; }
        public bool zoe7 { get; set; }
        public bool zoe8 { get; set; }
        public bool method1 { get; set; }
        public bool method2 { get; set; }
        public bool method3 { get; set; }
        public bool method4 { get; set; }

        public bool effect { get; set; }
        public int LimitFps { get; set; } = 60;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
