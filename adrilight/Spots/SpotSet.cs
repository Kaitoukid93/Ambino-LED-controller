﻿using adrilight.DesktopDuplication;
using adrilight.Extensions;
using NLog;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace adrilight
{
    internal sealed class SpotSet : ISpotSet
    {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        public SpotSet(IUserSettings userSettings)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));


            UserSettings.PropertyChanged += (_, e) => DecideRefresh(e.PropertyName);
            Refresh();
            if (Screen.AllScreens.Length == 2)
            {
                Refresh2();
            }
            

            _log.Info($"SpotSet created.");
        }

        private void DecideRefresh(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(UserSettings.BorderDistanceX):
                case nameof(UserSettings.BorderDistanceY):
                case nameof(UserSettings.MirrorX):
                case nameof(UserSettings.MirrorY):
                case nameof(UserSettings.OffsetLed):
                case nameof(UserSettings.OffsetX):
                case nameof(UserSettings.OffsetY):
                case nameof(UserSettings.SpotHeight):
                case nameof(UserSettings.SpotsX):
                case nameof(UserSettings.SpotsY):
                case nameof(UserSettings.SpotsX2):
                case nameof(UserSettings.SpotsY2):
                case nameof(UserSettings.SpotWidth):
                    Refresh();
                    if (Screen.AllScreens.Length == 2)
                    {
                        Refresh2();
                    }
                    
                    break;
            }
        }

        public ISpot[] Spots { get; set; }
        public ISpot[] Spots2 { get; set; }

        public object Lock { get; } = new object();
        public object Lock2 { get; } = new object();

        /// <summary>
        /// returns the number of leds
        /// </summary>
        public int CountLeds(int spotsX, int spotsY)
        {
            if (spotsX <= 1 || spotsY <= 1)
            {
                //special case because it is not really a rectangle of lights but a single light or a line of lights
                return spotsX * spotsY;
            }

            //normal case
            return 2 * spotsX + 2 * spotsY - 4;
        }

        static public Screen GetSecondaryScreen()
        {
            if (Screen.AllScreens.Length == 1)
            {
                return null;
            }

            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary == false)
                {
                    return screen;
                }
            }

            return null;
        }
        public int ExpectedScreenWidth => Screen.PrimaryScreen.Bounds.Width / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenHeight => Screen.PrimaryScreen.Bounds.Height / DesktopDuplicator.ScalingFactor;

        readonly Screen screen = GetSecondaryScreen();
        public int ExpectedScreenWidth2 => screen.Bounds.Width / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenHeight2 => screen.Bounds.Height / DesktopDuplicator.ScalingFactor;
        private IUserSettings UserSettings { get; }


        private void Refresh()
        {
            lock (Lock)
            {
                Spots = BuildSpots(ExpectedScreenWidth, ExpectedScreenHeight, UserSettings);
            }
        }
        private void Refresh2()
        {
            lock (Lock2)
            {
                Spots2 = BuildSpots2(ExpectedScreenWidth2, ExpectedScreenHeight2, UserSettings);
            }
        }

        internal ISpot[] BuildSpots(int screenWidth, int screenHeight, IUserSettings userSettings)
        {
            var spotsX = userSettings.SpotsX;
            var spotsY = userSettings.SpotsY;
            ISpot[] spots = new Spot[CountLeds(spotsX, spotsY)];


            var scalingFactor = DesktopDuplicator.ScalingFactor;
            var borderDistanceX = userSettings.BorderDistanceX / scalingFactor;
            var borderDistanceY = userSettings.BorderDistanceY / scalingFactor;
            var spotWidth = userSettings.SpotWidth / scalingFactor;
            var spotHeight = userSettings.SpotHeight / scalingFactor;

            var canvasSizeX = screenWidth - 2 * borderDistanceX;
            var canvasSizeY = screenHeight - 2 * borderDistanceY;


            var xResolution = spotsX > 1 ? (canvasSizeX - spotWidth) / (spotsX - 1) : 0;
            var xRemainingOffset = spotsX > 1 ? ((canvasSizeX - spotWidth) % (spotsX - 1)) / 2 : 0;
            var yResolution = spotsY > 1 ? (canvasSizeY - spotHeight) / (spotsY - 1) : 0;
            var yRemainingOffset = spotsY > 1 ? ((canvasSizeY - spotHeight) % (spotsY - 1)) / 2 : 0;

            var counter = 0;
            var relationIndex = spotsX - spotsY + 1;

            for (var j = 0; j < spotsY; j++)
            {
                for (var i = 0; i < spotsX; i++)
                {
                    var isFirstColumn = i == 0;
                    var isLastColumn = i == spotsX - 1;
                    var isFirstRow = j == 0;
                    var isLastRow = j == spotsY - 1;

                    if (isFirstColumn || isLastColumn || isFirstRow || isLastRow) // needing only outer spots
                    {
                        var x = (xRemainingOffset + borderDistanceX + userSettings.OffsetX / scalingFactor + i * xResolution)
                                .Clamp(0, screenWidth);

                        var y = (yRemainingOffset + borderDistanceY + userSettings.OffsetY / scalingFactor + j * yResolution)
                                .Clamp(0, screenHeight);

                        var index = counter++; // in first row index is always counter

                        if (spotsX > 1 && spotsY > 1)
                        {
                            if (!isFirstRow && !isLastRow)
                            {
                                if (isFirstColumn)
                                {
                                    index += relationIndex + ((spotsY - 1 - j) * 3);
                                }
                                else if (isLastColumn)
                                {
                                    index -= j;
                                }
                            }

                            if (!isFirstRow && isLastRow)
                            {
                                index += relationIndex - i * 2;
                            }
                        }

                        spots[index] = new Spot(x, y, spotWidth, spotHeight);
                    }
                }
            }



            //TODO totally broken :(

            if (userSettings.OffsetLed != 0) Offset(ref spots, userSettings.OffsetLed);
            if (spotsY > 1 && userSettings.MirrorX) MirrorX(spots, spotsX, spotsY);
            if (spotsX > 1 && userSettings.MirrorY) MirrorY(spots, spotsX, spotsY);

            spots[0].IsFirst = true;
            return spots;
        }

        internal ISpot[] BuildSpots2(int screenWidth, int screenHeight, IUserSettings userSettings)
        {
            var spotsX2 = userSettings.SpotsX2;
            var spotsY2 = userSettings.SpotsY2;
            ISpot[] spots2 = new Spot[CountLeds(spotsX2, spotsY2)];


            var scalingFactor = DesktopDuplicator.ScalingFactor;
            var borderDistanceX = userSettings.BorderDistanceX / scalingFactor;
            var borderDistanceY = userSettings.BorderDistanceY / scalingFactor;
            var spotWidth = userSettings.SpotWidth / scalingFactor;
            var spotHeight = userSettings.SpotHeight / scalingFactor;

            var canvasSizeX = screenWidth - 2 * borderDistanceX;
            var canvasSizeY = screenHeight - 2 * borderDistanceY;


            var xResolution = spotsX2 > 1 ? (canvasSizeX - spotWidth) / (spotsX2 - 1) : 0;
            var xRemainingOffset = spotsX2 > 1 ? ((canvasSizeX - spotWidth) % (spotsX2 - 1)) / 2 : 0;
            var yResolution = spotsY2 > 1 ? (canvasSizeY - spotHeight) / (spotsY2 - 1) : 0;
            var yRemainingOffset = spotsY2 > 1 ? ((canvasSizeY - spotHeight) % (spotsY2 - 1)) / 2 : 0;

            var counter = 0;
            var relationIndex = spotsX2 - spotsY2 + 1;

            for (var j = 0; j < spotsY2; j++)
            {
                for (var i = 0; i < spotsX2; i++)
                {
                    var isFirstColumn = i == 0;
                    var isLastColumn = i == spotsX2 - 1;
                    var isFirstRow = j == 0;
                    var isLastRow = j == spotsY2 - 1;

                    if (isFirstColumn || isLastColumn || isFirstRow || isLastRow) // needing only outer spots
                    {
                        var x = (xRemainingOffset + borderDistanceX + userSettings.OffsetX / scalingFactor + i * xResolution)
                                .Clamp(0, screenWidth);

                        var y = (yRemainingOffset + borderDistanceY + userSettings.OffsetY / scalingFactor + j * yResolution)
                                .Clamp(0, screenHeight);

                        var index = counter++; // in first row index is always counter

                        if (spotsX2 > 1 && spotsY2 > 1)
                        {
                            if (!isFirstRow && !isLastRow)
                            {
                                if (isFirstColumn)
                                {
                                    index += relationIndex + ((spotsY2 - 1 - j) * 3);
                                }
                                else if (isLastColumn)
                                {
                                    index -= j;
                                }
                            }

                            if (!isFirstRow && isLastRow)
                            {
                                index += relationIndex - i * 2;
                            }
                        }

                        spots2[index] = new Spot(x, y, spotWidth, spotHeight);
                    }
                }
            }



            //TODO totally broken :(

            if (userSettings.OffsetLed != 0) Offset(ref spots2, userSettings.OffsetLed);
            if (spotsY2 > 1 && userSettings.MirrorX) MirrorX(spots2, spotsX2, spotsY2);
            if (spotsX2 > 1 && userSettings.MirrorY) MirrorY(spots2, spotsX2, spotsY2);

            spots2[0].IsFirst = true;
            return spots2;
        }


        private static void Mirror(ISpot[] spots, int startIndex, int length)
        {
            var halfLength = (length / 2);
            var endIndex = startIndex + length - 1;

            for (var i = 0; i < halfLength; i++)
            {
                spots.Swap(startIndex + i, endIndex - i);
            }
        }

        private static void MirrorX(ISpot[] spots, int spotsX, int spotsY)
        {
            // copy swap last row to first row inverse
            for (var i = 0; i < spotsX; i++)
            {
                var index1 = i;
                var index2 = (spots.Length - 1) - (spotsY - 2) - i;
                spots.Swap(index1, index2);
            }

            // mirror first column
            Mirror(spots, spotsX, spotsY - 2);

            // mirror last column
            if (spotsX > 1)
                Mirror(spots, 2 * spotsX + spotsY - 2, spotsY - 2);
        }

        private static void MirrorY(ISpot[] spots, int spotsX, int spotsY)
        {
            // copy swap last row to first row inverse
            for (var i = 0; i < spotsY - 2; i++)
            {
                var index1 = spotsX + i;
                var index2 = (spots.Length - 1) - i;
                spots.Swap(index1, index2);
            }

            // mirror first row
            Mirror(spots, 0, spotsX);

            // mirror last row
            if (spotsY > 1)
                Mirror(spots, spotsX + spotsY - 2, spotsX);
        }

        private static void Offset(ref ISpot[] spots, int offset)
        {
            ISpot[] temp = new Spot[spots.Length];
            for (var i = 0; i < spots.Length; i++)
            {
                temp[(i + temp.Length + offset) % temp.Length] = spots[i];
            }
            spots = temp;
        }

        public void IndicateMissingValues()
        {
            foreach (var spot in Spots)
            {
                spot.IndicateMissingValue();
            }
        }
    }


}
