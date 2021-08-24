using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   public class SettingInfo
    {
        public bool autoaddnewdevice { get; set; }
        public bool autoconnectnewdevice { get; set; }
        public string defaultname { get; set; }
        public bool displayconnectionstatus { get; set; }
        public bool displaylightingstatus { get; set; }
        public bool autodeleteconfigwhendisconected { get; set; }
        public bool isdarkmode { get; set; }
        public bool autostartwithwindows { get; set; }
        public bool pushnotificationwhennewdeviceconnected { get; set; }
        public bool pushnotificationwhennewdevicedisconnected { get; set; }
        public bool startminimum { get; set; }
        public string primarycolor { get; set; }
    }
}
