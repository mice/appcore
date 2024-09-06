

using System;
using UnityEngine;
namespace Utils
{
    public static partial class UIUtils
    {
        public static Color FromRGBString(string s)
        {
            return FromRGBInt(Convert.ToUInt32(s, 16));
        }

        public static Color FromRGBInt(uint val)
        {
            return new Color((val >> 16 & 0xff) / 255f, (val >> 8 & 0xff) / 255f, (val & 0xff) / 255f, 1);
        }

        public static Color FromRGB(short r, short g, short b)
        {
            return new Color(r / 255f, g / 255f, b / 255f, 1);
        }

        public static Color FromRGB(short r, short g, short b, short a)
        {
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        
    }
}

