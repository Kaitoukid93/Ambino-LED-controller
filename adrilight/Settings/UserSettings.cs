using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace adrilight
{
    internal class UserSettings : ViewModelBase, IUserSettings
    {
        public static System.Windows.Media.Color AliceBlue { get; }
        
        private bool _autostart = true;
        private int _borderDistanceX = 0;
        private int _borderDistanceY = 100;
        private string _comPort = "Không có";
        private string _comPort2 = "Không có";
        private string _comPort3 = "Không có";
        //private string _comPort4 = "Không có";
        private string _comPort5 = "Không có";
        private DateTime? _lastUpdateCheck=DateTime.UtcNow;
        private int _ledsPerSpot = 1;
        private bool _mirrorX = true;
        private bool _mirrorY = false;
        private int _offsetLed = 10;
        private int _offsetLed2 = 10;
        private int _offsetLed3 = 10;
        private int _offsetX = 0;
        private int _offsetY = 0;
        private bool _isPreviewEnabled = false;
        private byte _saturationTreshold = 10;
        private int _spotHeight = 150;
        private int _spotsX = 11;
        private int _spotsY = 6;
        private int _spotsX2 = 11;
        private int _spotsY2 = 6;
        private int _spotsX3 = 11;
        private int _spotsY3 = 6;
        private int _spotWidth = 150;
        private bool _startMinimized = false;
        private bool _transferActive = true;
        private bool _CaptureActive = true;
        private bool _Shuffle = false;
        private bool _shuffle = false;
        private bool _ComportOpen = true;
        private bool _Comport2Open = true;
        private bool _Comport3Open = true;
        //private bool _Comport4Open = true;
        private bool _Comport5Open = true;
        private bool _Advancesettings = false;
        private bool _useLinearLighting = true;
        private byte _whitebalanceRed = 100;
        private byte _whitebalanceGreen = 100;
        private byte _whitebalanceBlue = 100;
        private byte _zoecounter = 200;
        private byte _Faneffectcounter = 0;
        private int _devindex = 0;
        private byte _resetcounter = 1;
        private byte _brightnesscounter = 200;
        private byte _edgebrightnesscounter = 200;
        private byte _fanspeedcounter = 200;
        private byte _color1R = 255;
        private byte _color1G = 0;
        private byte _color1B = 0;
        private byte _color2R = 255;
        private byte _color2G = 0;
        private byte _color2B = 0;
        private byte _color3R = 255;
        private byte _color3G = 0;
        private byte _color3B = 0;
        private byte _color4R = 255;
        private byte _color4G = 0;
        private byte _color4B = 0;
        private byte _color5R = 255;
        private byte _color5G = 0;
        private byte _color5B = 0;
        private byte _color6R = 255;
        private byte _color6G = 0;
        private byte _color6B = 0;
        private byte _color7R = 255;
        private byte _color7G = 0;
        private byte _color7B = 0;
        private byte _color8R = 255;
        private byte _color8G = 0;
        private byte _color8B = 255;
        private byte _color9R= 0;
        private byte _color9G = 0;
        private byte _color9B = 0;
        private byte _color10R = 255;
        private byte _color10G = 255;
        private byte _color10B = 255;
        private byte _sincounter = 47;
        private byte _fanbrightnesscounter = 200;
        private byte _huecounter = 200;
        private byte _edgehuecounter = 200;
        private Color _color1 = AliceBlue;
        private Color _color2 = AliceBlue;
        private Color _color3 = AliceBlue;
        private Color _color4 = AliceBlue;
        private Color _color5 = AliceBlue;
        private Color _color6 = AliceBlue;
        private Color _color7 = AliceBlue;
        private Color _color8 = AliceBlue;
        private Color _color10 = AliceBlue;
        private Color _CaseStatic = AliceBlue;
        private Color _ScreenStatic = AliceBlue;
        private Color _DeskStatic = AliceBlue;


        private byte _speedcounter = 5;
        private string _filemau = "Blackout.txt";
        private string _filemauchip = "Blackout.txt";
        private byte _methodcounter = 0;
        private int _musiccounter = 0;
        private byte _screeneffectcounter = 0;
        private byte _screencounter = 0;
        private bool _nodevice = false;
        private byte _Port4Config = 0;
        private byte _Port3Config = 0;
        private byte _Port2Config = 0;
        private byte _Port1Config = 0;

        private byte _genre = 0;
        private byte _tabindex = 0;
        private bool _caseenable = false;
        private byte _deskdirrection = 0;
        private byte _holdtimecounter = 0;
        private byte _buttoneffectcounter = 0;
        private int _order_data0 = 0;
        private int _order_data1 = 1;
        private int _order_data2 = 2;
        private int _order_data3 = 3;
        private int _order_data4 = 4;
        private int _order_data5 = 5;
        private int _order_data6 = 6;
        private int _order_data7 = 7;
        private int _order_data8 = 8;
        private int _order_data9 = 9;
        private int _order_data10 = 10;
        private int _order_data11 =11;
        private int _order_data12 = 12;
        private int _order_data13 =13;
        private int _order_data14 = 14;
        private int _order_data15 = 15;
        private byte _holdeffectcounter = 0;
        private byte _effectcounter = 0;
        private byte _fanmodecounter = 0;
        private byte _zone1speedcounter = 200;
        private byte _zone2speedcounter = 200;
        private byte _zone3speedcounter = 200;
        private byte _LEDfanmodecounter = 0;
        private byte _buteffectcounter = 0;
        private byte _visualcounter = 1;
        private byte _screensizecounter = 2;
        private byte _screen2sizecounter = 2;
        private byte _screen3sizecounter = 2;
        private byte _audiodevice = 1;
        private byte _devicecounter = 0;


        private bool _sendRandomColors = false;
        private bool _fixedcolor = false;
        private byte _lightstatus = 0;
        private bool _music = false;
        private bool _musichue = false;
        private bool _zoe1 = true;
        private bool _zoe2 = false;
        private bool _zoe3 = false;
        private bool _zoe4 = false;
        private bool _zoe5 = false;
        private bool _zoe6 = false;
        private bool _zoe7 = false;
        private bool _zoe8 = false;
        private bool _method1 = true;
        private bool _method4 = false;
        private bool _method2 = false;
        private bool _method3 = false;

        private bool _effect = false;
        private int _limitFps = 60;

        //support future config file migration
        public int ConfigFileVersion { get; set; } = 1;


        // Add new
        private bool _screenOne = false;
        private bool _hasPCI = false;
        private bool _hasRainpow = false;
        private bool _hasNode = false;
        private bool _hasUSB = false;
        private bool _hasPCISecond = false;
        private bool _hasUSBSecond = false;
        private bool _hasUSBTwo = false;
        private bool _hasScreenTwo = false;


        private bool _Pro11 = false;
        private bool _Pro12 = false;
        private bool _Pro13 = false;
        private bool _Pro14 = false;
        private bool _Pro21 = false;
        private bool _Pro22 = false;
        private bool _Pro31 = false;

        public bool Pro11 { get => _Pro11; set { Set(() => Pro11, ref _Pro11, value); } }
        public bool Pro12 { get => _Pro12; set { Set(() => Pro12, ref _Pro12, value); } }
        public bool Pro13 { get => _Pro13; set { Set(() => Pro13, ref _Pro13, value); } }
        public bool Pro14 { get => _Pro14; set { Set(() => Pro14, ref _Pro14, value); } }
        public bool Pro21 { get => _Pro21; set { Set(() => Pro21, ref _Pro21, value); } }
        public bool Pro22 { get => _Pro22; set { Set(() => Pro22, ref _Pro22, value); } }
        public bool Pro31 { get => _Pro31; set { Set(() => Pro31, ref _Pro31, value); } }


        public bool Autostart { get => _autostart; set { Set(() => Autostart, ref _autostart, value); } }
        public int BorderDistanceX { get => _borderDistanceX; set { Set(() => BorderDistanceX, ref _borderDistanceX, value); } }
        public int BorderDistanceY { get => _borderDistanceY; set { Set(() => BorderDistanceY, ref _borderDistanceY, value); } }
        public string ComPort { get => _comPort; set { Set(() => ComPort, ref _comPort, value); } }
        public string ComPort2 { get => _comPort2; set { Set(() => ComPort2, ref _comPort2, value); } }
        public string ComPort3 { get => _comPort3; set { Set(() => ComPort3, ref _comPort3, value); } }
        //public string ComPort4 { get => _comPort4; set { Set(() => ComPort4, ref _comPort4, value); } }
        public string ComPort5 { get => _comPort5; set { Set(() => ComPort5, ref _comPort5, value); } }
        public DateTime? LastUpdateCheck { get => _lastUpdateCheck; set { Set(() => LastUpdateCheck, ref _lastUpdateCheck, value); } }

        [Obsolete]
        public int LedsPerSpot { get => _ledsPerSpot; set { Set(() => LedsPerSpot, ref _ledsPerSpot, value); } }
        public bool MirrorX { get => _mirrorX; set { Set(() => MirrorX, ref _mirrorX, value); } }
        public bool MirrorY { get => _mirrorY; set { Set(() => MirrorY, ref _mirrorY, value); } }
        public int OffsetLed { get => _offsetLed; set { Set(() => OffsetLed, ref _offsetLed, value); } }
        public int OffsetLed2 { get => _offsetLed2; set { Set(() => OffsetLed2, ref _offsetLed2, value); } }
        public int OffsetLed3 { get => _offsetLed3; set { Set(() => OffsetLed3, ref _offsetLed3, value); } }
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
        public int SpotsX2 { get => _spotsX2; set { Set(() => SpotsX2, ref _spotsX2, value); } }
        public int SpotsY2 { get => _spotsY2; set { Set(() => SpotsY2, ref _spotsY2, value); } }
        public int SpotsX3 { get => _spotsX3; set { Set(() => SpotsX3, ref _spotsX3, value); } }
        public int SpotsY3 { get => _spotsY3; set { Set(() => SpotsY3, ref _spotsY3, value); } }
        public int SpotWidth { get => _spotWidth; set { Set(() => SpotWidth, ref _spotWidth, value); } }
        public bool StartMinimized { get => _startMinimized; set { Set(() => StartMinimized, ref _startMinimized, value); } }
        public bool TransferActive { get => _transferActive; set { Set(() => TransferActive, ref _transferActive, value); } }
        public bool Shuffle { get => _Shuffle; set { Set(() => Shuffle, ref _Shuffle, value); } }
        public bool shuffle { get => _shuffle; set { Set(() => shuffle, ref _shuffle, value); } }
        public bool CaptureActive { get => _CaptureActive; set { Set(() => CaptureActive, ref _CaptureActive, value); } }
        public bool ComportOpen { get => _ComportOpen; set { Set(() => ComportOpen, ref _ComportOpen, value); } }
        public bool Comport2Open { get => _Comport2Open; set { Set(() => Comport2Open, ref _Comport2Open, value); } }
        public bool Comport3Open { get => _Comport3Open; set { Set(() => Comport3Open, ref _Comport3Open, value); } }
        //public bool Comport4Open { get => _Comport4Open; set { Set(() => Comport4Open, ref _Comport4Open, value); } }
        public bool Comport5Open { get => _Comport5Open; set { Set(() => Comport5Open, ref _Comport5Open, value); } }
        public bool Advancesettings { get => _Advancesettings; set { Set(() => Advancesettings, ref _Advancesettings, value); } }


        public bool UseLinearLighting { get => _useLinearLighting; set { Set(() => UseLinearLighting, ref _useLinearLighting, value); } }
        

        public byte WhitebalanceRed { get => _whitebalanceRed; set { Set(() => WhitebalanceRed, ref _whitebalanceRed, value); } }
        public byte WhitebalanceGreen { get => _whitebalanceGreen; set { Set(() => WhitebalanceGreen, ref _whitebalanceGreen, value); } }
        public byte WhitebalanceBlue { get => _whitebalanceBlue; set { Set(() => WhitebalanceBlue, ref _whitebalanceBlue, value); } }
        public byte zoecounter { get => _zoecounter; set { Set(() => zoecounter, ref _zoecounter, value); } }
        public byte resetcounter { get => _resetcounter; set { Set(() => resetcounter, ref _resetcounter, value); } }
        public byte screeneffectcounter { get => _screeneffectcounter; set { Set(() => screeneffectcounter, ref _screeneffectcounter, value); } }
        public byte deskdirrection { get => _deskdirrection; set { Set(() =>deskdirrection, ref _deskdirrection, value); } }
        public byte tabindex { get => _tabindex; set { Set(() => tabindex, ref _tabindex, value); } }
        public byte genre { get => _genre; set { Set(() => genre, ref _genre, value); } }
        public bool caseenable{ get => _caseenable; set { Set(() => caseenable, ref _caseenable, value); } }
        public bool nodevice { get => _nodevice; set { Set(() => nodevice, ref _nodevice, value); } }
        public byte screencounter { get => _screencounter; set { Set(() => screencounter, ref _screencounter, value); } }
        public byte holdtimecounter { get => _holdtimecounter; set { Set(() => holdtimecounter, ref _holdtimecounter, value); } }
        public byte holdeffectcounter { get => _holdeffectcounter; set { Set(() => holdeffectcounter, ref _holdeffectcounter, value); } }
        public byte Faneffectcounter { get => _Faneffectcounter; set { Set(() => Faneffectcounter, ref _Faneffectcounter, value); } }
        public int devindex { get => _devindex; set { Set(() => devindex, ref _devindex, value); } }
        public int order_data0 { get => _order_data0; set { Set(() => order_data0, ref _order_data0, value); } }
        public int order_data1 { get => _order_data1; set { Set(() => order_data1, ref _order_data1, value); } }
        public int order_data2 { get => _order_data2; set { Set(() => order_data2, ref _order_data2, value); } }
        public int order_data3 { get => _order_data3; set { Set(() => order_data3, ref _order_data3, value); } }
        public int order_data4 { get => _order_data4; set { Set(() => order_data4, ref _order_data4, value); } }
        public int order_data5 { get => _order_data5; set { Set(() => order_data5, ref _order_data5, value); } }
        public int order_data6 { get => _order_data6; set { Set(() => order_data6, ref _order_data6, value); } }
        public int order_data7 { get => _order_data7; set { Set(() => order_data7, ref _order_data7, value); } }
        public int order_data8 { get => _order_data8; set { Set(() => order_data8, ref _order_data8, value); } }
        public int order_data9 { get => _order_data9; set { Set(() => order_data9, ref _order_data9, value); } }
        public int order_data10 { get => _order_data10; set { Set(() => order_data10, ref _order_data10, value); } }
        public int order_data11 { get => _order_data11; set { Set(() => order_data11, ref _order_data11, value); } }
        public int order_data12 { get => _order_data12; set { Set(() => order_data12, ref _order_data12, value); } }
        public int order_data13 { get => _order_data13; set { Set(() => order_data13, ref _order_data13, value); } }
        public int order_data14 { get => _order_data14; set { Set(() => order_data14, ref _order_data14, value); } }
        public int order_data15 { get => _order_data15; set { Set(() => order_data15, ref _order_data15, value); } }
        public byte buttoneffectcounter { get => _buttoneffectcounter; set { Set(() => buttoneffectcounter, ref _buttoneffectcounter, value); } }
        public byte audiodevice { get => _audiodevice; set { Set(() => audiodevice, ref _audiodevice, value); } }
        public byte screensizecounter { get => _screensizecounter; set { Set(() => screensizecounter, ref _screensizecounter, value); } }
        public byte screen2sizecounter { get => _screen2sizecounter; set { Set(() => screen2sizecounter, ref _screen2sizecounter, value); } }
        public byte screen3sizecounter { get => _screen3sizecounter; set { Set(() => screen3sizecounter, ref _screen3sizecounter, value); } }
        public byte devicecounter { get => _devicecounter; set { Set(() => devicecounter, ref _devicecounter, value); } }
        public byte visualcounter { get => _visualcounter; set { Set(() => visualcounter, ref _visualcounter, value); } }
        public byte effectcounter { get => _effectcounter; set { Set(() => effectcounter, ref _effectcounter, value); } }
        public byte fanmodecounter { get => _fanmodecounter; set { Set(() => fanmodecounter, ref _fanmodecounter, value); } }
        public byte zone1speedcounter { get => _zone1speedcounter; set { Set(() => zone1speedcounter, ref _zone1speedcounter, value); } }
        public byte zone2speedcounter { get => _zone2speedcounter; set { Set(() => zone2speedcounter, ref _zone2speedcounter, value); } }
        public byte zone3speedcounter { get => _zone3speedcounter; set { Set(() => zone3speedcounter, ref _zone3speedcounter, value); } }
        public byte LEDfanmodecounter { get => _LEDfanmodecounter; set { Set(() => LEDfanmodecounter, ref _LEDfanmodecounter, value); } }
        public byte buteffectcounter { get => _buteffectcounter; set { Set(() => buteffectcounter, ref _buteffectcounter, value); } }
        public byte speedcounter { get => _speedcounter; set { Set(() => speedcounter, ref _speedcounter, value); } }
        public string filemau { get => _filemau; set { Set(() => filemau, ref _filemau, value); } }
        public string filemauchip { get => _filemauchip; set { Set(() => filemauchip, ref _filemauchip, value); } }
        public byte methodcounter { get => _methodcounter; set { Set(() => methodcounter, ref _methodcounter, value); } }
        public byte color1R { get => _color1R; set { Set(() => color1R, ref _color1R, value); } }
        public byte color1G { get => _color1G; set { Set(() => color1G, ref _color1G, value); } }
        public byte color1B { get => _color1B; set { Set(() => color1B, ref _color1B, value); } }
        public byte color2R { get => _color2R; set { Set(() => color2R, ref _color2R, value); } }
        public byte color2G { get => _color2G; set { Set(() => color2G, ref _color2G, value); } }
        public byte color2B { get => _color2B; set { Set(() => color2B, ref _color2B, value); } }
        public byte color3R { get => _color3R; set { Set(() => color3R, ref _color3R, value); } }
        public byte color3G { get => _color3G; set { Set(() => color3G, ref _color3G, value); } }
        public byte color3B { get => _color3B; set { Set(() => color3B, ref _color3B, value); } }
        public byte color4R { get => _color4R; set { Set(() => color4R, ref _color4R, value); } }
        public byte color4G { get => _color4G; set { Set(() => color4G, ref _color4G, value); } }
        public byte color4B { get => _color4B; set { Set(() => color4B, ref _color4B, value); } }
        public byte color5R { get => _color5R; set { Set(() => color5R, ref _color5R, value); } }
        public byte color5G { get => _color5G; set { Set(() => color5G, ref _color5G, value); } }
        public byte color5B { get => _color5B; set { Set(() => color5B, ref _color5B, value); } }
        public byte color6R { get => _color6R; set { Set(() => color6R, ref _color6R, value); } }
        public byte color6G { get => _color6G; set { Set(() => color6G, ref _color6G, value); } }
        public byte color6B { get => _color6B; set { Set(() => color6B, ref _color6B, value); } }
        public byte color7R { get => _color7R; set { Set(() => color7R, ref _color7R, value); } }
        public byte color7G { get => _color7G; set { Set(() => color7G, ref _color7G, value); } }
        public byte color7B { get => _color7B; set { Set(() => color7B, ref _color7B, value); } }
        public byte color8R { get => _color8R; set { Set(() => color8R, ref _color8R, value); } }
        public byte color8G { get => _color8G; set { Set(() => color8G, ref _color8G, value); } }
        public byte color8B { get => _color8B; set { Set(() => color8B, ref _color8B, value); } }
        public byte color9R { get => _color9R; set { Set(() => color9R, ref _color9R, value); } }
        public byte color9G { get => _color9G; set { Set(() => color9G, ref _color9G, value); } }
        public byte color9B { get => _color9B; set { Set(() => color9B, ref _color9B, value); } }
        public byte color10R { get => _color10R; set { Set(() => color10R, ref _color10R, value); } }
        public byte color10G { get => _color10G; set { Set(() => color10G, ref _color10G, value); } }
        public byte color10B { get => _color10B; set { Set(() => color10B, ref _color10B, value); } }
        public Color color1 { get => _color1; set { Set(() => color1, ref _color1, value); } }
        public Color color2 { get => _color2; set { Set(() => color2, ref _color2, value); } }
        public Color color3 { get => _color3; set { Set(() => color3, ref _color3, value); } }
        public Color color4{ get => _color4; set { Set(() => color4, ref _color4, value); } }
        public Color color5 { get => _color5; set { Set(() => color5, ref _color5, value); } }
        public Color color6 { get => _color6; set { Set(() => color6, ref _color6, value); } }
        public Color color7 { get => _color7; set { Set(() => color7, ref _color7, value); } }
        public Color color8 { get => _color8; set { Set(() => color8, ref _color8, value); } }
        public Color CaseStatic { get => _CaseStatic; set { Set(() => CaseStatic, ref _CaseStatic, value); } }
        public Color ScreenStatic { get => _ScreenStatic; set { Set(() => ScreenStatic, ref _ScreenStatic, value); } }
        public Color DeskStatic { get => _DeskStatic; set { Set(() => DeskStatic, ref _DeskStatic, value); } }

        public Color color10 { get => _color10; set { Set(() => color10, ref _color10, value); } }
        public byte sincounter { get => _sincounter; set { Set(() => sincounter, ref _sincounter, value); } }
        
        public byte brightnesscounter { get => _brightnesscounter; set { Set(() => brightnesscounter, ref _brightnesscounter, value); } }
        public byte Port4Config { get => _Port4Config; set { Set(() => Port4Config, ref _Port4Config, value); } }
        public byte Port3Config { get => _Port3Config; set { Set(() => Port3Config, ref _Port3Config, value); } }
        public byte Port2Config { get => _Port2Config; set { Set(() => Port2Config, ref _Port2Config, value); } }
        public byte Port1Config { get => _Port1Config; set { Set(() => Port1Config, ref _Port1Config, value); } }

        public byte edgebrightnesscounter { get => _edgebrightnesscounter; set { Set(() => edgebrightnesscounter, ref _edgebrightnesscounter, value); } }
        public byte fanspeedcounter { get => _fanspeedcounter; set { Set(() => fanspeedcounter, ref _fanspeedcounter, value); } }
        public byte huecounter { get => _huecounter; set { Set(() => huecounter, ref _huecounter, value); } }
        public byte edgehuecounter { get => _edgehuecounter; set { Set(() => edgehuecounter, ref _edgehuecounter, value); } }
        public byte fanbrightnesscounter { get => _fanbrightnesscounter; set { Set(() => fanbrightnesscounter, ref _fanbrightnesscounter, value); } }
        public int musiccounter { get => _musiccounter; set { Set(() => musiccounter, ref _musiccounter, value); } }


        public bool SendRandomColors { get => _sendRandomColors; set { Set(() => SendRandomColors, ref _sendRandomColors, value); } }
        public bool fixedcolor { get => _fixedcolor; set { Set(() => fixedcolor, ref _fixedcolor, value); } }
        public bool method1 { get => _method1; set { Set(() => method1, ref _method1, value); } }
        public bool method2 { get => _method2; set { Set(() => method2, ref _method2, value); } }
        public bool method3 { get => _method3; set { Set(() => method3, ref _method3, value); } }
        public bool method4 { get => _method4; set { Set(() => method4, ref _method4, value); } }
        public bool zoe1 { get => _zoe1; set { Set(() => zoe1, ref _zoe1, value); } }
        public bool zoe2 { get => _zoe2; set { Set(() => zoe2, ref _zoe2, value); } }
        public bool zoe3 { get => _zoe3; set { Set(() => zoe3, ref _zoe3, value); } }
        public bool zoe4 { get => _zoe4; set { Set(() => zoe4, ref _zoe4, value); } }
        public bool zoe5 { get => _zoe5; set { Set(() => zoe5, ref _zoe5, value); } }
        public bool zoe6 { get => _zoe6; set { Set(() => zoe6, ref _zoe6, value); } }
        public bool zoe7 { get => _zoe7; set { Set(() => zoe7, ref _zoe7, value); } }
        public bool zoe8 { get => _zoe8; set { Set(() => zoe8, ref _zoe8, value); } }
        public bool effect { get => _effect; set { Set(() => effect, ref _effect, value); } }
        public bool music { get => _music; set { Set(() => music, ref _music, value); } }
        public bool musichue { get => _musichue; set { Set(() => musichue, ref _musichue, value); } }
        public byte lightstatus { get => _lightstatus; set { Set(() => lightstatus, ref _lightstatus, value); } }



        // Add new
        public bool screenOne { get => _screenOne; set { Set(() => screenOne, ref _screenOne, value); } }
        public bool hasPCI { get => _hasPCI; set { Set(() => hasPCI, ref _hasPCI, value); } }
        public bool hasRainpow { get => _hasRainpow; set { Set(() => hasRainpow, ref _hasRainpow, value); } }
        public bool hasNode { get => _hasNode; set { Set(() => hasNode, ref _hasNode, value); } }
        public bool hasUSB { get => _hasUSB; set { Set(() => hasUSB, ref _hasUSB, value); } }
        public bool hasPCISecond { get => _hasPCISecond; set { Set(() => hasPCISecond, ref _hasPCISecond, value); } }
        public bool hasUSBSecond { get => _hasUSBSecond; set { Set(() => hasUSBSecond, ref _hasUSBSecond, value); } }
        public bool hasUSBTwo { get => _hasUSBTwo; set { Set(() => hasUSBTwo, ref _hasUSBTwo, value); } }
        public bool hasScreenTwo { get => _hasScreenTwo; set { Set(() => hasScreenTwo, ref _hasScreenTwo, value); } }



        public Guid InstallationId { get; set; } = Guid.NewGuid();

    }
}
