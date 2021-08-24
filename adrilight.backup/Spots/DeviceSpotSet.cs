using adrilight.DesktopDuplication;
using adrilight.Extensions;
using adrilight.Spots;
using BO;
using NLog;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace adrilight
{
    internal sealed class DeviceSpotSet : IDeviceSpotSet
    {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        public DeviceSpotSet(IDeviceSettings deviceSettings, IGeneralSettings generalSettings)
        {
            DeviceSettings = deviceSettings ?? throw new ArgumentNullException(nameof(deviceSettings));
            GeneralSettings = generalSettings ?? throw new ArgumentNullException(nameof(generalSettings));

            DeviceSettings.PropertyChanged += (_, e) => DecideRefresh(e.PropertyName);
            GeneralSettings.PropertyChanged += (_, e) => DecideRefresh(e.PropertyName);
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
                case nameof(DeviceSettings.SpotsX):
                case nameof(DeviceSettings.SpotsY):
                case nameof(GeneralSettings.ScreenSize):
                case nameof(DeviceSettings.SelectedDisplay):
                case nameof(DeviceSettings.SelectedEffect):
                case nameof(GeneralSettings.ScreenSizeSecondary):
                case nameof(GeneralSettings.ScreenSizeThird):


                    Refresh();
                    break;
            }
        }


        public IDeviceSpot[] Spots { get; set; }


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
        private int _iD;

        public int ID {
            get { return DeviceSettings.DeviceID; }
            set
            {
                _iD = value;
            }
        }

        private int _parrentLocation;

        public int ParrentLocation 
            { 
            get { return DeviceSettings.ParrentLocation; }
            set
            {
            _parrentLocation= value;
        }
        }
        private int _outputLocation;
        public int OutputLocation {
            get { return DeviceSettings.OutputLocation; }
            set
            {
                _outputLocation = value;
            }
        }




        private IDeviceSettings DeviceSettings { get; }
        private IGeneralSettings GeneralSettings { get; }
        private void Refresh()
        {
            lock (Lock)
            {
                Spots = BuildSpots( DeviceSettings, GeneralSettings);
            }
        }

      



        internal IDeviceSpot[] BuildSpots( IDeviceSettings userSettings, IGeneralSettings generalSettings) // general settings is for compare each device setting
        {
            //this section build spot according to user selection
            //in the future, user will select the rectangle on a matrix map then we take that rectangle cordinate
            // this kind of selection only available at gifxelation and screen capture mode
            // with others mode, we arrange as the default shape ( square for screen, round for fan, straight for desk led)

            var spotsX = userSettings.SpotsX;
            var spotsY = userSettings.SpotsY;
            var offset = 10;
            IDeviceSpot[] devicespots = new DeviceSpot[160]; //maximum number of spot
            if(DeviceSettings.SelectedEffect==0)
            {
                if(DeviceSettings.DeviceType=="ABRev1" || DeviceSettings.DeviceType=="ABRev2")
                {

                  
                if (DeviceSettings.SelectedDisplay == 0) // 
                {
                    if (spotsX != generalSettings.SpotsX || spotsY != generalSettings.SpotsY)// check if user input over kill the parrent's matrix that precreated
                    {
                        spotsX = generalSettings.SpotsX;
                        spotsY = generalSettings.SpotsY; // revert to default value of spotsX and spotsY
                        // MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                        devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                    }
                    else

                    {
                        devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                    }
                }
                else if (DeviceSettings.SelectedDisplay == 1)
                {
                    if (spotsX != generalSettings.SpotsX2 || spotsY != generalSettings.SpotsY2)// check if user input over kill the parrent's matrix that precreated
                    {
                        spotsX = generalSettings.SpotsX2;
                        spotsY = generalSettings.SpotsY2; // revert to default value of spotsX and spotsY
                        // MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                        devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                    }
                    else

                    {
                        devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                    }
                }
                else if (DeviceSettings.SelectedDisplay == 2)
                {
                    if (spotsX != generalSettings.SpotsX3 || spotsY != generalSettings.SpotsY3)// check if user input over kill the parrent's matrix that precreated
                    {
                        spotsX = generalSettings.SpotsX3;
                        spotsY = generalSettings.SpotsY3; // revert to default value of spotsX and spotsY
                        // MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                        devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                    }
                    else

                    {
                        devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                    }
                }
                }
                else if(DeviceSettings.DeviceType=="ABEDGE")
                {
                    devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                }
                else if(DeviceSettings.DeviceType=="Strip")
                {
                    devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                }
                else if (DeviceSettings.DeviceType == "Square")
                {
                    if (DeviceSettings.SelectedDisplay == 0) // 
                    {
                        if (spotsX != generalSettings.SpotsX || spotsY != generalSettings.SpotsY)// check if user input over kill the parrent's matrix that precreated
                        {
                            spotsX = generalSettings.SpotsX;
                            spotsY = generalSettings.SpotsY; // revert to default value of spotsX and spotsY
                                                             // MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                            devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                        }
                        else

                        {
                            devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                        }
                    }
                    else if (DeviceSettings.SelectedDisplay == 1)
                    {
                        if (spotsX != generalSettings.SpotsX2 || spotsY != generalSettings.SpotsY2)// check if user input over kill the parrent's matrix that precreated
                        {
                            spotsX = generalSettings.SpotsX2;
                            spotsY = generalSettings.SpotsY2; // revert to default value of spotsX and spotsY
                                                              // MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                            devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                        }
                        else

                        {
                            devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                        }
                    }
                    else if (DeviceSettings.SelectedDisplay == 2)
                    {
                        if (spotsX != generalSettings.SpotsX3 || spotsY != generalSettings.SpotsY3)// check if user input over kill the parrent's matrix that precreated
                        {
                            spotsX = generalSettings.SpotsX3;
                            spotsY = generalSettings.SpotsY3; // revert to default value of spotsX and spotsY
                                                              // MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                            devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                        }
                        else

                        {
                            devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                        }
                    }
                }
                else 
                {
                    devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
                }
            }

            else
            {
                devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];

            }


            //just this is enough????
            // next, everytime a frame update, spotsetreader (which attached to every single device) will set the color of each device spots in devicespotset acording to index
            // SpotSet reader only service gifxelation and screen capture mode
            // the treeview is Desktopduplicator(x)-> desktopduplicatorReader(x)-> SpotSetReader(device)->Viewmodel(device)->SerialStream(device)
            var screenWidth = 240;
            var screenHeight = 135;
            if (userSettings.DeviceSize==0|| userSettings.DeviceSize == 1|| userSettings.DeviceSize == 3)
            {
                 screenWidth = 240;
                 screenHeight = 135;
            }
            else
            {
                screenWidth = 320;
                screenHeight = 135;
            }
            
            var scalingFactor = DesktopDuplicator.ScalingFactor;
            var borderDistanceX = userSettings.BorderDistanceX / scalingFactor;
            var borderDistanceY = userSettings.BorderDistanceY / scalingFactor;
            var spotWidth = screenWidth / userSettings.SpotsX;
            var spotHeight = spotWidth;

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

                        devicespots[index] = new DeviceSpot(x, y, spotWidth, spotHeight,i,j);
                    }
                }
            }
            if (DeviceSettings.SelectedEffect==0)
            {


                if (DeviceSettings.SelectedDisplay == 0)
                {
                    if (GeneralSettings.OffsetLed != 0) Offset(ref devicespots, GeneralSettings.OffsetLed);
                }
                else if (DeviceSettings.SelectedDisplay == 1)
                {
                    if (GeneralSettings.OffsetLed2 != 0) Offset(ref devicespots, GeneralSettings.OffsetLed2);
                }
                else if (DeviceSettings.SelectedDisplay == 2)
                {
                    if (GeneralSettings.OffsetLed3 != 0) Offset(ref devicespots, GeneralSettings.OffsetLed3);
                }
            }
                
            if (spotsY > 1 && GeneralSettings.MirrorX) MirrorX(devicespots, spotsX, spotsY);
            if (spotsX > 1 && GeneralSettings.MirrorY) MirrorY(devicespots, spotsX, spotsY);

            devicespots[0].IsFirst = true;

            return devicespots;
        }


        private static void Mirror(IDeviceSpot[] spots, int startIndex, int length)
        {
            var halfLength = (length / 2);
            var endIndex = startIndex + length - 1;

            for (var i = 0; i < halfLength; i++)
            {
                spots.Swap(startIndex + i, endIndex - i);
            }
        }

        private static void MirrorX(IDeviceSpot[] spots, int spotsX, int spotsY)
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

        private static void MirrorY(IDeviceSpot[] spots, int spotsX, int spotsY)
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

        private static void Offset(ref IDeviceSpot[] spots, int offset)
        {
            IDeviceSpot[] temp = new DeviceSpot[spots.Length];
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
