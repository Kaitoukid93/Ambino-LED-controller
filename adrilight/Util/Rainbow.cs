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
        public static Color[] small = new Color[30];
        public static Color[] paletteOutput = new Color[256];

        public static void RainbowCreator(int numLED, double _pixframeIndex, Canvas playground, bool isMusic,double musicValue, int numColor)

        {

            

            var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numColor, _pixframeIndex, 1, 1, 1);

            int counter = 0;
            if(isMusic)
            {
                double percent = musicValue / 255;
                int height = (int)(numLED * percent);
                foreach (var color in newcolor.Take(height))
                {

                    // Console.WriteLine("color R " + color.R + " color G " + color.G + " color B " + color.B);
                    small[counter++] = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);

                }


                for (int i = height; i < numLED; i++)
                {

                    small[counter++] = System.Windows.Media.Color.FromRgb(0, 0, 0);

                }
            }

            else
            {
                foreach (var color in newcolor.Take(numLED))
                {

                    // Console.WriteLine("color R " + color.R + " color G " + color.G + " color B " + color.B);
                    small[counter++] = System.Windows.Media.Color.FromRgb(color.R, color.G, color.B);
                }
            }

            //finally

            fillRectFromColor(small, playground, numLED);

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
            int factor = 256 / colorCollection.Count(); //scaling factor
            int colorcount =(int) startIndex;

            
            for (int i=0;i<colorCollection.Count();i++)
            {
                for (int j=0;j<factor;j++)
                {
                    
                    if(colorcount>255)
                    {
                        colorcount = 0;
                    }
                    paletteOutput[colorcount++] = colorCollection[i];
                }
            }

            //finally
            fillRectFromColor(paletteOutput, playground, numLED);



        }


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




















