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
        
        bool Breathing { get; set; }
        int AtmosphereStart { get; set; }
        int AtmosphereStop { get; set; }
        byte SelectedEffect { get; set; }
        byte SelectedMusicMode { get; set; }
   

        //gifxelation//
        bool GifPlayPause { get; set; }
         byte IMInterpolationModeIndex { get; set; } 
         int IMX1 { get; set; } 
        
         int IMY1 { get; set; } 
       
         int IMX2 { get; set; }
         int IMY2 { get; set; } 
         bool IMLockDim { get; set; } 
        LEDOrder[] LEDorder { get; set; }
        //gifxelation//

       
        int SelectedAudioDevice { get; set; }
       

  

        byte SelectedSize { get; set; }
        byte SelectedPalette { get; set; }

        Color StaticColor { get; set; }
        
        int LimitFps { get; set; }
    }
}