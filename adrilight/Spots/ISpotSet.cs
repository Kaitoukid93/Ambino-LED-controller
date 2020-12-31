using System.Drawing;

namespace adrilight
{
    public interface ISpotSet
    {
        int ExpectedScreenWidth { get; }
        int ExpectedScreenHeight { get; }
        int ExpectedScreenWidth2 { get; }
        int ExpectedScreenHeight2 { get; }
        int ExpectedScreenWidth3 { get; }
        int ExpectedScreenHeight3 { get; }

        ISpot[] Spots { get; set; }
        ISpot[] Spots2 { get; set; }
        ISpot[] Spots3 { get; set; }
        object Lock { get; }
        object Lock2 { get; }
        object Lock3 { get; }

        int CountLeds(int spotsX, int spotsY);
        
        void IndicateMissingValues();
    }
}