using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace adrilight.ViewModel
{
   public class ChangePortViewModel: ViewModelBase
    {
       
        private IDeviceSettings _device;
        public IDeviceSettings Device {
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
        public IList<String> _AvailableComPorts;
        public IList<String> AvailableComPorts {
            get
            {


                _AvailableComPorts = SerialPort.GetPortNames().Concat(new[] { "Không có" }).ToList();
                _AvailableComPorts.Remove("COM1");

                return _AvailableComPorts;
            }
        }
   

    public ChangePortViewModel(ViewModelBase parent, IDeviceSettings device)
        {
            Device = device;
            _parentVm = parent;
            BackCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                ((AddNewDeviceViewModel)_parentVm).GoAllDeviceView();
            });
            NextCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                ((AddNewDeviceViewModel)_parentVm).GoToChangeNameView(Device);
            });
        }
    }
}
