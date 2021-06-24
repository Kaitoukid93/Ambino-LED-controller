using BO;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel
{
   public class GeneralDeviceViewModel : ViewModelBase
    {
        private IDeviceSettings _card;
        public IDeviceSettings Card {
            get { return _card; }
            set
            {
                if (_card == value) return;
                _card = value;
                RaisePropertyChanged();
            }
        }
        public IList<String> _AvailableComPorts;
        public IList<String> AvailableComPorts {
            get
            {


                _AvailableComPorts = SerialPort.GetPortNames().Concat(new[] { "Không có" }).ToList();
                _AvailableComPorts.Remove("COM1");

                return _AvailableComPorts;
            }
        }
        private readonly ViewModelBase _parentVm;
        public GeneralDeviceViewModel(IDeviceSettings device, ViewModelBase parent)
        {
            _parentVm = parent;
            Card = device;
        }
       
    }
}
