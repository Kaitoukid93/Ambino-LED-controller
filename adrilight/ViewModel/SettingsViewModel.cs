using adrilight.DesktopDuplication;
using adrilight.Resources;
using adrilight.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.ApplicationInsights;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.CoreAudioApi;
using System.Management;
using System.IO;
using System.Collections.ObjectModel;
using adrilight.Settings;
using System.Windows.Forms;
using Color = System.Windows.Media.Color;
using Un4seen.BassWasapi;

namespace adrilight.ViewModel
{

    class SettingsViewModel : ViewModelBase
    {
        private static ILogger _log = LogManager.GetCurrentClassLogger();
        public ObservableCollection<string> Effects { get; private set; }
        public ObservableCollection<string> AvailablePalette { get; private set; }
        public ObservableCollection<string> AvailableFrequency { get; private set; }
        public ObservableCollection<string> AvailableMusicPalette { get; private set; }


        private static int _gifFrameIndex = 0;

        private const string ProjectPage = "http://ambino.net";
        private const string IssuesPage = "https://www.messenger.com/t/109869992932970";
        private const string LatestReleasePage = "https://github.com/fabsenet/adrilight/releases/latest";
        ManagementEventWatcher watcher = new ManagementEventWatcher();
        public SettingsViewModel(IUserSettings userSettings, IList<ISelectableViewPart> selectableViewParts
            , ISpotSet spotSet, IContext context, TelemetryClient tc, ISerialStream serialStream)
        {
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
            watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += (s, e) => RaisePropertyChanged(() => AvailableComPorts);
            // watcher.Query = query;
            watcher.Start();
            // watcher.WaitForNextEvent();
            if (selectableViewParts == null)
            {
                throw new ArgumentNullException(nameof(selectableViewParts));
            }

            this.Settings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            this.spotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            Context = context ?? throw new ArgumentNullException(nameof(context));
            this.serialStream = serialStream ?? throw new ArgumentNullException(nameof(serialStream));
            SelectableViewParts = selectableViewParts.OrderBy(p => p.Order).ToList();
            BackUpView = selectableViewParts.OrderBy(p => p.Order).ToList();



            Effects = new ObservableCollection<string>
         {
           "Sáng theo màn hình",
           "Sáng theo dải màu",
           "Sáng màu tĩnh",
           "Sáng theo nhạc",
           "Atmosphere",
           "Gifxelation"

        };

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



#if DEBUG
            SelectedViewPart = SelectableViewParts.First();
#else
            SelectedViewPart = SelectableViewParts.First();
#endif

            PossibleLedCountsVertical = Enumerable.Range(10, 190).ToList();
            PossibleLedCountsHorizontal = Enumerable.Range(10, 290).ToList();

            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(SelectedViewPart):
                        var name = SelectedViewPart?.ViewPartName ?? "nothing";
                        tc.TrackPageView(name);
                        break;
                }
            };








            Settings.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(Settings.SpotsX):
                    Settings.OffsetLed = Settings.SpotsX - 1;
                    RaisePropertyChanged(() => SpotsXMaximum);
                    RaisePropertyChanged(() => LedCount);
                    RaisePropertyChanged(() => OffsetLedMaximum);
                    RaisePropertyChanged(() => Settings.OffsetLed);
                    break;

                case nameof(Settings.SpotsY):
                    RaisePropertyChanged(() => SpotsYMaximum);
                    RaisePropertyChanged(() => LedCount);
                    RaisePropertyChanged(() => OffsetLedMaximum);
                    break;

              

                   


                case nameof(Settings.UseLinearLighting):
                    RaisePropertyChanged(() => UseNonLinearLighting);
                    break;

              
                case nameof(Settings.SelectedAudioDevice):
                    RaisePropertyChanged(() => AudioDeviceID);
                    break;





                case nameof(Settings.OffsetLed):
                    RaisePropertyChanged(() => OffsetLedMaximum);
                    break;

                case nameof(Settings.Autostart):
                    if (Settings.Autostart)
                    {
                        StartUpManager.AddApplicationToCurrentUserStartup();
                    }
                    else
                    {
                        StartUpManager.RemoveApplicationFromCurrentUserStartup();
                    }
                    break;

