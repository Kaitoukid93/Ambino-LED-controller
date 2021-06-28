using adrilight.DesktopDuplication;
using adrilight.Fakes;
using adrilight.Resources;
using adrilight.Spots;
using adrilight.Util;
using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace adrilight.ViewModel
{
    class LightingViewModel : ViewModelBase
    {

        //private string _gifFilePath = "";
        //public string GifFilePath {
        //    get { return _gifFilePath; }
        //    set
        //    {
        //        _gifFilePath = value;
        //        Card.GifFilePath = value;
        //    }
        //}

      
        GifBitmapDecoder decoder;
        
        public IDeviceSettings Settings { get; }

        private SettingInfoDTO _settingInfo;
        public SettingInfoDTO SettingInfo {
            get { return _settingInfo; }
            set
            {
                if (_settingInfo == value) return;
                _settingInfo = value;
                RaisePropertyChanged();
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
        public IGeneralSpot[] _previewSpots;
        public IGeneralSpot[] PreviewSpots {
            get => _previewSpots;
            set
            {
                _previewSpots = value;
                RaisePropertyChanged();
            }
        }
        private readonly ViewModelBase _parentVm;

        //private IGeneralSpotSet generalSpotSet;
        //public IGeneralSpotSet GeneralSpotSet {
        //    get { return generalSpotSet; }
        //    set
        //    {
        //        if (generalSpotSet == value) return;
        //        generalSpotSet = value;
        //        RaisePropertyChanged("SpotSet");
        //        PreviewSpots = generalSpotSet.Spots;
        //    }
        //}
     
    
     
       
       
     
     
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
                if (Settings.SelectedAudioDevice > AvailableAudioDevice.Count)
                {
                    System.Windows.MessageBox.Show("Last Selected Audio Device is not Available");
                    return -1;
                }
                else
                {
                    var currentDevice = AvailableAudioDevice.ElementAt(Settings.SelectedAudioDevice);

                    var array = currentDevice.Split(' ');
                    _audioDeviceID = Convert.ToInt32(array[0]);
                    return _audioDeviceID;
                }

            }



        }
        public ObservableCollection<string> AvailablePalette { get; private set; }
        public LightingViewModel(IDeviceSettings deviceSettings,ViewModelBase parent, SettingInfoDTO setting, IDeviceSpotSet deviceSpotSet, IGeneralSpotSet generalSpotSet) // cái này sẽ bỏ, kiểu gì thì kiểu khi chuyển tab cũng
                                                                                                      //sẽ bị tạo mới
        {

            
            ReadData();
          //  Context = context ?? throw new ArgumentNullException(nameof(context));
            Settings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
            SettingInfo = setting;
           // this.SpotSet = new SpotSet(Card);
          
            //PreviewSpots = generalSpotSet.Spots;
            _parentVm = parent;
            //ReadData();
            // Card = device;
            // Card.LEDNumber = 30;
            //switch (Card.SelectedEffect)
            //{
            //    case 0:
            //        if (Reader is null)
            //            Reader = new DesktopDuplicatorReader(Card, SpotSet, this, SettingInfo);

            //        break;
            //    case 1:
            //        Rainbow = new Rainbow(Card, SpotSet, this, SettingInfo);
            //        break;
            //    case 2:
            //        Music = new Music(Card, SpotSet, this, SettingInfo);
            //        break;
            //    case 3:
            //        StaticColor = new StaticColor(Card, SpotSet, this, SettingInfo);
            //        break;
            //    case 4:
            //        Atmosphere = new Atmosphere(Card, SpotSet, this, SettingInfo);
            //        break;
            //    case 5:
            //        Gif = new Gifxelation(Card, SpotSet, this, SettingInfo);
            //        break;

            //}
            //Stream = new SerialStream(SettingInfo, SpotSet, Card);


        }
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
        
        public void ReadData()
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
                        Settings.GifFilePath = gifile.FileName;
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
            //CollectionItems = new ObservableCollection<CollectionItem>();
            //CollectionItems.Add(new CollectionItem() { Value = 1, Text = "ARGB-1" });
            //CollectionItems.Add(new CollectionItem() { Value = 1, Text = "ARGB-2",IsSelected=true });
            //CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-1" });
            //CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-2" });
            //CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-3" });
            //CollectionItems.Add(new CollectionItem() { Value = 1, Text = "PCI-4" });

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    Rainbow.Dispose();
            //    Atmosphere.Dispose();
            //    Music.Dispose();
            //    Stream.Dispose();
            //    StaticColor.Dispose();
            //    Gif.Dispose();
            //}
        }
    }
}
