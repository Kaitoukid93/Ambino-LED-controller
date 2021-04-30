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
       private ObservableCollection<string> _screenSizeSource;
       public ObservableCollection<string> ScreenSizeSource {
            get { return _screenSizeSource; }
            set
            {
                if (_screenSizeSource == value) return;
                _screenSizeSource = value;
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
        private readonly ViewModelBase _parentVm;
        public LightingViewModel(DeviceInfoDTO device, ViewModelBase parent)
        {
            _parentVm = parent;
            Card = device;
            ReadData();
        }
        public void ReadData()
        {
            ScreenSizeSource = new ObservableCollection<string>();
            ScreenSizeSource.Add("19-22 inch");
            ScreenSizeSource.Add("23-27 inch");
            ScreenSizeSource.Add("29 inch");
            ScreenSizeSource.Add("29 inch");
            ScreenSizeSource.Add("32 inch");
            ScreenSizeSource.Add("34 inch");
            ScreenSizeSource.Add("Kích thước khác");
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
        }
    }
}