                case nameof(Settings.ComPort):
                    RaisePropertyChanged(() => TransferCanBeStarted);
                    RaisePropertyChanged(() => TransferCanNotBeStarted);
                    break;




                    //gifxelation//

                    case nameof(Settings.IMX1):
                    if (Settings.IMLockDim == true)
                    {
                        if (Settings.IMX1 + ImageProcesser.ImageRect.Width > IMXMax)
                        {
                            Settings.IMX1 = IMXMax - ImageProcesser.ImageRect.Width;
                            Settings.IMX2 = IMXMax;
                        }
                        else
                        {
                            Settings.IMX2 = Settings.IMX1 + ImageProcesser.ImageRect.Width;
                        }


                    }
                    else
                    {
                        Settings.IMX1 = Settings.IMX1 > Settings.IMX2 - MatrixFrame.Width ? Settings.IMX2 - MatrixFrame.Width : Settings.IMX1;
                    }

                    RaisePropertyChanged(() => Settings.IMX1);
                    RaisePropertyChanged(() => Settings.IMX2);
                    IMDimensionsChanged();
                    break;


                case nameof(Settings.IMX2):



                    Settings.IMX2 = Settings.IMX2 < Settings.IMX1 + MatrixFrame.Width ? Settings.IMX1 + MatrixFrame.Width : Settings.IMX2;



                    RaisePropertyChanged(() => Settings.IMX2);
                    IMDimensionsChanged();
                    break;

                case nameof(Settings.IMY1):
                    if (Settings.IMLockDim == true)
                    {
                        if (Settings.IMY1 + ImageProcesser.ImageRect.Height > IMYMax)
                        {
                            Settings.IMY1 = IMYMax - ImageProcesser.ImageRect.Height;
                            Settings.IMY2 = IMYMax;
                        }
                        else
                        {
                            Settings.IMY2 = Settings.IMY1 + ImageProcesser.ImageRect.Height;
                        }


                    }
                    else
                    {
                        Settings.IMY1 = Settings.IMY1 > Settings.IMY2 - MatrixFrame.Height ? Settings.IMY2 - MatrixFrame.Height : Settings.IMY1;
                    }

                    RaisePropertyChanged(() => Settings.IMY1);
                    RaisePropertyChanged(() => Settings.IMY2);
                    IMDimensionsChanged();
                    break;


                case nameof(Settings.IMY2):



                    Settings.IMY2 = Settings.IMY2 < Settings.IMY1 + MatrixFrame.Height ? Settings.IMY1 + MatrixFrame.Height : Settings.IMY2;



                    RaisePropertyChanged(() => Settings.IMY2);
                    IMDimensionsChanged();
                    break;
                case nameof(Settings.IMInterpolationModeIndex):

                    switch (Settings.IMInterpolationModeIndex)
                    {
                        case 0: ImageProcesser.InterpMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor; break;
                        case 1: ImageProcesser.InterpMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic; break;
                        case 2: ImageProcesser.InterpMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear; break;
                        case 3: ImageProcesser.InterpMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic; break;
                        case 4: ImageProcesser.InterpMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear; break;
                    }

                    RefreshStillImage();
                    RefreshGifImages();
                    IMDimensionsChanged();








                    break;

