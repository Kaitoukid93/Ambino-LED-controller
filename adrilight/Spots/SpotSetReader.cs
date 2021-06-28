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
        public SpotSetReader(IDeviceSettings userSettings, IGeneralSpotSet generalSpotSet, IDeviceSpotSet deviceSpotSet)
        {
           
            DeviceSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = generalSpotSet ?? throw new ArgumentNullException(nameof(generalSpotSet));
            DeviceSpotSet = deviceSpotSet ?? throw new ArgumentNullException(nameof(deviceSpotSet));

            DeviceSettings.PropertyChanged += PropertyChanged;
            RefreshReadingState();

            _log.Info($"SpotSetReader created for device Named.");
        }
        //SpotSetReader take color from General SpotSet then only select what it needs depends on DeviceSettings
        private IDeviceSettings DeviceSettings { get; }
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
                    lock (DeviceSpotSet.Lock)
                    {
                        for (var i = 0; i < DeviceSpotSet.Spots.Count(); i++)//topology
                        {
                            DeviceSpotSet.Spots[i].SetColor(SpotSet.Spots[i].Red, SpotSet.Spots[i].Green, SpotSet.Spots[i].Blue, true); // do a 1-1 cast
                                                                                                                                        //in ther future, each position selected will cast different position
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

        // set color for device spotset reading from General spotset




    }
}
