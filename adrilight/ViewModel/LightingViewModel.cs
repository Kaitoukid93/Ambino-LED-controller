using adrilight.DesktopDuplication;
using adrilight.Fakes;
using adrilight.Resources;
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
  public  class LightingViewModel : ViewModelBase, IDisposable
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
        public ISpot[] _previewSpots;
        public ISpot[] PreviewSpots {
            get => _previewSpots;
            set
            {
                _previewSpots = value;
                RaisePropertyChanged();
            }
        }
        private readonly ViewModelBase _parentVm;

        private ISpotSet spotSet;
        public ISpotSet SpotSet {
            get { return spotSet; }
            set
            {
                if (spotSet == value) return;
                spotSet = value;
                RaisePropertyChanged("SpotSet");
                PreviewSpots = spotSet.Spots;
            }
        }
        private Rainbow _rainbow;
        public Rainbow Rainbow {
            get { return _rainbow; }
            set
            {
                if (_rainbow == value) return;
                _rainbow = value;
                RaisePropertyChanged();
            }
        }
        private StaticColor _staticcolor;
        public StaticColor StaticColor {
            get { return _staticcolor; }
            set
            {
                if (_staticcolor == value) return;
                _staticcolor = value;
                RaisePropertyChanged();
            }
        }
        private Music _music;
        public Music Music {
            get { return _music; }
            set
            {
                if (_music == value) return;
                _music = value;
                RaisePropertyChanged();
            }
        }
        private Atmosphere _atmosphere;
        public Atmosphere Atmosphere {
            get { return _atmosphere; }
            set
            {
                if (_atmosphere == value) return;
                _atmosphere = value;
                RaisePropertyChanged();
            }
        }
        private DesktopDuplicatorReader _reader;
        public DesktopDuplicatorReader Reader {
            get { return _reader; }
            set
            {
                if (_reader == value) return;
                _reader = value;
                RaisePropertyChanged();
            }
        }
        private SerialStream _stream;
        public SerialStream Stream {
            get { return _stream; }
            set
            {
                if (_stream == value) return;
                _stream = value;
                RaisePropertyChanged();
            }
        }
        private Gifxelation _gif;
        public Gifxelation Gif {
            get { return _gif; }
            set
            {
                if (_gif == value) return;
                _gif = value;
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
                if (Card.SelectedAudioDevice > AvailableAudioDevice.Count)
                {
                    System.Windows.MessageBox.Show("Last Selected Audio Device is not Available");
                    return -1;
                }
                else
                {
                    var currentDevice = AvailableAudioDevice.ElementAt(Card.SelectedAudioDevice);

                    var array = currentDevice.Split(' ');
                    _audioDeviceID = Convert.ToInt32(array[0]);
                    return _audioDeviceID;
                }

            }



        }
        public ObservableCollection<string> AvailablePalette { get; private set; }
        public LightingViewModel(DeviceInfoDTO device, ViewModelBase parent, SettingInfoDTO setting)
        {
            this.Card = device;
            this.SettingInfo = setting;
            this.SpotSet = new SpotSet(Card);
            PreviewSpots = SpotSet.Spots;
            _parentVm = parent;
            ReadData();
            Card = device;
           // Card.LEDNumber = 30;
            Rainbow = new Rainbow(Card, SpotSet, this, SettingInfo);
            StaticColor = new StaticColor(Card, SpotSet, this, SettingInfo);
            Atmosphere = new Atmosphere(Card, SpotSet, this, SettingInfo);
            Music = new Music(Card, SpotSet, this, SettingInfo);
            Reader = new DesktopDuplicatorReader(Card, SpotSet, this, SettingInfo);
            Stream = new SerialStream(SettingInfo, SpotSet,  Card);
            Gif= new Gifxelation(Card, SpotSet, this, SettingInfo);
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
                        Card.GifFilePath = gifile.FileName;
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
            if (disposing)
            {
                Rainbow.Dispose();
                Atmosphere.Dispose();
                Music.Dispose();
                Stream.Dispose();
                StaticColor.Dispose();
                Gif.Dispose();
            }
        }
    }
}
