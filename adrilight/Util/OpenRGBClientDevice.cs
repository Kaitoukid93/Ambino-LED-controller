using Castle.Core.Logging;
using NLog;
using OpenRGB.NET;
using OpenRGB.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Util
{
    internal class OpenRGBClientDevice : IOpenRGBClientDevice
    {
        private NLog.ILogger _log = LogManager.GetCurrentClassLogger();
        public OpenRGBClientDevice()
        {
            //GeneralSettings = generalSettings ?? throw new ArgumentException(nameof(generalSettings));
            //DeviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
           
           // DeviceSettings.PropertyChanged += UserSettings_PropertyChanged;
            RefreshOpenRGBDeviceState();

            _log.Info($"SerialStream created.");


        }

        private void RefreshOpenRGBDeviceState()
        {
            try
            {

            
            using var client = new OpenRGBClient(name: "My OpenRGB Client", autoconnect: true, timeout: 1000);

            var deviceCount = client.GetControllerCount();
            var devices = client.GetAllControllerData();
            DeviceList = devices;
            

            for (int i = 0; i < devices.Length; i++)
            {
                var leds = Enumerable.Range(0, devices[i].Colors.Length)
                    .Select(_ => new Color(255, 255, 255))
                    .ToArray();
                client.UpdateLeds(i, leds);
            }
            foreach (var device in devices)
            {
                _log.Info($"Device found : " + device.Name.ToString());
            }
            }
            catch(TimeoutException)
            {
                HandyControl.Controls.MessageBox.Show("OpenRGB server Không khả dụng, hãy start server trong app OpenRGB (SDK Server)");
            }
        }

        private Device[] _deviceList;
        public Device[] DeviceList 
        {
            get { return _deviceList; }
            set
            {
                _deviceList = value;
            }

        }
        //private IDeviceSettings DeviceSettings { get; }
        //private IGeneralSettings GeneralSettings { get; }
    }
}
