using BO;
using ColorPickerWPF;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace adrilight.ViewModel
{
  public  class AppSettingViewModel: BaseViewModel
    {
        public ICommand SelectColorCommand { get; set; }
        private Color _selectedColor;
        public Color SelectedColor {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor == value) return;
                _selectedColor = value;
                RaisePropertyChanged();
                if(SettingInfo!=null)
                SettingInfo.PrimaryColor = value;
            }
        }
        private SettingInfoDTO _settingInfo;
        public SettingInfoDTO SettingInfo {
            get { return _settingInfo; }
            set
            {
                if (_settingInfo == value) return;
                if (_settingInfo != null)
                {
                    _settingInfo.PropertyChanged -= _settingInfo_PropertyChanged;
                }
                _settingInfo = value;
                if (value != null)
                {
                    _settingInfo.PropertyChanged += _settingInfo_PropertyChanged;
                }
                RaisePropertyChanged();
            }
        }
        private bool _isWriting = false;
        private void _settingInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_isWriting) return;
            if (_mainView != null)
            {
                _isWriting = true;
                _mainView.WriteSettingJson();
                _isWriting = false;
            }
        }
       
        private readonly ViewModelBase _parentVm;
        private MainViewViewModel _mainView => _parentVm as MainViewViewModel;
        public AppSettingViewModel(ViewModelBase parent, SettingInfoDTO setting)
        {
            _isWriting = true;
            _parentVm = parent;
            SettingInfo = setting;
            SelectedColor = SettingInfo.PrimaryColor;
            ReadData();
            _isWriting = false;
        }
        public override void ReadData()
        {
           
            SelectColorCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                Color color;

                bool ok = ColorPickerWindow.ShowDialog(out color);
                if (ok)
                {
                    SelectedColor = color;
                }
            });
        }
    }
}
