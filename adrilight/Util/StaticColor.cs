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
using BO;

namespace adrilight.Util
{
    public class StaticColor : IStaticColor, IDisposable
    {
        
        
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        private double point = 0;
        public StaticColor(DeviceInfoDTO device, ISpotSet spotSet, LightingViewModel viewViewModel, SettingInfoDTO setting)
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



        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
               case nameof(settingInfo.TransferActive):
               case nameof(deviceInfo.StaticColor):
               case nameof(deviceInfo.LightingMode):
                    RefreshColorState();
                    break;

            }
        }

        private void SettingInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }
        private Thread _workerThread;
        private DeviceInfoDTO deviceInfo { get; }
        private LightingViewModel SettingsViewModel { get; }
        private SettingInfoDTO settingInfo { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private void RefreshColorState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = settingInfo.TransferActive && deviceInfo.LightingMode == "Sáng màu tĩnh";
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
                _workerThread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "StaticColorCreator"
                };
                _workerThread.Start();
            }
        }

        private ISpotSet SpotSet { get; }

        public void Run(CancellationToken token)//static color creator
        {
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
                    var numLED = (deviceInfo.SpotsX - 1) * 2 + (deviceInfo.SpotsY - 1) * 2;
                    Color currentStaticColor = deviceInfo.StaticColor;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                    double peekBrightness = 0.0;

                    bool isPreviewRunning = deviceInfo.LightingMode == "Sáng màu tĩnh";
                    bool isBreathing = deviceInfo.IsBreathing;
                    lock (SpotSet.Lock)
                    {

                        if(isBreathing)
                        {
                          if(point<500)
                            {
                                //peekBrightness = 1.0 - Math.Abs((2.0 * (point / 500)) - 1.0);
                                peekBrightness = Math.Exp(-(Math.Pow(((point / 500) - 0.5) /0.14, 2.0)) / 2.0);
                                point++;
                            }
                          else
                            {
                                point = 0;
                            }
                        

                          
                        }
                        else
                        {
                            peekBrightness = deviceInfo.Brightness / 100.0;
                        }

                                   int counter = 0;
                        foreach (ISpot spot in SpotSet.Spots)
                        {
                            colorOutput[counter] = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(currentStaticColor.R, currentStaticColor.G, currentStaticColor.B), peekBrightness);
                            spot.SetColor(colorOutput[counter].R, colorOutput[counter].G, colorOutput[counter].B, true);
                            counter++;


                        }

                            if (isPreviewRunning)
                            {
                                //copy all color data to the preview
                                var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                                SettingsViewModel.PreviewSpots = SpotSet.Spots;
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
