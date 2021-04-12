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
  public  class AddNewDeviceViewModel : ViewModelBase
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
        public AddNewDeviceViewModel()
        {
            ReadData();

        }
        public void ReadData()
        {
            Devices = new ObservableCollection<DeviceCard>();
            Devices.Add(new DeviceCard() { Title = "Ambino Test" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test1" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test2" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test3" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test4" });
            Devices.Add(new DeviceCard() { Title = "Ambino Test5" });
        }
    }
}
