using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using System.Linq;
using System.Windows.Controls;
using OpenRGB.NET;



namespace adrilight.Util
{
    class Brightness
    {
        
        //this class take final form of color collection and apply the general brightness before display to the screen or send out to serial port
        public static OpenRGB.NET.Models.Color applyBrightness (OpenRGB.NET.Models.Color inputColor,double brightness)
        {
            
 
                //now we can turn this color to HSV
                var hue = inputColor.ToHsv().h;
                var saturation = inputColor.ToHsv().s;
                var value = inputColor.ToHsv().v;
            //apply brightness value
            if (brightness > 1.0)
                brightness = 1.0;
            var percent = brightness / 1.0;
                var returnColor = OpenRGB.NET.Models.Color.FromHsv(hue, saturation, value*percent);
            return returnColor;
        }

    }
}
