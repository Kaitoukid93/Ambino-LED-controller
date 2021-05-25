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
        public LightingViewModel(DeviceInfoDTO device, ViewModelBase parent, ISpotSet spotSet)
        {
            this.SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            PreviewSpots = spotSet.Spots;
            _parentVm = parent;
            ReadData();
            Card = device;
            Card.LEDNumber = 30;
           
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
