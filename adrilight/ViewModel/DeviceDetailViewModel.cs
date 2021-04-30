using BO;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace adrilight.ViewModel
{
    public class DeviceDetailViewModel : ViewModelBase
    {

        private DeviceInfoDTO _card;
        public DeviceInfoDTO Card {
            get { return _card; }
            set
            {
                if (_card == value) return;
                _card = value;
                RaisePropertyChanged();
            }
        }
        private DeviceTab _tabType;
        public DeviceTab TabType {
            get { return _tabType; }
            set
            {
                if (_tabType == value) return;
                _tabType = value;
                RaisePropertyChanged();
                if (!_isInit)
                {
                    OnChangeTab(value);
                }
            }
        }
        public ICommand BackCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private readonly ViewModelBase _parentVm;
        private ViewModelBase _currentView;
        private ViewModelBase _generalView;
        private ViewModelBase _lightingView;
        private ViewModelBase _advanceView;
        private ViewModelBase _previewView;
        public ViewModelBase CurrentView {
            get { return _currentView; }
            set
            {
                _currentView = value;
                RaisePropertyChanged("CurrentView");
            }
        }
        private bool _isInit = false;
        public DeviceDetailViewModel(DeviceInfoDTO device, ViewModelBase parent)
        {
            _parentVm = parent;
            Card = device;
            ReadData();
        }
        public async void ShowDeleteDialog()
        {
            var view = new View.DeleteMessageDialog();
            DeleteMessageDialogViewModel dialogViewModel = new DeleteMessageDialogViewModel(_parentVm, Card);
            view.DataContext = dialogViewModel;
            await DialogHost.Show(view, "mainDialog");
        }
        public void ReadData()
        {
            _isInit = true;
            TabType = DeviceTab.General;
            _generalView = new GeneralDeviceViewModel(Card, _parentVm);
            CurrentView = _generalView;
            DeleteCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                ShowDeleteDialog();
            });
            BackCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                (_parentVm as MainViewViewModel).BackToDashboard();
            });
            _isInit = false;
        }
        public void OnChangeTab(DeviceTab tab)
        {
            switch (tab)
            {
                case DeviceTab.General:
                    _generalView = new GeneralDeviceViewModel(Card, _parentVm);
                    CurrentView = _generalView;
                    break;
                case DeviceTab.Lighting:
                    _lightingView = new LightingViewModel(Card, _parentVm);
                    CurrentView = _lightingView;
                    break;
                case DeviceTab.Preview:
                    _previewView = new PreviewViewModel();
                    CurrentView = _previewView;
                    break;
                case DeviceTab.Advance:
                    break;
                default:
                    break; 
            }
        }
    }
    public enum DeviceTab : byte
    {
        General=1,
        Lighting=2,
        Preview=3,
        Advance=4
    }
}
