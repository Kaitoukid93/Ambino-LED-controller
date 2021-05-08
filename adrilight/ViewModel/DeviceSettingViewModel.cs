using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel
{
   public class DeviceSettingViewModel : BaseViewModel
    {
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
        public DeviceSettingViewModel(ViewModelBase parent, SettingInfoDTO setting)
        {
            _parentVm = parent;
            SettingInfo = setting;
        }
    }
}
