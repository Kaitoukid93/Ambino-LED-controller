using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace adrilight.ViewModel
{
    class AllNewDeviceViewModel : ViewModelBase
    {
        private ObservableCollection<DeviceSettings> _devices;
        public ObservableCollection<DeviceSettings> Devices {
            get { return _devices; }
            set
            {
                if (_devices == value) return;
                _devices = value;
                RaisePropertyChanged();
            }
        }
        public ICommand SelectDeviceCommand { get; set; }
        public ViewModelBase _parentVm;
        public AllNewDeviceViewModel(ViewModelBase parent)
        {
            _parentVm = parent;
            ReadData();

        }
        public void ReadData()
        {
            Devices = new ObservableCollection<DeviceSettings>();
            Devices.Add(new DeviceSettings() { DeviceName = "Ambino Basic", DeviceId = 1 });
            Devices.Add(new DeviceSettings() { DeviceName = "Ambino Edge", DeviceId = 2 });
            Devices.Add(new DeviceSettings() { DeviceName = "Ambino HUBV2", DeviceId = 3 });
            SelectDeviceCommand = new RelayCommand<DeviceSettings>((p) => {
                return true;
            }, (p) =>
            {
                ((AddNewDeviceViewModel)_parentVm).GoToChangePort(p);
            });
        }
    }
}
