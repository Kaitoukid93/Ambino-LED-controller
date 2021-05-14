using OpenRGB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using adrilight.Util;
using adrilight.ViewModel;
using System.Threading;
using NLog;
using System.Threading.Tasks;
using Un4seen.BassWasapi;
using Un4seen.Bass;
using System.Windows;

namespace adrilight
{
    internal class Music : IMusic
    {
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        public static byte[] spectrumdata = new byte[16];
        public static float[] _fft;
        public static int _lastlevel;
        public static int _hanctr;
        public static int volume;
        private WASAPIPROC _process;

        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Music(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            UserSettings.PropertyChanged += PropertyChanged;
            SettingsViewModel.PropertyChanged += PropertyChanged;
            BassNet.Registration("saorihara93@gmail.com", "2X2831021152222");
            _process = new WASAPIPROC(Process);
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;
            
            bool result = BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            //  BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            if (!result)
            {
                var error = Bass.BASS_ErrorGetCode();
                MessageBox.Show(error.ToString());
            }
            else
            {
                //_initialized = true;
                //  Bassbox.IsEnabled = false;
            }
          
            BassWasapi.BASS_WASAPI_Start();

            RefreshAudioState();
            _log.Info($"MusicColor Created");

        }

        private IUserSettings UserSettings { get; }
        private SettingsViewModel SettingsViewModel { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserSettings.TransferActive):
                case nameof(UserSettings.SelectedEffect):
                case nameof(UserSettings.Brightness):
                case nameof(UserSettings.SelectedAudioDevice):
                case nameof(UserSettings.SelectedAudioDeviceName):
                case nameof(UserSettings.SelectedMusicMode):
                case nameof(SettingsViewModel.IsSettingsWindowOpen):

