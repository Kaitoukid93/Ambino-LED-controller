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

namespace adrilight
{
    public static class Rainbow
    {
       // public static Color[] small = new Color[30];
        public static Color[] paletteOutput = new Color[256];

        public static void RainbowCreator(int numLED, double _pixframeIndex, Canvas playground, bool isMusic,double musicValue, int numColor, byte[] fft,int musicmode)

        {

            

            var newcolor =OpenRGB.NET.Models.Color.GetHueRainbow(numColor, _pixframeIndex, 1, 1, 1);

            int counter = 0;
            if (isMusic) // music reaction mode
            {
                Util.Music.SpectrumCreator(fft, 0, musicValue, musicmode, numLED);
                //run spectrum creator to get brightness map
               
                    foreach (var color in newcolor)
                    {

                        //turn each color to HSV
                        var hue= color.ToHsv().h;
                        var saturation = color.ToHsv().s;
                        var value = color.ToHsv().v;

                        // Apply V(Value) then turn back to RGB   
                        var returnColor = OpenRGB.NET.Models.Color.FromHsv(hue, saturation, Util.Music.brightnessMap[counter]/255);

                        //now put this color to paletteOutput
                        paletteOutput[counter] = System.Windows.Media.Color.FromRgb(returnColor.R,returnColor.G,returnColor.B);

                        counter++;
                        //increase counter

                    }
                      

            }

            else // normal chasing mode
            {
                foreach (var color in newcolor.Take(numLED))
                {

                    // Console.WriteLine("color R " + color.R + " color G " + color.G + " color B " + color.B);
                    paletteOutput[counter++] = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                }
            }

            //finally

            fillRectFromColor(paletteOutput, playground, numLED);

        }




        public static void PaletteCreator(int numLED, double startIndex, Canvas playground, bool isMusic, double musicValue,Color[] colorCollection)
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
            int colorcount =(int) startIndex;

            
            for (int i=0;i<colorCollection.Count();i++)
            {
                for (int j=0;j<factor;j++)
                {
                    
                    if(colorcount>numLED)
                    {
                        colorcount = 0;
                    }
                    paletteOutput[colorcount++] = colorCollection[i];
                }
            }

            //finally
            fillRectFromColor(paletteOutput, playground, numLED);

        }

        public static void GradientCreator(int numLED, double startIndex, Canvas playground, bool isMusic, double musicValue, double[] hueCollection)
        {
            int colorcount = (int)startIndex;
            var colorBlend= GetHueBlend(256/numLED, hueCollection[0], hueCollection[1], 1, 1);
            var colorBlend1 = GetHueBlend(256 / numLED, hueCollection[1], hueCollection[2], 1, 1);
            var colorBlend2 = GetHueBlend(256 / numLED, hueCollection[2], hueCollection[3], 1, 1);
            var colorBlend3 = GetHueBlend(256 / numLED, hueCollection[3], hueCollection[4], 1, 1);
            var colorBlend4 = GetHueBlend(256 / numLED, hueCollection[4], hueCollection[5], 1, 1);
            var colorBlend5 = GetHueBlend(256 / numLED, hueCollection[5], hueCollection[6], 1, 1);
            var colorBlend6 = GetHueBlend(256 / numLED, hueCollection[6], hueCollection[7], 1, 1);

           


            foreach (var color in colorBlend)
                {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);
               
            }
            foreach (var color in colorBlend1)
            {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);

            }
            foreach (var color in colorBlend2)
            {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);

            }
            foreach (var color in colorBlend3)
            {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);

            }
            foreach (var color in colorBlend4)
            {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);

            }
            foreach (var color in colorBlend5)
            {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);

            }
            foreach (var color in colorBlend6)
            {
                if (colorcount > 255)
                {
                    colorcount = 0;
                }
                paletteOutput[colorcount++] = Color.FromRgb(color.R, color.G, color.B);

            }
           
            //finally
            fillRectFromColor(paletteOutput, playground, numLED);
                
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
        public static IEnumerable<OpenRGB.NET.Models.Color> GetHueBlend(int amount, double hueStart = 0, double hueStop=0.3,
                                                              double saturation = 1.0, double value = 1.0) =>
          Enumerable.Range(0, amount)
                    .Select(i => OpenRGB.NET.Models.Color.FromHsv(hueStart + (360.0d * (hueStop-hueStart) / amount * i), saturation, value));

        public static void fillRectFromColor (Color[] colorarray, Canvas playground, int numLED)
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

    }
}




















