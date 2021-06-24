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
 public  class DeleteMessageDialogViewModel: ViewModelBase
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
        public ICommand DeleteCommand { get; set; }
        public ViewModelBase _parentVm;
        public DeleteMessageDialogViewModel(ViewModelBase parent, IDeviceSettings device)
        {
            Card = device;
            _parentVm = parent;
            DeleteCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
               // some action
            });
            
        }
    }
}
