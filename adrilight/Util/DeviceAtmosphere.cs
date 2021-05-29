using adrilight.ViewModel;
using BO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace adrilight.Util
{
   public class DeviceAtmosphere : IAtmosphere
    {
        // public static Color[] small = new Color[30];
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette

        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        public DeviceAtmosphere(DeviceInfoDTO device, ISpotSet spotSet, LightingViewModel viewViewModel, SettingInfoDTO setting)
        {
            deviceInfo = device ?? throw new ArgumentNullException(nameof(device));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = viewViewModel ?? throw new ArgumentNullException(nameof(viewViewModel));
            settingInfo = setting ?? throw new ArgumentNullException(nameof(setting));
            deviceInfo.PropertyChanged += PropertyChanged;
            settingInfo.PropertyChanged += SettingInfo_PropertyChanged;
            RefreshColorState();
            _log.Info($"RainbowColor Created");

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
            //switch (e.PropertyName)
            //{
            //    case nameof(UserSettings.TransferActive):
            //    case nameof(UserSettings.SelectedEffect):
            //    case nameof(UserSettings.Brightness):
            //    case nameof(SettingsViewModel.IsSettingsWindowOpen):
            //        RefreshColorState();
            //        break;
            //}
            switch (e.PropertyName)
            {
                case nameof(deviceInfo.Brightness):
                case nameof(deviceInfo.LightingMode):
                    RefreshColorState();
                    break;
            }
        }
        private void RefreshColorState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = deviceInfo.LightingMode == "Atmosphere";//UserSettings.TransferActive && UserSettings.SelectedEffect == 4;
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
        public void Run(CancellationToken token)

        {
            if (IsRunning) throw new Exception(" Atmosphere Color is already running!");

            IsRunning = true;

            _log.Debug("Started Atmosphere Color.");

            try
            {

                while (!token.IsCancellationRequested)
                {
                    double brightness = deviceInfo.Brightness / 100d;
                    int paletteSource = deviceInfo.Palette;
                    var numLED = (deviceInfo.SpotsX - 1) * 2 + (deviceInfo.SpotsY - 1) * 2;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];



                    bool isPreviewRunning = deviceInfo.LightingMode == "Atmosphere";// (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 4);
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
                        foreach (ISpot spot in SpotSet.Spots)
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
                     .Select(i => OpenRGB.NET.Models.Color.FromHsv(hueStart + ((hueStart - hueStop) / amount * i), saturation, value));

















    }
}
