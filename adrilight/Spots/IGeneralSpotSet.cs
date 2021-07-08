﻿using System.Drawing;

namespace adrilight
{
    public interface IGeneralSpotSet
    {
        int ExpectedScreenWidth { get; }
        int ExpectedScreenHeight { get; }

        IGeneralSpot[] Spots { get; set; }
        object Lock { get; }
        int CountLeds(int spotsX, int spotsY);
  
        void IndicateMissingValues();
    }
}