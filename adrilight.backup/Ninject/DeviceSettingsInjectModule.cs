using adrilight.Resources;
using adrilight.Settings;
using adrilight.Spots;
using adrilight.Util;
using adrilight.ViewModel;
using adrilight.ViewModel.Factories;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Ninject
{
    class DeviceSettingsInjectModule : NinjectModule
    {
        public override void Load()
        {
            var settingsManager = new UserSettingsManager();
            var generalSettings = settingsManager.LoadIfExists() ?? settingsManager.MigrateOrDefault();
            var alldevicesettings = settingsManager.LoadDeviceIfExists();
            Bind<IDesktopDuplicatorReader>().To<DesktopDuplicatorReader>().InSingletonScope().WithConstructorArgument("graphicAdapter",0).WithConstructorArgument("output",0);
            Bind<IDesktopDuplicatorReaderSecondary>().To<DesktopDuplicatorReaderSecondary>().InSingletonScope().WithConstructorArgument("graphicAdapter", 0).WithConstructorArgument("output", 1);
            Bind<IDesktopDuplicatorReaderThird>().To<DesktopDuplicatorReaderThird>().InSingletonScope().WithConstructorArgument("graphicAdapter", 0).WithConstructorArgument("output", 2);
            Bind<IGeneralSpotSet>().To<GeneralSpotSet>().InSingletonScope();
            Bind<IGeneralSettings>().ToConstant(generalSettings);
            Bind<IOpenRGBClientDevice>().To<OpenRGBClientDevice>().InSingletonScope();
            Bind<ISerialDeviceDetection>().To<SerialDeviceDetection>().InSingletonScope();
          //  Bind<IViewModelFactory<AllDeviceViewModel>>().To<AllDeviceViewModelFactory>().InSingletonScope();
            if (alldevicesettings!=null)
            {
                if (alldevicesettings.Count > 0)
                {
                    foreach (var devicesetting in alldevicesettings)
                    {
                        var devicename = devicesetting.DeviceID.ToString();
                        if (devicename == "151293")//non Ambino Device
                        {
                            var DeviceSerial = devicesetting.DeviceSerial;
                            Bind<IDeviceSettings>().ToConstant(devicesetting).Named(DeviceSerial);
                        }
                        else
                        {
                            Bind<IDeviceSettings>().ToConstant(devicesetting).Named(devicename);
                        }
                            
                      //  Bind<IContext>().To<WpfContext>().InTransientScope().Named(devicename);
                      //  Bind<IDeviceSpotSet>().To<DeviceSpotSet>().InSingletonScope().Named(devicename);
                      //  Bind<ISpotSetReader>().To<SpotSetReader>().InSingletonScope().Named(devicename);
                      //  Bind<ISerialStream>().To<SerialStream>().InSingletonScope().Named(devicename);
                      ////  Bind<LightingViewModel>().ToSelf().InSingletonScope().Named(devicename);
                      //  Bind<IStaticColor>().To<StaticColor>().InTransientScope().Named(devicename);
                      //  Bind<IRainbow>().To<Rainbow>().InTransientScope().Named(devicename);
                      //  Bind<IMusic>().To<Music>().InTransientScope().Named(devicename);
                      //  Bind<IAtmosphere>().To<Atmosphere>().InTransientScope().Named(devicename);


                    }
                }
            }
            else
            {
                // require user to add device then restart the app
            }
           
           
          
        }
    }
}
