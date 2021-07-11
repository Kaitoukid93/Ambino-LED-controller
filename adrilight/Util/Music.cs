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
using System.Diagnostics;
using adrilight.Spots;

namespace adrilight
{
    internal class Music : IMusic
    {
       

        public static float[] _fft;
        public static int _lastlevel;
        public static int _hanctr;
        public static int volumeLeft;
        public static int volumeRight;
        public static int height = 0;
        public static int heightL = 0;
        public static int heightR = 0;
        public WASAPIPROC _process;
        public static byte lastvolume = 0;
        public static byte volume = 0;
        public static int lastheight = 0;

        public static bool bump = false;

        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Music(IDeviceSettings deviceSettings, IDeviceSpotSet deviceSpotSet)
        {
            DeviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
            DeviceSpotSet = deviceSpotSet ?? throw new ArgumentNullException(nameof(deviceSpotSet));
            //SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            //Remove SettingsViewmodel from construction because now we pass SpotSet Dirrectly to MainViewViewModel
            DeviceSettings.PropertyChanged += PropertyChanged;
           // SettingsViewModel.PropertyChanged += PropertyChanged;
            BassNet.Registration("saorihara93@gmail.com", "2X2831021152222");
            _process = new WASAPIPROC(Process);
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;
            RefreshAudioState();
            _log.Info($"MusicColor Created");

        }
        //Dependency Injection//
        private IDeviceSettings DeviceSettings { get; }
       //private SettingsViewModel SettingsViewModel { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private IDeviceSpotSet DeviceSpotSet { get; }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DeviceSettings.TransferActive):
                case nameof(DeviceSettings.SelectedEffect):
                case nameof(DeviceSettings.Brightness):
                case nameof(DeviceSettings.SelectedMusicMode):
                case nameof(DeviceSettings.SelectedMusicPalette):
                case nameof(DeviceSettings.SpotsX):
                case nameof(DeviceSettings.SpotsY):
                

                    RefreshAudioState();
                    break;
                case nameof(DeviceSettings.SelectedAudioDevice):
                    RefreshAudioDevice();

