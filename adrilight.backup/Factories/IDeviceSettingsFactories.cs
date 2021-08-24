using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Factories
{
   public interface IDeviceSettingsFactories
    {
        IDeviceSettings CreateDeviceSettings(string deviceName);
    }
}
