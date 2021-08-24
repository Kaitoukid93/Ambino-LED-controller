using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BO
{
  public  class SettingInfoDTO : NotifyObject
    {
        private bool _autoaddnewdevice;
        public bool AutoAddNewDevice
        {
            get { return _autoaddnewdevice; }
            set
            {
                if (_autoaddnewdevice == value) return;
                _autoaddnewdevice = value;
                OnPropertyChanged();
            }
        }
        private bool _autoconnectnewdevice;
        public bool AutoConnectNewDevice
        {
            get { return _autoconnectnewdevice; }
            set
            {
                if (_autoconnectnewdevice == value) return;
                _autoconnectnewdevice = value;
                OnPropertyChanged();
            }
        }
        private string _defaultname;
        public string DefaultName
        {
            get { return _defaultname; }
            set
            {
                if (_defaultname == value) return;
                _defaultname = value;
                OnPropertyChanged();
            }
        }
        private bool _displayconnectionstatus;
        public bool DisplayConnectionStatus
        {
            get { return _displayconnectionstatus; }
            set
            {
                if (_displayconnectionstatus == value) return;
                _displayconnectionstatus = value;
                OnPropertyChanged();
            }
        }
        private bool _displaylightingstatus;
        public bool DisplayLightingStatus
        {
            get { return _displaylightingstatus; }
            set
            {
                if (_displaylightingstatus == value) return;
                _displaylightingstatus = value;
                OnPropertyChanged();
            }
        }
        private bool _autodeleteconfigwhendisconected;
        public bool AutoDeleteConfigWhenDisconnected
        {
            get { return _autodeleteconfigwhendisconected; }
            set
            {
                if (_autodeleteconfigwhendisconected == value) return;
                _autodeleteconfigwhendisconected = value;
                OnPropertyChanged();
            }
        }
        private bool _isdarkmode;
        public bool IsDarkMode
        {
            get { return _isdarkmode; }
            set
            {
                if (_isdarkmode == value) return;
                _isdarkmode = value;
                OnPropertyChanged();
            }
        }
        private bool _autostartwithwindows;
        public bool AutoStartWithWindows
        {
            get { return _autostartwithwindows; }
            set
            {
                if (_autostartwithwindows == value) return;
                _autostartwithwindows = value;
                OnPropertyChanged();
            }
        }
        private bool _pushnotificationwhennewdeviceconnected;
        public bool PushNotificationWhenNewDeviceConnected
        {
            get { return _pushnotificationwhennewdeviceconnected; }
            set
            {
                if (_pushnotificationwhennewdeviceconnected == value) return;
                _pushnotificationwhennewdeviceconnected = value;
                OnPropertyChanged();
            }
        }
        private bool _pushnotificationwhennewdevicedisconnected;
        public bool PushNotificationWhenNewDeviceDisconnected
        {
            get { return _pushnotificationwhennewdevicedisconnected; }
            set
            {
                if (_pushnotificationwhennewdevicedisconnected == value) return;
                _pushnotificationwhennewdevicedisconnected = value;
                OnPropertyChanged();
            }
        }
        private bool _startminimum;
        public bool StartMinimum
        {
            get { return _startminimum; }
            set
            {
                if (_startminimum == value) return;
                _startminimum = value;
                OnPropertyChanged();
            }
        }
        private Color _primaryColor;
        public Color PrimaryColor
        {
            get { return _primaryColor; }
            set
            {
                if (_primaryColor == value) return;
                _primaryColor = value;
                OnPropertyChanged();
            }
        }
        private bool _transferActive = true;
        public bool TransferActive
        {
            get { return _transferActive; }
            set
            {
                if (_transferActive == value) return;
                _transferActive = value;
                OnPropertyChanged();
            }
        }
        public SettingInfo GetSettingInfo()
        {
            return new SettingInfo()
            {
                autoaddnewdevice = AutoAddNewDevice,
                autoconnectnewdevice = AutoConnectNewDevice,
                autodeleteconfigwhendisconected = AutoDeleteConfigWhenDisconnected,
                autostartwithwindows = AutoStartWithWindows,
                defaultname = DefaultName,
                displayconnectionstatus = DisplayConnectionStatus,
                displaylightingstatus = DisplayLightingStatus,
                isdarkmode = IsDarkMode,
                pushnotificationwhennewdeviceconnected = PushNotificationWhenNewDeviceConnected,
                pushnotificationwhennewdevicedisconnected = PushNotificationWhenNewDeviceDisconnected,
                startminimum = StartMinimum,
                primarycolor = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", PrimaryColor.A, PrimaryColor.R, PrimaryColor.G, PrimaryColor.B)

            };
        }
    }
}
