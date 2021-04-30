using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private int _brightness;
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
            get { return _capturesource; }
            set
            {
                if (_capturesource == value) return;
                _capturesource = value;
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
        private string _staticcolor;
        public string Staticcolor
        {
            get { return _staticcolor; }
            set
            {
                if (_staticcolor == value) return;
                _staticcolor = value;
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
        public string StartChar => GetStartChar();
       public string GetStartChar()
        {
            return string.IsNullOrEmpty(DeviceName) ? string.Empty : DeviceName[0].ToString().ToUpper();
        }
    }


}
