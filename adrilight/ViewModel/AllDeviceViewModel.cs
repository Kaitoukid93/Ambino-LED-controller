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
            Cards = new ObservableCollection<IDeviceSettings>();
            var settingsmanager = new UserSettingsManager();
            var devices = settingsmanager.LoadDeviceIfExists();
            if (devices != null)
            {
                foreach (var item in devices)
                {
                    var deviceInfo = new DeviceSettings() {
                        Brightness = item.Brightness,
                        SelectedDisplay = item.SelectedDisplay,
                        WhitebalanceRed = item.WhitebalanceRed,
                        DeviceId = item.DeviceID,
                        DeviceName = item.DeviceName,
                        DevicePort = item.DevicePort,
                        DeviceSize = item.DeviceSize,
                        DeviceType = item.DeviceType,
                      //  FadeEnd = item.fadeend,
                      //  FadeStart = item.fadestart,
                       // GifMode = item.gifmode,
                       // GifSource = item.gifsource,
                        IsBreathing = item.IsBreathing,
                        IsConnected = item.IsConnected,
                        SelectedEffect = item.SelectedEffect,
                        SelectedMusicMode = item.SelectedMusicMode,
                        MSens = item.MSens,
                        SelectedAudioDevice = item.SelectedAudioDevice,
                        SelectedPalette = item.SelectedPalette,
                        EffectSpeed = item.EffectSpeed,
                        StaticColor = item.StaticColor,
                         AtmosphereStart=item.AtmosphereStart,
                          AtmosphereStop=item.AtmosphereStop,
                           BreathingSpeed=item.BreathingSpeed,
                            ColorFrequency=item.ColorFrequency,
                            
                              SelectedMusicPalette=item.SelectedMusicPalette,
                               SpotHeight=item.SpotHeight,
                               SpotsX=item.SpotsX,
                               SpotsY=item.SpotsY,
                                SpotWidth=item.SpotWidth,
                                UseLinearLighting=item.UseLinearLighting,
                                 WhitebalanceBlue=item.WhitebalanceBlue,
                                
                                 WhitebalanceGreen=item.WhitebalanceGreen
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
            SelectCardCommand = new RelayCommand<IDeviceSettings>((p) => {
                return p != null;
            }, (p) =>
              {
                  (_parentVm as MainViewViewModel).GotoChild(p);
              });
            LoadCard();
            ShowAddNewCommand = new RelayCommand<IDeviceSettings>((p) => {
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
            File.WriteAllText(JsonFileNameAndPath, json);
        }
       
    }
}
