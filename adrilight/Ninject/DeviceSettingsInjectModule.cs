using adrilight.Resources;
using adrilight.Util;
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
           // var settings = settingsManager.LoadIfExists() ?? settingsManager.MigrateOrDefault();
            var alldevicesettings = settingsManager.LoadDeviceIfExists();
            for (var i = 0; i < alldevicesettings.Count; i++)
            {
               var devicename = i.ToString();

                Bind<IDeviceSettings>().ToConstant(alldevicesettings.ElementAt(i)).WhenParentNamed(devicename).InSingletonScope().Named(devicename);
                Bind<IContext>().To<WpfContext>().InTransientScope().Named(devicename);
                Bind<ISpotSet>().To<SpotSet>().WhenParentNamed(devicename).InSingletonScope().Named(devicename); ;
                Bind<ISerialStream>().To<SerialStream>().InSingletonScope().Named(devicename);
                Bind<IDesktopDuplicatorReader>().To<DesktopDuplicatorReader>().InTransientScope().Named(devicename);
                Bind<IStaticColor>().To<StaticColor>().InTransientScope().Named(devicename);
                Bind<IRainbow>().To<Rainbow>().InTransientScope().Named(devicename);
                Bind<IMusic>().To<Music>().InTransientScope().Named(devicename);
                Bind<IAtmosphere>().To<Atmosphere>().InTransientScope().Named(devicename);


            }
        }
    }
}
