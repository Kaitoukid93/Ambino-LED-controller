using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using Castle.Core.Logging;
using NLog;
using adrilight.ViewModel;
using System.Diagnostics;
using adrilight.Spots;

namespace adrilight.Util
{
    internal class StaticColor : IStaticColor
    {


        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        
        public StaticColor(IDeviceSettings deviceSettings, IDeviceSpotSet deviceSpotSet)
        {
            DeviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
            DeviceSpotSet = deviceSpotSet ?? throw new ArgumentNullException(nameof(deviceSpotSet));
            //SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            //Remove SettingsViewmodel from construction because now we pass SpotSet Dirrectly to MainViewViewModel
           DeviceSettings.PropertyChanged += PropertyChanged;
            RefreshColorState();
            _log.Info($"Static Color Created");

        }


        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(DeviceSettings.TransferActive):
                case nameof(DeviceSettings.StaticColor):
                case nameof(DeviceSettings.SelectedEffect):
                    RefreshColorState();
                    break;

            }
        }

        //DependencyInjection//
        private IDeviceSettings DeviceSettings { get; }
        private IDeviceSpotSet DeviceSpotSet { get; }


        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private void RefreshColorState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = DeviceSettings.TransferActive && DeviceSettings.SelectedEffect == 2;
            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the StaticColor");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }


            else if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the StaticColor");
                _cancellationTokenSource = new CancellationTokenSource();
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "StaticColorCreator"
                };
                thread.Start();
            }
        }

        

        public void Run(CancellationToken token)//static color creator
        {
            int point = 0;
            if (IsRunning) throw new Exception(" Static Color is already running!");

            IsRunning = true;

            _log.Debug("Started Static Color.");

            Color defaultColor = Color.FromRgb(127, 127, 0);


            try
            {
                // BitmapData bitmapData = new BitmapData();
                //  BitmapData bitmapData2 = new BitmapData();
                //  int colorcount = 0;


                while (!token.IsCancellationRequested)
                {
                    var numLED = (DeviceSettings.SpotsX - 1) * 2 + (DeviceSettings.SpotsY - 1) * 2;
                    Color currentStaticColor = DeviceSettings.StaticColor;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                    double peekBrightness = 0.0;
                    int breathingSpeed = DeviceSettings.BreathingSpeed;

                   
                    bool isBreathing = DeviceSettings.IsBreathing;
                    lock (DeviceSpotSet.Lock)
                    {
                        for (var i = 0; i < colorOutput.Count(); i++)
                        {
                            if (isBreathing)
                            {

                                Color colorPoint = Color.FromRgb(0, 0, 0);
                                colorPoint = GetColorByOffset(GradientStaticColor(currentStaticColor), point);

                                var newColor = new OpenRGB.NET.Models.Color(colorPoint.R, colorPoint.G, colorPoint.B);

                                colorOutput[i] = newColor;
                            }




                            else
                            {
                                peekBrightness = DeviceSettings.Brightness / 100.0;
                                colorOutput[i] = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(currentStaticColor.R, currentStaticColor.G, currentStaticColor.B), peekBrightness);
                            }
                        }

                        point += breathingSpeed;
                        if (point > 5000)
                        {
                            point = 0;
                        }

                        int counter = 0;
                        foreach (IDeviceSpot spot in DeviceSpotSet.Spots)
                        {

                            spot.SetColor(colorOutput[counter].R, colorOutput[counter].G, colorOutput[counter].B, true);
                            counter++;


                        }

                        Thread.Sleep(5);
                    }







                }
                //motion speed

            }








            catch (OperationCanceledException)
            {
                _log.Debug("OperationCanceledException catched. returning.");

                return;
            }
            catch (Exception ex)
            {
                _log.Debug(ex, "Exception catched.");
                //to be safe, we reset the serial port
                //if (currentStaticColor == null)
                //{
                //    for (int i = 0; i < numLED; i++) //fill all LED with default static color
                //    {
                //        paletteOutput[i] = defaultColor;
                //    }



                //allow the system some time to recover
                Thread.Sleep(500);
            }
            finally
            {


                _log.Debug("Stopped Static Color Creator.");
                IsRunning = false;
            }

        }

        public GradientStopCollection GradientStaticColor(Color staticColor)
        {
            Color startColor = Color.FromRgb(0, 0, 0);
            OpenRGB.NET.Models.Color staticColorHalf = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(staticColor.R, staticColor.G, staticColor.B), 0.5);
            Color staticColorMiddle = Color.FromRgb(staticColorHalf.R, staticColorHalf.G, staticColorHalf.B);
            OpenRGB.NET.Models.Color staticColorQuad = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(staticColor.R, staticColor.G, staticColor.B), 0.25);
            Color staticColorQuat = Color.FromRgb(staticColorQuad.R, staticColorQuad.G, staticColorQuad.B);

            GradientStopCollection gradientPalette = new GradientStopCollection(2);
            gradientPalette.Add(new GradientStop(staticColor, 0.000));
            gradientPalette.Add(new GradientStop(staticColorMiddle, 0.250));
            gradientPalette.Add(new GradientStop(staticColorQuat, 0.400));
            gradientPalette.Add(new GradientStop(startColor, 0.500));
            gradientPalette.Add(new GradientStop(staticColorQuat, 0.600));
            gradientPalette.Add(new GradientStop(staticColorMiddle, 0.750));
            gradientPalette.Add(new GradientStop(staticColor, 1.000));
            //gradientPalette.Add(new GradientStop(startColor, 0.600));
            //gradientPalette.Add(new GradientStop(staticColorMiddle, 0.800));
            //gradientPalette.Add(new GradientStop(staticColor, 1.000));

            return gradientPalette;
        }

        private static Color GetColorByOffset(GradientStopCollection collection, double position)
        {
            double offset = position / 5000.0;
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
    }
}