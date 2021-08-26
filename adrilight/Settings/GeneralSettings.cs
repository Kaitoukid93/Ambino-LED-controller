using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace adrilight
{
    internal class GeneralSettings : ViewModelBase, IGeneralSettings
    {
       // private bool _autostart = true;
        private int _borderDistanceX = 0;
        private int _borderDistanceY = 0;
        private bool _autostart = true;
        
        private bool _mirrorX = true;
        private bool _mirrorY = false;
        private int _offsetLed = 10;
        private int _offsetLed2 = 10;
        private int _offsetLed3 = 10;
        private bool _shouldbeRunning = true;
        private bool _shouldbeRunningSecondary = false;
        private bool _shouldbeRunningThird = false;
        int _smoothFactor = 3;
        //ambilight smooth choice///
        private int _screenSize = 0;
        private int _deskSize = 0;
        private int _screenSizeSecondary = 0;
        private int _screenSizeThird = 0;

        private bool _startMinimized = false;

        private int _spotHeight = 150;
        private int _spotsX = 11;
        private int _spotsX2 = 11;
        private int _spotsX3 = 11;
        private int _shaderX = 70;
        private int _shaderY = 70;
        private int _shaderCanvasWidth = 500;
        private int _shaderCanvasHeight = 500;

        private byte _whitebalanceRed = 100;
        private byte _whitebalanceGreen =95;
        private byte _whitebalanceBlue = 90;

        private int _spotsY = 6;
        private int _spotsY2 = 6;
        private int _spotsY3 = 6;


        private int _spotWidth = 150;

        public byte WhitebalanceRed { get => _whitebalanceRed; set { Set(() => WhitebalanceRed, ref _whitebalanceRed, value); } }
        public byte WhitebalanceGreen { get => _whitebalanceGreen; set { Set(() => WhitebalanceGreen, ref _whitebalanceGreen, value); } }
        public byte WhitebalanceBlue { get => _whitebalanceBlue; set { Set(() => WhitebalanceBlue, ref _whitebalanceBlue, value); } }

        private int _selectedDisplay = 0;
        private int _selectedAdapter = 0;
        private byte _saturationTreshold = 10;
        private int _sentryMode = 0;

        private int _limitFps = 100;

        private int _useLinearLighting = 0;













        public int DeskSize { get => _deskSize; set { Set(() => DeskSize, ref _deskSize, value); } }
        public int ScreenSize { get => _screenSize; set { Set(() => ScreenSize, ref _screenSize, value); } }
        public int ScreenSizeSecondary { get => _screenSizeSecondary; set { Set(() => ScreenSizeSecondary, ref _screenSizeSecondary, value); } }
        public int ScreenSizeThird { get => _screenSizeThird; set { Set(() => ScreenSizeThird, ref _screenSizeThird, value); } }

        public int ShaderCanvasWidth { get => _shaderCanvasWidth; set { Set(() => ShaderCanvasWidth, ref _shaderCanvasWidth, value); } }

        public int ShaderCanvasHeight { get => _shaderCanvasHeight; set { Set(() => ShaderCanvasHeight, ref _shaderCanvasHeight, value); } }
        public bool StartMinimized { get => _startMinimized; set { Set(() => StartMinimized, ref _startMinimized, value); } }
        public bool Autostart { get => _autostart; set { Set(() => Autostart, ref _autostart, value); } }

        public int BorderDistanceX { get => _borderDistanceX; set { Set(() => BorderDistanceX, ref _borderDistanceX, value); } }
        public int SentryMode { get => _sentryMode; set { Set(() => SentryMode, ref _sentryMode, value); } }
        public int BorderDistanceY { get => _borderDistanceY; set { Set(() => BorderDistanceY, ref _borderDistanceY, value); } }
        public bool ShouldbeRunningThird { get => _shouldbeRunningThird; set { Set(() => ShouldbeRunningThird, ref _shouldbeRunningThird, value); } }
        public bool ShouldbeRunning { get => _shouldbeRunning; set { Set(() => ShouldbeRunning, ref _shouldbeRunning, value); } }
        public bool ShouldbeRunningSecondary { get => _shouldbeRunningSecondary; set { Set(() => ShouldbeRunningSecondary, ref _shouldbeRunningSecondary, value); } }
        public int SmoothFactor { get => _smoothFactor; set { Set(() => SmoothFactor, ref _smoothFactor, value); } }
        public bool MirrorX { get => _mirrorX; set { Set(() => MirrorX, ref _mirrorX, value); } }
        public bool MirrorY { get => _mirrorY; set { Set(() => MirrorY, ref _mirrorY, value); } }
        public int OffsetLed { get => _offsetLed; set { Set(() => OffsetLed, ref _offsetLed, value); } }
        public int OffsetLed2 { get => _offsetLed2; set { Set(() => OffsetLed2, ref _offsetLed2, value); } }
        public int OffsetLed3 { get => _offsetLed3; set { Set(() => OffsetLed3, ref _offsetLed3, value); } }

        public int UseLinearLighting { get => _useLinearLighting; set { Set(() => UseLinearLighting, ref _useLinearLighting, value); } }

        public int SpotHeight { get => _spotHeight; set { Set(() => SpotHeight, ref _spotHeight, value); } }
        public int SpotsX { get => _spotsX; set { Set(() => SpotsX, ref _spotsX, value); } }

        public int SpotsY { get => _spotsY; set { Set(() => SpotsY, ref _spotsY, value); } }
        public int ShaderX { get => _shaderX; set { Set(() => ShaderX, ref _shaderX, value); } }
        public int ShaderY { get => _shaderY; set { Set(() => ShaderY, ref _shaderY, value); } }
        public int SpotsY2 { get => _spotsY2; set { Set(() => SpotsY2, ref _spotsY2, value); } }
        public int SpotsX2 { get => _spotsX2; set { Set(() => SpotsX2, ref _spotsX2, value); } }
        public int SpotsY3 { get => _spotsY3; set { Set(() => SpotsY3, ref _spotsY3, value); } }
        public int SpotsX3{ get => _spotsX3; set { Set(() => SpotsX3, ref _spotsX3, value); } }

        public int SpotWidth { get => _spotWidth; set { Set(() => SpotWidth, ref _spotWidth, value); } }


        public byte SaturationTreshold { get => _saturationTreshold; set { Set(() => SaturationTreshold, ref _saturationTreshold, value); } }


        public int LimitFps { get => _limitFps; set { Set(() => LimitFps, ref _limitFps, value); }  }


        public int SelectedDisplay { get => _selectedDisplay; set { Set(() => SelectedDisplay, ref _selectedDisplay, value); } }
        public int SelectedAdapter { get => _selectedAdapter; set { Set(() => SelectedAdapter, ref _selectedAdapter, value); } }
       

       




    }
}
