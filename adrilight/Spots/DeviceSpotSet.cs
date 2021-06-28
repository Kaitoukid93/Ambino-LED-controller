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

        public DeviceSpotSet(IDeviceSettings userSettings, IGeneralSettings generalSettings)
        {
            DeviceSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            GeneralSettings = generalSettings ?? throw new ArgumentNullException(nameof(generalSettings));

            DeviceSettings.PropertyChanged += (_, e) => DecideRefresh(e.PropertyName);
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

                    Refresh();
                    break;
            }
        }


        public IDeviceSpot[] Spots { get; set; }
     

        public object Lock { get; } = new object();
        public object Lock2 { get; } = new object();
        public object Lock3 { get; } = new object();

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
            return 2 * spotsX + 2 * spotsY-4 ;
            //return spotsX * spotsY;
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
            IDeviceSpot[] devicespots;
            if (spotsX!= generalSettings.SpotsX || spotsY != generalSettings.SpotsY)// check if user input over kill the parrent's matrix that precreated
            {
                spotsX = 10;
                spotsY = 6; // revert to default value of spotsX and spotsY
                MessageBox.Show("Can not create a matrix that greater than original matrix, please enter a smaller value!!!");
                devicespots = new DeviceSpot[CountLeds(spotsX, spotsY)];
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

                        devicespots[index] = new DeviceSpot(x, y, spotWidth, spotHeight,i,j);
                    }
                }
            }



        
            return devicespots;
        }





        
    }


}
