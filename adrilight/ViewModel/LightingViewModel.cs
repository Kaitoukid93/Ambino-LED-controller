using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly ViewModelBase _parentVm;
        public LightingViewModel(DeviceInfoDTO device, ViewModelBase parent)
        {
            _parentVm = parent;
            ReadData();
            Card = device;
           
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
