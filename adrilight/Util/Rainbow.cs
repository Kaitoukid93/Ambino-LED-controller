using OpenRGB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using adrilight.Util;

namespace adrilight
{
    public static class Rainbow
    {
        // public static Color[] small = new Color[30];
        public static double _huePosIndex = 0;//index for rainbow mode only
        public static double _palettePosIndex = 0;//index for other custom palette
        public static Color[] paletteOutput = new Color[256];

        public static void RainbowCreator(int numLED, Canvas playground, int numColor, int paletteSource, double effectSpeed, double currentBrightness)

        {
            if (paletteSource == 0)
            {
                var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numColor, _huePosIndex, 1, 1, 1);

                int counter = 0;



                foreach (var color in newcolor.Take(numLED))
                {

                    // Console.WriteLine("color R " + color.R + " color G " + color.G + " color B " + color.B);
                    paletteOutput[counter++] = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                }

                if (_huePosIndex > 360)
                {
                    _huePosIndex = 0;
                }
                else
                {
                    _huePosIndex += effectSpeed;
                }

            }
            else 
            {
                if(paletteSource==1)//party color palette
                {
                    PaletteCreator(32, _palettePosIndex, playground, Rainbow.party);
                }
                if (paletteSource == 2)//cloud color palette
                {
                    PaletteCreator(32, _palettePosIndex, playground, Rainbow.cloud);
                }


                if (_palettePosIndex > 32)
                {
                    _palettePosIndex = 0;
                }
                else
                {
                    _palettePosIndex += effectSpeed;
                }
            }




            //apply current brightness
            paletteOutput = Brightness.applyBrightness(paletteOutput, currentBrightness);
            //finally

            fillRectFromColor(paletteOutput, playground, numLED);

        }




        public static void PaletteCreator(int numLED, double startIndex, Canvas playground, Color[] colorCollection)
        {
            //numLED: number of LED to create on the view
            //startIndex: index to start drawing palette
            //playground: canvans to draw into
            //isMusic: sound reaction boolean
            //musicValue: value of current sound level
            //numColor: number of color to create from colorCollection
            //colorCollection: actually the palette
            //expand color from Collection
            int factor = numLED / colorCollection.Count(); //scaling factor
            int colorcount = (int)startIndex;

            //todo: expand current palette to 256 color for smooth effect


            for (int i = 0; i < colorCollection.Count(); i++)
            {
                for (int j = 0; j < factor; j++)
                {

                    if (colorcount > numLED)
                    {
                        colorcount = 0;
                    }
                    paletteOutput[colorcount++] = colorCollection[i];
                }
            }

            //finally
          //  fillRectFromColor(paletteOutput, playground, numLED);

        }

      













        /// <summary>
        /// Generates a smooth rainbow with the given amount of colors.
        /// Uses HSV conversion to get a hue-based rainbow.
        /// </summary>
        /// <param name="amount">How many colors to generate.</param>
        /// <param name="hueStart">The hue of the first color</param>
        /// <param name="hueStop">stop hue.</param>
        /// <param name="saturation">The HSV saturation of the colors</param>
        /// <param name="value">The HSV value of the colors.</param>
        /// <returns>An collection of Colors in a rainbow pattern.</returns>


        public static void fillRectFromColor(Color[] colorarray, Canvas playground, int numLED)
        {
            playground.Children.Clear();
            for (int i = 0; i < numLED; i++)
            {
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle {
                    Width = playground.ActualWidth / numLED,
                    Height = 20,
                };

                System.Windows.Media.Brush brush = new SolidColorBrush(colorarray[i]);
                rectangle.Fill = brush;
                playground.Children.Add(rectangle);

                Canvas.SetLeft(rectangle, i * (playground.ActualWidth / numLED + 3));
                Canvas.SetTop(rectangle, 0);
            }
        }

        public static Color[] party = {
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#5500AB"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#84007C"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#B5004B"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#E5001B"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#E81700"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#B84700"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#AB7700"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#ABAB00"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#AB5500"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#DD2200"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#F2000E"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#C2003E"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#8F0071"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#5F00A1"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#2F00D0"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#0007F9")

 

    };
        public static Color[] cloud = {
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#845EC2"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#D65DB1"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF6F91"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF9671"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFC75F"),
            (Color)System.Windows.Media.ColorConverter.ConvertFromString("#F9F871")

    };
    }
}




















