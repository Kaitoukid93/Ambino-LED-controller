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

namespace adrilight.Util
{
    internal class StaticColor : IStaticColor
    {
        
        
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        private double point = 0;
        public StaticColor(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            SettingsViewModel.PropertyChanged += PropertyChanged;
            UserSettings.PropertyChanged += PropertyChanged;
            RefreshColorState();
            _log.Info($"Static Color Created");

        }


        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
               case nameof(UserSettings.TransferActive):
               case nameof(UserSettings.StaticColor):
               case nameof(UserSettings.SelectedEffect):
                    RefreshColorState();
                    break;

            }
        }
        private IUserSettings UserSettings { get; }
        private SettingsViewModel SettingsViewModel { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private void RefreshColorState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.SelectedEffect == 2;
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
                    var numLED = (UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2;
                    Color currentStaticColor = UserSettings.StaticColor;
                    var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                    double peekBrightness = 0.0;

                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 2);
                    bool isBreathing = UserSettings.Breathing;
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
                            peekBrightness = UserSettings.Brightness / 100.0;
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
    }
}
