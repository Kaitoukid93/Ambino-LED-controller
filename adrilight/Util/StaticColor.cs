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
        private static double peekBrightness = 0;
        public static Color[] paletteOutput = new Color[256];
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();


        public StaticColor(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));


            UserSettings.PropertyChanged += UserSettings_PropertyChanged;
            RefreshTransferState();
            _log.Info($"StaticColorCreated");

        }


        private void UserSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
               case nameof(UserSettings.TransferActive):
               case nameof(UserSettings.ScreenStatic):
               case nameof(UserSettings.screeneffectcounter):
                    RefreshTransferState();
                    break;

            }
        }
        private IUserSettings UserSettings { get; }
        private SettingsViewModel SettingsViewModel { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private void RefreshTransferState()
        {

            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.screeneffectcounter==2;
            if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the StaticColor");
                _cancellationTokenSource = new CancellationTokenSource();
                var thread = new Thread(() => StaticCreator(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "StaticColorCreator"
                };
                thread.Start();
            }
        }



        public void StaticCreator(CancellationToken token)//static color creator
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
                    Color currentStaticColor = UserSettings.ScreenStatic;

                    for (int i = 0; i < numLED; i++) //fill all LED with static color
                    {
                        paletteOutput[i] = currentStaticColor;
                    }
                    //apply current brightness

                    //if(isBreathing)
                    //{
                    for (int i = 0; i < 360; i++)
                    {

                        peekBrightness = (Math.Sin(i * 0.017) + 1) * 0.5;
                        paletteOutput = Brightness.applyBrightness(paletteOutput, peekBrightness);
                        Thread.Sleep(10);
                    }


                    //}
                    //else
                    //{
                    //    paletteOutput = Brightness.applyBrightness(paletteOutput, currentBrightness);
                    //}

                    //finnaly display created color to canvans

                  SettingsViewModel.SetPreviewRectangle(paletteOutput);
                    // Rainbow.fillRectFromColor(paletteOutput, playground, numLED); this moved to UI thread



                }
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

            }

        }
    }
}
