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

namespace adrilight
{
    internal class Atmosphere : IAtmosphere
    {
        // public static Color[] small = new Color[30];
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public Atmosphere(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel)//, MainViewViewModel mainViewViewModel)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
           // MainViewViewModel = mainViewViewModel ?? throw new ArgumentNullException(nameof(mainViewViewModel));
            UserSettings.PropertyChanged += PropertyChanged;
            SettingsViewModel.PropertyChanged += PropertyChanged;
            //MainViewViewModel.PropertyChanged += MainViewViewModel_PropertyChanged;
            RefreshColorState();
            _log.Info($"Atmosphere Color Created");

        }

        private IUserSettings UserSettings { get; }
        private SettingsViewModel SettingsViewModel { get; }
        //private MainViewViewModel MainViewViewModel { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        //private void MainViewViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    switch (e.PropertyName)
        //    {
        //        case "LightingMode":

        //            //setting here
        //            UserSettings.Brightness = (byte)MainViewViewModel.CurrentDevice.Brightness;
        //            switch (MainViewViewModel.CurrentDevice.LightingMode)
        //            {
        //                case "Sáng theo màn hình":
        //                    UserSettings.SelectedEffect = 0; break;
        //                case "Sáng theo dải màu":
        //                    UserSettings.SelectedEffect = 1; break;
        //                case "Sáng màu tĩnh":
        //                    UserSettings.SelectedEffect = 2; break;
        //                case "Sáng theo nhạc":
        //                    UserSettings.SelectedEffect = 3; break;
        //                case "Atmosphere":
        //                    UserSettings.SelectedEffect = 4; break;
        //            }
        //            RefreshColorState();
        //            break;
        //    }
        //}
        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserSettings.TransferActive):
                case nameof(UserSettings.SelectedEffect):
                case nameof(UserSettings.Brightness):
                case nameof(SettingsViewModel.IsSettingsWindowOpen):
                    RefreshColorState();
                    break;
            }
        }
        private void RefreshColorState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.SelectedEffect== 4;
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
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "AtmosphereColorCreator"
                };
                thread.Start();
            }
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
                    double brightness = UserSettings.Brightness / 100d;
                    int paletteSource = UserSettings.SelectedPalette;
                    var numLED = (UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];

                

                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect==4); 
                    if (isPreviewRunning)
                    {
                       // SettingsViewModel.SetPreviewImage(backgroundimage);
                    }


                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;
                    int hueStart = UserSettings.AtmosphereStart;
                    int hueStop = UserSettings.AtmosphereStop;
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
                        if (isPreviewRunning)
                        {
                            //copy all color data to the preview
                            var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                            SettingsViewModel.PreviewSpots = SpotSet.Spots;
                        }
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




















