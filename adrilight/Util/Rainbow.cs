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
using System.Diagnostics;
using adrilight.Spots;

namespace adrilight
{
    internal class Rainbow : IRainbow
    {
        // public static Color[] small = new Color[30];
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        public static double _startIndex = 0;

        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Rainbow(IDeviceSettings deviceSettings, IDeviceSpotSet deviceSpotSet)
        {
            DeviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
            DeviceSpotSet = deviceSpotSet ?? throw new ArgumentNullException(nameof(deviceSpotSet));
            //SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            //Remove SettingsViewmodel from construction because now we pass SpotSet Dirrectly to MainViewViewModel
            DeviceSettings.PropertyChanged += PropertyChanged;
           // SettingsViewModel.PropertyChanged += PropertyChanged;
            RefreshColorState();
            _log.Info($"RainbowColor Created");

        }
        //Dependency Injection//
        private IDeviceSettings DeviceSettings { get; }
        private IDeviceSpotSet DeviceSpotSet { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DeviceSettings.TransferActive):
                case nameof(DeviceSettings.SelectedEffect):
                case nameof(DeviceSettings.SelectedPalette):
                case nameof(DeviceSettings.Brightness):
                    RefreshColorState();
                    break;
            }
        }
        private void RefreshColorState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = DeviceSettings.TransferActive && DeviceSettings.SelectedEffect == 1;
            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the Rainbow Color");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }
            else if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the Rainbow Color");
                _cancellationTokenSource = new CancellationTokenSource();
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "RainbowColorCreator" + DeviceSettings
                };
                thread.Start();
            }
        }




       
        public void Run(CancellationToken token)

        {
            if (IsRunning) throw new Exception(" Rainbow Color is already running!");

            IsRunning = true;

            _log.Debug("Started Rainbow Color.");

            try
            {

                while (!token.IsCancellationRequested)
                {
                    double brightness = DeviceSettings.Brightness / 100d;
                    int paletteSource = DeviceSettings.SelectedPalette;
                    var numLED = (DeviceSettings.SpotsX - 1) * 2 + (DeviceSettings.SpotsY - 1) * 2;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                    var effectSpeed = DeviceSettings.EffectSpeed;
                    var frequency = DeviceSettings.ColorFrequency + 1;



                   


                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;
                    lock (DeviceSpotSet.Lock)
                    {
                        if (paletteSource == 0)
                        {
                            var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);

                            foreach (var color in newcolor)
                            {
                                outputColor[counter++] = Brightness.applyBrightness(color, brightness);

                            }
                            if (_huePosIndex > 360)
                            {
                                _huePosIndex = 0;
                            }
                            else
                            {
                                _huePosIndex += effectSpeed;
                            }

                        }
                        else
                        {


                            //var newcolor = PaletteCreator(party);//get fullsize palette
                            //fill led from fullsize palette color
                            // int factor = 256 / numLED;
                            // int position = _startIndex;
                            for (int i = 0; i < numLED; i++)
                            {
                                //double position = i / (double)numLED;
                                var position = _startIndex + (1000d / (frequency * numLED)) * i;

                                if (position > 1000)
                                    position = position - 1000;
                                Color colorPoint = Color.FromRgb(0, 0, 0);
                                if (paletteSource == 1)//party color palette
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(cloud), position);
                                }
                                else if (paletteSource == 2)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(forest), position);
                                }
                                else if (paletteSource == 3)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(sunset), position);
                                }
                                else if (paletteSource == 4)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(scarlet), position);
                                }
                                else if (paletteSource == 5)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(aurora), position);
                                }
                                else if (paletteSource == 6)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(france), position);
                                }
                                else if (paletteSource == 7)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(lemon), position);
                                }
                                else if (paletteSource == 8)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(badtrip), position);
                                }
                                else if (paletteSource == 9)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(police), position);
                                }
                                else if (paletteSource == 10)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(iceandfire), position);
                                }
                                else if (paletteSource == 11)
                                {
                                    colorPoint = GetColorByOffset(GradientPaletteColor(custom), position);
                                }
                                var newColor = new OpenRGB.NET.Models.Color(colorPoint.R, colorPoint.G, colorPoint.B);
                                outputColor[i] = Brightness.applyBrightness(newColor, brightness);
                            }



                            _startIndex += effectSpeed;
                            if (_startIndex > 1000)
                            {
                                _startIndex = 0;
                            }




                        }

                        counter = 0;
                        foreach (IDeviceSpot spot in DeviceSpotSet.Spots)
                        {
                            spot.SetColor(outputColor[counter].R, outputColor[counter].G, outputColor[counter].B, true);
                            counter++;

                        }


                    }
                    Thread.Sleep(10); 

                }
            }
            catch (OperationCanceledException)
            {
                _log.Debug("OperationCanceledException catched. returning.");

      
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




        private Bitmap DrawFilledRectangle(int x, int y)
        {
            Bitmap bmp = new Bitmap(x, y);
            using (Graphics graph = Graphics.FromImage(bmp))
            {
                Rectangle ImageSize = new Rectangle(0, 0, x, y);
                graph.FillRectangle(System.Drawing.Brushes.White, ImageSize);
            }
            return bmp;
        }




        private static OpenRGB.NET.Models.Color[] PaletteCreator(OpenRGB.NET.Models.Color[] colorCollection)
        {
            //numLED: number of LED to create on the view
            //startIndex: index to start drawing palette
            //playground: canvans to draw into
            //isMusic: sound reaction boolean
            //musicValue: value of current sound level
            //numColor: number of color to create from colorCollection
            //colorCollection: actually the palette
            //expand color from Collection
            // int factor = numLED / colorCollection.Count(); //scaling factor
            int colorcount = 0;
            var colorOutput = new OpenRGB.NET.Models.Color[256];
            //todo: expand current palette to 256 color for smooth effect


            for (int i = 0; i < 16; i++)
            {
                //for (int j = 0; j < factor; j++)
                //{

                //    if (colorcount > numLED)
                //    {
                //        colorcount = 0;
                //    }
                //    colorOutput[colorcount++] = colorCollection[i];
                //}
                for (int j = 0; j < 16; j++)
                {
                    colorOutput[colorcount++] = colorCollection[i];
                }
            }

            //finally
            //  fillRectFromColor(paletteOutput, playground, numLED);
            return colorOutput;

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
            gradientPalette.Add(new GradientStop(ColorCollection[0], 1));
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





        // predefined color palette

        public static Color[] party = {
             Color.FromRgb (88,53,148),
             Color.FromRgb (129,39,122),
             Color.FromRgb (181,30,78),
             Color.FromRgb (228,30,38),
             Color.FromRgb (0,255,0),
             Color.FromRgb (183,74,38),
             Color.FromRgb (170,121,43),
             Color.FromRgb (169,171,54),
             Color.FromRgb (170,87,38),
             Color.FromRgb (220,39,38),
             Color.FromRgb (237,28,36),
             Color.FromRgb (194,31,65),
             Color.FromRgb (140,36,114),
             Color.FromRgb (96,45,144),
             Color.FromRgb (67,70,157),
             Color.FromRgb (88,53,148)




        };
        public static Color[] cloud = {
             Color.FromRgb (0,152,255),
             Color.FromRgb (0,104,255),
             Color.FromRgb (0,62,216),
             Color.FromRgb (4,54,178),
             Color.FromRgb (0,43,150),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (220,245,255),
             Color.FromRgb (202,243,255),
             Color.FromRgb (123,224,255),
             Color.FromRgb (16,170,255),
             Color.FromRgb (0,152,255)

    };

        public static Color[] forest = {
             Color.FromRgb (134,255,64),
             Color.FromRgb (53,214,0),
             Color.FromRgb (0,165,39),
             Color.FromRgb (3,114,50),
             Color.FromRgb (1,96,40),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (232,255,220),
             Color.FromRgb (209,255,184),
             Color.FromRgb (181,255,133),
             Color.FromRgb (170,255,80),
             Color.FromRgb (134,255,64)

    };

        public static Color[] sunset = {
             Color.FromRgb (244,126,32),
             Color.FromRgb (239,76,35),
             Color.FromRgb (237,52,36),
             Color.FromRgb (236,30,36),
             Color.FromRgb (169,30,34),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (246,235,15),
             Color.FromRgb (255,206,3),
             Color.FromRgb (252,181,20),
             Color.FromRgb (247,150,29),
             Color.FromRgb (244,126,32)

    };
        public static Color[] scarlet = {
             Color.FromRgb (236,34,143),
             Color.FromRgb (236,24,94),
             Color.FromRgb (236,30,36),
             Color.FromRgb (203,32,39),
             Color.FromRgb (146,26,29),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (193,111,172),
             Color.FromRgb (194,86,159),
             Color.FromRgb (214,93,161),
             Color.FromRgb (202,75,154),
             Color.FromRgb (236,34,143)

    };
        public static Color[] aurora = {
             Color.FromRgb (0,255,133),
             Color.FromRgb (0,249,255),
             Color.FromRgb (0,146,255),
             Color.FromRgb (0,104,255),
             Color.FromRgb (0,73,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (187,255,0),
             Color.FromRgb (137,255,0),
             Color.FromRgb (100,255,0),
             Color.FromRgb (94,255,0),
             Color.FromRgb (0,255,133)

    };
        public static Color[] france = {
             Color.FromRgb (255,0,19),
             Color.FromRgb (255,0,19),
             Color.FromRgb (255,0,19),
             Color.FromRgb (0,92,255),
             Color.FromRgb (0,92,255),
             Color.FromRgb (0,92,255),
             Color.FromRgb (0,92,255),
             Color.FromRgb (0,92,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,255,255),
             Color.FromRgb (255,0,19),
             Color.FromRgb (255,0,19),
             Color.FromRgb (255,0,19)

    };
        public static Color[] lemon = {
             Color.FromRgb (206,255,0),
             Color.FromRgb (144,255,0),
             Color.FromRgb (112,255,0),
             Color.FromRgb (19,255,0),
             Color.FromRgb (0,255,78),
             Color.FromRgb (246,235,15),
             Color.FromRgb (206,255,0),
             Color.FromRgb (144,255,0),
             Color.FromRgb (19,255,0),
             Color.FromRgb (0,255,78),
             Color.FromRgb (246,235,15),
             Color.FromRgb (246,235,15),
             Color.FromRgb (255,206,3),
             Color.FromRgb (255,255,0),
             Color.FromRgb (231,255,0),
             Color.FromRgb (206,255,0)

    };

        public static Color[] badtrip = {
             Color.FromRgb (34,34,242),
             Color.FromRgb (36,142,237),
             Color.FromRgb (0,189,255),
             Color.FromRgb (0,231,255),
             Color.FromRgb (0,255,242),
             Color.FromRgb (174,0,255),
             Color.FromRgb (99,0,255),
             Color.FromRgb (34,34,242),
             Color.FromRgb (34,34,242),
             Color.FromRgb (36,142,237),
             Color.FromRgb (0,189,255),
             Color.FromRgb (0,189,255),
             Color.FromRgb (0,255,242),
             Color.FromRgb (174,0,255),
             Color.FromRgb (99,0,255),
             Color.FromRgb (34,34,242)

    };
        public static Color[] police = {
             Color.FromRgb (18,0,255),
             Color.FromRgb (18,0,255),
             Color.FromRgb (0,0,0),
             Color.FromRgb (0,0,0),
             Color.FromRgb (0,0,0),
             Color.FromRgb (0,0,0),
             Color.FromRgb (255,0,37),
             Color.FromRgb (255,0,37),
             Color.FromRgb (255,0,37),
             Color.FromRgb (255,0,37),
             Color.FromRgb (0,0,0),
             Color.FromRgb (0,0,0),
             Color.FromRgb (0,0,0),
             Color.FromRgb (0,0,0),
             Color.FromRgb (18,0,255),
             Color.FromRgb (18,0,255)

    };
        public static Color[] iceandfire = {
             Color.FromRgb (0,255,224),
              Color.FromRgb (0,255,224),
             Color.FromRgb (141,255,249),
             Color.FromRgb (141,255,249),
             Color.FromRgb (255,176,0),
             Color.FromRgb (255,176,0),
             Color.FromRgb (255,109,0),
             Color.FromRgb (255,109,0),
             Color.FromRgb (255,109,0),
             Color.FromRgb (255,109,0),
             Color.FromRgb (255,176,0),
             Color.FromRgb (255,176,0),
             Color.FromRgb (141,255,249),
             Color.FromRgb (141,255,249),
              Color.FromRgb (0,255,224),
              Color.FromRgb (0,255,224)

    };

        //Custom color by color picker value
        public Color[] custom = new Color[16];
        public void GetCustomColor()
        {
            custom[0] = DeviceSettings.Color0;
            custom[1] = DeviceSettings.Color1;
            custom[2] = DeviceSettings.Color2;
            custom[3] = DeviceSettings.Color3;
            custom[4] = DeviceSettings.Color4;
            custom[5] = DeviceSettings.Color5;
            custom[6] = DeviceSettings.Color6;
            custom[7] = DeviceSettings.Color7;
            custom[8] = DeviceSettings.Color8;
            custom[9] = DeviceSettings.Color9;
            custom[10] = DeviceSettings.Color10;
            custom[11] = DeviceSettings.Color11;
            custom[12] = DeviceSettings.Color12;
            custom[13] = DeviceSettings.Color13;
            custom[14] = DeviceSettings.Color14;
            custom[15] = DeviceSettings.Color15;


        }


    }
}
