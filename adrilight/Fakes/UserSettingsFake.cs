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
        public string GifFilePath { get; set; } = "";
        //public string ComPort4 { get; set; } = "Không có";
        public byte[] SnapShot { get; set; } = new byte[256];
        public DateTime? LastUpdateCheck { get; set; } = DateTime.Now;
        public int LedsPerSpot { get; set; } = 1;
        public bool MirrorX { get; set; } = true;
        public bool MirrorY { get; set; } = false;
        public int OffsetLed { get; set; } = 10;
        public int OffsetX { get; set; } = 0;
        public int OffsetY { get; set; } = 0;
        public bool IsPreviewEnabled { get; set; } = false;
        public byte SaturationTreshold { get; set; } = 4;
        public int SpotsX { get; set; } = 11;
        public int SpotsY { get; set; } = 6;
        public int SpotHeight { get; set; } = 40;
        public int SpotWidth { get; set; } = 40;
        public bool StartMinimized { get; set; } = true;
        public bool TransferActive { get; set; } = true;
        public bool CaptureActive { get; set; } = true;
        public bool Shuffle { get; set; } = false;
        public bool shuffle { get; set; } = false;
        public bool ComportOpen { get; set; } = true;// an extra button for enable send out data
        public bool Advancesettings { get; set; } = false;
        public bool UseLinearLighting { get; set; } = true;
        public bool Breathing { get; set; } = false;
        public string filemau { get; set; } = null;
        public string filemauchip { get; set; } = null;
        public byte WhitebalanceRed { get; set; } = 100;
        public byte WhitebalanceGreen { get; set; } = 100;
        public byte WhitebalanceBlue { get; set; } = 100;
        public int SelectedAudioDevice { get; set; } = 0;
        public byte SelectedPalette { get; set; } = 0;
        public byte SelectedMusicMode { get; set; } = 0;
        public int AtmosphereStart { get; set; } = 1;
        public int AtmosphereStop { get; set; } = 255;
        public int EffectSpeed { get; set; } = 5;
        public int ColorFrequency { get; set; } = 0;
        public int SelectedMusicPalette { get; set; } = 0;

        public byte screensizecounter { get; set; } = 2;

      
        public byte SelectedEffect { get; set; } = 0;

        //gifxelation//
        public bool GifPlayPause { get; set; } = false;
        public byte IMInterpolationModeIndex { get; set; } = 0;
        public int IMX1 { get; set; } = 0;
     
        public int IMY1 { get; set; } = 0;
      
        public int IMX2 { get; set; } = 0;
        public int IMY2 { get; set; } = 0;
        public bool IMLockDim { get; set; } = false;
        public LEDOrder[] LEDorder { get; set; } = null;
        //gifxelation//




        public byte Brightness { get; set; } = 80;


        public Color StaticColor { get; set; } = AliceBlue;
        public int  BreathingSpeed { get; set; } = 5;
        //Color Palette//
        public Color Color0 { get; set; } = AliceBlue;
        public Color Color1 { get; set; } = AliceBlue;
        public Color Color2 { get; set; } = AliceBlue;
        public Color Color3 { get; set; } = AliceBlue;
        public Color Color4 { get; set; } = AliceBlue;
        public Color Color5 { get; set; } = AliceBlue;
        public Color Color6 { get; set; } = AliceBlue;
        public Color Color7 { get; set; } = AliceBlue;
        public Color Color8 { get; set; } = AliceBlue;
        public Color Color9 { get; set; } = AliceBlue;
        public Color Color10 { get; set; } = AliceBlue;
        public Color Color11 { get; set; } = AliceBlue;
        public Color Color12 { get; set; } = AliceBlue;
        public Color Color13 { get; set; } = AliceBlue;
        public Color Color14 { get; set; } = AliceBlue;
        public Color Color15 { get; set; } = AliceBlue;
        //Color Palette//
        public byte SelectedSize { get; set; } = 1;




        public Guid InstallationId { get; set; } = Guid.NewGuid();

        public bool SendRandomColors { get; set; }


        


        public int LimitFps { get; set; } = 100;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
