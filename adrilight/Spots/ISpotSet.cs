﻿using System.Drawing;

namespace adrilight
{
    public interface ISpotSet
    {
        int ExpectedScreenWidth { get; }
        int ExpectedScreenHeight { get; }

        ISpot[] Spots { get; set; }
        ISpot[] Spots2 { get; set; }
        object Lock { get; }

        int CountLeds(int spotsX, int spotsY);
        
        void IndicateMissingValues();
    }
}