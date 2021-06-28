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
        private ObservableCollection<IDeviceSettings> _devices;
        public ObservableCollection<IDeviceSettings> Devices {
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
            Devices = new ObservableCollection<IDeviceSettings>();
            Devices.Add(new DeviceSettings() { DeviceName = "Ambino Basic", DeviceType = "Basic" });
            Devices.Add(new DeviceSettings() { DeviceName = "Ambino Edge", DeviceType = "EDGE" });
            Devices.Add(new DeviceSettings() { DeviceName = "Ambino HUBV2", DeviceType = "Hub" });
            SelectDeviceCommand = new RelayCommand<IDeviceSettings>((p) => {
                return true;
            }, (p) =>
            {
                ((AddNewDeviceViewModel)_parentVm).GoToChangePort(p);
            });
        }
    }
}
