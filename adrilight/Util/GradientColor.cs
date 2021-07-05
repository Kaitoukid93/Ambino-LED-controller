using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using OpenRGB.NET;

namespace adrilight.Util
{
    class GradientColor
    {
        public static Color[] paletteOutput = new Color[256];
        public static double gradientIndex = 0.0;

        public static void GradientCreator(int numLED, Canvas playground, bool isMoving, double hueStart, double hueStop, double currentBrightness)//gradient color creator
        {

            var newcolor = GetHueGradient(numLED, hueStart,hueStop, 1.0, 1.0);
            int counter = 0;


            //now fill new color to palette output to display and send out serial
            foreach ( var color in newcolor)
                {

                  paletteOutput[counter++] = Color.FromRgb(color.R, color.G, color.B);

                }

        }



        public static IEnumerable<OpenRGB.NET.Models.Color> GetHueGradient(int amount, double hueStart = 0, double hueStop = 1.0,
                                                                double saturation = 1.0, double value = 1.0) =>
            Enumerable.Range(0, amount)
                      .Select(i => OpenRGB.NET.Models.Color.FromHsv(hueStart + (360.0d * (hueStart-hueStop) / amount * i), saturation, value));

    }
}
