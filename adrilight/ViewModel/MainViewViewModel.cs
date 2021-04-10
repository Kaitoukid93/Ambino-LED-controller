using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adrilight.View;
using BO;
using GalaSoft.MvvmLight;

namespace adrilight.ViewModel
{
  public  class MainViewViewModel: ViewModelBase
    {
        public const string ImagePathFormat= "pack://application:,,,/adrilight;component/View/Images/{0}";
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
        private ViewModelBase _generalView;
        public ViewModelBase CurrentView {
            get { return _currentView; }
            set
            {
                _currentView = value;
                RaisePropertyChanged("CurrentView");
            }
        }
        #endregion
        public void ReadData()
        {
            LoadMenu();
            LoadMenuByType(true);
            _allDeviceView = new AllDeviceViewModel(this);
            
            CurrentView = _allDeviceView;
        }
        public void GotoChild(DeviceCard card)
        {
            _generalView = new GeneralDeviceViewModel(card,this);
            CurrentView = _generalView;
            IsDashboardType = false;
        }
        /// <summary>
        /// Load vertical menu
        /// </summary>
        public void LoadMenu()
        {
            MenuItems = new ObservableCollection<VerticalMenuItem>();
            MenuItems.Add(new VerticalMenuItem() { Text = "Dashboard", Images = string.Format(ImagePathFormat, "new theme/advance@2x.png"), IsActive = true, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = "Device Settings", Images = string.Format(ImagePathFormat, "new theme/2x/new usb@2x.png"), IsActive = false, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = "App Settings", Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = "FAQ", Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.Dashboard });
            MenuItems.Add(new VerticalMenuItem() { Text = "General", Images = string.Format(ImagePathFormat, "new theme/advance@2x.png"), IsActive = true, Type = MenuButtonType.General });
            MenuItems.Add(new VerticalMenuItem() { Text = "Lighting", Images = string.Format(ImagePathFormat, "new theme/2x/new usb@2x.png"), IsActive = false, Type = MenuButtonType.General });
            MenuItems.Add(new VerticalMenuItem() { Text = "Preview", Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.General });
            MenuItems.Add(new VerticalMenuItem() { Text = "Advance", Images = string.Format(ImagePathFormat, "new theme/General setup@2x.png"), IsActive = false, Type = MenuButtonType.General });
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
        
        public MainViewViewModel()
        {
            ReadData();
        }
    }
}
