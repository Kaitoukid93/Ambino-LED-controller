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
using BO;

namespace adrilight
{
    internal class Atmosphere : IAtmosphere, IDisposable
    {
        // public static Color[] small = new Color[30];
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Atmosphere(IDeviceSettings device, ISpotSet spotSet, SettingInfoDTO setting)
        {
            deviceInfo = device ?? throw new ArgumentNullException(nameof(device));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
           // ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            settingInfo = setting ?? throw new ArgumentNullException(nameof(setting));
            deviceInfo.PropertyChanged += PropertyChanged;
            settingInfo.PropertyChanged += SettingInfo_PropertyChanged;
            RefreshColorState();
            _log.Info($"RainbowColor Created");

        }

        private void SettingInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
        private Thread _workerThread;
        private IDeviceSettings deviceInfo { get; }
       // private LightingViewModel ViewModel { get; }
        private SettingInfoDTO settingInfo { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
       
        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(deviceInfo.TransferActive):
                case nameof(deviceInfo.SelectedEffect):
                case nameof(deviceInfo.Brightness):
                case nameof(deviceInfo.AtmosphereStart):
                case nameof(deviceInfo.AtmosphereStop):
                    RefreshColorState();
                    break;
            }
        }
        private void RefreshColorState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = settingInfo.TransferActive && deviceInfo.SelectedEffect == 4;
            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the Atmosphere Color");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }
            else if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the Rainbow Color");
                _cancellationTokenSource = new CancellationTokenSource();
                _workerThread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "AtmosphereColorCreator"
                };
                _workerThread.Start();
            }
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

        private ISpotSet SpotSet { get; }
        public void Run (CancellationToken token)

        {
            if (IsRunning) throw new Exception(" Atmosphere Color is already running!");

            IsRunning = true;

            _log.Debug("Started Atmosphere Color.");

            try
            {

                while (!token.IsCancellationRequested)
                {
                    double brightness = deviceInfo.Brightness / 100d;
                    int paletteSource = deviceInfo.SelectedPalette;
                    var numLED = (deviceInfo.SpotsX - 1) * 2 + (deviceInfo.SpotsY - 1) * 2;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];

                

                    bool isPreviewRunning = (deviceInfo.SelectedEffect == 4); 
                    if (isPreviewRunning)
                    {
                       // SettingsViewModel.SetPreviewImage(backgroundimage);
                    }


                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;
                    int hueStart = deviceInfo.AtmosphereStart;
                    int hueStop = deviceInfo.AtmosphereStop;
                    lock (SpotSet.Lock)
                    {
                        
                            

                            var newcolor = GetHueGradient(numLED, hueStart, hueStop, 1.0, 1.0);
                           


                            //now fill new color to palette output to display and send out serial
                          

                            foreach (var color in newcolor)
                            {
                                outputColor[counter++] = Brightness.applyBrightness(color, brightness);

                            }
                            counter = 0;
                            foreach(ISpot spot in SpotSet.Spots)
                            {
                                spot.SetColor(outputColor[counter].R, outputColor[counter].G, outputColor[counter].B, true);
                                counter++;
                              
                            }

                           

                        }
                        //if (isPreviewRunning)
                        //{
                        //    //copy all color data to the preview
                        //    var needsNewArray = ViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                        //    ViewModel.PreviewSpots = SpotSet.Spots;
                        //}
                  //  MainViewViewModel.SpotSet = SpotSet;
                    }
                    Thread.Sleep(5); //motion speed

                
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


                _log.Debug("Stopped Atmosphere Color Creator.");
                IsRunning = false;
            }



               

               



            }






        public static IEnumerable<OpenRGB.NET.Models.Color> GetHueGradient(int amount, double hueStart = 0, double hueStop = 1.0,
                                                               double saturation = 1.0, double value = 1.0) =>
           Enumerable.Range(0, amount)
                     .Select(i => OpenRGB.NET.Models.Color.FromHsv(hueStart + ( (hueStart - hueStop) / amount * i), saturation, value));

















    }
}




















