using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;


namespace adrilight.Util
{
    class Brightness
    {
        private static Color[] brightnessOutput = new Color[256];
        //this class take final form of color collection and apply the general brightness before display to the screen or send out to serial port
        public static Color[] applyBrightness (Color[] inputPalette,double brightness)
        {
            //first turn each color to OpenRGB color struct
            int counter = 0;
            var newcolor = new OpenRGB.NET.Models.Color[inputPalette.Count()];
            foreach(var color in inputPalette)
            {
                newcolor[counter++] = new OpenRGB.NET.Models.Color(color.R, color.G, color.B);
            }
            //now we can turn all color to HSV
            counter = 0;
            foreach (var color in newcolor)
            {

                //turn each color to HSV
                var hue = color.ToHsv().h;
                var saturation = color.ToHsv().s;
               
                var returnColor = OpenRGB.NET.Models.Color.FromHsv(hue, saturation, brightness);

                //now put this color to paletteOutput
                
                brightnessOutput[counter] = System.Windows.Media.Color.FromRgb(returnColor.R, returnColor.G, returnColor.B);

                counter++;
                //increase counter

            }
            return brightnessOutput;
        }

    }
}
