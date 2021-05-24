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
        
        public static float[] _fft;
        public static int _lastlevel;
        public static int _hanctr;
        public static int volumeLeft;
        public static int volumeRight;
        public static int height = 0;
        public static int heightL = 0;
        public static int heightR = 0;
        private WASAPIPROC _process;
        public static byte lastvolume=0;
        public static byte volume = 0;
        
        public static bool bump = false;

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
                case nameof(UserSettings.SelectedMusicMode):
                case nameof(UserSettings.SpotsX):
                case nameof(UserSettings.SpotsY):
                case nameof(SettingsViewModel.IsSettingsWindowOpen):

                    RefreshAudioState();
                    break;
                case nameof(SettingsViewModel.AudioDeviceID):
                    RefreshAudioDevice();
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

                Init();
                int deviceID = SettingsViewModel.AudioDeviceID;
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
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "MusicColorCreator"
                };
                thread.Start();
            }
        }
        private void RefreshAudioDevice()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.SelectedEffect == 3;
            //var shouldBeRunning = DeviceSettings.TransferActive && DeviceSettings.SelectedEffect == 3;
            if (isRunning && shouldBeRunning)
            {

                _log.Debug("Refreshing the Music Color");
                Init();
                int deviceID = SettingsViewModel.AudioDeviceID;
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
            double brightness = UserSettings.Brightness / 100d;
            int paletteSource = UserSettings.SelectedPalette;
            var numLED = (UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2;
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
                    byte musicMode = UserSettings.SelectedMusicMode;
                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 3);
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


                double percent = ((levelLeft + levelRight) / 2) / 16384;
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
                    if (numLED - i <= height / 2)
                        brightnessMap[i] = maxbrightness / 255.0;
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
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }
            }
            else if (musicMode == 4)//Symetric VU
            {

                double percentleft = levelLeft / 16384;
                heightL = (int)(percentleft * numLED);
                double percentright = levelRight / 16384;
                heightR = (int)(percentright * numLED);


                for (int i = 0; i < numLED / 2; i++)
                {
                    if (i <= heightR)
                        brightnessMap[i] = maxbrightness;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightL)
                        brightnessMap[i] = maxbrightness;
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

                for (int i = 0; i < numLED / 2; i++)
                {
                    if (Math.Abs(0 - i) <= heightR)
                        brightnessMap[i] = 0.0;
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

                for (int i = numLED / 2; i > 0; i--)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightL)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightR)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0;

                }
            }

            else if (musicMode == 7)// jumping bass?
            {
                Random random = new Random();
                var equalizer = EqualizerPick(0, numLED);
                for (int i = 0; i < brightnessMap.Count(); i++)
                {

                    brightnessMap[i] = fft[equalizer[i]]/255.0;

                }

            }
                return brightnessMap;

        }

        //utilities for creative music mode
        //bleed, credit for Sparkfun music Neopixel https://github.com/mbartlet/SparkFun-RGB-LED-Music-Sound-Visualizer-Arduino-Code/blob/master/Visualizer_Program/Visualizer_Program.ino
        //void bleed(int Point, int numLED, OpenRGB.NET.Models.Color[] currentcolor)
        //{
        //    for (int i = 1; i < numLED; i++)
        //    {

        //        //Starts by look at the pixels left and right of "Point"
        //        //  then slowly works its way out
        //        int[] sides = { Point - i, Point + i };

        //        for (int j = 0; j < 2; j++)
        //        {

        //            //For each of Point+i and Point-i, the pixels to the left and right, plus themselves, are averaged together.
        //            //  Basically, it's setting one pixel to the average of it and its neighbors, starting on the left and right
        //            //  of the starting "Point," and moves to the ends of the strand
        //            int point = sides[j];
        //            int[] colors = { currentcolor(point - 1),currentcolor(point), currentcolor(point + 1) };

        //            //Sets the new average values to just the central point, not the left and right points.
        //            strand.setPixelColor(point, strand.Color(
        //                                   float(split(colors[0], 0) + split(colors[1], 0) + split(colors[2], 0)) / 3.0,
        //                                   float(split(colors[0], 1) + split(colors[1], 1) + split(colors[2], 1)) / 3.0,
        //                                   float(split(colors[0], 2) + split(colors[1], 2) + split(colors[2], 2)) / 3.0)
        //                                );
        //        }
        //    }
        //}

        OpenRGB.NET.Models.Color fade (double damper,OpenRGB.NET.Models.Color color)
        {

            //"damper" must be between 0 and 1, or else you'll end up brightening the lights or doing nothing.

            OpenRGB.NET.Models.Color returncolor = new OpenRGB.NET.Models.Color();

                //Retrieve the color at the current position.


            //If it's black, you can't fade that any further.
            if (color.R == 0&& color.G==0&&color.B==0)
            {
                returncolor.R = 0;
                returncolor.G = 0;
                returncolor.B = 0;
            }
            else
            {
                returncolor.R = (byte)(color.R * damper);
                returncolor.G = (byte)(color.G * damper);
                returncolor.B = (byte)(color.B * damper);
            }

            return returncolor;
               
        }
        public static int[] EqualizerPick(int mode,int numLED)
        {
           
            int[] equalizerPick = new int[numLED];
            if (mode == 0)
            {
                for (int i = 0; i < numLED; i++)
                {
                    if(i<numLED/4)
                    {
                        equalizerPick[i] = 2;
                    }
                    if(i>numLED/2&&i<3*numLED/4)
                    { 
                        equalizerPick[i] = 2;
                    }
                    if(i>=numLED/4&&i<numLED/2)
                    {
                        equalizerPick[i] = numLED/2;
                    }
                    if(i>=3*numLED/4)
                    {
                        equalizerPick[i] = numLED/2;
                    }
                }

            }
           
            
           
            return equalizerPick;

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


