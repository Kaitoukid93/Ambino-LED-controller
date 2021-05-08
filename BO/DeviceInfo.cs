using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   public class DeviceInfo
    {
        public int deviceid { get; set; }
        public string devicename { get; set; }
        public string devicetype { get; set; }
        public int devicesize { get; set; }
        public string deviceport { get; set; }
        public bool isConnected { get; set; }
        public string lightingmode { get; set; }
        public int brightness { get; set; }
        public int colortemp { get; set; }
        public int capturesource { get; set; }
        public int rainbowmode { get; set; }
        public int rainbowspeed { get; set; }
        public int musicmode { get; set; }
        public int musicsens { get; set; }
        public int musicsource { get; set; }
        public int gifmode { get; set; }
        public string gifsource { get; set; }
        public bool isbreathing { get; set; }
        public string staticcolor { get; set; }
        public string fadestart { get; set; }
        public string fadeend { get; set; }
        public bool isshowondashboard { get; set; }
        public int lednumber { get; set; }
    }
}
