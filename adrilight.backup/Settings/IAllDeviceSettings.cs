using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Settings
{
    public interface IAllDeviceSettings<T> where T:IDeviceSettings 
    {
        IList<T> AllDevices { get; set; }
    }
}
