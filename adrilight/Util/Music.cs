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
using BO;

namespace adrilight
{
    public class Music : IMusic, IDisposable
    {
        private Thread _workerThread;
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        
        public static float[] _fft;
        public static int _lastlevel;
        public static int _hanctr;
        public static int volumeLeft;
        public static int volumeRight;
        public static int height = 0;
        public static int heightL = 0;
        public static int heightR = 0;
        private WASAPIPROC _process;

        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Music(DeviceInfoDTO device, ISpotSet spotSet, LightingViewModel viewViewModel, SettingInfoDTO setting)
        {
            deviceInfo = device ?? throw new ArgumentNullException(nameof(device));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = viewViewModel ?? throw new ArgumentNullException(nameof(viewViewModel));
            settingInfo = setting ?? throw new ArgumentNullException(nameof(setting));
            deviceInfo.PropertyChanged += PropertyChanged;
            settingInfo.PropertyChanged += SettingInfo_PropertyChanged;
            BassNet.Registration("saorihara93@gmail.com", "2X2831021152222");
            _process = new WASAPIPROC(Process);
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;


            RefreshAudioState();
            _log.Info($"MusicColor Created");

        }
        private void SettingInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private DeviceInfoDTO deviceInfo { get; }
        private LightingViewModel SettingsViewModel { get; }
        private SettingInfoDTO settingInfo { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
       

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(settingInfo.TransferActive):
                case nameof(deviceInfo.LightingMode):
                case nameof(deviceInfo.Brightness):
                case nameof(deviceInfo.SelectedAudioDevice):
                case nameof(deviceInfo.MusicMode):
                case nameof(deviceInfo.SpotsX):
                case nameof(deviceInfo.SpotsY):

                    RefreshAudioState();
                    break;
                //case nameof(SettingsViewModel.AudioDeviceID):
                //    RefreshAudioDevice();
                //    break;
            }
        }
        private void RefreshAudioState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = settingInfo.TransferActive && deviceInfo.LightingMode == "Sáng theo nhạc";
         

            
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

