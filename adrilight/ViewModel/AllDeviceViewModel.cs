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
using System.IO;
using Newtonsoft.Json;
using System.Windows.Media;

namespace adrilight.ViewModel
{
   public class AllDeviceViewModel : BaseViewModel
    {
        private string JsonPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "adrilight\\");

        private string JsonFileNameAndPath => Path.Combine(JsonPath, "adrilight-deviceInfos.json");
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
        private bool _isAddnew = false;
        public AllDeviceViewModel(ViewModelBase parent)
        {
            _parentVm = parent;
            ReadData();
        }
        
        public void LoadCard()
        {
            Cards = new ObservableCollection<DeviceInfoDTO>();
            var devices = LoadIfExists();
            if (devices != null)
            {
                foreach (var item in devices)
                {
                    var deviceInfo = new DeviceInfoDTO() {
                        Brightness = item.brightness,
                        CaptureSource = item.capturesource,
                        ColorTemp = item.colortemp,
                        DeviceId = item.deviceid,
                        DeviceName = item.devicename,
                        DevicePort = item.deviceport,
                        DeviceSize = item.devicesize,
                        DeviceType = item.devicetype,
                        FadeEnd = item.fadeend,
                        FadeStart = item.fadestart,
                        GifMode = item.gifmode,
                        GifSource = item.gifsource,
                        IsBreathing = item.isbreathing,
                        IsConnected = item.isConnected,
                        LightingMode = item.lightingmode,
                        MusicMode = item.musicmode,
                        MusicSens = item.musicsens,
                        MusicSource = item.musicsource,
                        RainbowMode = item.rainbowmode,
                        RainbowSpeed = item.rainbowspeed,
                        StaticColor =string.IsNullOrEmpty(item.staticcolor)? Color.FromArgb(0, 0, 255, 255) : (Color)System.Windows.Media.ColorConverter.ConvertFromString(item.staticcolor), 
                        IsShowOnDashboard=item.isshowondashboard,
                         AtmosphereStart=item.atmospherestart,
                          AtmosphereStop=item.atmospherestop,
                           BreathingSpeed=item.breathingspeed,
                            ColorFrequency=item.colorfrequency,
                             EffectSpeed=item.effectspeed,
                              Palette=item.palette,
                               SpotHeight=item.spotheight,
                               SpotsX=item.spotx,
                               SpotsY=item.spoty,
                                SpotWidth=item.spotwidth,
                                UseLinearLighting=item.uselinearlighting,
                                 WhitebalanceBlue=item.whitebalanceblue,
                                 WhitebalanceRed=item.whitebalancered,
                                 WhitebalanceGreen=item.whitebalancegreen
                    };

                    deviceInfo.PropertyChanged += DeviceInfo_PropertyChanged;
                    Cards.Add(deviceInfo);
                }
            }

        }

        private void DeviceInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (_isAddnew) return;
            //_isAddnew = true;
            //WriteJson();
            //_isAddnew = false;
        }

        public override void ReadData()
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
                vm.Device.PropertyChanged += DeviceInfo_PropertyChanged;
                _isAddnew = true;
                Cards.Add(vm.Device);
                WriteJson();
                _isAddnew = false;
            }
        }
        public void DeleteCard(DeviceInfoDTO deviceInfo)
        {
            Cards.Remove(deviceInfo);
            WriteJson();
        }
      
        public void WriteJson()
        {
            var devices = new List<DeviceInfo>();
            foreach (var item in Cards)
            {
                devices.Add(item.GetDeviceInfo());
            }
            var json = JsonConvert.SerializeObject(devices, Formatting.Indented);
            Directory.CreateDirectory(JsonPath);
            File.WriteAllText(JsonFileNameAndPath, json);
        }
        public List<DeviceInfo> LoadIfExists()
        {
            if (!File.Exists(JsonFileNameAndPath)) return null;

            var json = File.ReadAllText(JsonFileNameAndPath);

            var devices = JsonConvert.DeserializeObject<List< DeviceInfo>>(json);
          
            return devices;
        }
    }
}
