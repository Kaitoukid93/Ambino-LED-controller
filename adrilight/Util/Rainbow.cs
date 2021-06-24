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
    internal class Rainbow : IRainbow, IDisposable
    {
        // public static Color[] small = new Color[30];
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();
        private Thread _workerThread;
        public Rainbow(IDeviceSettings device, ISpotSet spotSet, LightingViewModel viewViewModel, SettingInfoDTO setting)
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

        private IDeviceSettings deviceInfo { get; }
        private LightingViewModel SettingsViewModel { get; }
        private SettingInfoDTO settingInfo { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(settingInfo.TransferActive):
                case nameof(deviceInfo.SelectedEffect):
                case nameof(deviceInfo.Brightness):
                    RefreshColorState();
                    break;
            }
        }
        private void RefreshColorState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = settingInfo.TransferActive && deviceInfo.SelectedEffect == 1;
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
                _workerThread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "RainbowColorCreator"
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
            if (IsRunning) throw new Exception(" Rainbow Color is already running!");

            IsRunning = true;

            _log.Debug("Started Rainbow Color.");

            try
            {

                while (!token.IsCancellationRequested)
                {
                    double brightness = deviceInfo.Brightness / 100d;
                    int paletteSource = deviceInfo.SelectedPalette;
                    var numLED = (deviceInfo.SpotsX - 1) * 2 + (deviceInfo.SpotsY - 1) * 2;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];

                

                    bool isPreviewRunning = (deviceInfo.SelectedEffect == 1); 
                    if (isPreviewRunning)
                    {
                       // SettingsViewModel.SetPreviewImage(backgroundimage);
                    }


                    OpenRGB.NET.Models.Color[] outputColor = new OpenRGB.NET.Models.Color[numLED];
                    int counter = 0;               
                    lock (SpotSet.Lock)
                    {
                        if (paletteSource == 0)
                        {
                            var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePosIndex, 1, 1, 1);

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




        private static  OpenRGB.NET.Models.Color[] PaletteCreator (int numLED, double startIndex, OpenRGB.NET.Models.Color[] colorCollection)
        {
            //numLED: number of LED to create on the view
            //startIndex: index to start drawing palette
            //playground: canvans to draw into
            //isMusic: sound reaction boolean
            //musicValue: value of current sound level
            //numColor: number of color to create from colorCollection
            //colorCollection: actually the palette
            //expand color from Collection
            int factor = numLED / colorCollection.Count(); //scaling factor
            int colorcount = (int)startIndex;
        var colorOutput = new OpenRGB.NET.Models.Color[numLED];
        //todo: expand current palette to 256 color for smooth effect


        for (int i = 0; i < colorCollection.Count(); i++)
            {
                for (int j = 0; j < factor; j++)
                {

                    if (colorcount > numLED)
                    {
                        colorcount = 0;
                    }
                    colorOutput[colorcount++] = colorCollection[i];
                }
            }

        //finally
        //  fillRectFromColor(paletteOutput, playground, numLED);
        return colorOutput;

        }

      














       

        public static Color[] party = {
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#5500AB"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#84007C"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#B5004B"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#E5001B"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#E81700"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#B84700"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#AB7700"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#ABAB00"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#AB5500"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#DD2200"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#F2000E"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#C2003E"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#8F0071"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#5F00A1"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#2F00D0"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#0007F9")

 

    };
        public static Color[] cloud = {
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#845EC2"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#D65DB1"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF6F91"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF9671"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFC75F"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#F9F871")

    };
    }
}




