                Init();
                int deviceID = 0;//SettingsViewModel.AudioDeviceID;
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
                bool result = BassWasapi.BASS_WASAPI_Init(deviceID, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero); // this is the function to init the device according to device index

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
                //BassWasapi.BASS_WASAPI_Init(-3, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                _cancellationTokenSource = new CancellationTokenSource();
                _workerThread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "MusicColorCreator"
                };
                _workerThread.Start();
            }
        }
        private void RefreshAudioDevice()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = settingInfo.TransferActive && deviceInfo.LightingMode == "Sáng theo nhạc";
            if (isRunning && shouldBeRunning)
            {

                _log.Debug("Refreshing the Music Color");
                Init();
                int deviceID = 0;//SettingsViewModel.AudioDeviceID;
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
                bool result = BassWasapi.BASS_WASAPI_Init(deviceID, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero); // this is the function to init the device according to device index

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
                _log.Debug("Music Color Refreshed Successfully");
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
            double brightness = deviceInfo.Brightness / 100d;
            int paletteSource = deviceInfo.Palette;
            var numLED = (deviceInfo.SpotsX - 1) * 2 + (deviceInfo.SpotsY - 1) * 2;
            byte[] spectrumdata = new byte[numLED];

            try
            {

                while (!token.IsCancellationRequested)
                {
                    
                    //audio capture section//
                    int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048);// get channel fft data
                    if (ret < -1) return;
                    int x, y;
                    int b0 = 0;
                    //computes the spectrum data, the code is taken from a bass_wasapi sample.
                    for (x = 0; x < numLED; x++)
                    {
                        float peak = 0;
                        int b1 = (int)Math.Pow(2, x * 10.0 / (numLED - 1));
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
                    volumeLeft = (volumeLeft*6+ Utils.LowWord32(level)*2)/8;
                    volumeRight =(volumeRight*6+Utils.HighWord32(level)*2)/8;
                    _lastlevel = level;
                    byte musicMode = (byte)deviceInfo.MusicMode;
                    bool isPreviewRunning = (deviceInfo.LightingMode == "Sáng theo nhạc");
                    //audio capture section//




                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;
                    lock (SpotSet.Lock)
                    {
                        if (paletteSource == 0)
                        {
                            var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);
                            var brightnessMap=SpectrumCreator(spectrumdata, 0, volumeLeft,volumeRight, musicMode, numLED);// get brightness map based on spectrum data
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


                _log.Debug("Stopped MusicColor Color Creator.");
                IsRunning = false;
            }



        }




        public static double[] SpectrumCreator(byte[] fft, int sensitivity, double levelLeft, double levelRight, byte musicMode, int numLED)//create brightnessmap based on input fft or volume
        {

            int counter = 0;
            int factor = numLED / fft.Length;
            byte maxbrightness = 255;
            double[] brightnessMap = new double[numLED];
            //this function take the input as frequency and output the color but the brightness change as the frequency band's value
            if (musicMode == 0)//equalizer mode, each block of LED is respond to 1 band of frequency spectrum
            {
              
                
               
               
                for (int i = 0; i < fft.Length; i++)
                {
                   
                        brightnessMap[counter++] = fft[i] / 255.0;
                   
                }

            }
            else if (musicMode == 1)//vu mode
            {


                double percent = ((levelLeft+levelRight)/2) / 16384;
                height = (int)(percent * numLED);

                foreach (var brightness in brightnessMap.Take(height))
                {

                    brightnessMap[counter++] = maxbrightness / 255.0;

                }


                for (int i = height; i < numLED; i++)
                {

                    brightnessMap[counter++] = 0;

                }

            }
            else if (musicMode == 2)// End to End
            {
                double percent = ((levelLeft + levelRight) / 2) / 16384;
                height = (int)(percent*numLED);

                for (int i=0; i<numLED/2;i++)
                {
                    if (i <= height/2)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }
               

                for (int i = numLED/2; i < numLED; i++)
                {
                    if (numLED - i <= height / 2)
                        brightnessMap[i] = maxbrightness/255.0;
                    else
                        brightnessMap[i] = 0.0;

                }
            }
            else if (musicMode == 3)//Push Pull
            {
                double percent = ((levelLeft + levelRight) / 2) / 16384;
                height = (int)(percent * numLED);

                for (int i = 0; i < numLED / 2; i++)
                {
                    if (i <= height / 2)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (numLED - i >= height / 2)
                        brightnessMap[i] = maxbrightness/255.0;
                    else
                        brightnessMap[i] = 0.0;

                }
            }
            else if (musicMode == 4)//Symetric VU
            {
                double percentleft = levelLeft / 16384;
                heightL = (int)(percentleft*numLED);
                double percentright = levelRight / 16384;
                heightR = (int)(percentright*numLED);


                for (int i = 0; i < numLED / 2; i++)
                {
                    if (i <= heightR )
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2   ; i< numLED; i++)
                {
                    if (Math.Abs(numLED/2-i) <= heightL )
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0;

                }
            }
            else if (musicMode == 5)//Floating VU
            {
                double percentleft = levelLeft / 16384;
                heightL = (int)(percentleft * numLED);
                double percentright = levelRight / 16384;
                heightR = (int)(percentright * numLED);

                for (int i = 0; i < numLED/2; i++)
                {
                    if (Math.Abs(0 - i) <= heightR)
                        brightnessMap[i] =0.0 ;
                    else
                        brightnessMap[i] = maxbrightness / 255.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightL)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0;

                }
            }
            else if (musicMode == 6)//Center VU
            {
                double percentleft = levelLeft / 16384;
                heightL = (int)(percentleft * numLED);
                double percentright = levelRight / 16384;
                heightR = (int)(percentright * numLED);

                for (int i = numLED/2; i>0; i--)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightL)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] =0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightR)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0;

                }
            }

            else if (musicMode==7)// jumping bass?
            {
                //declair position to jump to when bass rise
                //which bass to chose
                //var Ba
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




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }
        public void Stop()
        {
            _log.Debug("Stop called.");
            if (_workerThread == null) return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            _workerThread?.Join();
            _workerThread = null;
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


