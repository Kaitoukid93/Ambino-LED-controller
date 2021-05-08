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

/// <summary>
/// this class create a brightness map based on the volume level or sound spectrum get from bass
/// get input spectrum as fft and output the array of map
/// </summary>



namespace adrilight.Util
{
    class Music
    {
        public static byte[] brightnessMap=new byte[256];
        public static int height = 0;
        public static byte maxbrightness = 255;
        public static byte currentBrightness=255;
        

        public static void SpectrumCreator(byte[] fft, int sensitivity, double level, int musicMode, int numFreq)
        {

            int counter = 0;
            int factor = numFreq / fft.Length;
            //this function take the input as frequency and output the color but the brightness change as the frequency band's value
           if(musicMode==0)//pulse mode, take current level and blackout the rest of the strip
            {
                

                double percent = level / 16384;
                height = (int)((height*4 +numFreq * percent*4+7)/8);
                //byte step = 0;
                //if (height>0)
                //{
                //     step = (byte)(255 / height);
                //}
                
                foreach (var brightness in brightnessMap.Take(height))
                {
                    
                    // Console.WriteLine("color R " + color.R + " color G " + color.G + " color B " + color.B);

                    brightnessMap[counter++] = maxbrightness;
                   // currentBrightness -= step;

                }


                for (int i = height; i < numFreq; i++)
                {

                    brightnessMap[counter++] =0;

                }
            }
           else if(musicMode==1)
            {

            }
            else if (musicMode == 2)
            {

            }
            else if (musicMode == 3)
            {

            }
            else if (musicMode == 4)
            {

            }
            else if (musicMode == 5)
            {

            }
            

           

        }


    }
}
