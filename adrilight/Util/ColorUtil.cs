using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Util
{
    class ColorUtil
    {
        //source https://stackoverflow.com/a/18840768
        public static System.Windows.Media.Color FromAhsb(byte alpha, double hue, double saturation, double brightness)
        {
            if (0 > alpha
                || 255 < alpha)
            {
                throw new ArgumentOutOfRangeException(
                    "alpha",
                    alpha,
                    "Value must be within a range of 0 - 255.");
            }

            if (0f > hue
                || 360f < hue)
            {
                throw new ArgumentOutOfRangeException(
                    "hue",
                    hue,
                    "Value must be within a range of 0 - 360.");
            }

            if (0f > saturation
                || 1f < saturation)
            {
                throw new ArgumentOutOfRangeException(
                    "saturation",
                    saturation,
                    "Value must be within a range of 0 - 1.");
            }

            if (0f > brightness
                || 1f < brightness)
            {
                throw new ArgumentOutOfRangeException(
                    "brightness",
                    brightness,
                    "Value must be within a range of 0 - 1.");
            }

            if (0 == saturation)
            {
                return System.Windows.Media.Color.FromArgb(
                                    alpha,
                                    Convert.ToByte(brightness * 255),
                                    Convert.ToByte(brightness * 255),
                                    Convert.ToByte(brightness * 255));
            }

            double fMax, fMid, fMin;
            int iSextant;
            byte iMax, iMid, iMin;

            if (0.5 < brightness)
            {
                fMax = brightness - (brightness * saturation) + saturation;
                fMin = brightness + (brightness * saturation) - saturation;
            }
            else
            {
                fMax = brightness + (brightness * saturation);
                fMin = brightness - (brightness * saturation);
            }

            iSextant = (int)Math.Floor(hue / 60f);
            if (300f <= hue)
            {
                hue -= 360f;
            }

            hue /= 60f;
            hue -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = (hue * (fMax - fMin)) + fMin;
            }
            else
            {
                fMid = fMin - (hue * (fMax - fMin));
            }

            iMax = Convert.ToByte(fMax * 255);
            iMid = Convert.ToByte(fMid * 255);
            iMin = Convert.ToByte(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return System.Windows.Media.Color.FromArgb(alpha, iMid, iMax, iMin);
                case 2:
                    return System.Windows.Media.Color.FromArgb(alpha, iMin, iMax, iMid);
                case 3:
                    return System.Windows.Media.Color.FromArgb(alpha, iMin, iMid, iMax);
                case 4:
                    return System.Windows.Media.Color.FromArgb(alpha, iMid, iMin, iMax);
                case 5:
                    return System.Windows.Media.Color.FromArgb(alpha, iMax, iMin, iMid);
                default:
                    return System.Windows.Media.Color.FromArgb(alpha, iMax, iMid, iMin);
            }
        }
    }
}
