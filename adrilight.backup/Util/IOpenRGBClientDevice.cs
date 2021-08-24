using OpenRGB.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Util
{
   public interface IOpenRGBClientDevice
    {
        Device[] DeviceList { get; set; }
    }
}
