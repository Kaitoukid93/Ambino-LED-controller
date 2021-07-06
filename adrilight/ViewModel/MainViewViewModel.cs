using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using adrilight.Resources;
using adrilight.Spots;
using adrilight.View;
using adrilight.ViewModel.Factories;
using BO;
using GalaSoft.MvvmLight;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Ninject;
using Un4seen.BassWasapi;

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
        private VerticalMenuItem _selectedVerticalMenuItem ;
        public VerticalMenuItem SelectedVerticalMenuItem {
            get { return _selectedVerticalMenuItem; }
            set
            {
                if (_selectedVerticalMenuItem == value) return;
                _selectedVerticalMenuItem = value;
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
        private string _buildVersion = "";
        public string BuildVersion {
            get { return _buildVersion; }
            set
            {
                if (_buildVersion == value) return;
                _buildVersion = value;
                RaisePropertyChanged();

            }
        }
        private DateTime? _lastUpdate;
        public DateTime? LastUpdate {
            get { return _lastUpdate; }
            set
            {
                if (_lastUpdate == value) return;
                _lastUpdate = value;
                RaisePropertyChanged();

            }
        }
        private string _author = "";
        public string Author {
            get { return _author; }
            set
            {
                if (_author == value) return;
                _author = value;
                RaisePropertyChanged();

            }
        }
        private string _git = "";
        public string Git {
            get { return _git; }
            set
            {
                if (_git == value) return;
                _git = value;
                RaisePropertyChanged();

            }
        }
        private string _faq = "";
        public string FAQ {
            get { return _faq; }
            set
            {
                if (_faq == value) return;
                _faq = value;
                RaisePropertyChanged();

            }
        }
        private string _appName = "";
        public string AppName {
            get { return _appName; }
            set
            {
                if (_appName == value) return;
                _appName = value;
                RaisePropertyChanged();

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
        public ICommand BackCommand { get; set; }
        #endregion
        private ObservableCollection<IDeviceSettings> _cards;
        public ObservableCollection<IDeviceSettings> Cards {
            get { return _cards; }
            set
            {
                if (_cards == value) return;
                _cards = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<IDeviceSpotSet> _spotSets;
        public ObservableCollection<IDeviceSpotSet> SpotSets {
            get { return _spotSets; }
            set
            {
                if (_spotSets == value) return;
                _spotSets = value;
                RaisePropertyChanged();
            }
        }
        // [Inject, Named("0")]
        // public IDeviceSettings Card1 { get; set; }

        public ICommand SelectCardCommand { get; set; }
        public ICommand ShowAddNewCommand { get; set; }
        private IViewModelFactory<AllDeviceViewModel> _allDeviceView;
        private bool isPreview = false;
        private bool _isAddnew = false;
        private string JsonDeviceNameAndPath => Path.Combine(JsonPath, "adrilight-deviceInfos.json");
        public IList<String> _AvailableComPorts;
        public IList<String> AvailableComPorts {
            get
            {


                _AvailableComPorts = SerialPort.GetPortNames().Concat(new[] { "Không có" }).ToList();
                _AvailableComPorts.Remove("COM1");

                return _AvailableComPorts;
            }
        }
        private ObservableCollection<string> _caseEffects;
        public ObservableCollection<string> CaseEffects {
            get { return _caseEffects; }
            set
            {
                if (_caseEffects == value) return;
                _caseEffects = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<CollectionItem> _collectionItm;
        public ObservableCollection<CollectionItem> CollectionItems {
            get { return _collectionItm; }
            set
            {
                if (_collectionItm == value) return;
                _collectionItm = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<string> _screenEffects;
        public ObservableCollection<string> ScreenEffects {
            get { return _screenEffects; }
            set
            {
                if (_screenEffects == value) return;
                _screenEffects = value;
                RaisePropertyChanged();
            }
        }
        public IDeviceSpot[] _previewSpots;
        public IDeviceSpot[] PreviewSpots {
            get => _previewSpots;
            set
            {
                _previewSpots = value;
                RaisePropertyChanged();
            }
        }
        public IList<string> _AvailableAudioDevice = new List<string>();
        public IList<String> AvailableAudioDevice {
            get
            {
                _AvailableAudioDevice.Clear();
                int devicecount = BassWasapi.BASS_WASAPI_GetDeviceCount();
                string[] devicelist = new string[devicecount];
                for (int i = 0; i < devicecount; i++)
                {

                    var devices = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);

                    if (devices.IsEnabled && devices.IsLoopback)
                    {
                        var device = string.Format("{0} - {1}", i, devices.name);

                        _AvailableAudioDevice.Add(device);
                    }

                }

                return _AvailableAudioDevice;
            }
        }
        public int _audioDeviceID = -1;
        public int AudioDeviceID {
            get
            {
                if (CurrentDevice.SelectedAudioDevice > AvailableAudioDevice.Count)
                {
                    System.Windows.MessageBox.Show("Last Selected Audio Device is not Available");
                    return -1;
                }
                else
                {
                    var currentDevice = AvailableAudioDevice.ElementAt(CurrentDevice.SelectedAudioDevice);

                    var array = currentDevice.Split(' ');
                    _audioDeviceID = Convert.ToInt32(array[0]);
                    return _audioDeviceID;
                }

            }



        }
        public ObservableCollection<string> AvailablePalette { get; private set; }
        public IContext Context { get; }
        public IList<String> _AvailableDisplays;
        public IList<String> AvailableDisplays {
            get
            {
                var listDisplay = new List<String>();
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {

                    listDisplay.Add(screen.DeviceName);
                }
                _AvailableDisplays = listDisplay;
                return _AvailableDisplays;
            }
        }
        public ISpot[] _previewGif;
        public ISpot[] PreviewGif {
            get => _previewGif;
            set
            {
                _previewGif = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<string> AvailableFrequency { get; private set; }
        public ObservableCollection<string> AvailableMusicPalette { get; private set; }
        public ObservableCollection<string> AvailableMusicMode { get; private set; }
        public ICommand SelectGif { get; set; }
        public BitmapImage gifimage;
        public Stream gifStreamSource;
        private static int _gifFrameIndex = 0;
        private BitmapSource _contentBitmap;
        public BitmapSource ContentBitmap {
            get { return _contentBitmap; }
            set
            {
                if (value != _contentBitmap)
                {
                    _contentBitmap = value;
                    RaisePropertyChanged(() => ContentBitmap);

                }
            }
        }
        GifBitmapDecoder decoder;
       
        public MainViewViewModel(IDeviceSettings[] cards, IDeviceSpotSet[] deviceSpotSets)
        {
            Cards = new ObservableCollection<IDeviceSettings>();
            SpotSets = new ObservableCollection<IDeviceSpotSet>();
            foreach(IDeviceSettings card in cards)
            {
                Cards.Add(card);
            }
            foreach(IDeviceSpotSet spotSet in deviceSpotSets)
            {
                SpotSets.Add(spotSet);
            }
            


        }
        public void LoadCard()
        {
            //Cards = new ObservableCollection<IDeviceSettings>();
           // Cards.Add(Card1);


            //var settingsmanager = new UserSettingsManager();
            //var devices = settingsmanager.LoadDeviceIfExists();
            //if (devices != null)
            //{
            //    foreach (var item in devices)
            //    {
            //        var deviceInfo = new DeviceSettings() {
            //            Brightness = item.Brightness,
            //            SelectedDisplay = item.SelectedDisplay,
            //            WhitebalanceRed = item.WhitebalanceRed,
            //            DeviceId = item.DeviceID,
            //            DeviceName = item.DeviceName,
            //            DevicePort = item.DevicePort,
            //            DeviceSize = item.DeviceSize,
            //            DeviceType = item.DeviceType,
            //            //  FadeEnd = item.fadeend,
            //            //  FadeStart = item.fadestart,
            //            // GifMode = item.gifmode,
            //            // GifSource = item.gifsource,
            //            IsBreathing = item.IsBreathing,
            //            IsConnected = item.IsConnected,
            //            SelectedEffect = item.SelectedEffect,
            //            SelectedMusicMode = item.SelectedMusicMode,
            //            MSens = item.MSens,
            //            SelectedAudioDevice = item.SelectedAudioDevice,
            //            SelectedPalette = item.SelectedPalette,
            //            EffectSpeed = item.EffectSpeed,
            //            StaticColor = item.StaticColor,
            //            AtmosphereStart = item.AtmosphereStart,
            //            AtmosphereStop = item.AtmosphereStop,
            //            BreathingSpeed = item.BreathingSpeed,
            //            ColorFrequency = item.ColorFrequency,

            //            SelectedMusicPalette = item.SelectedMusicPalette,
            //            SpotHeight = item.SpotHeight,
            //            SpotsX = item.SpotsX,
            //            SpotsY = item.SpotsY,
            //            SpotWidth = item.SpotWidth,
            //            UseLinearLighting = item.UseLinearLighting,
            //            WhitebalanceBlue = item.WhitebalanceBlue,

            //            WhitebalanceGreen = item.WhitebalanceGreen
            //        };

            //        deviceInfo.PropertyChanged += DeviceInfo_PropertyChanged;
            //        Cards.Add(deviceInfo);
            //    }
            //}

        }
        private void DeviceInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (_isAddnew) return;
            //_isAddnew = true;
            //WriteJson();
            //_isAddnew = false;
        }
        public override  void ReadData()
        {
            LoadMenu();
            LoadMenuByType(true);
            ReadDataDevice();
            ReadFAQ();
            
            //CurrentView = _allDeviceView.CreateViewModel();
            SelectMenuItem = new RelayCommand<VerticalMenuItem>((p) => {
                return true;
            }, (p) =>
            {
                ChangeView(p);
            });
            SelectedVerticalMenuItem = MenuItems.FirstOrDefault();
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
            SelectCardCommand = new RelayCommand<IDeviceSettings>((p) => {
                return p != null;
            }, (p) =>
            {
                this.GotoChild(p);
            });
            ShowAddNewCommand = new RelayCommand<IDeviceSettings>((p) => {
                return true;
            }, (p) =>
            {
                ShowAddNewDialog();
            });
            BackCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
               BackToDashboard();
            });
        }
        public void ReadFAQ()
        {
            AppName = $"adrilight {App.VersionNumber}";
            BuildVersion = "xxxxxxxxxxxxxxxxxxxxxxxxxxx";
            LastUpdate = new DateTime(2020, 06, 01);
            Author = "zOe";
            Git = "xxxxxxx";
            FAQ = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.";
        }
        public void ReadDataDevice()
        {
            SelectGif = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                OpenFileDialog gifile = new OpenFileDialog();
                gifile.Title = "Chọn file gif";
                gifile.CheckFileExists = true;
                gifile.CheckPathExists = true;
                gifile.DefaultExt = "gif";
                gifile.Filter = "Image Files(*.gif)| *.gif";
                gifile.FilterIndex = 2;
                gifile.ShowDialog();

                if (!string.IsNullOrEmpty(gifile.FileName) && File.Exists(gifile.FileName))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(gifile.FileName);
                    image.EndInit();
                    gifimage = image;
                    var gifilepath = gifile.FileName;
                    gifStreamSource = new FileStream(gifilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    decoder = new GifBitmapDecoder(gifStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    // ImageBehavior.SetAnimatedSource(gifxel, image);



                    // _controller = ImageBehavior.GetAnimationController(gifxel);

                    // _controller.
                    // image.CopyPixels

                    // GifPlayPause = false;
                    ImageProcesser.DisposeGif();
                    ImageProcesser.DisposeStill();
                    if (ImageProcesser.LoadGifFromDisk(gifile.FileName))
                    {
                        CurrentDevice.GifFilePath = gifile.FileName;
                        //FrameToPreview();
                        // SerialManager.PushFrame();
                        ImageProcesser.ImageLoadState = ImageProcesser.LoadState.Gif;
                        //ResetSliders();
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Cannot load image.");
                    }

                }
            });
            AvailablePalette = new ObservableCollection<string>
       {
           "Rainbow",
           "Cloud",
           "Forest",
           "Sunset",
           "Scarlet",
           "Aurora",
           "France",
           "Lemon",
           "Badtrip",
           "Police",
           "Ice and Fire",
           "Custom"

        };
            CaseEffects = new ObservableCollection<string>
      {
           "Sáng theo hiệu ứng",
           "Sáng theo màn hình",
           "Sáng màu tĩnh",
           "Sáng theo nhạc",
           "Đồng bộ Mainboard",
           "Tắt",
           "Gifxelation",
           "Pixelation",
           "Ambilation"


        };
            ScreenEffects = new ObservableCollection<string>
      {
            "Sáng theo màn hình",
           "Sáng theo dải màu",
           "Sáng màu tĩnh",
           "Sáng theo nhạc",
           "Atmosphere",
            "Gifxelation"
        };
            AvailableMusicPalette = new ObservableCollection<string>
{
           "Rainbow",
           "Cafe",
           "Jazz",
           "Party",
           "Custom"


        };
            AvailableFrequency = new ObservableCollection<string>
{
           "1",
           "2",
           "3",
           "4"


        };
            AvailableMusicMode = new ObservableCollection<string>
{
          "Equalizer",
           "VU metter",
           "End to End",
           "Push Pull",
          "Symetric VU",
          "Floating VU",
          "Center VU",
          "Naughty boy"

        };
          

        }
        public async void ShowAddNewDialog()
        {
            var vm = new ViewModel.AddNewDeviceViewModel();
            var view = new View.AddNewDevice();
            view.DataContext = vm;
            bool addResult = (bool)(await DialogHost.Show(view, "mainDialog"));
            if (addResult)
            {
                try
                {
                    vm.Device.PropertyChanged += DeviceInfo_PropertyChanged;
                    _isAddnew = true;
                      Cards.Add(vm.Device);
                    WriteJson();
                    _isAddnew = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
               
            }
        }
        public void DeleteCard(IDeviceSettings deviceInfo)
        {
            Cards.Remove(deviceInfo);
            WriteJson();
        }

        public void WriteJson()
        {
            var devices = new List<IDeviceSettings>();
            foreach (var item in Cards)
            {
                devices.Add(item);
            }
            var json = JsonConvert.SerializeObject(devices, Formatting.Indented);
            Directory.CreateDirectory(JsonPath);
            File.WriteAllText(JsonDeviceNameAndPath, json);
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
            SelectedVerticalMenuItem = menuItem;
            //switch (menuItem.Text)
            //{
            //    case dashboard:

            //        CurrentView = _allDeviceView.CreateViewModel();
            //        IsDashboardType = true;
            //        break;
            //    case deviceSetting:
            //        _deviceSettingView = new DeviceSettingViewModel(this,SettingInfo);
            //        CurrentView = _deviceSettingView;
            //        IsDashboardType = true;
            //        break;
            //    case appSetting:
            //        _appSettingView = new AppSettingViewModel(this, SettingInfo);
            //        CurrentView = _appSettingView;
            //        IsDashboardType = true; 
            //        break;
            //    case faq:
            //        _faqSettingView = new FAQViewModel();
            //        CurrentView = _faqSettingView;
            //        IsDashboardType = true; 
            //        break;
            //    case general:
            //        _detailView = new DeviceDetailViewModel(CurrentDevice,this,SettingInfo);
            //        CurrentView = _detailView;
            //        IsDashboardType = false;

            //        break;
            //    case lighting:
            //        if (_detailView==null)
            //            _detailView = new DeviceDetailViewModel(CurrentDevice,this, SettingInfo);
            //        ((DeviceDetailViewModel)_detailView).TabType = DeviceTab.Lighting;
            //        CurrentView = _detailView;
            //        IsDashboardType = false;
            //        break;
            //    default:
            //        break;
            //}
            //if (menuItem.Text is lighting or general)
            //{
            //    ReadDataDevice();
            //}
            //SetMenuItemActiveStatus(menuItem.Text);
        }
         public void WriteDeviceInfoJson()
        {
            if (_allDeviceView == null) return;
            ((AllDeviceViewModel)_allDeviceView).WriteJson();
        }
        public void GotoChild(IDeviceSettings card)
        {
            //  _detailView = new DeviceDetailViewModel(card, this,SettingInfo);
            //CurrentView = _detailView;
            SelectedVerticalMenuItem = MenuItems.FirstOrDefault(t => t.Text == general);
            IsDashboardType = false;
            CurrentDevice = card;
            PreviewSpots = SpotSets[0].Spots;
            SetMenuItemActiveStatus(lighting);
        }
        public void BackToDashboard()
        {

            //CurrentView = _allDeviceView.CreateViewModel();
            IsDashboardType = true;
            SelectedVerticalMenuItem = MenuItems.FirstOrDefault();
            SetMenuItemActiveStatus(dashboard);
        }
        public void BackToDashboardAndDelete(IDeviceSettings device)
        {
            Cards.Remove(device);
            //CurrentView = _allDeviceView.CreateViewModel();
            IsDashboardType = true;
            SelectedVerticalMenuItem = MenuItems.FirstOrDefault();
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