                    RefreshAudioState();
                    break;
            }
        }
        private void RefreshAudioState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.SelectedEffect == 3;
            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the Music Color");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
                Free();
            }
            else if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the Music Color");
                _cancellationTokenSource = new CancellationTokenSource();
                Init();
                int devindex = -1;
                if (UserSettings.SelectedAudioDeviceName!=null)
                {
                  //  var dvcstr = UserSettings.SelectedAudioDeviceName as string;
                   // var audiodevice = dvcstr.Split(' ');
                  //  devindex = Convert.ToInt32(audiodevice[0]);
                }
                else
                {
                    devindex = -1;
                }
                 

  
                bool result = BassWasapi.BASS_WASAPI_Init(35, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero); // this is the function to init the device according to device index
                                                                                                                                               
                if (!result)
                {
                    var error = Bass.BASS_ErrorGetCode();
                    MessageBox.Show(error.ToString());
                }
                else
                {
                    //_initialized = true;
                    //  Bassbox.IsEnabled = false;
                }
                //BassWasapi.BASS_WASAPI_Init(-3, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);

                BassWasapi.BASS_WASAPI_Start();

                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "MusicColorCreator"
                };
                thread.Start();
            }
        }

        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        public object Lock { get; } = new object();
        private ISpotSet SpotSet { get; }
        public void Run(CancellationToken token)

        {
            if (IsRunning) throw new Exception(" Music Color is already running!");

            IsRunning = true;

            _log.Debug("Started Music Color.");

            try
            {

                while (!token.IsCancellationRequested)
                {
                    double brightness = UserSettings.Brightness / 100d;
                    int paletteSource = UserSettings.SelectedPalette;
                    var numLED = (UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2;
                    //audio capture section//
                    int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048);// get channel fft data
                    if (ret < -1) return;
                    int x, y;
                    int b0 = 0;
                    //computes the spectrum data, the code is taken from a bass_wasapi sample.
                    for (x = 0; x < 16; x++)
                    {
                        float peak = 0;
                        int b1 = (int)Math.Pow(2, x * 10.0 / (16 - 1));
                        if (b1 > 1023) b1 = 1023;
                        if (b1 <= b0) b1 = b0 + 1;
                        for (; b0 < b1; b0++)
                        {
                            if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                        }
                        y = (int)(Math.Sqrt(peak) * 3 * 250 - 4);
                        if (y > 255) y = 255;
                        if (y < 10) y = 0;
                        spectrumdata[x] = (byte)((spectrumdata[x] * 6 + y * 2 + 7) / 8); //Smoothing out the value (take 5/8 of old value and 3/8 of new value to make finnal value)
                        if (spectrumdata[x] > 255)
                            spectrumdata[x] = 255;
                        if (spectrumdata[x] < 15)
                            spectrumdata[x] = 0;

                    }

                    int level = BassWasapi.BASS_WASAPI_GetLevel(); // Get level (VU metter) for Old AMBINO Device (remove in the future)
                    if (level == _lastlevel && level != 0) _hanctr++;
                    volume = Utils.LowWord32(level);
                    _lastlevel = level;
                    byte musicMode = UserSettings.SelectedMusicMode;
                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 3);
                    //audio capture section//


                    //if (isPreviewRunning)
                    //{
                    //    // SettingsViewModel.SetPreviewImage(backgroundimage);// replace with grey gradient instead
                    //}


                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;
                    lock (SpotSet.Lock)
                    {
                        if (paletteSource == 0)
                        {
                            var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);
                            var brightnessMap=SpectrumCreator(spectrumdata, 0, volume, musicMode, numLED);// get brightness map based on spectrum data
                                foreach (var color in newcolor)
                            {
                                outputColor[counter] = Brightness.applyBrightness(color, brightnessMap[counter]);
                                counter++;

                            }
                            counter = 0;
                            foreach (ISpot spot in SpotSet.Spots)
                            {
                                spot.SetColor(outputColor[counter].R, outputColor[counter].G, outputColor[counter].B, true);
                                counter++;

                            }

                            if (_huePosIndex > 360)
                            {
                                _huePosIndex = 0;
                            }
                            else
                            {
                                _huePosIndex += 1;
                            }

                        }
                        else
                        {
                            if (paletteSource == 1)//party color palette
                            {
                                //  PaletteCreator(numLED, _palettePosIndex, Rainbow.party);
                            }
                            else if (paletteSource == 2)//cloud color palette
                            {
                                //  PaletteCreator(numLED, _palettePosIndex, Rainbow.cloud);
                            }

                            if (_palettePosIndex > numLED)
                            {
                                _palettePosIndex = 0;
                            }
                            else
                            {
                                _palettePosIndex += 1;
                            }
                        }
                        if (isPreviewRunning)
                        {
                            //copy all color data to the preview
                            var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                            SettingsViewModel.PreviewSpots = SpotSet.Spots;
                        }
                    }
                    Thread.Sleep(5); //motion speed

                }
            }
            catch (OperationCanceledException)
            {
                _log.Debug("OperationCanceledException catched. returning.");

                // return;
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "Exception catched.");



                //allow the system some time to recover
                Thread.Sleep(500);
            }
            finally
            {


                _log.Debug("Stopped Rainbow Color Creator.");
                IsRunning = false;
            }



        }




        public static double[] SpectrumCreator(byte[] fft, int sensitivity, double level, byte musicMode, int numLED)//create brightnessmap based on input fft or volume
        {

            int counter = 0;
            int factor = numLED / fft.Length;
            byte maxbrightness = 255;
            double[] brightnessMap = new double[numLED];
            //this function take the input as frequency and output the color but the brightness change as the frequency band's value
            if (musicMode == 0)//pulse mode, take current level and blackout the rest of the strip
            {
              
                int height = 0;
                double percent = level / 16384;
                height = (int)((height * 4 + numLED * percent * 4 + 7) / 8);

                foreach (var brightness in brightnessMap.Take(height))
                {

                    brightnessMap[counter++] = maxbrightness/255.0;

                }


                for (int i = height; i < numLED; i++)
                {

                    brightnessMap[counter++] = 0;

                }
            }
            else if (musicMode == 1)//equalizer mode, each block of LED is respond to 1 band of frequency spectrum
            {
                byte[] holdarray = new byte[256];
                for (int i = 0; i < fft.Length; i++)
                {
                    for (int j = 0; j < factor; j++)
                    {
                        holdarray[i + j] = fft[i];
                        brightnessMap[counter++] = holdarray[i]/255.0;
                    }
                }




            }
            else if (musicMode == 2)
            {

            }
            else if (musicMode == 3)
            {

            }
            else if (musicMode == 4)
            {

            }
            else if (musicMode == 5)
            {

            }


            return brightnessMap;

        }




        private void Init()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            var result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }




        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }













    }
}


























//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using System.Windows.Media;
//using Un4seen.Bass;
//using Un4seen.BassWasapi;

//namespace adrilight.Util
//{
//    class Audio
//    {

//        public static byte[] spectrumdata = new byte[16];
//        public static int _lastlevel;
//        public static int _hanctr;
//        public static int volume;
//        private static double _huePos = 0;//rainbow color only cuz it's using Hue to shift
//        public static Color[] paletteOutput = new Color[256];


//        public static void MusicCreator(int musicMode, int paletteSource, Canvas playground, int numLED, float[] _fft, double effectSpeed)
//        {

//           



//            //get collection of color and brightness combine based on spectrum data or volume above


//            }
//        }


//            }

//        }


