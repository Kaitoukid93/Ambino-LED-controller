using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel
{
   public class GeneralDeviceViewModel : ViewModelBase
    {
        private DeviceCard _card;
        public DeviceCard Card {
            get { return _card; }
            set
            {
                if (_card == value) return;
                _card = value;
                RaisePropertyChanged();
            }
        }
        private readonly ViewModelBase _parentVm;
        public GeneralDeviceViewModel(DeviceCard device, ViewModelBase parent)
        {
            _parentVm = parent;
            Card = device;
        }
    }
}
