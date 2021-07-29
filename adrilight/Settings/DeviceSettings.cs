using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace adrilight
{
    internal class DeviceSettings : ViewModelBase, IDeviceSettings
    {
       // private bool _autostart = true;
        private int _borderDistanceX = 0;
        private int _borderDistanceY = 0;
        private string _devicePort = "Không có";
        private string _gifFilePath = "";
        private string _deviceName = "Ambino Basic";
        private int _deviceID = 1;
        private string _deviceSerial = "151293";
        private string _deviceType = "Generic Device";
        private int _rGBOrder = 0;

        private int _mSens = 0;
       // private DateTime? _lastUpdateCheck = DateTime.UtcNow;
        private int _ledsPerSpot = 1;
        private bool _mirrorX = true;
        private bool _mirrorY = false;
        private int _offsetLed = 0;

        //ambilight smooth choice///

       

        private int _offsetX = 0;
        private int _offsetY = 0;
        private bool _isPreviewEnabled = false;
        private byte _saturationTreshold = 10;
        private int _spotHeight = 150;
        private int _spotsX = 11;
        private byte _deviceSize = 1;


        private int _spotsY = 6;
        private int _effectSpeed = 5;
        private int _colorFrequency = 0;
        private int _selectedMusicPalette = 0;

        private int _spotWidth = 150;
       // private bool _startMinimized = true;
        private bool _transferActive = true;
        private bool _captureActive = true;

        //static color/
        private bool _isBreathing = false;
        private Color _staticColor = (Color)ColorConverter.ConvertFromString("#FF326CF3");
        int _breathingSpeed = 5;
        //static color//

        private bool _isConnected = true;
        private byte _selectedPalette = 0;
        private byte[] _snapShot = new byte[256];
        private int _numLED = 32;

        private bool _useLinearLighting = true;
      

        private int _selectedAudioDevice = 0;
        private int _selectedDisplay = 0;
        private int _selectedAdapter = 0;

        private int _atmosphereStart = 1;
        private int _atmosphereStop = 255;

        private byte _brightness = 80;

        
        public byte _selectedMusicMode = 0;


        private string _filemau = "Blackout.txt";
        private string _filemauchip = "Blackout.txt";

        private byte _selectedEffect = 0;
        private Color _color0 = Color.FromArgb(255, 0, 255, 255);
        private Color _color1 = Color.FromArgb(0, 0, 255, 255);
        private Color _color2 = Color.FromArgb(0, 0, 255, 255);
        private Color _color3 = Color.FromArgb(0, 0, 255, 255);
        private Color _color4 = Color.FromArgb(0, 0, 255, 255);
        private Color _color5 = Color.FromArgb(0, 0, 255, 255);
        private Color _color6 = Color.FromArgb(0, 0, 255, 255);
        private Color _color7 = Color.FromArgb(0, 0, 255, 255);
        private Color _color8 = Color.FromArgb(0, 0, 255, 255);
        private Color _color9 = Color.FromArgb(0, 0, 255, 255);
        private Color _color10 = Color.FromArgb(0, 0, 255, 255);
        private Color _color11 = Color.FromArgb(0, 0, 255, 255);
        private Color _color12 = Color.FromArgb(0, 0, 255, 255);
        private Color _color13 = Color.FromArgb(0, 0, 255, 255);
        private Color _color14 = Color.FromArgb(0, 0, 255, 255);
        private Color _color15 = Color.FromArgb(0, 0, 255, 255);


        private Color _mcolor0 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor1 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor2 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor3 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor4 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor5 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor6 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor7 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor8 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor9 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor10 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor11 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor12 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor13 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor14 = Color.FromArgb(0, 0, 255, 255);
        private Color _mcolor15 = Color.FromArgb(0, 0, 255, 255);




        //gifxelation//
        private bool _GifPlayPause = false;
        private byte _IMInterpolationModeIndex = 0;
        private int _IMX1 = 0;

        private int _IMY1 = 0;

        private int _IMX2 = 0;
        private int _IMY2 = 0;
        private bool _IMLockDim = false;
        //gifxelation//










        private int _limitFps = 60;

        //support future config file migration
        public int ConfigFileVersion { get; set; } = 1;





        public int MSens { get => _mSens; set { Set(() => MSens, ref _mSens, value); } }
       // public bool Autostart { get => _autostart; set { Set(() => Autostart, ref _autostart, value); } }
        public int BorderDistanceX { get => _borderDistanceX; set { Set(() => BorderDistanceX, ref _borderDistanceX, value); } }
        public int BorderDistanceY { get => _borderDistanceY; set { Set(() => BorderDistanceY, ref _borderDistanceY, value); } }
        public string DevicePort { get => _devicePort; set { Set(() => DevicePort, ref _devicePort, value); } }
        public string GifFilePath { get => _gifFilePath; set { Set(() => GifFilePath, ref _gifFilePath, value); } }
        public string DeviceName { get => _deviceName; set { Set(() => DeviceName, ref _deviceName, value); } }
        public int DeviceID { get => _deviceID; set { Set(() => DeviceID, ref _deviceID, value); } }
        public string DeviceSerial { get => _deviceSerial; set { Set(() => DeviceSerial, ref _deviceSerial, value); } }
        public string DeviceType { get => _deviceType; set { Set(() => DeviceType, ref _deviceType, value); } }
        //public string ComPort4 { get => _comPort4; set { Set(() => ComPort4, ref _comPort4, value); } }
        public int RGBOrder { get => _rGBOrder; set { Set(() => RGBOrder, ref _rGBOrder, value); } }

        //public DateTime? LastUpdateCheck { get => _lastUpdateCheck; set { Set(() => LastUpdateCheck, ref _lastUpdateCheck, value); } }
        public int NumLED { get => _numLED; set { Set(() => NumLED, ref _numLED, value); } }
        [Obsolete]
        public int LedsPerSpot { get => _ledsPerSpot; set { Set(() => LedsPerSpot, ref _ledsPerSpot, value); } }
        public bool MirrorX { get => _mirrorX; set { Set(() => MirrorX, ref _mirrorX, value); } }
        public bool MirrorY { get => _mirrorY; set { Set(() => MirrorY, ref _mirrorY, value); } }
        public int OffsetLed { get => _offsetLed; set { Set(() => OffsetLed, ref _offsetLed, value); } }

        [Obsolete]
        public int OffsetX { get => _offsetX; set { Set(() => OffsetX, ref _offsetX, value); } }
        [Obsolete]
        public int OffsetY { get => _offsetY; set { Set(() => OffsetY, ref _offsetY, value); } }

        public int LimitFps { get => _limitFps; set { Set(() => LimitFps, ref _limitFps, value); } }

        public bool IsPreviewEnabled { get => _isPreviewEnabled; set { Set(() => IsPreviewEnabled, ref _isPreviewEnabled, value); } }
        public byte SaturationTreshold { get => _saturationTreshold; set { Set(() => SaturationTreshold, ref _saturationTreshold, value); } }
        public int SpotHeight { get => _spotHeight; set { Set(() => SpotHeight, ref _spotHeight, value); } }
        public int SpotsX { get => _spotsX; set { Set(() => SpotsX, ref _spotsX, value); } }

        public int SpotsY { get => _spotsY; set { Set(() => SpotsY, ref _spotsY, value); } }

        public int SpotWidth { get => _spotWidth; set { Set(() => SpotWidth, ref _spotWidth, value); } }
       // public bool StartMinimized { get => _startMinimized; set { Set(() => StartMinimized, ref _startMinimized, value); } }
        public bool TransferActive { get => _transferActive; set { Set(() => TransferActive, ref _transferActive, value); } }

        public bool CaptureActive { get => _captureActive; set { Set(() => CaptureActive, ref _captureActive, value); } }
        public bool IsConnected { get => _isConnected; set { Set(() => IsConnected, ref _isConnected, value); } }
        public byte DeviceSize { get => _deviceSize; set { Set(() => DeviceSize, ref _deviceSize, value); } }
        public Color StaticColor { get => _staticColor; set { Set(() => StaticColor, ref _staticColor, value); } }
        public bool IsBreathing { get => _isBreathing; set { Set(() => IsBreathing, ref _isBreathing, value); } }
        public int BreathingSpeed { get => _breathingSpeed; set { Set(() => BreathingSpeed, ref _breathingSpeed, value); } }
        public int AtmosphereStart { get => _atmosphereStart; set { Set(() => AtmosphereStart, ref _atmosphereStart, value); } }
        public int AtmosphereStop { get => _atmosphereStop; set { Set(() => AtmosphereStop, ref _atmosphereStop, value); } }
        public byte[] SnapShot { get => _snapShot; set { Set(() => SnapShot, ref _snapShot, value); } }

        //public bool Comport4Open { get => _Comport4Open; set { Set(() => Comport4Open, ref _Comport4Open, value); } }



        public bool UseLinearLighting { get => _useLinearLighting; set { Set(() => UseLinearLighting, ref _useLinearLighting, value); } }
        //gifxelation//

        public bool GifPlayPause { get => _GifPlayPause; set { Set(() => GifPlayPause, ref _GifPlayPause, value); } }
        public bool IMLockDim { get => _IMLockDim; set { Set(() => IMLockDim, ref _IMLockDim, value); } }

        public int IMY2 { get => _IMY2; set { Set(() => IMY2, ref _IMY2, value); } }
        public int IMY1 { get => _IMY1; set { Set(() => IMY1, ref _IMY1, value); } }
        public int IMX2 { get => _IMX2; set { Set(() => IMX2, ref _IMX2, value); } }
        public int IMX1 { get => _IMX1; set { Set(() => IMX1, ref _IMX1, value); } }
        public byte IMInterpolationModeIndex { get => _IMInterpolationModeIndex; set => Set(() => IMInterpolationModeIndex, ref _IMInterpolationModeIndex, value); }

        //gifxelation//

    


       

        public string filemau { get => _filemau; set { Set(() => filemau, ref _filemau, value); } }
        public string filemauchip { get => _filemauchip; set { Set(() => filemauchip, ref _filemauchip, value); } }

        public byte SelectedPalette { get => _selectedPalette; set { Set(() => SelectedPalette, ref _selectedPalette, value); } }
        public byte SelectedMusicMode { get => _selectedMusicMode; set { Set(() => SelectedMusicMode, ref _selectedMusicMode, value); } }

        public byte Brightness { get => _brightness; set { Set(() => Brightness, ref _brightness, value); } }

        //ambilight smooth 

        
        public byte SelectedEffect { get => _selectedEffect; set { Set(() => SelectedEffect, ref _selectedEffect, value); } }
        public int SelectedAudioDevice { get => _selectedAudioDevice; set { Set(() => SelectedAudioDevice, ref _selectedAudioDevice, value); } }
        public int SelectedDisplay { get => _selectedDisplay; set { Set(() => SelectedDisplay, ref _selectedDisplay, value); } }
        public int SelectedAdapter { get => _selectedAdapter; set { Set(() => SelectedAdapter, ref _selectedAdapter, value); } }
        public int EffectSpeed { get => _effectSpeed; set { Set(() => EffectSpeed, ref _effectSpeed, value); } }
        public int ColorFrequency { get => _colorFrequency; set { Set(() => ColorFrequency, ref _colorFrequency, value); } }
        public int SelectedMusicPalette { get => _selectedMusicPalette; set { Set(() => SelectedMusicPalette, ref _selectedMusicPalette, value); } }
        //Color Palette

        public Color Color0 { get => _color0; set { Set(() => Color0, ref _color0, value); } }
        public Color Color1 { get => _color1; set { Set(() => Color1, ref _color1, value); } }
        public Color Color2 { get => _color2; set { Set(() => Color2, ref _color2, value); } }
        public Color Color3 { get => _color3; set { Set(() => Color3, ref _color3, value); } }
        public Color Color4 { get => _color4; set { Set(() => Color4, ref _color4, value); } }
        public Color Color5 { get => _color5; set { Set(() => Color5, ref _color5, value); } }
        public Color Color6 { get => _color6; set { Set(() => Color6, ref _color6, value); } }
        public Color Color7 { get => _color7; set { Set(() => Color7, ref _color7, value); } }
        public Color Color8 { get => _color8; set { Set(() => Color8, ref _color8, value); } }
        public Color Color9 { get => _color9; set { Set(() => Color9, ref _color9, value); } }
        public Color Color10 { get => _color10; set { Set(() => Color10, ref _color10, value); } }
        public Color Color11 { get => _color11; set { Set(() => Color11, ref _color11, value); } }
        public Color Color12 { get => _color12; set { Set(() => Color12, ref _color12, value); } }
        public Color Color13 { get => _color13; set { Set(() => Color13, ref _color13, value); } }
        public Color Color14 { get => _color14; set { Set(() => Color14, ref _color14, value); } }
        public Color Color15 { get => _color15; set { Set(() => Color15, ref _color15, value); } }

        //Color Palette
        public Color MColor0 { get => _mcolor0; set { Set(() => MColor0, ref _mcolor0, value); } }
        public Color MColor1 { get => _mcolor1; set { Set(() => MColor1, ref _mcolor1, value); } }
        public Color MColor2 { get => _mcolor2; set { Set(() => MColor2, ref _mcolor2, value); } }
        public Color MColor3 { get => _mcolor3; set { Set(() => MColor3, ref _mcolor3, value); } }
        public Color MColor4 { get => _mcolor4; set { Set(() => MColor4, ref _mcolor4, value); } }
        public Color MColor5 { get => _mcolor5; set { Set(() => MColor5, ref _mcolor5, value); } }
        public Color MColor6 { get => _mcolor6; set { Set(() => MColor6, ref _mcolor6, value); } }
        public Color MColor7 { get => _mcolor7; set { Set(() => MColor7, ref _mcolor7, value); } }
        public Color MColor8 { get => _mcolor8; set { Set(() => MColor8, ref _mcolor8, value); } }
        public Color MColor9 { get => _mcolor9; set { Set(() => MColor9, ref _mcolor9, value); } }
        public Color MColor10 { get => _mcolor10; set { Set(() => MColor10, ref _mcolor10, value); } }
        public Color MColor11 { get => _mcolor11; set { Set(() => MColor11, ref _mcolor11, value); } }
        public Color MColor12 { get => _mcolor12; set { Set(() => MColor12, ref _mcolor12, value); } }
        public Color MColor13 { get => _mcolor13; set { Set(() => MColor13, ref _mcolor13, value); } }
        public Color MColor14 { get => _mcolor14; set { Set(() => MColor14, ref _mcolor14, value); } }
        public Color MColor15 { get => _mcolor15; set { Set(() => MColor15, ref _mcolor15, value); } }
        private int _parentDeviceId = -1;
        public int ParentDeviceId { get => _parentDeviceId; set { Set(() => _parentDeviceId, ref _parentDeviceId, value); } }
        // Add new



        public Guid InstallationId { get; set; } = Guid.NewGuid();
        private ObservableCollection<IDeviceSettings> _childCard;
        public ObservableCollection<IDeviceSettings> ChildCard {
            get { return _childCard; }
            set
            {
                if (_childCard == value) return;
                _childCard = value;
                RaisePropertyChanged();
            }
        }
        public bool IsHub => DeviceType == "ABHV2";
    }
}
