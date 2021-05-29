using adrilight.Fakes;
using adrilight.Util;
using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace adrilight.ViewModel
{
  public  class LightingViewModel : ViewModelBase
    {
        private DeviceInfoDTO _card;
        public DeviceInfoDTO Card {
            get { return _card; }
            set
            {
                if (_card == value) return;
                
                _card = value;
                
                RaisePropertyChanged();
            }
        }
        private SettingInfoDTO _settingInfo;
        public SettingInfoDTO SettingInfo {
            get { return _settingInfo; }
            set
            {
                if (_settingInfo == value) return;
                _settingInfo = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> _caseEffects;
        public ObservableCollection<string> CaseEffects {
            get { return _caseEffects; }
            set
            {
                if (_caseEffects == value) return;
                _caseEffects = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<CollectionItem> _collectionItm;
        public ObservableCollection<CollectionItem> CollectionItems {
            get { return _collectionItm; }
            set
            {
                if (_collectionItm == value) return;
                _collectionItm = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<string> _screenEffects;
        public ObservableCollection<string> ScreenEffects {
            get { return _screenEffects; }
            set
            {
                if (_screenEffects == value) return;
                _screenEffects = value;
                RaisePropertyChanged();
            }
        }
        public ISpot[] _previewSpots;
        public ISpot[] PreviewSpots {
            get => _previewSpots;
            set
            {
                _previewSpots = value;
                RaisePropertyChanged();
            }
        }
        private readonly ViewModelBase _parentVm;

        private ISpotSet spotSet;
        public ISpotSet SpotSet {
            get { return spotSet; }
            set
            {
                if (spotSet == value) return;
                spotSet = value;
                RaisePropertyChanged("SpotSet");
                PreviewSpots = spotSet.Spots;
            }
        }
        private DeviceRainbow _rainbow;
        public DeviceRainbow Rainbow {
            get { return _rainbow; }
            set
            {
                if (_rainbow == value) return;
                _rainbow = value;
                RaisePropertyChanged();
            }
        }
        private DeviceStaticColor _staticcolor;
        public DeviceStaticColor StaticColor {
            get { return _staticcolor; }
            set
            {
                if (_staticcolor == value) return;
                _staticcolor = value;
                RaisePropertyChanged();
            }
        }
        private DeviceMusic _music;
        public DeviceMusic Music {
            get { return _music; }
            set
            {
                if (_music == value) return;
                _music = value;
                RaisePropertyChanged();
            }
        }
        private DeviceAtmosphere _atmosphere;
        public DeviceAtmosphere Atmosphere {
            get { return _atmosphere; }
            set
            {
                if (_atmosphere == value) return;
                _atmosphere = value;
                RaisePropertyChanged();
            }
        }
        public LightingViewModel(DeviceInfoDTO device, ViewModelBase parent, SettingInfoDTO setting)
        {
            this.Card = device;
            this.SettingInfo = setting;
            this.SpotSet = new SpotSet(Card);
            PreviewSpots = SpotSet.Spots;
            _parentVm = parent;
            ReadData();
            Card = device;
           // Card.LEDNumber = 30;
            Rainbow = new DeviceRainbow(Card, SpotSet, this, SettingInfo);
            StaticColor = new DeviceStaticColor(Card, SpotSet, this, SettingInfo);
            Atmosphere = new DeviceAtmosphere(Card, SpotSet, this, SettingInfo);
            Music = new DeviceMusic(Card, SpotSet, this, SettingInfo);
        }
        public IList<String> _AvailableDisplays;
        public IList<String> AvailableDisplays {
            get
            {
                var listDisplay = new List<String>();
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    
                    listDisplay.Add(screen.DeviceName);
                }
                _AvailableDisplays = listDisplay;
                return _AvailableDisplays;
            }
        }
       
        public void ReadData()
        {
          
            CaseEffects = new ObservableCollection<string>
      {
           "Sáng theo hiệu ứng",
           "Sáng theo màn hình",
           "Sáng màu tĩnh",
           "Sáng theo nhạc",
           "Đồng bộ Mainboard",
           "Tắt",
           "Gifxelation",
           "Pixelation",
           "Ambilation"


        };
            ScreenEffects = new ObservableCollection<string>
      {
            "Sáng theo màn hình",
           "Sáng theo dải màu",
           "Sáng màu tĩnh",
           "Sáng theo nhạc",
           "Atmosphere"

        };
            CollectionItems = new ObservableCollection<CollectionItem>();
            CollectionItems.Add(new CollectionItem() { Value = 1, Text = "ARGB-1" });
            CollectionItems.Add(new CollectionItem() { Value = 1, Text = "ARGB-2",IsSelected=true });
            CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-1" });
            CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-2" });
            CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-3" });
            CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-4" });
           
        }
        
    }
}
