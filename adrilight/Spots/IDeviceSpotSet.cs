using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Spots
{
    public interface IDeviceSpotSet
    {
        

        IDeviceSpot[] Spots { get; set; }
        object Lock { get; }
       
        int CountLeds(int spotsX, int spotsY);
        int ID { get; set; }
        int ParrentLocation { get; set; }
        int OutputLocation { get; set; }
        
    }
}
