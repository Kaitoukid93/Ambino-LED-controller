﻿using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Buffers;
using System.Windows.Media;
using adrilight.Util;
using System.Linq;
using Newtonsoft.Json;
using adrilight.View.SettingsWindowComponents;
using System.Windows.Forms;

namespace adrilight
{
    internal sealed class
        SerialStream : IDisposable, ISerialStream
    {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        public SerialStream(IUserSettings userSettings, ISpotSet spotSet)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SpotSet2 = spotSet ?? throw new ArgumentNullException(nameof(spotSet));

            UserSettings.PropertyChanged += UserSettings_PropertyChanged;
            RefreshTransferState();
            _log.Info($"SerialStream created.");

            if (!IsValid())
            {
                UserSettings.TransferActive = false;
                UserSettings.ComPort = null;
            }
        }
       

        private void UserSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserSettings.TransferActive):
                    RefreshTransferState();
                    break;
            }
        }

        public bool IsValid() => SerialPort.GetPortNames().Contains(UserSettings.ComPort) || UserSettings.ComPort == "Không có";
       
        private void RefreshTransferState()
        {
            if (UserSettings.TransferActive && !IsRunning)
            {
                if (IsValid())
                {

                    //start it
                    _log.Debug("starting the serial stream");
                    Start();
                }
                else
                {
                    UserSettings.TransferActive = false;
                    UserSettings.ComPort = null;
                }
            }
            else if (!UserSettings.TransferActive && IsRunning)
            {
                //stop it
                _log.Debug("stopping the serial stream");
                Stop();
            }
        }

        private readonly byte[] _messagePreamble = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09 };
        private readonly byte[] _messagePostamble = { 85, 204, 165 };
        private readonly byte[] _messageZoeamble = { 15, 12, 93 };
        

        private Thread _workerThread;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private int frameCounter;
        private int blackFrameCounter;
        
        

        public void Start()
        {
            _log.Debug("Start called.");
            if (_workerThread != null) return;

            _cancellationTokenSource = new CancellationTokenSource();
            _workerThread = new Thread(DoWork) {
                Name = "Serial sending",
                IsBackground = true
            };
            _workerThread.Start(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _log.Debug("Stop called.");
            if (_workerThread == null) return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            _workerThread?.Join();
            _workerThread = null;
        }

        public bool IsRunning => _workerThread != null && _workerThread.IsAlive;

        private IUserSettings UserSettings { get; }
        private ISpotSet SpotSet { get; }
        private ISpotSet SpotSet2 { get; }
        

        private (byte[] Buffer, int OutputLength) GetOutputStream()
        {
            byte[] outputStream;

            int counter = _messagePreamble.Length;
            lock (SpotSet.Lock)
            {
                const int colorsPerLed = 3;
                int bufferLength = _messagePreamble.Length
                    + (UserSettings.LedsPerSpot * SpotSet.Spots.Length * colorsPerLed)
                    + _messagePostamble.Length + 9 + 27 + 3 + 6 + 21 + 9;
                if (UserSettings.effect && UserSettings.SendRandomColors)
                {
                    outputStream = ArrayPool<byte>.Shared.Rent(bufferLength);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStream, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messageZoeamble, 0, outputStream, bufferLength - _messageZoeamble.Length, _messageZoeamble.Length);
                }
                else
                {

                    outputStream = ArrayPool<byte>.Shared.Rent(bufferLength);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStream, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messagePostamble, 0, outputStream, bufferLength - _messagePostamble.Length, _messagePostamble.Length);
                }







                /*
                    outputStream[counter++] = UserSettings.brightnesscounter;
                
              

                    outputStream[counter++] = UserSettings.methodcounter;
               
                
                outputStream[counter++] = UserSettings.speedcounter;
                outputStream[counter++] = UserSettings.effectcounter;
                outputStream[counter++] = UserSettings.sincounter;
                outputStream[counter++] = UserSettings.lightstatus;


                outputStream[counter++] = UserSettings.edgebrightnesscounter;
                outputStream[counter++] = Convert.ToByte(ComPortSetup.volume);

                outputStream[counter++] = UserSettings.huecounter;

               

               

                outputStream[counter++] = UserSettings.color1.R;
                outputStream[counter++] = UserSettings.color1.G;
                outputStream[counter++] = UserSettings.color1.B;

                outputStream[counter++] = UserSettings.color2.R;
                outputStream[counter++] = UserSettings.color2.G;
                outputStream[counter++] = UserSettings.color2.B;

                outputStream[counter++] = UserSettings.color3.R;
                outputStream[counter++] = UserSettings.color3.G;
                outputStream[counter++] = UserSettings.color3.B;

                outputStream[counter++] = UserSettings.color4.R;
                outputStream[counter++] = UserSettings.color4.G;
                outputStream[counter++] = UserSettings.color4.B;

                outputStream[counter++] = UserSettings.color5.R;
                outputStream[counter++] = UserSettings.color5.G;
                outputStream[counter++] = UserSettings.color5.B;

                outputStream[counter++] = UserSettings.color6.R;
                outputStream[counter++] = UserSettings.color6.G;
                outputStream[counter++] = UserSettings.color6.B;

                outputStream[counter++] = UserSettings.color7.R;
                outputStream[counter++] = UserSettings.color7.G;
                outputStream[counter++] = UserSettings.color7.B;

                outputStream[counter++] = UserSettings.color8.R;
                outputStream[counter++] = UserSettings.color8.G;
                outputStream[counter++] = UserSettings.color8.B;

                outputStream[counter++] = Convert.ToByte((UserSettings.SpotsX-1)*2+(UserSettings.SpotsY-1)*2);
                outputStream[counter++] = Convert.ToByte(UserSettings.music);
                outputStream[counter++] = UserSettings.visualcounter;

    */

                outputStream[counter++] = UserSettings.zone1speedcounter;
                outputStream[counter++] = UserSettings.zone2speedcounter;
                outputStream[counter++] = UserSettings.zone3speedcounter;
                if (ComPortSetup.DFU == 1)
                {
                    outputStream[counter++] = 99;
                }
                else
                {
                    outputStream[counter++] = UserSettings.fanmodecounter;
                }

                outputStream[counter++] = UserSettings.LEDfanmodecounter;
                outputStream[counter++] = UserSettings.zoecounter;

                outputStream[counter++] = UserSettings.brightnesscounter;
                outputStream[counter++] = UserSettings.methodcounter;
                outputStream[counter++] = UserSettings.speedcounter;

                outputStream[counter++] = UserSettings.effectcounter;
                outputStream[counter++] = UserSettings.sincounter;
                outputStream[counter++] = UserSettings.lightstatus;


                outputStream[counter++] = UserSettings.fanbrightnesscounter;
                outputStream[counter++] = Convert.ToByte(ComPortSetup.volume);
                outputStream[counter++] = UserSettings.huecounter;

                outputStream[counter++] = UserSettings.color1.R;
                outputStream[counter++] = UserSettings.color1.G;
                outputStream[counter++] = UserSettings.color1.B;

                outputStream[counter++] = UserSettings.color2.R;
                outputStream[counter++] = UserSettings.color2.G;
                outputStream[counter++] = UserSettings.color2.B;

                outputStream[counter++] = UserSettings.color3.R;
                outputStream[counter++] = UserSettings.color3.G;
                outputStream[counter++] = UserSettings.color3.B;

                outputStream[counter++] = UserSettings.color4.R;
                outputStream[counter++] = UserSettings.color4.G;
                outputStream[counter++] = UserSettings.color4.B;

                outputStream[counter++] = UserSettings.color5.R;
                outputStream[counter++] = UserSettings.color5.G;
                outputStream[counter++] = UserSettings.color5.B;

                outputStream[counter++] = UserSettings.color6.R;
                outputStream[counter++] = UserSettings.color6.G;
                outputStream[counter++] = UserSettings.color6.B;

                outputStream[counter++] = UserSettings.color7.R;
                outputStream[counter++] = UserSettings.color7.G;
                outputStream[counter++] = UserSettings.color7.B;

                outputStream[counter++] = UserSettings.color8.R;
                outputStream[counter++] = UserSettings.color8.G;
                outputStream[counter++] = UserSettings.color8.B;





                outputStream[counter++] = Convert.ToByte((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2);
                outputStream[counter++] = Convert.ToByte(UserSettings.music);
                outputStream[counter++] = UserSettings.visualcounter;

                outputStream[counter++] = Convert.ToByte((UserSettings.SpotsX2 - 1) * 2 + (UserSettings.SpotsY2 - 1) * 2);
                outputStream[counter++] = UserSettings.edgebrightnesscounter;
                outputStream[counter++] = UserSettings.edgehuecounter;

                outputStream[counter++] = ComPortSetup.output_spectrumdata[0];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[1];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[2];

                outputStream[counter++] = ComPortSetup.output_spectrumdata[3];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[4];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[5];

                outputStream[counter++] = ComPortSetup.output_spectrumdata[6];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[7];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[8];

                outputStream[counter++] = ComPortSetup.output_spectrumdata[9];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[10];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[11];

                outputStream[counter++] = ComPortSetup.output_spectrumdata[12];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[13];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[14];

                outputStream[counter++] = ComPortSetup.output_spectrumdata[15];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[15];
                outputStream[counter++] = ComPortSetup.output_spectrumdata[15];


                outputStream[counter++] = UserSettings.screeneffectcounter;
                outputStream[counter++] = UserSettings.Faneffectcounter;
                outputStream[counter++] = UserSettings.screeneffectcounter;

                outputStream[counter++] = UserSettings.CaseStatic.R;
                outputStream[counter++] = UserSettings.CaseStatic.G;
                outputStream[counter++] = UserSettings.CaseStatic.B;

                outputStream[counter++] = UserSettings.ScreenStatic.R;
                outputStream[counter++] = UserSettings.ScreenStatic.G;
                outputStream[counter++] = UserSettings.ScreenStatic.B;

                outputStream[counter++] = UserSettings.DeskStatic.R;
                outputStream[counter++] = UserSettings.DeskStatic.G;
                outputStream[counter++] = UserSettings.DeskStatic.B;

                var allBlack = true;
                foreach (Spot spot in SpotSet.Spots)
                {
                    for (int i = 0; i < UserSettings.LedsPerSpot; i++)
                    {
                        if (!UserSettings.SendRandomColors)
                        {
                            outputStream[counter++] = spot.Blue; // blue
                            outputStream[counter++] = spot.Green; // green
                            outputStream[counter++] = spot.Red; // red

                            allBlack = allBlack && spot.Red == 0 && spot.Green == 0 && spot.Blue == 0;
                           
                        }

                        else
                        {
                            allBlack = false;
                            var n = UserSettings.zoecounter;
                            var m = UserSettings.brightnesscounter;
                           
                            if (UserSettings.fixedcolor)
                            {
                                if (n == 255)
                                {
                                    outputStream[counter++] = 255; // blue
                                    outputStream[counter++] = 255; // green
                                    outputStream[counter++] = 255; // red

                                }
                                else
                                {
                                    var c = ColorUtil.FromAhsb(255, n, 1, 0.5f);
                                    outputStream[counter++] = c.B; // blue
                                    outputStream[counter++] = c.G; // green
                                    outputStream[counter++] = c.R; // red
                                }
                            }
                           
                               
                                
                           
                            

                            





                           
                            

                        }
                        }

                    }



                    if (allBlack)
                    {
                        blackFrameCounter++;
                    }

                    return (outputStream, bufferLength);
                }
            }


        private (byte[] Buffer, int OutputLength) GetOutputStreamHUB()
        {
            byte[] outputStreamHUB;
            int bufferLength = 0;
            int counter = _messagePreamble.Length;
            lock (SpotSet.Lock)
            {
                const int colorsPerLed = 3;
                if(Screen.AllScreens.Length==2)
                { 
                 bufferLength = _messagePreamble.Length
                    + (UserSettings.LedsPerSpot * SpotSet.Spots.Length * colorsPerLed + UserSettings.LedsPerSpot * SpotSet2.Spots2.Length * colorsPerLed)
                    + _messagePostamble.Length + 9 + 27+3;
                }
                else
                {
                     bufferLength = _messagePreamble.Length
                                        + (UserSettings.LedsPerSpot * SpotSet.Spots.Length * colorsPerLed + UserSettings.LedsPerSpot * ((UserSettings.SpotsX2-1)*2+(UserSettings.SpotsY2-1)*2) * colorsPerLed)
                                        + _messagePostamble.Length + 9 + 27 + 3;
                }
                if (UserSettings.effect && UserSettings.SendRandomColors)
                {
                    outputStreamHUB = ArrayPool<byte>.Shared.Rent(bufferLength);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStreamHUB, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messageZoeamble, 0, outputStreamHUB, bufferLength - _messageZoeamble.Length, _messageZoeamble.Length);
                }
                else
                {

                    outputStreamHUB = ArrayPool<byte>.Shared.Rent(bufferLength);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStreamHUB, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messagePostamble, 0, outputStreamHUB, bufferLength - _messagePostamble.Length, _messagePostamble.Length);
                }








                outputStreamHUB[counter++] = UserSettings.brightnesscounter;
                outputStreamHUB[counter++] = UserSettings.methodcounter;
                outputStreamHUB[counter++] = UserSettings.speedcounter;

                outputStreamHUB[counter++] = UserSettings.effectcounter;
                outputStreamHUB[counter++] = UserSettings.sincounter;
                outputStreamHUB[counter++] = UserSettings.lightstatus;


                outputStreamHUB[counter++] = UserSettings.fanbrightnesscounter;
                outputStreamHUB[counter++] = Convert.ToByte(ComPortSetup.volume / 4);
                outputStreamHUB[counter++] = UserSettings.huecounter;

                outputStreamHUB[counter++] = UserSettings.color1.R;
                outputStreamHUB[counter++] = UserSettings.color1.G;
                outputStreamHUB[counter++] = UserSettings.color1.B;

                outputStreamHUB[counter++] = UserSettings.color2.R;
                outputStreamHUB[counter++] = UserSettings.color2.G;
                outputStreamHUB[counter++] = UserSettings.color2.B;

                outputStreamHUB[counter++] = UserSettings.color3.R;
                outputStreamHUB[counter++] = UserSettings.color3.G;
                outputStreamHUB[counter++] = UserSettings.color3.B;

                outputStreamHUB[counter++] = UserSettings.color4.R;
                outputStreamHUB[counter++] = UserSettings.color4.G;
                outputStreamHUB[counter++] = UserSettings.color4.B;

                outputStreamHUB[counter++] = UserSettings.color5.R;
                outputStreamHUB[counter++] = UserSettings.color5.G;
                outputStreamHUB[counter++] = UserSettings.color5.B;

                outputStreamHUB[counter++] = UserSettings.color6.R;
                outputStreamHUB[counter++] = UserSettings.color6.G;
                outputStreamHUB[counter++] = UserSettings.color6.B;

                outputStreamHUB[counter++] = UserSettings.color7.R;
                outputStreamHUB[counter++] = UserSettings.color7.G;
                outputStreamHUB[counter++] = UserSettings.color7.B;

                outputStreamHUB[counter++] = UserSettings.color8.R;
                outputStreamHUB[counter++] = UserSettings.color8.G;
                outputStreamHUB[counter++] = UserSettings.color8.B;

                outputStreamHUB[counter++] = Convert.ToByte((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2);
                outputStreamHUB[counter++] = Convert.ToByte(UserSettings.music);
                outputStreamHUB[counter++] = UserSettings.visualcounter;

                outputStreamHUB[counter++] = Convert.ToByte((UserSettings.SpotsX2 - 1) * 2 + (UserSettings.SpotsY2 - 1) * 2);
                outputStreamHUB[counter++] = UserSettings.edgebrightnesscounter;
                outputStreamHUB[counter++] = UserSettings.edgehuecounter;

                var allBlack = true;
                foreach (Spot spot in SpotSet.Spots)
                {
                    for (int i = 0; i < UserSettings.LedsPerSpot; i++)
                    {
                        if (!UserSettings.SendRandomColors)
                        {
                            outputStreamHUB[counter++] = spot.Blue; // blue
                            outputStreamHUB[counter++] = spot.Green; // green
                            outputStreamHUB[counter++] = spot.Red; // red

                            allBlack = allBlack && spot.Red == 0 && spot.Green == 0 && spot.Blue == 0;

                        }

                        else
                        {
                            allBlack = false;
                            var n = UserSettings.zoecounter;
                            var m = UserSettings.brightnesscounter;
                           
                            if (UserSettings.fixedcolor)
                            {
                                if (n == 255)
                                {
                                    outputStreamHUB[counter++] = 255; // blue
                                    outputStreamHUB[counter++] = 255; // green
                                    outputStreamHUB[counter++] = 255; // red

                                }
                                else
                                {
                                    var c = ColorUtil.FromAhsb(255, n, 1, 0.5f);
                                    outputStreamHUB[counter++] = c.B; // blue
                                    outputStreamHUB[counter++] = c.G; // green
                                    outputStreamHUB[counter++] = c.R; // red
                                }
                            }















                        }
                    }

                }
                if (Screen.AllScreens.Length == 2)
                {
                    foreach (Spot spot2 in SpotSet2.Spots2)
                    {
                        for (int i = 0; i < UserSettings.LedsPerSpot; i++)
                        {
                            if (!UserSettings.SendRandomColors)
                            {
                                outputStreamHUB[counter++] = spot2.Blue; // blue
                                outputStreamHUB[counter++] = spot2.Green; // green
                                outputStreamHUB[counter++] = spot2.Red; // red

                                allBlack = allBlack && spot2.Red == 0 && spot2.Green == 0 && spot2.Blue == 0;

                            }

                            else
                            {
                                allBlack = false;
                                var n = UserSettings.zoecounter;
                                var m = UserSettings.brightnesscounter;
                               
                                if (UserSettings.fixedcolor)
                                {
                                    if (n == 255)
                                    {
                                        outputStreamHUB[counter++] = 255; // blue
                                        outputStreamHUB[counter++] = 255; // green
                                        outputStreamHUB[counter++] = 255; // red

                                    }
                                    else
                                    {
                                        var c = ColorUtil.FromAhsb(255, n, 1, 0.5f);
                                        outputStreamHUB[counter++] = c.B; // blue
                                        outputStreamHUB[counter++] = c.G; // green
                                        outputStreamHUB[counter++] = c.R; // red
                                    }
                                }















                            }
                        }

                    }
                }
                else
                {
                    for(int i=0;i<(UserSettings.SpotsX2+UserSettings.SpotsY2-2)*2;i++)
                    {
                        outputStreamHUB[counter++] = 0; // blue
                        outputStreamHUB[counter++] = 0; // green
                        outputStreamHUB[counter++] = 0; // red
                    }
                }

            if (allBlack)
                {
                    blackFrameCounter++;
                }

                return (outputStreamHUB, bufferLength);
            }
        }

        private (byte[] Buffer, int OutputLength) GetOutputStreamHUBV2()
        {
            byte[] outputStreamHUBV2;
            int bufferHUBV2Length = 0;
            int counter = _messagePreamble.Length;
            lock (SpotSet.Lock)
            {
                const int colorsPerLed = 3;
                if (Screen.AllScreens.Length == 2)
                {
                    bufferHUBV2Length = _messagePreamble.Length
                       + (UserSettings.LedsPerSpot * SpotSet.Spots.Length * colorsPerLed + UserSettings.LedsPerSpot * SpotSet2.Spots2.Length * colorsPerLed)
                       + _messagePostamble.Length + 9 + 27 + 3 +6+21+9;
                }
                else
                {
                    bufferHUBV2Length = _messagePreamble.Length
                                       + (UserSettings.LedsPerSpot * SpotSet.Spots.Length * colorsPerLed + UserSettings.LedsPerSpot * ((UserSettings.SpotsX2 - 1) * 2 + (UserSettings.SpotsY2 - 1) * 2) * colorsPerLed)
                                       + _messagePostamble.Length + 9 + 27 + 3 + 6+21+9;
                }
                if (UserSettings.effect && UserSettings.SendRandomColors)
                {
                    outputStreamHUBV2 = ArrayPool<byte>.Shared.Rent(bufferHUBV2Length);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStreamHUBV2, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messageZoeamble, 0, outputStreamHUBV2, bufferHUBV2Length - _messageZoeamble.Length, _messageZoeamble.Length);
                }
                else
                {

                    outputStreamHUBV2 = ArrayPool<byte>.Shared.Rent(bufferHUBV2Length);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStreamHUBV2, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messagePostamble, 0, outputStreamHUBV2, bufferHUBV2Length - _messagePostamble.Length, _messagePostamble.Length);
                }






                outputStreamHUBV2[counter++] = UserSettings.zone1speedcounter;
                outputStreamHUBV2[counter++] = UserSettings.zone2speedcounter;
                outputStreamHUBV2[counter++] = UserSettings.zone3speedcounter;
                if (ComPortSetup.DFU == 1)
                {
                    outputStreamHUBV2[counter++] = 99;
                }
                else
                {
                    outputStreamHUBV2[counter++] = UserSettings.fanmodecounter;
                }
                
                outputStreamHUBV2[counter++] = UserSettings.LEDfanmodecounter;
                outputStreamHUBV2[counter++] = UserSettings.zoecounter;

                outputStreamHUBV2[counter++] = UserSettings.brightnesscounter;
                outputStreamHUBV2[counter++] = UserSettings.methodcounter;
                outputStreamHUBV2[counter++] = UserSettings.speedcounter;

                outputStreamHUBV2[counter++] = UserSettings.effectcounter;
                outputStreamHUBV2[counter++] = UserSettings.sincounter;
                outputStreamHUBV2[counter++] = UserSettings.lightstatus;


                outputStreamHUBV2[counter++] = UserSettings.fanbrightnesscounter;
                outputStreamHUBV2[counter++] = Convert.ToByte(ComPortSetup.volume);
                outputStreamHUBV2[counter++] = UserSettings.huecounter;

                outputStreamHUBV2[counter++] = UserSettings.color1.R;
                outputStreamHUBV2[counter++] = UserSettings.color1.G;
                outputStreamHUBV2[counter++] = UserSettings.color1.B;

                outputStreamHUBV2[counter++] = UserSettings.color2.R;
                outputStreamHUBV2[counter++] = UserSettings.color2.G;
                outputStreamHUBV2[counter++] = UserSettings.color2.B;

                outputStreamHUBV2[counter++] = UserSettings.color3.R;
                outputStreamHUBV2[counter++] = UserSettings.color3.G;
                outputStreamHUBV2[counter++] = UserSettings.color3.B;

                outputStreamHUBV2[counter++] = UserSettings.color4.R;
                outputStreamHUBV2[counter++] = UserSettings.color4.G;
                outputStreamHUBV2[counter++] = UserSettings.color4.B;

                outputStreamHUBV2[counter++] = UserSettings.color5.R;
                outputStreamHUBV2[counter++] = UserSettings.color5.G;
                outputStreamHUBV2[counter++] = UserSettings.color5.B;

                outputStreamHUBV2[counter++] = UserSettings.color6.R;
                outputStreamHUBV2[counter++] = UserSettings.color6.G;
                outputStreamHUBV2[counter++] = UserSettings.color6.B;

                outputStreamHUBV2[counter++] = UserSettings.color7.R;
                outputStreamHUBV2[counter++] = UserSettings.color7.G;
                outputStreamHUBV2[counter++] = UserSettings.color7.B;

                outputStreamHUBV2[counter++] = UserSettings.color8.R;
                outputStreamHUBV2[counter++] = UserSettings.color8.G;
                outputStreamHUBV2[counter++] = UserSettings.color8.B;

              



                outputStreamHUBV2[counter++] = Convert.ToByte((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2);
                outputStreamHUBV2[counter++] = Convert.ToByte(UserSettings.music);
                outputStreamHUBV2[counter++] = UserSettings.visualcounter;

                outputStreamHUBV2[counter++] = Convert.ToByte((UserSettings.SpotsX2 - 1) * 2 + (UserSettings.SpotsY2 - 1) * 2);
                outputStreamHUBV2[counter++] = UserSettings.edgebrightnesscounter;
                outputStreamHUBV2[counter++] = UserSettings.edgehuecounter;

                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[0];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[1];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[2];

                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[3];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[4];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[5];

                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[6];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[7];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[8];

                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[9];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[10];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[11];

                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[12];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[13];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[14];

                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[15];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[15];
                outputStreamHUBV2[counter++] = ComPortSetup.output_spectrumdata[15];


                outputStreamHUBV2[counter++] = UserSettings.screeneffectcounter;
                outputStreamHUBV2[counter++] = UserSettings.Faneffectcounter;
                outputStreamHUBV2[counter++] = UserSettings.screeneffectcounter;

                outputStreamHUBV2[counter++] = UserSettings.CaseStatic.R;
                outputStreamHUBV2[counter++] = UserSettings.CaseStatic.G;
                outputStreamHUBV2[counter++] = UserSettings.CaseStatic.B;

                outputStreamHUBV2[counter++] = UserSettings.ScreenStatic.R;
                outputStreamHUBV2[counter++] = UserSettings.ScreenStatic.G;
                outputStreamHUBV2[counter++] = UserSettings.ScreenStatic.B;

                outputStreamHUBV2[counter++] = UserSettings.DeskStatic.R;
                outputStreamHUBV2[counter++] = UserSettings.DeskStatic.G;
                outputStreamHUBV2[counter++] = UserSettings.DeskStatic.B;




                var allBlack = true;
                foreach (Spot spot in SpotSet.Spots)
                {
                    for (int i = 0; i < UserSettings.LedsPerSpot; i++)
                    {
                        if (!UserSettings.SendRandomColors)
                        {
                            outputStreamHUBV2[counter++] = spot.Blue; // blue
                            outputStreamHUBV2[counter++] = spot.Green; // green
                            outputStreamHUBV2[counter++] = spot.Red; // red

                            allBlack = allBlack && spot.Red == 0 && spot.Green == 0 && spot.Blue == 0;

                        }

                        else
                        {
                            allBlack = false;
                            var n = UserSettings.zoecounter;
                            var m = UserSettings.brightnesscounter;
                           
                            if (UserSettings.fixedcolor)
                            {
                                if (n == 255)
                                {
                                    outputStreamHUBV2[counter++] = 255; // blue
                                    outputStreamHUBV2[counter++] = 255; // green
                                    outputStreamHUBV2[counter++] = 255; // red

                                }
                                else
                                {
                                    var c = ColorUtil.FromAhsb(255, n, 1, 0.5f);
                                    outputStreamHUBV2[counter++] = c.B; // blue
                                    outputStreamHUBV2[counter++] = c.G; // green
                                    outputStreamHUBV2[counter++] = c.R; // red
                                }
                            }















                        }
                    }

                }
                if (Screen.AllScreens.Length == 2)
                {
                    foreach (Spot spot2 in SpotSet2.Spots2)
                    {
                        for (int i = 0; i < UserSettings.LedsPerSpot; i++)
                        {
                            if (!UserSettings.SendRandomColors)
                            {
                                outputStreamHUBV2[counter++] = spot2.Blue; // blue
                                outputStreamHUBV2[counter++] = spot2.Green; // green
                                outputStreamHUBV2[counter++] = spot2.Red; // red

                                allBlack = allBlack && spot2.Red == 0 && spot2.Green == 0 && spot2.Blue == 0;

                            }

                            else
                            {
                                allBlack = false;
                                var n = UserSettings.zoecounter;
                                var m = UserSettings.brightnesscounter;
                               
                                if (UserSettings.fixedcolor)
                                {
                                    if (n == 255)
                                    {
                                        outputStreamHUBV2[counter++] = 255; // blue
                                        outputStreamHUBV2[counter++] = 255; // green
                                        outputStreamHUBV2[counter++] = 255; // red

                                    }
                                    else
                                    {
                                        var c = ColorUtil.FromAhsb(255, n, 1, 0.5f);
                                        outputStreamHUBV2[counter++] = c.B; // blue
                                        outputStreamHUBV2[counter++] = c.G; // green
                                        outputStreamHUBV2[counter++] = c.R; // red
                                    }
                                }















                            }
                        }

                    }
                }
                else
                {
                    for (int i = 0; i < (UserSettings.SpotsX2 + UserSettings.SpotsY2 - 2) * 2; i++)
                    {
                        outputStreamHUBV2[counter++] = 0; // blue
                        outputStreamHUBV2[counter++] = 0; // green
                        outputStreamHUBV2[counter++] = 0; // red
                    }
                }

                if (allBlack)
                {
                    blackFrameCounter++;
                }

                return (outputStreamHUBV2, bufferHUBV2Length);
            }
        }

        private (byte[] Buffer, int OutputLength) GetOutputStreambut()
        {
            byte[] outputStreambut;

            int counter = _messagePreamble.Length;
            lock (SpotSet.Lock)
            {
                const int colorsPerLed = 3;
                int bufferbutLength = _messagePreamble.Length
                    + _messagePostamble.Length + 9 + 27+3;
                if (UserSettings.effect && UserSettings.SendRandomColors)
                {
                    outputStreambut = ArrayPool<byte>.Shared.Rent(bufferbutLength);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStreambut, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messageZoeamble, 0, outputStreambut, bufferbutLength - _messageZoeamble.Length, _messageZoeamble.Length);
                }
                else
                {

                    outputStreambut = ArrayPool<byte>.Shared.Rent(bufferbutLength);

                    Buffer.BlockCopy(_messagePreamble, 0, outputStreambut, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messagePostamble, 0, outputStreambut, bufferbutLength - _messagePostamble.Length, _messagePostamble.Length);
                }






                outputStreambut[counter++] = UserSettings.buteffectcounter;



                outputStreambut[counter++] = UserSettings.methodcounter;



                outputStreambut[counter++] = UserSettings.speedcounter;

                outputStreambut[counter++] = UserSettings.effectcounter;
                outputStreambut[counter++] = UserSettings.sincounter;
                outputStreambut[counter++] = UserSettings.holdeffectcounter;


                





                outputStreambut[counter++] = UserSettings.color1.R;
                outputStreambut[counter++] = UserSettings.color1.G;
                outputStreambut[counter++] = UserSettings.color1.B;

                outputStreambut[counter++] = UserSettings.color2.R;
                outputStreambut[counter++] = UserSettings.color2.G;
                outputStreambut[counter++] = UserSettings.color2.B;

                outputStreambut[counter++] = UserSettings.color3.R;
                outputStreambut[counter++] = UserSettings.color3.G;
                outputStreambut[counter++] = UserSettings.color3.B;

                outputStreambut[counter++] = UserSettings.color4.R;
                outputStreambut[counter++] = UserSettings.color4.G;
                outputStreambut[counter++] = UserSettings.color4.B;

                outputStreambut[counter++] = UserSettings.color5.R;
                outputStreambut[counter++] = UserSettings.color5.G;
                outputStreambut[counter++] = UserSettings.color5.B;

                outputStreambut[counter++] = UserSettings.color6.R;
                outputStreambut[counter++] = UserSettings.color6.G;
                outputStreambut[counter++] = UserSettings.color6.B;

                outputStreambut[counter++] = UserSettings.color7.R;
                outputStreambut[counter++] = UserSettings.color7.G;
                outputStreambut[counter++] = UserSettings.color7.B;

                outputStreambut[counter++] = UserSettings.color8.R;
                outputStreambut[counter++] = UserSettings.color8.G;
                outputStreambut[counter++] = UserSettings.color8.B;

                outputStreambut[counter++] = UserSettings.color10.R;
                outputStreambut[counter++] = UserSettings.color10.G;
                outputStreambut[counter++] = UserSettings.color10.B;

                outputStreambut[counter++] = Convert.ToByte(ComPortSetup.volume);
                outputStreambut[counter++] = Convert.ToByte(UserSettings.music);
                outputStreambut[counter++] = Convert.ToByte(UserSettings.music);

























                return (outputStreambut, bufferbutLength);
            }
        }
       private (byte[] Buffer, int OutputLength) GetSecondOutputStream()
        {
            byte[] secondoutputstream;

            int counter = _messagePreamble.Length;
            lock (SpotSet2.Lock2)
            {
                const int colorsPerLed = 3;
                int secondbufferLength = _messagePreamble.Length
                    + (UserSettings.LedsPerSpot * SpotSet2.Spots2.Length * colorsPerLed)
                    + _messagePostamble.Length + 9 + 27 + 3 + 6 + 21 + 9;
                if (UserSettings.effect && UserSettings.SendRandomColors)
                {
                    secondoutputstream = ArrayPool<byte>.Shared.Rent(secondbufferLength);

                    Buffer.BlockCopy(_messagePreamble, 0, secondoutputstream, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messageZoeamble, 0, secondoutputstream, secondbufferLength - _messageZoeamble.Length, _messageZoeamble.Length);
                }
                else
                {

                    secondoutputstream = ArrayPool<byte>.Shared.Rent(secondbufferLength);

                    Buffer.BlockCopy(_messagePreamble, 0, secondoutputstream, 0, _messagePreamble.Length);
                    Buffer.BlockCopy(_messagePostamble, 0, secondoutputstream, secondbufferLength - _messagePostamble.Length, _messagePostamble.Length);
                }




                /*

                secondoutputstream[counter++] = UserSettings.brightnesscounter;



                secondoutputstream[counter++] = UserSettings.methodcounter;



                secondoutputstream[counter++] = UserSettings.speedcounter;

                secondoutputstream[counter++] = UserSettings.effectcounter;
                secondoutputstream[counter++] = UserSettings.sincounter;
                secondoutputstream[counter++] = UserSettings.lightstatus;


                secondoutputstream[counter++] = UserSettings.edgebrightnesscounter;

                secondoutputstream[counter++] = Convert.ToByte(ComPortSetup.volume);


                secondoutputstream[counter++] = UserSettings.huecounter;





                secondoutputstream[counter++] = UserSettings.color1.R;
                secondoutputstream[counter++] = UserSettings.color1.G;
                secondoutputstream[counter++] = UserSettings.color1.B;

                secondoutputstream[counter++] = UserSettings.color2.R;
                secondoutputstream[counter++] = UserSettings.color2.G;
                secondoutputstream[counter++] = UserSettings.color2.B;

                secondoutputstream[counter++] = UserSettings.color3.R;
                secondoutputstream[counter++] = UserSettings.color3.G;
                secondoutputstream[counter++] = UserSettings.color3.B;

                secondoutputstream[counter++] = UserSettings.color4.R;
                secondoutputstream[counter++] = UserSettings.color4.G;
                secondoutputstream[counter++] = UserSettings.color4.B;

                secondoutputstream[counter++] = UserSettings.color5.R;
                secondoutputstream[counter++] = UserSettings.color5.G;
                secondoutputstream[counter++] = UserSettings.color5.B;

                secondoutputstream[counter++] = UserSettings.color6.R;
                secondoutputstream[counter++] = UserSettings.color6.G;
                secondoutputstream[counter++] = UserSettings.color6.B;

                secondoutputstream[counter++] = UserSettings.color7.R;
                secondoutputstream[counter++] = UserSettings.color7.G;
                secondoutputstream[counter++] = UserSettings.color7.B;

                secondoutputstream[counter++] = UserSettings.color8.R;
                secondoutputstream[counter++] = UserSettings.color8.G;
                secondoutputstream[counter++] = UserSettings.color8.B;

                secondoutputstream[counter++] = Convert.ToByte((UserSettings.SpotsX2 - 1) * 2 + (UserSettings.SpotsY2 - 1) * 2);
                secondoutputstream[counter++] = Convert.ToByte(UserSettings.music);
                secondoutputstream[counter++] = UserSettings.visualcounter;
                */

                secondoutputstream[counter++] = UserSettings.zone1speedcounter;
                secondoutputstream[counter++] = UserSettings.zone2speedcounter;
                secondoutputstream[counter++] = UserSettings.zone3speedcounter;
                if (ComPortSetup.DFU == 1)
                {
                    secondoutputstream[counter++] = 99;
                }
                else
                {
                    secondoutputstream[counter++] = UserSettings.fanmodecounter;
                }

                secondoutputstream[counter++] = UserSettings.LEDfanmodecounter;
                secondoutputstream[counter++] = UserSettings.zoecounter;

                secondoutputstream[counter++] = UserSettings.brightnesscounter;
                secondoutputstream[counter++] = UserSettings.methodcounter;
                secondoutputstream[counter++] = UserSettings.speedcounter;

                secondoutputstream[counter++] = UserSettings.effectcounter;
                secondoutputstream[counter++] = UserSettings.sincounter;
                secondoutputstream[counter++] = UserSettings.lightstatus;


                secondoutputstream[counter++] = UserSettings.fanbrightnesscounter;
                secondoutputstream[counter++] = Convert.ToByte(ComPortSetup.volume);
                secondoutputstream[counter++] = UserSettings.huecounter;

                secondoutputstream[counter++] = UserSettings.color1.R;
                secondoutputstream[counter++] = UserSettings.color1.G;
                secondoutputstream[counter++] = UserSettings.color1.B;

                secondoutputstream[counter++] = UserSettings.color2.R;
                secondoutputstream[counter++] = UserSettings.color2.G;
                secondoutputstream[counter++] = UserSettings.color2.B;

                secondoutputstream[counter++] = UserSettings.color3.R;
                secondoutputstream[counter++] = UserSettings.color3.G;
                secondoutputstream[counter++] = UserSettings.color3.B;

                secondoutputstream[counter++] = UserSettings.color4.R;
                secondoutputstream[counter++] = UserSettings.color4.G;
                secondoutputstream[counter++] = UserSettings.color4.B;

                secondoutputstream[counter++] = UserSettings.color5.R;
                secondoutputstream[counter++] = UserSettings.color5.G;
                secondoutputstream[counter++] = UserSettings.color5.B;

                secondoutputstream[counter++] = UserSettings.color6.R;
                secondoutputstream[counter++] = UserSettings.color6.G;
                secondoutputstream[counter++] = UserSettings.color6.B;

                secondoutputstream[counter++] = UserSettings.color7.R;
                secondoutputstream[counter++] = UserSettings.color7.G;
                secondoutputstream[counter++] = UserSettings.color7.B;

                secondoutputstream[counter++] = UserSettings.color8.R;
                secondoutputstream[counter++] = UserSettings.color8.G;
                secondoutputstream[counter++] = UserSettings.color8.B;



                

                secondoutputstream[counter++] = Convert.ToByte((UserSettings.SpotsX2 - 1) * 2 + (UserSettings.SpotsY2 - 1) * 2);
                secondoutputstream[counter++] = Convert.ToByte(UserSettings.music);
                secondoutputstream[counter++] = UserSettings.visualcounter;

                secondoutputstream[counter++] = Convert.ToByte((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2);
                secondoutputstream[counter++] = UserSettings.edgebrightnesscounter;
                secondoutputstream[counter++] = UserSettings.edgehuecounter;

                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[0];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[1];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[2];

                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[3];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[4];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[5];

                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[6];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[7];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[8];

                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[9];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[10];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[11];

                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[12];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[13];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[14];

                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[15];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[15];
                secondoutputstream[counter++] = ComPortSetup.output_spectrumdata[15];


                secondoutputstream[counter++] = UserSettings.screeneffectcounter;
                secondoutputstream[counter++] = UserSettings.Faneffectcounter;
                secondoutputstream[counter++] = UserSettings.screeneffectcounter;

                secondoutputstream[counter++] = UserSettings.CaseStatic.R;
                secondoutputstream[counter++] = UserSettings.CaseStatic.G;
                secondoutputstream[counter++] = UserSettings.CaseStatic.B;

                secondoutputstream[counter++] = UserSettings.ScreenStatic.R;
                secondoutputstream[counter++] = UserSettings.ScreenStatic.G;
                secondoutputstream[counter++] = UserSettings.ScreenStatic.B;

                secondoutputstream[counter++] = UserSettings.DeskStatic.R;
                secondoutputstream[counter++] = UserSettings.DeskStatic.G;
                secondoutputstream[counter++] = UserSettings.DeskStatic.B;

                var allBlack = true;
                foreach (Spot spot2 in SpotSet2.Spots2)
                {
                    for (int i = 0; i < UserSettings.LedsPerSpot; i++)
                    {
                        if (!UserSettings.SendRandomColors)
                        {
                            secondoutputstream[counter++] = spot2.Blue; // blue
                            secondoutputstream[counter++] = spot2.Green; // green
                            secondoutputstream[counter++] = spot2.Red; // red

                            allBlack = allBlack && spot2.Red == 0 && spot2.Green == 0 && spot2.Blue == 0;

                        }

                        else
                        {
                            allBlack = false;
                            var n = UserSettings.zoecounter;
                            var m = UserSettings.brightnesscounter;
                          
                            if (UserSettings.fixedcolor)
                            {
                                if (n == 255)
                                {
                                    secondoutputstream[counter++] = 255; // blue
                                    secondoutputstream[counter++] = 255; // green
                                    secondoutputstream[counter++] = 255; // red

                                }
                                else
                                {
                                    var c = ColorUtil.FromAhsb(255, n, 1, 0.5f);
                                    secondoutputstream[counter++] = c.B; // blue
                                    secondoutputstream[counter++] = c.G; // green
                                    secondoutputstream[counter++] = c.R; // red
                                }
                            }















                        }
                    }

                }



                if (allBlack)
                {
                    blackFrameCounter++;
                }

                return (secondoutputstream, secondbufferLength);
            }
        }
        
       

        private void DoWork(object tokenObject)
            {
                var cancellationToken = (CancellationToken)tokenObject;
                ISerialPortWrapper serialPort = null;
                ISerialPortWrapper serialPort2 = null;
                ISerialPortWrapper serialPort3 = null;
                ISerialPortWrapper serialPort4 = null;
                ISerialPortWrapper serialPort5 = null;

            if (String.IsNullOrEmpty(UserSettings.ComPort))
                {
                    _log.Warn("Cannot start the serial sending because the comport is not selected.");
                    return;
                }

                frameCounter = 0;
                blackFrameCounter = 0;

                //retry after exceptions...
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                    const int baudRate = 1000000;
                    string openedComPort = null;
                    string openedComPort2 = null;
                    string openedComPort3 = null;
                    string openedComPort4 = null;
                    string openedComPort5 = null;


                    while (!cancellationToken.IsCancellationRequested)
                        {
                        //open or change the serial port
                        if (openedComPort != UserSettings.ComPort || openedComPort2 != UserSettings.ComPort2 || openedComPort3 != UserSettings.ComPort3 || openedComPort4 != UserSettings.ComPort4|| openedComPort5 != UserSettings.ComPort5)
                        {
                            serialPort?.Close();
                            serialPort = UserSettings.ComPort != "Không có" ? (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.ComPort, baudRate)) : new FakeSerialPort();
                            serialPort.Open();
                            openedComPort = UserSettings.ComPort;
                            

                            if ( openedComPort2 != UserSettings.ComPort2)
                            {

                                serialPort2?.Close();
                                serialPort2 = UserSettings.ComPort2 != "Không có" ? (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.ComPort2, baudRate)) : new FakeSerialPort();
                                serialPort2.Open();
                                openedComPort2 = UserSettings.ComPort2;
                            }
                            if (openedComPort3 != UserSettings.ComPort3)
                            {

                                serialPort3?.Close();
                                serialPort3 = UserSettings.ComPort3 != "Không có" ? (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.ComPort3, baudRate)) : new FakeSerialPort();
                                serialPort3.Open();
                                openedComPort3 = UserSettings.ComPort3;
                            }
                            if (openedComPort4 != UserSettings.ComPort4)
                            {

                                serialPort4?.Close();
                                serialPort4 = UserSettings.ComPort4 != "Không có" ? (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.ComPort4, baudRate)) : new FakeSerialPort();
                                serialPort4.Open();
                                openedComPort4 = UserSettings.ComPort4;
                            }
                            if (openedComPort5 != UserSettings.ComPort5)
                            {

                                serialPort5?.Close();
                                serialPort5 = UserSettings.ComPort5!= "Không có" ? (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.ComPort5, baudRate)) : new FakeSerialPort();
                                serialPort5.Open();
                                openedComPort5 = UserSettings.ComPort5;
                            }

                        }

                            //send frame data
                            var (outputBuffer, streamLength) = GetOutputStream();
                        var (outputbutBuffer, streambutLength) = GetOutputStreambut();
                        var (secondoutputstream, secondbufferLength) = GetOutputStream();
                       // var (HUBoutputstream, HUBbufferLength) = GetOutputStreamHUB();
                        var (HUBV2outputstream, HUBV2bufferLength) = GetOutputStreamHUBV2();
                        if (Screen.AllScreens.Length == 2)
                        {
                             (secondoutputstream, secondbufferLength) = GetSecondOutputStream();
                             
                        }
                       
                       
                        if (UserSettings.ComportOpen)

                        {
                            serialPort.Write(outputbutBuffer, 0, streambutLength);
                            if(serialPort.BytesToRead>0)
                            {
                                Int32 inputvalue;
                                inputvalue = serialPort.ReadByte();
                                UserSettings.effectcounter = Convert.ToByte(inputvalue);

                            }
                           
                        }
                        if (UserSettings.Comport2Open && Screen.AllScreens.Length == 2)
                        {
                            serialPort2.Write(secondoutputstream,0, secondbufferLength);
                        }
                        if (UserSettings.Comport3Open)
                        {
                            serialPort3.Write(outputBuffer, 0, streamLength);
                        }
                       // if (UserSettings.Comport4Open)
                       // {
                          //  if (Screen.AllScreens.Length == 2)
                          //  serialPort4.Write(HUBoutputstream, 0, HUBbufferLength);
                           // else serialPort4.Write(outputBuffer, 0, streamLength);
                       // }

                        if (UserSettings.Comport5Open)
                        {
                            //  if (Screen.AllScreens.Length == 2)
                            serialPort5.Write(HUBV2outputstream, 0, HUBV2bufferLength);
                            // else serialPort4.Write(outputBuffer, 0, streamLength);
                        }

                        if (++frameCounter == 1024 && blackFrameCounter > 1000)
                            {
                                //there is maybe something wrong here because most frames where black. report it once per run only
                                var settingsJson = JsonConvert.SerializeObject(UserSettings, Formatting.None);
                                _log.Info($"Sent {frameCounter} frames already. {blackFrameCounter} were completely black. Settings= {settingsJson}");
                            }
                            ArrayPool<byte>.Shared.Return(outputBuffer);

                            //ws2812b LEDs need 30 µs = 0.030 ms for each led to set its color so there is a lower minimum to the allowed refresh rate
                            //receiving over serial takes it time as well and the arduino does both tasks in sequence
                            //+1 ms extra safe zone
                            var fastLedTime = ((streamLength - _messagePreamble.Length - _messagePostamble.Length) / 3.0 * 0.030d)+256*0.030d;
                            var serialTransferTime = HUBV2bufferLength * 10*1000 / baudRate;
                            var minTimespan = (int)(fastLedTime + serialTransferTime) + 6;

                            Thread.Sleep(minTimespan);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _log.Debug("OperationCanceledException catched. returning.");

                        return;
                    }
                    catch (Exception ex)
                    {
                        _log.Debug(ex, "Exception catched.");
                        //to be safe, we reset the serial port
                        if (serialPort != null && serialPort.IsOpen)
                        {
                            serialPort.Close();
                        }
                        serialPort?.Dispose();
                        serialPort = null;

                        //allow the system some time to recover
                        Thread.Sleep(500);
                    }
                    finally
                    {
                        if (serialPort != null && serialPort.IsOpen)
                        {
                            serialPort.Close();
                            serialPort.Dispose();
                        }
                    if (serialPort2 != null && serialPort2.IsOpen)
                    {
                        serialPort2.Close();
                        serialPort2.Dispose();
                    }
                    if (serialPort3 != null && serialPort3.IsOpen)
                    {
                        serialPort3.Close();
                        serialPort3.Dispose();
                    }
                    if (serialPort4 != null && serialPort4.IsOpen)
                    {
                        serialPort4.Close();
                        serialPort4.Dispose();
                    }
                    if (serialPort5 != null && serialPort5.IsOpen)
                    {
                        serialPort5.Close();
                        serialPort5.Dispose();
                    }
                }
                }

            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Stop();
                }
            }
        }
    }
  