                    break;
            }
        }
        private void RefreshAudioState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = DeviceSettings.TransferActive &&DeviceSettings.SelectedEffect == 3;



            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the Music Color");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
               // Free();
            }



            else if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the Music Color");

                Init();
                int deviceID = AudioDeviceID;
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
            var shouldBeRunning = DeviceSettings.TransferActive && DeviceSettings.SelectedEffect == 3;
            //var shouldBeRunning = DeviceSettings.TransferActive && DeviceSettings.SelectedEffect == 3;
            if (isRunning && shouldBeRunning)
            {

                _log.Debug("Refreshing the Music Color");
                Init();
                int deviceID = AudioDeviceID;
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

       
        public void Run(CancellationToken token)

        {

              double _huePosIndex = 0;//index for rainbow mode only
       // public static double _palettePosIndex = 0;//index for other custom palette
            double _startIndex = 0;
            if (IsRunning) throw new Exception(" Music Color is already running!");

            IsRunning = true;

            _log.Debug("Started Music Color.");
            // double brightness = UserSettings.Brightness / 100d;

            var numLED = (DeviceSettings.SpotsX - 1) * 2 + (DeviceSettings.SpotsY - 1) * 2;
            byte[] spectrumdata = new byte[numLED];

            try
            {

                while (!token.IsCancellationRequested)
                {
                    double senspercent = DeviceSettings.MSens / 100d;
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
                        spectrumdata[x] = (byte)((spectrumdata[x] * 6 + y * 2 + 7) / 8);

                        spectrumdata[x] = (byte)((byte)(spectrumdata[x] * senspercent) + spectrumdata[x]);
                        //Smoothing out the value (take 5/8 of old value and 3/8 of new value to make finnal value)
                        if (spectrumdata[x] > 255)
                            spectrumdata[x] = 255;
                        if (spectrumdata[x] < 15)
                            spectrumdata[x] = 0;

                    }
                    int paletteSource = DeviceSettings.SelectedMusicPalette;
                    int level = BassWasapi.BASS_WASAPI_GetLevel(); // Get level (VU metter) for Old AMBINO Device (remove in the future)
                    if (level == _lastlevel && level != 0) _hanctr++;
                    volumeLeft = (volumeLeft * 6 + Utils.LowWord32(level) * 2) / 8;
                    volumeRight = (volumeRight * 6 + Utils.HighWord32(level) * 2) / 8;
                    _lastlevel = level;
                    byte musicMode = DeviceSettings.SelectedMusicMode;
                   // bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 3);
                    //audio capture section//


                    var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);

                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;
                    lock (DeviceSpotSet.Lock)
                    {
                        var brightnessMap = SpectrumCreator(spectrumdata, 0, volumeLeft, volumeRight, musicMode, numLED);// get brightness map based on spectrum data
                        if (paletteSource == 0)
                        {
                            newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);
                            foreach (var color in newcolor)
                            {
                                outputColor[counter] = Brightness.applyBrightness(color, brightnessMap[counter]);
                                counter++;

                            }
                        }
                        else
                        {
                            for (int i = 0; i < numLED; i++)
                            {
                                //double position = i / (double)numLED;
                                var position = _startIndex + (1000d / (1 * numLED)) * i;

                                if (position > 1000)
                                    position = position - 1000;
                                Color colorPoint = Color.FromRgb(0, 0, 0);
                                if (paletteSource == 1)//party color palette
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(cafe), position);
                                }
                                else if (paletteSource == 2)//cloud color palette
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(jazz), position);
                                }
                                else if (paletteSource == 3)//cloud color palette
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(party), position);
                                }
                                else if (paletteSource == 4)//cloud color palette
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(custom), position);
                                }
                                var newColor = new OpenRGB.NET.Models.Color(colorPoint.R, colorPoint.G, colorPoint.B);
                                outputColor[i] = Brightness.applyBrightness(newColor, brightnessMap[i]);


                            }
                            _startIndex += 1;
                            if (_startIndex > 1000)
                            {
                                _startIndex = 0;
                            }
                        }


                        counter = 0;
                        foreach (IDeviceSpot spot in DeviceSpotSet.Spots)
                        {
                            ApplySmoothing(outputColor[counter].R, outputColor[counter].G, outputColor[counter].B, out byte FinalR, out byte FinalG, out byte FinalB, spot.Red, spot.Green, spot.Blue);
                            spot.SetColor(FinalR, FinalG, FinalB, true);
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

        public int _audioDeviceID = -1;
        public int AudioDeviceID {
            get
            {
                if (DeviceSettings.SelectedAudioDevice > AvailableAudioDevice.Count)
                {
                    System.Windows.MessageBox.Show("Last Selected Audio Device is not Available");
                    return -1;
                }
                else
                {
                    var currentDevice = AvailableAudioDevice.ElementAt(DeviceSettings.SelectedAudioDevice);

                    var array = currentDevice.Split(' ');
                    _audioDeviceID = Convert.ToInt32(array[0]);
                    return _audioDeviceID;
                }

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
        private void ApplySmoothing(float r, float g, float b, out byte semifinalR, out byte semifinalG, out byte semifinalB,
        byte lastColorR, byte lastColorG, byte lastColorB)
        {
            ;

            semifinalR = (byte)((r + 3 * lastColorR) / (3 + 1));
            semifinalG = (byte)((g + 3 * lastColorG) / (3 + 1));
            semifinalB = (byte)((b + 3 * lastColorB) / (3 + 1));
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
                int finalheight = (height * 4 + lastheight * 4 + 7) / 8;
                foreach (var brightness in brightnessMap.Take(finalheight))
                {

                    brightnessMap[counter++] = maxbrightness / 255.0;

                }


                for (int i = finalheight; i < numLED; i++)
                {

                    brightnessMap[counter++] = 0;

                }
                lastheight = finalheight;
            }
            else if (musicMode == 2)// End to End
            {
                double percent = ((levelLeft + levelRight) / 2) / 16384;
                height = (int)(percent * numLED);
                height = (int)(percent * numLED);
                int finalheight = (height * 4 + lastheight * 4 + 7) / 8;
                for (int i = 0; i < numLED / 2; i++)
                {
                    if (i <= finalheight / 2)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (numLED - i <= finalheight / 2)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }
                lastheight = finalheight;
            }
            else if (musicMode == 3)//Push Pull
            {
                double percent = ((levelLeft + levelRight) / 2) / 16384;
                height = (int)(percent * numLED);
                height = (int)(percent * numLED);
                int finalheight = (height * 4 + lastheight * 4 + 7) / 8;
                for (int i = 0; i < numLED / 2; i++)
                {
                    if (i <= finalheight / 2)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (numLED - i >= finalheight / 2)
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }
                lastheight = finalheight;
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

                    brightnessMap[i] = fft[equalizer[i]] / 255.0;

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

        OpenRGB.NET.Models.Color fade(double damper, OpenRGB.NET.Models.Color color)
        {

            //"damper" must be between 0 and 1, or else you'll end up brightening the lights or doing nothing.

            OpenRGB.NET.Models.Color returncolor = new OpenRGB.NET.Models.Color();

            //Retrieve the color at the current position.


            //If it's black, you can't fade that any further.
            if (color.R == 0 && color.G == 0 && color.B == 0)
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
        public static int[] EqualizerPick(int mode, int numLED)
        {


            int[] equalizerPick = new int[numLED];
            if (mode == 0)
            {
                for (int i = 0; i < numLED; i++)
                {
                    if (i < numLED / 4)
                    {
                        equalizerPick[i] = 2;
                    }
                    if (i > numLED / 2 && i < 3 * numLED / 4)
                    {
                        equalizerPick[i] = 2;
                    }
                    if (i >= numLED / 4 && i < numLED / 2)
                    {
                        equalizerPick[i] = numLED / 2;
                    }
                    if (i >= 3 * numLED / 4)
                    {
                        equalizerPick[i] = numLED / 2;
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






        private static Color GetColorByOffset(GradientStopCollection collection, double position)
        {
            double offset = position / 1000.0;
            GradientStop[] stops = collection.OrderBy(x => x.Offset).ToArray();
            if (offset <= 0) return stops[0].Color;
            if (offset >= 1) return stops[stops.Length - 1].Color;
            GradientStop left = stops[0], right = null;
            foreach (GradientStop stop in stops)
            {
                if (stop.Offset >= offset)
                {
                    right = stop;
                    break;
                }
                left = stop;
            }
            Debug.Assert(right != null);
            offset = Math.Round((offset - left.Offset) / (right.Offset - left.Offset), 2);

            byte r = (byte)((right.Color.R - left.Color.R) * offset + left.Color.R);
            byte g = (byte)((right.Color.G - left.Color.G) * offset + left.Color.G);
            byte b = (byte)((right.Color.B - left.Color.B) * offset + left.Color.B);
            return Color.FromRgb(r, g, b);
        }
        public GradientStopCollection GradientPaletteColor(Color[] ColorCollection)
        {
            GetCustomColor();
            GradientStopCollection gradientPalette = new GradientStopCollection(16);
            gradientPalette.Add(new GradientStop(ColorCollection[0], 0.00));
            gradientPalette.Add(new GradientStop(ColorCollection[1], 0.066));
            gradientPalette.Add(new GradientStop(ColorCollection[2], 0.133));
            gradientPalette.Add(new GradientStop(ColorCollection[3], 0.199));
            gradientPalette.Add(new GradientStop(ColorCollection[4], 0.265));
            gradientPalette.Add(new GradientStop(ColorCollection[5], 0.331));
            gradientPalette.Add(new GradientStop(ColorCollection[6], 0.397));
            gradientPalette.Add(new GradientStop(ColorCollection[7], 0.464));
            gradientPalette.Add(new GradientStop(ColorCollection[8], 0.529));
            gradientPalette.Add(new GradientStop(ColorCollection[9], 0.595));
            gradientPalette.Add(new GradientStop(ColorCollection[10], 0.661));
            gradientPalette.Add(new GradientStop(ColorCollection[11], 0.727));
            gradientPalette.Add(new GradientStop(ColorCollection[12], 0.793));
            gradientPalette.Add(new GradientStop(ColorCollection[13], 0.859));
            gradientPalette.Add(new GradientStop(ColorCollection[14], 0.925));
            gradientPalette.Add(new GradientStop(ColorCollection[15], 1));
            //gradientPalette.Add(new GradientStop(ColorCollection[0], 1));
            //gradientPalette.Add(new GradientStop(ColorCollection[0], 0.0000));
            //gradientPalette.Add(new GradientStop(ColorCollection[1], 0.0625));
            //gradientPalette.Add(new GradientStop(ColorCollection[2], 0.1250));
            //gradientPalette.Add(new GradientStop(ColorCollection[3], 0.1875));
            //gradientPalette.Add(new GradientStop(ColorCollection[4], 0.2500));
            //gradientPalette.Add(new GradientStop(ColorCollection[5], 0.3125));
            //gradientPalette.Add(new GradientStop(ColorCollection[6], 0.3750));
            //gradientPalette.Add(new GradientStop(ColorCollection[7], 0.4375));
            //gradientPalette.Add(new GradientStop(ColorCollection[8], 0.5000));
            //gradientPalette.Add(new GradientStop(ColorCollection[9], 0.5625));
            //gradientPalette.Add(new GradientStop(ColorCollection[10], 0.6250));
            //gradientPalette.Add(new GradientStop(ColorCollection[11], 0.6875));
            //gradientPalette.Add(new GradientStop(ColorCollection[12], 0.7500));
            //gradientPalette.Add(new GradientStop(ColorCollection[13], 0.8125));
            //gradientPalette.Add(new GradientStop(ColorCollection[14], 0.8750));
            //gradientPalette.Add(new GradientStop(ColorCollection[15], 0.9375));
            //gradientPalette.Add(new GradientStop(ColorCollection[0], 1.000));
            return gradientPalette;
        }


        //Custom color by color picker value
        public Color[] custom = new Color[16];
        public void GetCustomColor()
        {
            custom[0] = DeviceSettings.MColor0;
            custom[1] = DeviceSettings.MColor1;
            custom[2] = DeviceSettings.MColor2;
            custom[3] = DeviceSettings.MColor3;
            custom[4] = DeviceSettings.MColor4;
            custom[5] = DeviceSettings.MColor5;
            custom[6] = DeviceSettings.MColor6;
            custom[7] = DeviceSettings.MColor7;
            custom[8] = DeviceSettings.MColor8;
            custom[9] = DeviceSettings.MColor9;
            custom[10] = DeviceSettings.MColor10;
            custom[11] = DeviceSettings.MColor11;
            custom[12] = DeviceSettings.MColor12;
            custom[13] = DeviceSettings.MColor13;
            custom[14] = DeviceSettings.MColor14;
            custom[15] = DeviceSettings.MColor15;


        }

        public static Color[] cafe = {
             Color.FromRgb (253,237,204),
              Color.FromRgb (253,237,204),
             Color.FromRgb (246,172,51),
             Color.FromRgb (246,172,51),
             Color.FromRgb (67,168,150),
             Color.FromRgb (67,168,150),
             Color.FromRgb (253,237,204),
             Color.FromRgb (253,237,204),
             Color.FromRgb (253,237,204),
             Color.FromRgb (253,237,204),
             Color.FromRgb (67,168,150),
             Color.FromRgb (67,168,150),
             Color.FromRgb (246,172,51),
             Color.FromRgb (246,172,51),
             Color.FromRgb (253,237,204),
              Color.FromRgb (253,237,204)

    };
        public static Color[] jazz = {
             Color.FromRgb (227,74,39),
             Color.FromRgb (227,74,39),
             Color.FromRgb (254,166,85),
            Color.FromRgb (254,166,85),
             Color.FromRgb (86,99,87),
             Color.FromRgb (86,99,87),
             Color.FromRgb (144,148,115),
             Color.FromRgb (144,148,115),
             Color.FromRgb (255,217,142),
              Color.FromRgb (255,217,142),
              Color.FromRgb (86,99,87),
             Color.FromRgb (86,99,87),
             Color.FromRgb (254,166,85),
             Color.FromRgb (254,166,85),
             Color.FromRgb (227,74,39),
             Color.FromRgb (227,74,39)

    };
        public static Color[] party = {
             Color.FromRgb (73,145,1),
             Color.FromRgb (250,186,3),
             Color.FromRgb (52,195,203),
            Color.FromRgb (0,237,1),
             Color.FromRgb (251,50,164),
             Color.FromRgb (209,2,172),
             Color.FromRgb (255,1,100),
             Color.FromRgb (253,166,53),
             Color.FromRgb (5,96,140),
              Color.FromRgb (76,148,4),
              Color.FromRgb (228,0,77),
             Color.FromRgb (0,217,2),
             Color.FromRgb (205,2,154),
             Color.FromRgb (253,190,14),
             Color.FromRgb (233,157,73),
             Color.FromRgb (73,145,1)

    };


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


