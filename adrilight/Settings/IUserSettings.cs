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
        string GifFilePath { get; set; }


        DateTime? LastUpdateCheck { get; set; }
        [Obsolete]
        int LedsPerSpot { get; set; }
        bool MirrorX { get; set; }
        bool MirrorY { get; set; }
        int OffsetLed { get; set; }

        [Obsolete]
        int OffsetX { get; set; }
        [Obsolete]
        int OffsetY { get; set; }
        bool IsPreviewEnabled { get; set; }
        byte SaturationTreshold { get; set; }
        int SpotHeight { get; set; }
        int SpotsX { get; set; }

        int SpotsY { get; set; }
   
        int SpotWidth { get; set; }
        bool StartMinimized { get; set; }
        bool TransferActive { get; set; }
        bool CaptureActive { get; set; }
    
        bool ComportOpen { get; set; }

        bool UseLinearLighting { get; set; }


        Guid InstallationId { get; set; }

         byte WhitebalanceRed { get; set; } 
         byte WhitebalanceGreen { get; set; } 
         byte WhitebalanceBlue { get; set; } 

        byte Brightness { get; set; }
        string filemau { get; set; }
        string filemauchip { get; set; }
        //smooth choice
        bool InstantMode { get; set; }
        bool NormalMode { get; set; }
        bool SmoothMode { get; set; }

        int AtmosphereStart { get; set; }
        int AtmosphereStop { get; set; }
        byte SelectedEffect { get; set; }
        byte SelectedMusicMode { get; set; }
        int SelectedMusicPalette { get; set; }

        //gifxelation//
        bool GifPlayPause { get; set; }
         byte IMInterpolationModeIndex { get; set; } 
         int IMX1 { get; set; } 
        
         int IMY1 { get; set; } 
       
         int IMX2 { get; set; }
         int IMY2 { get; set; } 
         bool IMLockDim { get; set; } 
       
        //gifxelation//
        byte[] SnapShot { get; set; }
       
        int SelectedAudioDevice { get; set; }
        int SelectedDisplay { get; set; }
       

  

        byte SelectedSize { get; set; }

        //rainbow settings//
        byte SelectedPalette { get; set; }
        int  EffectSpeed { get; set; }
        int  ColorFrequency { get; set; }
        //rainbow settings//

        //static color settings//
        Color StaticColor { get; set; }
        bool Breathing { get; set; }
        int BreathingSpeed { get; set; }
        //static color settings//


        //Color Palette
        Color Color0 { get; set; }
        Color Color1 { get; set; }
        Color Color2 { get; set; }
        Color Color3 { get; set; }
        Color Color4 { get; set; }
        Color Color5 { get; set; }
        Color Color6 { get; set; }
        Color Color7 { get; set; }
        Color Color8 { get; set; }
        Color Color9 { get; set; }
        Color Color10 { get; set; }
        Color Color11 { get; set; }
        Color Color12 { get; set; }
        Color Color13 { get; set; }
        Color Color14 { get; set; }
        Color Color15 { get; set; }
        int MSens { get; set; }
        //Color Palette
        //Music Color Palette
         Color MColor0 { get; set; }
         Color MColor1 { get; set; } 
         Color MColor2 { get; set; } 
         Color MColor3 { get; set; } 
         Color MColor4 { get; set; } 
         Color MColor5 { get; set; } 
         Color MColor6 { get; set; } 
         Color MColor7 { get; set; } 
         Color MColor8 { get; set; } 
         Color MColor9 { get; set; }
         Color MColor10 { get; set; }
         Color MColor11 { get; set; } 
         Color MColor12 { get; set; }
         Color MColor13 { get; set; } 
         Color MColor14 { get; set; } 
         Color MColor15 { get; set; } 
        int LimitFps { get; set; }
    }
}