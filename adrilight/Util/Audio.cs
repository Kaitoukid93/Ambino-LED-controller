using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace adrilight.Util
{
    class Audio
    {
        
        public static byte[] spectrumdata = new byte[16];
        public static int _lastlevel;
        public static int _hanctr;
        public static int volume;
        private static double _huePos = 0;//rainbow color only cuz it's using Hue to shift
        public static Color[] paletteOutput = new Color[256];


        public static void MusicCreator(int musicMode, int paletteSource, Canvas playground, int numLED, float[] _fft, double effectSpeed)
        {

            int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
            if (ret < -1) return;
            int x, y;
            int b0 = 0;

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x = 0; x < 16; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (16 - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 250 - 4);
                if (y > 255) y = 255;
                if (y < 10) y = 0;
                spectrumdata[x] = (byte)((spectrumdata[x] * 6 + y * 2 + 7) / 8); //Smoothing out the value (take 5/8 of old value and 3/8 of new value to make finnal value)
                if (spectrumdata[x] > 255)
                    spectrumdata[x] = 255;
                if (spectrumdata[x] < 15)
                    spectrumdata[x] = 0;

                //  Console.Write("{0, 3} ", y);

            }
          

            int level = BassWasapi.BASS_WASAPI_GetLevel(); // Get level (VU metter) for Old AMBINO Device (remove in the future)

            // _l.Value = Utils.LowWord32(level);
            //  _r.Value = Utils.HighWord32(level);
            if (level == _lastlevel && level != 0) _hanctr++;
            volume = Utils.LowWord32(level);
            _lastlevel = level;
            //volume = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            //if (_hanctr > 3)
            // {
            //     _hanctr = 0;

            //     Free();
            //     Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            //     _initialized = false;

            // }

            //get collection of color and brightness combine based on spectrum data or volume above
            if (paletteSource == 0)//rainbow color
            {
                var newcolor = OpenRGB.NET.Models.Color.GetHueRainbow(numLED, _huePos, 1, 1, 1);

                int counter = 0;

                Util.Music.SpectrumCreator(spectrumdata, 0, volume, musicMode, numLED);
                //run spectrum creator to get brightness map

                foreach (var color in newcolor)
                {

                    //turn each color to HSV
                    var hue = color.ToHsv().h;
                    var saturation = color.ToHsv().s;
                    var value = (Util.Music.brightnessMap[counter]) / 255.0;
                    // Apply V(Value) then turn back to RGB   
                    var returnColor = OpenRGB.NET.Models.Color.FromHsv(hue, saturation, value);

                    //now put this color to paletteOutput
                    paletteOutput[counter] = System.Windows.Media.Color.FromRgb(returnColor.R, returnColor.G, returnColor.B);

                    counter++;
                    //increase counter

                }
                //apply current brightness
               // paletteOutput = Brightness.applyBrightness(paletteOutput, currentBrightness);
                //finally show Rectangle and send to LED
                Rainbow.fillRectFromColor(paletteOutput, playground, numLED);
                //increase huePos to make color move 
                if (_huePos > 360)
                {
                    _huePos = 0;
                }
                else
                {
                    _huePos += effectSpeed;
                }



            }
        }
           
                
            }

        }
   

