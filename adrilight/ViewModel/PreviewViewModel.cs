using adrilight.Util;
using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace adrilight.ViewModel
{
   public class PreviewViewModel : ViewModelBase
    {

        public ISpot[] _previewSpots;
        public ISpot[] PreviewSpots {
            get => _previewSpots;
            set
            {
                _previewSpots = value;
                RaisePropertyChanged();
            }
        }
        private DeviceInfoDTO _card;
        public DeviceInfoDTO Card {
            get { return _card; }
            set
            {
                if (_card == value) return;
                if (_card != null)
                {
                    _card.PropertyChanged -= _card_PropertyChanged;
                }
                _card = value;
                if (_card != null)
                {
                    _card.PropertyChanged += _card_PropertyChanged;
                }
                RaisePropertyChanged();
            }
        }

        private void _card_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           
        }
        private readonly ViewModelBase _parentVm;
        private MainViewViewModel _mainView => _parentVm as MainViewViewModel;
        public bool IsRunning { get; private set; } = false;
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

        private CancellationTokenSource _cancellationTokenSource;
        static readonly object _object = new object();
        double point = 0;
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }
        private readonly ISpotSet spotSet;
        public PreviewViewModel(DeviceInfoDTO device, ViewModelBase parent, ISpotSet spotSet)
        {
            this.spotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            PreviewSpots = spotSet.Spots;
            _parentVm = parent;
            Card = device;
            Card.LEDNumber = 30;
            BassNet.Registration("saorihara93@gmail.com", "2X2831021152222");
            _process = new WASAPIPROC(Process);
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;

            RefreshColorState();
        }
        private void RefreshColorState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = true;//UserSettings.TransferActive && UserSettings.SelectedEffect == 2;
                                       //if (isRunning && !shouldBeRunning)
                                       //{
                                       //    //stop it!
                                       //    //_log.Debug("stopping the StaticColor");
                                       //    _cancellationTokenSource.Cancel();
                                       //    _cancellationTokenSource = null;
                                       //}


            //else if (!isRunning && shouldBeRunning)
            //{
            //    //start it
            //   // _log.Debug("starting the StaticColor");
            //    _cancellationTokenSource = new CancellationTokenSource();
            //    var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
            //        IsBackground = true,
            //        Priority = ThreadPriority.BelowNormal,
            //        Name = "StaticColorCreator"
            //    };
            //    thread.Start();
            //}
            _cancellationTokenSource = new CancellationTokenSource();
            var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
                Name = "StaticColorCreator"
            };
            thread.Start();
        }
        public void Run(CancellationToken token)//static color creator
        {
            if (IsRunning) throw new Exception(" Static Color is already running!");

            IsRunning = true;

          

            Color defaultColor = Color.FromRgb(127, 127, 0);


            try
            {
                string mode = "Sáng theo nhạc";
               
                bool isBreathing = Card.IsBreathing;
                //PreviewSpots = new ISpot[30];
                
                while (!token.IsCancellationRequested)
                {
                    switch (mode)
                    {
                        case "Sáng màu tĩnh":
                            ColorModeStaticColor();break;
                        case "Sáng theo nhạc":
                            ColorModeMusic(); break;
                        default:
                            break;
                    }

                }
                //motion speed

            }








            catch (OperationCanceledException)
            {
              
                return;
            }
            catch (Exception ex)
            {

                Thread.Sleep(500);
            }
            finally
            {


              
                IsRunning = false;
            }

        }
        public void ColorModeStaticColor()
        {

            lock (_object)
            {
                var numLED = Card.LEDNumber;
                Color currentStaticColor = _mainView.SettingInfo.PrimaryColor;
                var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                double peekBrightness = 0.0;
                if (Card.IsBreathing)
                {
                    if (point < 500)
                    {
                        //peekBrightness = 1.0 - Math.Abs((2.0 * (point / 500)) - 1.0);
                        peekBrightness = Math.Exp(-(Math.Pow(((point / 500) - 0.5) / 0.14, 2.0)) / 2.0);
                        point++;
                    }
                    else
                    {
                        point = 0;
                    }



                }
                else
                {
                    peekBrightness = Card.Brightness / 100.0;
                }

                int counter = 0;
                foreach (ISpot spot in PreviewSpots)
                {
                    colorOutput[counter] = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(currentStaticColor.R, currentStaticColor.G, currentStaticColor.B), Card.Brightness / 100.0);
                    spot.SetColor(colorOutput[counter].R, colorOutput[counter].G, colorOutput[counter].B, true);
                    counter++;


                }


                Thread.Sleep(5);
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
                        brightnessMap[i] = maxbrightness / 255.0;
                    else
                        brightnessMap[i] = 0.0;

                }


                for (int i = numLED / 2; i < numLED; i++)
                {
                    if (Math.Abs(numLED / 2 - i) <= heightL)
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
                //declair position to jump to when bass rise
                //which bass to chose
                //var Ba
            }



            return brightnessMap;

        }
        public void ColorModeMusic()
        {
            double brightness = Card.Brightness / 100d;
            int paletteSource = Card.Palette;
            var numLED =Card.LEDNumber;
            byte[] spectrumdata = new byte[numLED];
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
            volumeLeft = (volumeLeft * 6 + Utils.LowWord32(level) * 2) / 8;
            volumeRight = (volumeRight * 6 + Utils.HighWord32(level) * 2) / 8;
            _lastlevel = level;
            byte musicMode =(byte) Card.MusicMode;
            OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
            int counter = 0;
            lock (_object)
            {
                if (paletteSource == 0)
                {
                    var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);
                    var brightnessMap = SpectrumCreator(spectrumdata, 0, volumeLeft, volumeRight, musicMode, numLED);// get brightness map based on spectrum data
                    foreach (var color in newcolor)
                    {
                        outputColor[counter] = Brightness.applyBrightness(color, brightnessMap[counter]);
                        counter++;

                    }
                    counter = 0;
                    foreach (ISpot spot in PreviewSpots)
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
                
            }
            Thread.Sleep(5); //motion speed

        }
        public void ColorModeRainbow()
        {
            double brightness = Card.Brightness / 100d;
            int paletteSource = Card.Palette;
            var numLED =Card.LEDNumber;
            var colorOutput = new OpenRGB.NET.Models.Color[numLED];

            OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
            int counter = 0;
            lock (_object)
            {
                if (paletteSource == 0)
                {
                    var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);

                    foreach (var color in newcolor)
                    {
                        outputColor[counter++] = Brightness.applyBrightness(color, brightness);

                    }
                    counter = 0;
                    foreach (ISpot spot in PreviewSpots)
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
              
            }
            Thread.Sleep(5); //motion speed

        }
    }
    }
    
    

