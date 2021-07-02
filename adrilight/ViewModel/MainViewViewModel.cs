using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using adrilight.View;
using adrilight.ViewModel.Factories;
using BO;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace adrilight.ViewModel
{
  public  class MainViewViewModel: BaseViewModel
    {
        private string JsonPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "adrilight\\");

        private string JsonFileNameAndPath => Path.Combine(JsonPath, "adrilight-settinginfo.json");
        #region constant string
        public const string ImagePathFormat= "pack://application:,,,/adrilight;component/View/Images/{0}";
        public const string dashboard = "Dashboard";
        public const string deviceSetting = "Device Settings";
        public const string appSetting = "App Settings";
        public const string faq = "FAQ";
        public const string general = "General";
        public const string lighting = "Lighting";
        #endregion
        #region property
        private ObservableCollection<VerticalMenuItem> _menuItems;
        public ObservableCollection<VerticalMenuItem> MenuItems {
            get { return _menuItems; }
            set
            {
                if (_menuItems == value) return;
                _menuItems = value;
                RaisePropertyChanged();
            }
        }
        
        private bool _isDashboardType = true;
        public bool IsDashboardType {
            get { return _isDashboardType; }
            set
            {
                if (_isDashboardType == value) return;
                _isDashboardType = value;
                RaisePropertyChanged();
                LoadMenuByType(value);
            }
        }
        private SettingInfoDTO _settingInfo ;
        public SettingInfoDTO SettingInfo {
            get { return _settingInfo; }
            set
            {
                if (_settingInfo == value) return;
                if (_settingInfo != null)
                    _settingInfo.PropertyChanged -= _settingInfo_PropertyChanged;
                _settingInfo = value;
                if(_settingInfo!=null)
                _settingInfo.PropertyChanged += _settingInfo_PropertyChanged;
                RaisePropertyChanged();
            }
        }

        private void _settingInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SettingInfo.AutoStartWithWindows):
                    if (SettingInfo.AutoStartWithWindows)
                    {
                        StartUpManager.AddApplicationToCurrentUserStartup();
                    }
                    else
                    {
                        StartUpManager.RemoveApplicationFromCurrentUserStartup();
                    }
                    break;
                default:
                    break;
            }
            
        }

        private ViewModelBase _currentView;
        
        private ViewModelBase _detailView;
        private ViewModelBase _deviceSettingView;
        private ViewModelBase _appSettingView;
        private ViewModelBase _faqSettingView;
        public ViewModelBase CurrentView {
            get { return _currentView; }
            set
            {
                _currentView = value;
                RaisePropertyChanged("CurrentView");
            }
        }
        private IDeviceSettings _currentDevice;
        public IDeviceSettings CurrentDevice {
            get { return _currentDevice; }
            set
            {
                if (_currentDevice == value) return;
                if (_currentDevice != null) _currentDevice.PropertyChanged -= _currentDevice_PropertyChanged;
                _currentDevice = value;
                if (_currentDevice != null) _currentDevice.PropertyChanged += _currentDevice_PropertyChanged;
                RaisePropertyChanged("CurrentDevice");
               
            }
        }

        private void _currentDevice_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!IsDashboardType)
            {
                WriteDeviceInfoJson();
            }
        }

        public ICommand SelectMenuItem { get; set; }
        #endregion

        private IViewModelFactory<AllDeviceViewModel> _allDeviceView;
        private bool isPreview = false;
        public MainViewViewModel()
        {


        }
        public override  void ReadData()
        {
            LoadMenu();
            LoadMenuByType(true);
          
            
            CurrentView = _allDeviceView.CreateViewModel();
            SelectMenuItem = new RelayCommand<VerticalMenuItem>((p) => {
                return true;
            }, (p) =>
            {
                ChangeView(p);
            });
            SettingInfo = new SettingInfoDTO();
          var setting=  LoadSettingIfExists();
            if (setting != null)
            {
                SettingInfo.AutoAddNewDevice = setting.autoaddnewdevice;
                SettingInfo.AutoConnectNewDevice = setting.autoconnectnewdevice;
                SettingInfo.AutoDeleteConfigWhenDisconnected = setting.autodeleteconfigwhendisconected;
                SettingInfo.AutoStartWithWindows = setting.autostartwithwindows;
                SettingInfo.DefaultName = setting.defaultname;
                SettingInfo.DisplayConnectionStatus = setting.displayconnectionstatus;
                SettingInfo.DisplayLightingStatus = setting.displaylightingstatus;
                SettingInfo.IsDarkMode = setting.isdarkmode;
                SettingInfo.PushNotificationWhenNewDeviceConnected = setting.pushnotificationwhennewdeviceconnected;
                SettingInfo.PushNotificationWhenNewDeviceDisconnected = setting.pushnotificationwhennewdevicedisconnected;
                SettingInfo.StartMinimum = setting.startminimum;
                SettingInfo.PrimaryColor=(Color )ColorConverter.ConvertFromString(setting.primarycolor);
                
            }
            else
            {
                SettingInfo.PrimaryColor = Colors.White;
            }
        }
        public void WriteSettingJson()
        {
            var json = JsonConvert.SerializeObject(SettingInfo.GetSettingInfo(), Formatting.Indented);
            Directory.CreateDirectory(JsonPath);
            File.WriteAllText(JsonFileNameAndPath, json);
        }
        public SettingInfo LoadSettingIfExists()
        {
            if (!File.Exists(JsonFileNameAndPath)) return null;

            var json = File.ReadAllText(JsonFileNameAndPath);

            var setting = JsonConvert.DeserializeObject<SettingInfo>(json);

            return setting;
        }
        /// <summary>
        /// Change View
        /// </summary>
        /// <param name="menuItem"></param>
        public void ChangeView(VerticalMenuItem menuItem)
        {
            switch (menuItem.Text)
            {
                case dashboard:
                    
                    CurrentView = _allDeviceView.CreateViewModel();
                    IsDashboardType = true;
                    break;
                case deviceSetting:
                    _deviceSettingView = new DeviceSettingViewModel(this,SettingInfo);
                    CurrentView = _deviceSettingView;
                    IsDashboardType = true;
                    break;
                case appSetting:
                    _appSettingView = new AppSettingViewModel(this, SettingInfo);
                    CurrentView = _appSettingView;
                    IsDashboardType = true; 
                    break;
                case faq:
                    _faqSettingView = new FAQViewModel();
                    CurrentView = _faqSettingView;
                    IsDashboardType = true; 
                    break;
                case general:
                    _detailView = new DeviceDetailViewModel(CurrentDevice,this,SettingInfo);
                    CurrentView = _detailView;
                    IsDashboardType = false;
                  
                    break;
                case lighting:
                    if (_detailView==null)
                        _detailView = new DeviceDetailViewModel(CurrentDevice,this, SettingInfo);
                    ((DeviceDetailViewModel)_detailView).TabType = DeviceTab.Lighting;
                    CurrentView = _detailView;
                    IsDashboardType = false;
                    break;
                default:
                    break;
            }
            SetMenuItemActiveStatus(menuItem.Text);
        }
         public void WriteDeviceInfoJson()
        {
            if (_allDeviceView == null) return;
            ((AllDeviceViewModel)_allDeviceView).WriteJson();
        }
        public void GotoChild(IDeviceSettings card)
        {
            _detailView = new DeviceDetailViewModel(card, this,SettingInfo);
            CurrentView = _detailView;
            IsDashboardType = false;
            CurrentDevice = card;
            SetMenuItemActiveStatus(general);
        }
        public void BackToDashboard()
        {
           
            CurrentView = _allDeviceView.CreateViewModel();
            IsDashboardType = true;
            SetMenuItemActiveStatus(dashboard);
        }
        public void BackToDashboardAndDelete(IDeviceSettings device)
        {
            if (_allDeviceView != null)
            {
                ((AllDeviceViewModel)_allDeviceView).DeleteCard(device);
                CurrentView = _allDeviceView.CreateViewModel();
                IsDashboardType = true;
                SetMenuItemActiveStatus(dashboard);
            }
        }
        /// <summary>
        /// Load vertical menu
        /// </summary>
        public void LoadMenu()
        {
            MenuItems = new ObservableCollection<VerticalMenuItem>();
            MenuItems.Add(new VerticalMenuItem() { Text =dashboard, Images = string.Format(ImagePathFormat, "new theme/advance@2x.png"), IsActive = true, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = deviceSetting, Images = string.Format(ImagePathFormat, "new theme/2x/new usb@2x.png"), IsActive = false, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text =appSetting, Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = faq, Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = general, Images = string.Format(ImagePathFormat, "new theme/advance@2x.png"), IsActive = true, Type = MenuButtonType.General });
            MenuItems.Add(new VerticalMenuItem() { Text = lighting, Images = string.Format(ImagePathFormat, "new theme/2x/new usb@2x.png"), IsActive = false, Type = MenuButtonType.General });
         
        }
        /// <summary>
        /// set active state
        /// </summary>
        /// <param name="key"></param>
        public void SetMenuItemActiveStatus(string key)
        {
            foreach (var item in MenuItems)
            {
                item.IsActive = item.Text == key;
            }
        }
        /// <summary>
        /// hide show vertical menu item
        /// </summary>
        /// <param name="isDashboard"></param>
        private void LoadMenuByType(bool isDashboard)
        {
            if (MenuItems == null) return;
            foreach (var item in MenuItems)
            {
                item.IsVisible =item.Type==MenuButtonType.Dashboard? isDashboard:!isDashboard;
            }
            RaisePropertyChanged(nameof(MenuItems));
        }
        
        
    }
}
