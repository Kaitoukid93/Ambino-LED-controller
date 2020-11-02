using System.Drawing;

namespace adrilight
{
    public interface ISpotSet
    {
        int ExpectedScreenWidth { get; }
        int ExpectedScreenHeight { get; }
        int ExpectedScreenWidth2 { get; }
        int ExpectedScreenHeight2 { get; }

        ISpot[] Spots { get; set; }
        ISpot[] Spots2 { get; set; }
        object Lock { get; }
        object Lock2 { get; }

        int CountLeds(int spotsX, int spotsY);
        
        void IndicateMissingValues();
    }
}