                        //gifxelation//



                }
        };






        }



        private void IMDimensionsChanged()
        {
            ImageProcesser.ImageRect = new Rectangle(Settings.IMX1, Settings.IMY1, Settings.IMX2 - Settings.IMX1, Settings.IMY2 - Settings.IMY1);
            //Text_IM_WidthHeight = "Width: " + ImageProcesser.ImageRect.Width.ToString() + " " + "Height: " + ImageProcesser.ImageRect.Height.ToString();

            RefreshStillImage();
            RefreshGifImages();
        }

        private void RefreshStillImage()
        {
            if (ImageProcesser.ImageLoadState == ImageProcesser.LoadState.Still)
            {
                ImageProcesser.DisposeWorkingBitmap();
                ImageProcesser.WorkingBitmap = ImageProcesser.CropBitmap(ImageProcesser.LoadedStillBitmap, ImageProcesser.ImageRect);
                ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
                MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
                //FrameToPreview();
                //SerialManager.PushFrame();
            }
        }

        private void RefreshGifImages()
        {
            if (ImageProcesser.ImageLoadState == ImageProcesser.LoadState.Gif)
            {

                ImageProcesser.DisposeWorkingBitmap();

                if (Settings.GifPlayPause != true) //if gif is not playing
                {
                    if (_gifFrameIndex > ImageProcesser.LoadedGifFrameCount - 1)
                        _gifFrameIndex = 0;

                    ImageProcesser.WorkingBitmap = ImageProcesser.CropBitmap(new Bitmap(ImageProcesser.LoadedGifImage), ImageProcesser.ImageRect);
                    ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
                    MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
                    //FrameToPreview();
                    //SerialManager.PushFrame();
                }

            }
        }

        public  void SnapShot()
        {
            int counter = 0;
            byte[] snapshot = new byte[256];
            foreach (ISpot spot in spotSet.Spots)
            {
                
                snapshot[counter++] = spot.Red;
                snapshot[counter++] = spot.Green;
                snapshot[counter++] = spot.Blue;
               // counter++;
            }
            Settings.SnapShot = snapshot;
            RaisePropertyChanged(() => Settings.SnapShot);
        }

        private int _imXMax = 800;
        private int _imYMax = 800;
        public int IMXMax {
            get { return _imXMax; }
            set
            {
                if (value != _imXMax)
                {
                    _imXMax = value;
                    RaisePropertyChanged(() => IMXMax);

                }
            }
        }
        public int IMYMax {
            get { return _imYMax; }
            set
            {
                if (value != _imYMax)
                {
                    _imYMax = value;
                    RaisePropertyChanged(() => IMYMax);

                }
            }
        }



        public string Title { get; } = $"adrilight {App.VersionNumber}";
        public int LedCount => spotSet.CountLeds(Settings.SpotsX, Settings.SpotsY) * Settings.LedsPerSpot;

        public bool TransferCanBeStarted => serialStream.IsValid();
        public bool TransferCanNotBeStarted => !TransferCanBeStarted;
       

        public bool UseNonLinearLighting {
            get => !Settings.UseLinearLighting;
            set => Settings.UseLinearLighting = !value;


        }


        private string _gifFilePath="" ;
        public string GifFilePath {
            get { return _gifFilePath; }
            set
            {
                _gifFilePath = value;
                Settings.GifFilePath = value;
            }
        }
        

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



        public IUserSettings Settings { get; }
        public IContext Context { get; }

        public IList<String> _AvailableComPorts;
        public IList<String> AvailableComPorts {
            get
            {


                _AvailableComPorts = SerialPort.GetPortNames().Concat(new[] { "Không có" }).ToList();

                _AvailableComPorts.Remove("COM1");

                return _AvailableComPorts;
            }
        }

        public IList<String> _AvailableDisplays;
        public IList<String> AvailableDisplays {
            get
            {
                var listDisplay = new List<String>();
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    Rectangle resolution = screen.Bounds;


                    listDisplay.Add(screen.DeviceName.ToString()+ " at "+ resolution.Width.ToString() +"x"+resolution.Height.ToString());
                }
                _AvailableDisplays = listDisplay;
                return _AvailableDisplays;
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
            get {
                if(Settings.SelectedAudioDevice>AvailableAudioDevice.Count-1)
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

      













        public IList<ISelectableViewPart> BackUpView { get; }

        public IList<ISelectableViewPart> SelectableViewParts { get; }

        public IList<int> PossibleLedCountsHorizontal { get; }
        public IList<int> PossibleLedCountsVertical { get; }

        public ISelectableViewPart _selectedViewPart;
        public ISelectableViewPart SelectedViewPart {
            get => _selectedViewPart;
            set
            {
                Set(ref _selectedViewPart, value);
                _log.Info($"SelectedViewPart is now {_selectedViewPart?.ViewPartName}");

                IsPreviewTabOpen = _selectedViewPart is View.SettingsWindowComponents.LedOutsideCase.LedOutsideCaseSelectableViewPart;

            }
        }

        private bool _isPreviewTabOpen;
        public bool IsPreviewTabOpen {
            get => _isPreviewTabOpen;
            private set
            {
                Set(ref _isPreviewTabOpen, value);
                _log.Info($"IsPreviewTabOpen is now {_isPreviewTabOpen}");
            }
        }





        private bool _isSettingsWindowOpen;
        public bool IsSettingsWindowOpen {
            get => _isSettingsWindowOpen;
            set
            {
                Set(ref _isSettingsWindowOpen, value);
                _log.Info($"IsSettingsWindowOpen is now {_isSettingsWindowOpen}");
            }
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);



        public void SetPreviewImage(Bitmap image)
        {
            Context.Invoke(() =>
            {
                if (PreviewImageSource == null)
                {
                    //first run creates writableimage
                    var imagePtr = image.GetHbitmap();
                    try
                    {
                        var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(imagePtr, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        PreviewImageSource = new WriteableBitmap(bitmapSource);
                    }
                    finally
                    {
                        var i = DeleteObject(imagePtr);
                    }
                }
                else
                {
                  
                    //next runs reuse the writable image
                    Rectangle colorBitmapRectangle = new Rectangle(0, 0, image.Width, image.Height);
                    Int32Rect colorBitmapInt32Rect = new Int32Rect(0, 0, PreviewImageSource.PixelWidth, PreviewImageSource.PixelHeight);

                    BitmapData data = image.LockBits(colorBitmapRectangle, ImageLockMode.WriteOnly, image.PixelFormat);

                   PreviewImageSource.WritePixels(colorBitmapInt32Rect, data.Scan0, data.Width * data.Height * 4, data.Stride);

                    image.UnlockBits(data);
                }



                //if (Settings.screeneffectcounter == 8)
                //{

                //    ImageProcesser.WorkingBitmap = image;
                //    ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
                //    MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
                //    ImageProcesser.DisposeWorkingBitmap();






                //}
            });
        }



        public WriteableBitmap _previewImageSource;
        public WriteableBitmap PreviewImageSource {
            get => _previewImageSource;
            set
            {
                _log.Info("PreviewImageSource created.");
                Set(ref _previewImageSource, value);

                RaisePropertyChanged(() => ScreenWidth);
                RaisePropertyChanged(() => ScreenHeight);
                RaisePropertyChanged(() => CanvasWidth);
                RaisePropertyChanged(() => CanvasHeight);
            }
        }


     
        //ambilation//



        //ambilation//


        public ICommand OpenUrlProjectPageCommand { get; } = new RelayCommand(() => OpenUrl(ProjectPage));
        public ICommand OpenUrlIssuesPageCommand { get; } = new RelayCommand(() => OpenUrl(IssuesPage));
        public ICommand OpenUrlLatestReleaseCommand { get; } = new RelayCommand(() => OpenUrl(LatestReleasePage));
        private static void OpenUrl(string url) => Process.Start(url);

        public ICommand ExitAdrilight { get; } = new RelayCommand(() => App.Current.Shutdown(0));

        private int _spotsXMaximum = 300;
        public int SpotsXMaximum {
            get
            {
                return _spotsXMaximum = Math.Max(Settings.SpotsX, _spotsXMaximum);
            }
        }




        private int _spotsYMaximum = 300;
        private readonly ISpotSet spotSet;
        private readonly ISerialStream serialStream;

        public int SpotsYMaximum {
            get
            {
                return _spotsYMaximum = Math.Max(Settings.SpotsY, _spotsYMaximum);
            }
        }

        public int OffsetLedMaximum => Math.Max(Settings.OffsetLed, LedCount);

        //public int ScreenWidth => (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        //public int ScreenHeight => (int)System.Windows.SystemParameters.PrimaryScreenHeight;

        public int ScreenWidth => PreviewImageSource?.PixelWidth ?? 1000;
        public int ScreenHeight => PreviewImageSource?.PixelHeight ?? 1000;

        public int CanvasPadding => 3 / DesktopDuplicator.ScalingFactor;

        public int CanvasWidth => ScreenWidth + 2 * CanvasPadding;
        public int CanvasHeight => ScreenHeight + 2 * CanvasPadding;


        public ISpot[] _previewSpots;
        public ISpot[] PreviewSpots {
            get => _previewSpots;
            set
            {
                _previewSpots = value;
                RaisePropertyChanged();
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

        private IEffect effect;

        public IEffect AEffect {
            get { return effect; }
            set { effect = value; }
        }


        public bool disEffect = false;
        public bool disScreen = false;
        public bool disStatic = false;
        public bool disMusic = false;
        public bool disMain = false;
        private BitmapSource _contentBitmap;

    }
}

   










