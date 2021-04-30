using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using adrilight.View;
using BO;
using GalaSoft.MvvmLight;

namespace adrilight.ViewModel
{
  public  class MainViewViewModel: BaseViewModel
    {
        #region constant string
        public const string ImagePathFormat= "pack://application:,,,/adrilight;component/View/Images/{0}";
        public const string dashboard = "Dashboard";
        public const string deviceSetting = "Device Settings";
        public const string appSetting = "App Settings";
        public const string faq = "FAQ";
        public const string general = "General";
        public const string lighting = "Lighting";
        public const string preview = "Preview";
        public const string advance = "Advance";
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
        private ViewModelBase _currentView;
        private ViewModelBase _allDeviceView;
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
        private DeviceInfoDTO _currentDevice;
        public DeviceInfoDTO CurrentDevice {
            get { return _currentDevice; }
            set
            {
                _currentDevice = value;
                RaisePropertyChanged("CurrentDevice");
            }
        }
        public ICommand SelectMenuItem { get; set; }
        #endregion
        public override  void ReadData()
        {
            LoadMenu();
            LoadMenuByType(true);
            _allDeviceView = new AllDeviceViewModel(this);
            
            CurrentView = _allDeviceView;
            SelectMenuItem = new RelayCommand<VerticalMenuItem>((p) => {
                return true;
            }, (p) =>
            {
                ChangeView(p);
            });
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
                    _allDeviceView = new AllDeviceViewModel(this);
                    CurrentView = _allDeviceView;
                    IsDashboardType = true;
                    break;
                case deviceSetting:
                    _deviceSettingView = new DeviceSettingViewModel();
                    CurrentView = _deviceSettingView;
                    IsDashboardType = true;
                    break;
                case appSetting:
                    _appSettingView = new AppSettingViewModel();
                    CurrentView = _appSettingView;
                    IsDashboardType = true;
                    break;
                case faq:
                    _faqSettingView = new FAQViewModel();
                    CurrentView = _faqSettingView;
                    IsDashboardType = true;
                    break;
                case general:
                    _detailView = new DeviceDetailViewModel(CurrentDevice,this);
                    CurrentView = _detailView;
                    IsDashboardType = false;
                    break;
                case lighting:
                    if(_detailView==null)
                        _detailView = new DeviceDetailViewModel(CurrentDevice,this);
                    ((DeviceDetailViewModel)_detailView).TabType = DeviceTab.Lighting;
                    CurrentView = _detailView;
                    IsDashboardType = false;
                    break;
                case preview:
                    if (_detailView == null)
                        _detailView = new DeviceDetailViewModel(CurrentDevice, this);
                    ((DeviceDetailViewModel)_detailView).TabType = DeviceTab.Preview;
                    CurrentView = _detailView;
                    IsDashboardType = false;
                    break;
                case advance:
                    if (_detailView == null)
                        _detailView = new DeviceDetailViewModel(CurrentDevice, this);
                    ((DeviceDetailViewModel)_detailView).TabType = DeviceTab.Advance;
                    CurrentView = _detailView;
                    IsDashboardType = false;
                    break;
                default:
                    break;
            }
            SetMenuItemActiveStatus(menuItem.Text);
        }
       
        public void GotoChild(DeviceInfoDTO card)
        {
            _detailView = new DeviceDetailViewModel(card, this);
            CurrentView = _detailView;
            IsDashboardType = false;
            CurrentDevice = card;
            SetMenuItemActiveStatus(general);
        }
        public void BackToDashboard()
        {
            _allDeviceView = new AllDeviceViewModel(this);

            CurrentView = _allDeviceView;
            IsDashboardType = true;
            SetMenuItemActiveStatus(dashboard);
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
            MenuItems.Add(new VerticalMenuItem() { Text =preview, Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.General });
            MenuItems.Add(new VerticalMenuItem() { Text = advance, Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.General });
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
