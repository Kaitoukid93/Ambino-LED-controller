using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace adrilight.ViewModel
{
   public class ChangeDeviceNameViewModel : ViewModelBase
    {
        private DeviceCard _device;
        public DeviceCard Device {
            get { return _device; }
            set
            {
                if (_device == value) return;
                _device = value;
                RaisePropertyChanged();
            }
        }
        public ICommand BackCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ViewModelBase _parentVm;
        public ChangeDeviceNameViewModel(ViewModelBase parent, DeviceCard device)
        {
            Device = device;
            _parentVm = parent;
            BackCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                ((AddNewDeviceViewModel)_parentVm).GoAllDeviceView();
            });
        }
    }
}
