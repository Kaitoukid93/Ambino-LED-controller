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
        private ObservableCollection<DeviceCard> _devices;
        public ObservableCollection<DeviceCard> Devices {
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
            Devices = new ObservableCollection<DeviceCard>();
            Devices.Add(new DeviceCard() { Title = "Ambino Test", Character = "A" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test1", Character = "B" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test2", Character = "C" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test3", Character = "D" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test4", Character = "D" });
            SelectDeviceCommand = new RelayCommand<DeviceCard>((p) => {
                return true;
            }, (p) =>
            {
                ((AddNewDeviceViewModel)_parentVm).GoToChangePort(p);
            });
        }
    }
}
