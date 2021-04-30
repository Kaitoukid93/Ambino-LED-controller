using BO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
namespace adrilight.ViewModel
{
   public class AllDeviceViewModel : BaseViewModel
    {
        private ObservableCollection<DeviceInfoDTO> _cards;
        public ObservableCollection<DeviceInfoDTO> Cards {
            get { return _cards; }
            set
            {
                if (_cards == value) return;
                _cards = value;
                RaisePropertyChanged();
            }
        }
        public ICommand SelectCardCommand { get; set; }
        public ICommand ShowAddNewCommand { get; set; }
        private readonly ViewModelBase _parentVm;
        public AllDeviceViewModel(ViewModelBase parent)
        {
            _parentVm = parent;
            ReadData();
        }
        
        public void LoadCard()
        {
            Cards = new ObservableCollection<DeviceInfoDTO>();
       
           
        }
        public void ReadData()
        {
            SelectCardCommand = new RelayCommand<DeviceInfoDTO>((p) => {
                return p != null;
            }, (p) =>
              {
                  (_parentVm as MainViewViewModel).GotoChild(p);
              });
            LoadCard();
            ShowAddNewCommand = new RelayCommand<DeviceInfoDTO>((p) => {
                return true;
            }, (p) =>
            {
                ShowAddNewDialog();
            });
        }
        public async void ShowAddNewDialog()
        {
            var vm = new ViewModel.AddNewDeviceViewModel();
            var view = new View.AddNewDevice();
            view.DataContext = vm;
            bool addResult =(bool) (await DialogHost.Show(view, "mainDialog"));
            if (addResult)
            {
                Cards.Add(vm.Device);
            }
        }
    }
}
