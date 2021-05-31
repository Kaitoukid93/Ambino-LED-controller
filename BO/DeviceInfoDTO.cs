using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BO
{
   public class DeviceInfoDTO : NotifyObject
    {
        private int _deviceid = 0;
        public int DeviceId
        {
            get { return _deviceid; }
            set
            {
                if (_deviceid == value) return;
                _deviceid = value;
                OnPropertyChanged();
            }
        }
        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set
            {
                if (_deviceName == value) return;
                _deviceName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StartChar));
            }
        }
        private string _deviceType;
        public string DeviceType
        {
            get { return _deviceType; }
            set
            {
                if (_deviceType == value) return;
                _deviceType = value;
                OnPropertyChanged();
            }
        }
        private int _deviceSize ;
        public int DeviceSize
        {
            get { return _deviceSize; }
            set
            {
                if (_deviceSize == value) return;
                _deviceSize = value;
                OnPropertyChanged();
            }
        }
        private string _devicePort;
        public string DevicePort
        {
            get { return _devicePort; }
            set
            {
                if (_devicePort == value) return;
                _devicePort = value;
                OnPropertyChanged();
            }
        }
        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected == value) return;
                _isConnected = value;
                OnPropertyChanged();
            }
        }
        private string _lightingmode;
        public string LightingMode
        {
            get { return _lightingmode; }
            set
            {
                if (_lightingmode == value) return;
                _lightingmode = value;
                OnPropertyChanged();
            }
        }
        private int _brightness=80;
        public int Brightness
        {
            get { return _brightness; }
            set
            {
                if (_brightness == value) return;
                _brightness = value;
                OnPropertyChanged();
            }
        }
        private int _colortemp;
        public int ColorTemp
        {
            get { return _colortemp; }
            set
            {
                if (_colortemp == value) return;
                _colortemp = value;
                OnPropertyChanged();
            }
        }
        private int _capturesource;
        public int CaptureSource
        {
            get { return _capturesource; }
            set
            {
                if (_capturesource == value) return;
                _capturesource = value;
                OnPropertyChanged();
            }
        }
        private int _rainbowmode;
        public int RainbowMode
        {
            get { return _rainbowmode; }
            set
            {
                if (_rainbowmode == value) return;
                _rainbowmode = value;
                OnPropertyChanged();
            }
        }
        private int _rainbowspeed;
        public int RainbowSpeed
        {
            get { return _rainbowspeed; }
            set
            {
                if (_rainbowspeed == value) return;
                _rainbowspeed = value;
                OnPropertyChanged();
            }
        }
        private int _musicmode;
        public int MusicMode
        {
            get { return _musicmode; }
            set
            {
                if (_musicmode == value) return;
                _musicmode = value;
                OnPropertyChanged();
            }
        }
        private int _musicsens;
        public int MusicSens
        {
            get { return _musicsens; }
            set
            {
                if (_musicsens == value) return;
                _musicsens = value;
                OnPropertyChanged();
            }
        }
        private int _musicsource;
        public int MusicSource
        {
            get { return _musicsource; }
            set
            {
                if (_musicsource == value) return;
                _musicsource = value;
                OnPropertyChanged();
            }
        }
        private int _gifmode;
        public int GifMode
        {
            get { return _gifmode; }
            set
            {
                if (_gifmode == value) return;
                _gifmode = value;
                OnPropertyChanged();
            }
        }
        private string _gifsource;
        public string GifSource
        {
            get { return _gifsource; }
            set
            {
                if (_gifsource == value) return;
                _gifsource = value;
                OnPropertyChanged();
            }
        }
        private bool _isbreathing;
        public bool IsBreathing
        {
            get { return _isbreathing; }
            set
            {
                if (_isbreathing == value) return;
                _isbreathing = value;
                OnPropertyChanged();
            }
        }
        
        private string _fadestart;
        public string FadeStart
        {
            get { return _fadestart; }
            set
            {
                if (_fadestart == value) return;
                _fadestart = value;
                OnPropertyChanged();
            }
        }
        private string _fadeend;
        public string FadeEnd
        {
            get { return _fadeend; }
            set
            {
                if (_fadeend == value) return;
                _fadeend = value;
                OnPropertyChanged();
            }
        }
        private bool _isshowondashboard;
        public bool IsShowOnDashboard
        {
            get { return _isshowondashboard; }
            set
            {
                if (_isshowondashboard == value) return;
                _isshowondashboard = value;
                OnPropertyChanged();
            }
        }
        private int _lednumber;
        public int LEDNumber
        {
            get { return _lednumber; }
            set
            {
                if (_lednumber == value) return;
                _lednumber = value;
                OnPropertyChanged();
            }
        }
        private int _palette;
        public int Palette
        {
            get { return _palette; }
            set
            {
                if (_palette == value) return;
                _palette = value;
                OnPropertyChanged();
            }
        }
        public string StartChar => GetStartChar();
       public string GetStartChar()
        {
            return string.IsNullOrEmpty(DeviceName) ? string.Empty : DeviceName[0].ToString().ToUpper();
        }
        private bool _autostart = true;
        private int _borderDistanceX = 0;
        private int _borderDistanceY = 100;
        private string _comPort = "Không có";
        private int _ledsPerSpot = 1;
        private bool _mirrorX = true;
        private bool _mirrorY = false;
        private int _offsetLed = 10;

        private int _offsetX = 0;
        private int _offsetY = 0;
        private bool _isPreviewEnabled = false;
        private byte _saturationTreshold = 10;
        private int _spotHeight = 150;
        private int _spotsX = 11;
        private byte _selectedSize = 1;


        private int _spotsY = 6;

        private int _spotWidth = 150;

        public bool MirrorX
        {
            get { return _mirrorX; }
            set
            {
                if (_mirrorX == value) return;
                _mirrorX = value;
                OnPropertyChanged();
            }
        }
        public bool MirrorY
        {
            get { return _mirrorY; }
            set
            {
                if (_mirrorY == value) return;
                _mirrorY = value;
                OnPropertyChanged();
            }
        }
        public int OffsetLed {
            get { return _offsetLed; }
            set
            {
                if (_offsetLed == value) return;
                _offsetLed = value;
                OnPropertyChanged();
            }
        }
        public int BorderDistanceX {
            get { return _borderDistanceX; }
            set
            {
                if (_borderDistanceX == value) return;
                _borderDistanceX = value;
                OnPropertyChanged();
            }
        }
        public int BorderDistanceY {
            get { return _borderDistanceY; }
            set
            {
                if (_borderDistanceY == value) return;
                _borderDistanceY = value;
                OnPropertyChanged();
            }
        }
        [Obsolete]
        public int OffsetX
        {
            get { return _offsetX; }
            set
            {
                if (_offsetX == value) return;
                _offsetX = value;
                OnPropertyChanged();
            }
        }
        [Obsolete]
        public int OffsetY {
            get { return _offsetY; }
            set
            {
                if (_offsetY == value) return;
                _offsetY = value;
                OnPropertyChanged();
            }
        }
        public int SpotHeight
        {
            get { return _spotHeight; }
            set
            {
                if (_spotHeight == value) return;
                _spotHeight = value;
                OnPropertyChanged();
            }
        }
        public int SpotsX {
            get { return _spotsX; }
            set
            {
                if (_spotsX == value) return;
                _spotsX = value;
                OnPropertyChanged();
            }
        }

        public int SpotsY {
            get { return _spotsY; }
            set
            {
                if (_spotsY == value) return;
                _spotsY = value;
                OnPropertyChanged();
            }
        }

        public int SpotWidth
        {
            get { return _spotWidth; }
            set
            {
                if (_spotWidth == value) return;
                _spotWidth = value;
                OnPropertyChanged();
            }
        }
        private int _atmosphereStart = 1;
        private int _atmosphereStop = 255;
        public int AtmosphereStart {
            get { return _atmosphereStart; }
            set
            {
                if (_atmosphereStart == value) return;
                _atmosphereStart = value;
                OnPropertyChanged();
            }
        }
        public int AtmosphereStop
        {
            get { return _atmosphereStop; }
            set
            {
                if (_atmosphereStop == value) return;
                _atmosphereStop = value;
                OnPropertyChanged();
            }
        }
        private int _selectedAudioDevice = 0;
        public int SelectedAudioDevice
        {
            get { return _selectedAudioDevice; }
            set
            {
                if (_selectedAudioDevice == value) return;
                _selectedAudioDevice = value;
                OnPropertyChanged();
            }
        }
        private bool _useLinearLighting = true;
        private bool _captureActive = true;
        public bool UseLinearLighting
        {
            get { return _useLinearLighting; }
            set
            {
                if (_useLinearLighting == value) return;
                _useLinearLighting = value;
                NoUseLinearLighting = !value;
                OnPropertyChanged();
            }
        }
        public bool CaptureActive
        {
            get { return _captureActive; }
            set
            {
                if (_captureActive == value) return;
                _captureActive = value;
                OnPropertyChanged();
            }
        }
        public byte SaturationTreshold
        {
            get { return _saturationTreshold; }
            set
            {
                if (_saturationTreshold == value) return;
                _saturationTreshold = value;
                OnPropertyChanged();
            }
        }
        private int _limitFps = 60;
        public int LimitFps {
            get { return _limitFps; }
            set
            {
                if (_limitFps == value) return;
                _limitFps = value;
                OnPropertyChanged();
            }
        }
        private byte _whitebalanceRed = 100;
        private byte _whitebalanceGreen = 100;
        private byte _whitebalanceBlue = 100;
        public byte WhitebalanceRed {
            get { return _whitebalanceRed; }
            set
            {
                if (_whitebalanceRed == value) return;
                _whitebalanceRed = value;
                OnPropertyChanged();
            }
        }
        public byte WhitebalanceGreen {
            get { return _whitebalanceGreen; }
            set
            {
                if (_whitebalanceGreen == value) return;
                _whitebalanceGreen = value;
                OnPropertyChanged();
            }
        }
        public byte WhitebalanceBlue {
            get { return _whitebalanceBlue; }
            set
            {
                if (_whitebalanceBlue == value) return;
                _whitebalanceBlue = value;
                OnPropertyChanged();
            }
        }
        private int _effectSpeed = 5;
        public int EffectSpeed
        {
            get { return _effectSpeed; }
            set
            {
                if (_effectSpeed == value) return;
                _effectSpeed = value;
                OnPropertyChanged();
            }
        }
        int _breathingSpeed = 5;
        public int BreathingSpeed {
            get { return _breathingSpeed; }
            set
            {
                if (_breathingSpeed == value) return;
                _breathingSpeed = value;
                OnPropertyChanged();
            }
        }
        private string _gifFilePath = "";
        public string GifFilePath {
            get { return _gifFilePath; }
            set
            {
                if (_gifFilePath == value) return;
                _gifFilePath = value;
                OnPropertyChanged();
            }
        }
        private bool _GifPlayPause = false;
        public bool GifPlayPause
        {
            get { return _GifPlayPause; }
            set
            {
                if (_GifPlayPause == value) return;
                _GifPlayPause = value;
                OnPropertyChanged();
            }
        }
        private int _colorFrequency = 0;
        public int ColorFrequency {
            get { return _colorFrequency; }
            set
            {
                if (_colorFrequency == value) return;
                _colorFrequency = value;
                OnPropertyChanged();
            }
        }
        private int _selectedMusicPalette = 0;
        public int SelectedMusicPalette
        {
            get { return _selectedMusicPalette; }
            set
            {
                if (_selectedMusicPalette == value) return;
                _selectedMusicPalette = value;
                OnPropertyChanged();
            }
        }
        private Color _staticColor = Color.FromArgb(0, 0, 255, 255);
        public Color StaticColor
        {
            get { return _staticColor; }
            set
            {
                if (_staticColor == value) return;
                _staticColor = value;
                OnPropertyChanged();
            }
        }
        private bool _nouseLinearLighting = true;
        public bool NoUseLinearLighting
        {
            get { return _nouseLinearLighting; }
            set
            {
                if (_nouseLinearLighting == value) return;
                _nouseLinearLighting = value;
               UseLinearLighting = !value;
                OnPropertyChanged();
            }
        }
        public DeviceInfo GetDeviceInfo()
        {
            return new DeviceInfo()
            {
                 brightness=Brightness,
                  capturesource=CaptureSource,
                   colortemp=ColorTemp,
                    deviceid=DeviceId,
                     devicename=DeviceName,
                      deviceport=DevicePort,
                       devicesize=DeviceSize,
                        devicetype=DeviceType,
                         fadeend=FadeEnd,
                          fadestart=FadeStart,
                           gifmode=GifMode,
                            gifsource=GifSource,
                             isbreathing=IsBreathing,
                             isConnected=IsConnected,
                             lightingmode=LightingMode,
                              musicmode=MusicMode,
                               musicsens=MusicSens,
                                musicsource=MusicSource,
                                 rainbowmode=RainbowMode,
                                  rainbowspeed=RainbowSpeed,
                                   staticcolor= string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", StaticColor.A, StaticColor.R, StaticColor.G, StaticColor.B),
                                   isshowondashboard =IsShowOnDashboard,
                                   lednumber=LEDNumber,
                                   palette=Palette,
                                    atmospherestart=AtmosphereStart,
                                     atmospherestop=AtmosphereStop,
                                      breathingspeed=BreathingSpeed,
                                       colorfrequency=ColorFrequency,
                                        effectspeed=EffectSpeed,
                                         selectedmusicpalette=SelectedMusicPalette,
                                          spotheight=SpotHeight,
                                           spotwidth=SpotWidth,
                                            spotx=SpotsX,
                                             spoty=SpotsY,
                                              uselinearlighting=UseLinearLighting,
                                               whitebalanceblue=WhitebalanceBlue,
                                                whitebalancegreen=WhitebalanceGreen,
                                                 whitebalancered=WhitebalanceRed
            };
        }
    }


}
