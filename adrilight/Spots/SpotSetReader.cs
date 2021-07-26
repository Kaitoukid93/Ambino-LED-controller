using adrilight.Util;
using Castle.Core.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace adrilight.Spots
{
    class SpotSetReader : ISpotSetReader
    {
        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();
        public SpotSetReader(IDeviceSettings deviceSettings, IGeneralSpotSet generalSpotSet, IDeviceSpotSet deviceSpotSet, IGeneralSettings generalSettings)
        {
           
            DeviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
            GeneralSettings = generalSettings ?? throw new ArgumentNullException(nameof(generalSettings));
            SpotSet = generalSpotSet ?? throw new ArgumentNullException(nameof(generalSpotSet));
            DeviceSpotSet = deviceSpotSet ?? throw new ArgumentNullException(nameof(deviceSpotSet));

            DeviceSettings.PropertyChanged += PropertyChanged;
            RefreshReadingState();

            _log.Info($"SpotSetReader created for device Named.");
        }
        //SpotSetReader take color from General SpotSet then only select what it needs depends on DeviceSettings
        private IDeviceSettings DeviceSettings { get; }
        private IGeneralSettings GeneralSettings { get; }
        private IGeneralSpotSet SpotSet { get; }
        private IDeviceSpotSet DeviceSpotSet { get; set; }
        public object Lock { get; } = new object();
        private CancellationTokenSource _cancellationTokenSource;
        public bool IsRunning { get; private set; } = false;

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {

                case nameof(DeviceSettings.IsConnected):
                case nameof(DeviceSettings.TransferActive):
                case nameof(DeviceSettings.SelectedDisplay):
                case nameof(DeviceSettings.SelectedAdapter):
                case nameof(DeviceSettings.SelectedEffect):
                
                    RefreshReadingState();
                    break;

                
                    
                    
            }
        }
        private void RefreshReadingState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = DeviceSettings.TransferActive && DeviceSettings.SelectedEffect == 0;
            //  var shouldBeRefreshing = NeededRefreshing;



            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the SpotSet Reading for device Named " + DeviceSettings.DeviceName);
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;

            }


            else if (!isRunning && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the SpotSet Reading for device Named " + DeviceSettings.DeviceName);
                _cancellationTokenSource = new CancellationTokenSource();
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "SpotSetReader"
                };
                thread.Start();


            }

        }

        public void Run(CancellationToken token)
        {
            if (IsRunning) throw new Exception(nameof(SpotSetReader) + " is already running!" + "for device Named" + DeviceSettings.DeviceName);

            IsRunning = true;
            
        
            _log.Debug("Started SpotSet Reader for device Named " + DeviceSettings.DeviceName);
            try
            {
                while (!token.IsCancellationRequested)
                {

                    var brightness = DeviceSettings.Brightness / 100d;
                    lock (DeviceSpotSet.Lock)
                    {
                       
      

                        if (DeviceSettings.SelectedDisplay == 0)
                        {
                            if (DeviceSettings.DeviceType == "ABRev1" || DeviceSettings.DeviceType == "ABRev2")
                            {
                                if (DeviceSpotSet.Spots.Count() > SpotSet.Spots.Count())
                                {
                                    for (var i = 0; i < SpotSet.Spots.Count(); i++)//topology
                                    {
                                        var RawColor = new OpenRGB.NET.Models.Color(SpotSet.Spots[i].Red, SpotSet.Spots[i].Green, SpotSet.Spots[i].Blue);
                                        var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                        DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast
                                                                                                                         //in ther future, each position selected will cast different position
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < DeviceSpotSet.Spots.Count(); i++)//topology
                                    {

                                        var RawColor = new OpenRGB.NET.Models.Color(SpotSet.Spots[i].Red, SpotSet.Spots[i].Green, SpotSet.Spots[i].Blue);
                                        var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                        DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast // do a 1-1 cast
                                                                                                                         //in ther future, each position selected will cast different position
                                    }
                                }
                            }
                            else if (DeviceSettings.DeviceType == "ABEDGE")
                            {
                                if (DeviceSpotSet.Spots.Count() > SpotSet.SpotsDesk.Count())
                                {
                                    for (var i = 0; i < SpotSet.SpotsDesk.Count(); i++)//topology
                                    {
                                        var RawColor = new OpenRGB.NET.Models.Color(SpotSet.SpotsDesk[i].Red, SpotSet.SpotsDesk[i].Green, SpotSet.SpotsDesk[i].Blue);
                                        var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                        DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast
                                                                                                                         //in ther future, each position selected will cast different position
                                    }
                                }
                                else
                                {
                                    for (var i = 0; i < DeviceSpotSet.Spots.Count(); i++)//topology
                                    {

                                        var RawColor = new OpenRGB.NET.Models.Color(SpotSet.SpotsDesk[i].Red, SpotSet.SpotsDesk[i].Green, SpotSet.SpotsDesk[i].Blue);
                                        var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                        DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast // do a 1-1 cast
                                                                                                                         //in ther future, each position selected will cast different position
                                    }
                                }

                            }

                        }
                        else if (DeviceSettings.SelectedDisplay==1)
                        {
                            if (DeviceSpotSet.Spots.Count() > SpotSet.Spots2.Count())
                            {
                                for (var i = 0; i < SpotSet.Spots2.Count(); i++)//topology
                                {

                                    var RawColor = new OpenRGB.NET.Models.Color(SpotSet.Spots2[i].Red, SpotSet.Spots2[i].Green, SpotSet.Spots2[i].Blue);
                                    var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                    DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast // do a 1-1 cast
                                                                                                                     //in ther future, each position selected will cast different position
                                }
                            }
                            else 
                            {
                                for (var i = 0; i < DeviceSpotSet.Spots.Count(); i++)//topology
                                {

                                    var RawColor = new OpenRGB.NET.Models.Color(SpotSet.Spots2[i].Red, SpotSet.Spots2[i].Green, SpotSet.Spots2[i].Blue);
                                    var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                    DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast // do a 1-1 cast
                                                                                                                     //in ther future, each position selected will cast different position
                                }
                            }
                        }

                        else if (DeviceSettings.SelectedDisplay == 2)
                        {
                            if (DeviceSpotSet.Spots.Count() > SpotSet.Spots3.Count())
                            {
                                for (var i = 0; i < SpotSet.Spots2.Count(); i++)//topology
                                {

                                    var RawColor = new OpenRGB.NET.Models.Color(SpotSet.Spots3[i].Red, SpotSet.Spots3[i].Green, SpotSet.Spots3[i].Blue);
                                    var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                    DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast // do a 1-1 cast
                                                                                                                     //in ther future, each position selected will cast different position
                                }
                            }
                            else
                            {
                                for (var i = 0; i < DeviceSpotSet.Spots.Count(); i++)//topology
                                {

                                    var RawColor = new OpenRGB.NET.Models.Color(SpotSet.Spots3[i].Red, SpotSet.Spots3[i].Green, SpotSet.Spots3[i].Blue);
                                    var FinalColor = Brightness.applyBrightness(RawColor, brightness);
                                    DeviceSpotSet.Spots[i].SetColor(FinalColor.R, FinalColor.G, FinalColor.B, true); // do a 1-1 cast // do a 1-1 cast
                                                                                                                     //in ther future, each position selected will cast different position
                                }
                            }
                        }




                    }
                    Thread.Sleep(8);
                }
            }

            finally
            {

                _log.Debug("Stopped SpotSet Reader for device Named " + DeviceSettings.DeviceName);
                IsRunning = false;
                GC.Collect();
            }
        }

        //private void ApplySmoothing(byte r, byte g, byte b, out byte semifinalR, out byte semifinalG, out byte semifinalB,
        //   byte lastColorR, byte lastColorG, byte lastColorB)
        //{
        //    int smoothingFactor = DeviceSettings.SmoothFactor;

        //    semifinalR = (byte)((r + smoothingFactor * lastColorR) / (smoothingFactor + 1));
        //    semifinalG = (byte)((g + smoothingFactor * lastColorG) / (smoothingFactor + 1));
        //    semifinalB = (byte)((b + smoothingFactor * lastColorB) / (smoothingFactor + 1));
        //}

        //private void ApplyWhiteBalance(byte r, byte g, byte b, out byte semifinalR, out byte semifinalG, out byte semifinalB)
        //{
        //    r *= (byte)(DeviceSettings.WhitebalanceRed / 100);
        //    g *= (byte)(DeviceSettings.WhitebalanceGreen / 100f);
        //    b *= (byte)(DeviceSettings.WhitebalanceBlue / 100f);
        //    semifinalR = r;
        //    semifinalG = g;
        //    semifinalB = b;
        //}
    }
}
