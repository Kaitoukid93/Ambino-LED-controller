using adrilight.DesktopDuplication;
using adrilight.Extensions;
using BO;
using Microsoft.Win32;
using NLog;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace adrilight
{
    internal sealed class GeneralSpotSet : IGeneralSpotSet
    {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        public GeneralSpotSet(IGeneralSettings userSettings)
        {
            DeviceInfo = userSettings ?? throw new ArgumentNullException(nameof(userSettings));

            SystemEvents.DisplaySettingsChanged += (_, e) => DecideRefresh("ResChange");
            DeviceInfo.PropertyChanged += (_, e) => DecideRefresh(e.PropertyName);
            Refresh();
   
            _log.Info($"SpotSet created.");
        }
        //public SpotSet(DeviceSettings userSettings)
        //{
        //    DeviceInfo = userSettings ?? throw new ArgumentNullException(nameof(userSettings));


        //    DeviceInfo.PropertyChanged += (_, e) => DecideRefresh(e.PropertyName);
        //    RefreshDevice();

        //    _log.Info($"SpotSet created.");
        //}
        private void DecideRefresh(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(DeviceInfo.BorderDistanceX):
                case nameof(DeviceInfo.BorderDistanceY):
                case nameof(DeviceInfo.MirrorX):
                case nameof(DeviceInfo.MirrorY):
                case nameof(DeviceInfo.OffsetLed):
                case "ResChange":
                case nameof(DeviceInfo.SpotHeight):
                case nameof(DeviceInfo.SpotsX):
                case nameof(DeviceInfo.SpotsY):
                case nameof(DeviceInfo.SpotsX2):
                case nameof(DeviceInfo.SpotsY2):
                case nameof(DeviceInfo.SpotsX3):
                case nameof(DeviceInfo.SpotsY3):
                case nameof(DeviceInfo.OffsetLed2):
                case nameof(DeviceInfo.OffsetLed3):


                case nameof(DeviceInfo.SpotWidth):
                    Refresh();
                    break;
            }
        }
       

        public IGeneralSpot[] Spots { get; set; }
        public IGeneralSpot[] Spots2 { get; set; }
        public IGeneralSpot[] Spots3 { get; set; }
        public IGeneralSpot[] SpotsDesk { get; set; }


        public object Lock { get; } = new object();
    

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
            //return spotsX * spotsY;
        }

      

     
        public int ExpectedScreenWidth => Screen.PrimaryScreen.Bounds.Width / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenHeight => Screen.PrimaryScreen.Bounds.Height / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenWidth2 => Screen.AllScreens.Length>1? Screen.AllScreens[1].Bounds.Width / DesktopDuplicator.ScalingFactor : Screen.PrimaryScreen.Bounds.Width / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenHeight2 => Screen.AllScreens.Length > 1 ? Screen.AllScreens[1].Bounds.Height / DesktopDuplicator.ScalingFactor : Screen.PrimaryScreen.Bounds.Height / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenWidth3 => Screen.AllScreens.Length > 3 ? Screen.AllScreens[2].Bounds.Width / DesktopDuplicator.ScalingFactor : Screen.PrimaryScreen.Bounds.Width / DesktopDuplicator.ScalingFactor;
        public int ExpectedScreenHeight3 => Screen.AllScreens.Length > 3 ? Screen.AllScreens[2].Bounds.Height / DesktopDuplicator.ScalingFactor : Screen.PrimaryScreen.Bounds.Height / DesktopDuplicator.ScalingFactor;


        private IGeneralSettings DeviceInfo { get; }
        private void Refresh()
        {
            lock (Lock)
            {
                Spots = BuildSpots(ExpectedScreenWidth, ExpectedScreenHeight, DeviceInfo);
                Spots2 = BuildSpots2(ExpectedScreenWidth2, ExpectedScreenHeight2, DeviceInfo);
                Spots3 = BuildSpots3(ExpectedScreenWidth3, ExpectedScreenHeight3, DeviceInfo);
                SpotsDesk = BuildSpotsDesk(ExpectedScreenWidth, ExpectedScreenHeight, DeviceInfo);
            }
        }

       
        

        internal IGeneralSpot[] BuildSpots(int screenWidth, int screenHeight, IGeneralSettings userSettings)
        {
            var spotsX = userSettings.SpotsX;
            var spotsY = userSettings.SpotsY;
            IGeneralSpot[] spots;
            if (spotsX > 0 && spotsY > 0)
            {
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }
            else
            {
                spotsX = 11;
                spotsY = 6;
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }




            var scalingFactor = DesktopDuplicator.ScalingFactor;
            var borderDistanceX = userSettings.BorderDistanceX / scalingFactor;
            var borderDistanceY = userSettings.BorderDistanceY / scalingFactor;
            var realSpotWidth = screenWidth / spotsX;
            var realSpotHeight = screenHeight/ spotsY; // this is really helpful, user is no longer has to input spotwidth and spotheight which can cause weird behavior
            
            //var spotWidth = realSpotWidth / scalingFactor;
            //var spotHeight = realSpotHeight / scalingFactor;
            var spotWidth = realSpotWidth;
            var spotHeight = realSpotHeight; 
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
                        var x = (xRemainingOffset + borderDistanceX  / scalingFactor + i * xResolution)
                                .Clamp(0, screenWidth);

                        var y = (yRemainingOffset + borderDistanceY  / scalingFactor + j * yResolution)
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

                        spots[index] = new GeneralSpot(x, y, spotWidth, spotHeight, i, j);
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


        internal IGeneralSpot[] BuildSpotsDesk(int screenWidth, int screenHeight, IGeneralSettings userSettings)
        {
            var spotsX=80;
            var spotsY=1;
            if(userSettings.DeskSize==0)//1m2
            {
                spotsX = 48;
                spotsY = 1;
            }
            else if (userSettings.DeskSize==1) //2m
            {
                spotsX = 80;
                spotsY = 1;
            }
            IGeneralSpot[] spots;
            if (spotsX > 0 && spotsY > 0)
            {
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }
            else
            {
                spotsX = 11;
                spotsY = 6;
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }




            var scalingFactor = DesktopDuplicator.ScalingFactor;
          
            var realSpotWidth = screenWidth / spotsX;
            var realSpotHeight = spotsX; // this is really helpful, user is no longer has to input spotwidth and spotheight which can cause weird behavior
            var borderDistanceX = userSettings.BorderDistanceX / scalingFactor;
            var borderDistanceY = 0 / scalingFactor;
            //var spotWidth = realSpotWidth / scalingFactor;
            //var spotHeight = realSpotHeight / scalingFactor;
            var spotWidth = realSpotWidth;
            var spotHeight = realSpotHeight;
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
                        var x = (xRemainingOffset + borderDistanceX / scalingFactor + i * xResolution)
                                .Clamp(0, screenWidth);

                        var y = screenHeight - borderDistanceY - spotHeight;

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

                        spots[index] = new GeneralSpot(x, y, spotWidth, spotHeight, i, j);
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



        internal IGeneralSpot[] BuildSpots2(int screenWidth, int screenHeight, IGeneralSettings userSettings)
        {
            var spotsX = userSettings.SpotsX2;
            var spotsY = userSettings.SpotsY2;
            IGeneralSpot[] spots;
            if (spotsX > 0 && spotsY > 0)
            {
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }
            else
            {
                spotsX = 11;
                spotsY = 6;
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }




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
                        var x = (xRemainingOffset + borderDistanceX / scalingFactor + i * xResolution)
                                .Clamp(0, screenWidth);

                        var y = (yRemainingOffset + borderDistanceY / scalingFactor + j * yResolution)
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

                        spots[index] = new GeneralSpot(x, y, spotWidth, spotHeight, i, j);
                    }
                }
            }



            //TODO totally broken :(

            if (userSettings.OffsetLed2 != 0) Offset(ref spots, userSettings.OffsetLed2);
            if (spotsY > 1 && userSettings.MirrorX) MirrorX(spots, spotsX, spotsY);
            if (spotsX > 1 && userSettings.MirrorY) MirrorY(spots, spotsX, spotsY);

            spots[0].IsFirst = true;
            return spots;
        }

        internal IGeneralSpot[] BuildSpots3(int screenWidth, int screenHeight, IGeneralSettings userSettings)
        {
            var spotsX = userSettings.SpotsX3;
            var spotsY = userSettings.SpotsY3;
            IGeneralSpot[] spots;
            if (spotsX > 0 && spotsY > 0)
            {
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }
            else
            {
                spotsX = 11;
                spotsY = 6;
                spots = new GeneralSpot[CountLeds(spotsX, spotsY)];
            }




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
                        var x = (xRemainingOffset + borderDistanceX / scalingFactor + i * xResolution)
                                .Clamp(0, screenWidth);

                        var y = (yRemainingOffset + borderDistanceY / scalingFactor + j * yResolution)
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

                        spots[index] = new GeneralSpot(x, y, spotWidth, spotHeight, i, j);
                    }
                }
            }



            //TODO totally broken :(

            if (userSettings.OffsetLed3 != 0) Offset(ref spots, userSettings.OffsetLed3);
            if (spotsY > 1 && userSettings.MirrorX) MirrorX(spots, spotsX, spotsY);
            if (spotsX > 1 && userSettings.MirrorY) MirrorY(spots, spotsX, spotsY);

            spots[0].IsFirst = true;
            return spots;
        }

        private static void Mirror(IGeneralSpot[] spots, int startIndex, int length)
        {
            var halfLength = (length / 2);
            var endIndex = startIndex + length - 1;

            for (var i = 0; i < halfLength; i++)
            {
                spots.Swap(startIndex + i, endIndex - i);
            }
        }

        private static void MirrorX(IGeneralSpot[] spots, int spotsX, int spotsY)
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

        private static void MirrorY(IGeneralSpot[] spots, int spotsX, int spotsY)
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

        private static void Offset(ref IGeneralSpot[] spots, int offset)
        {
            IGeneralSpot[] temp = new GeneralSpot[spots.Length];
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
