using System;
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
using OpenRGB;
using adrilight.ViewModel;
using System.Collections.Generic;
using System.Windows;

namespace adrilight
{
    internal sealed class
        SerialStream : IDisposable, ISerialStream
    {
        private ILogger _log = LogManager.GetCurrentClassLogger();

        public SerialStream(IDeviceSettings userSettings, ISpotSet spotSet)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));


            UserSettings.PropertyChanged += UserSettings_PropertyChanged;
            // ScanSerialPort();
            RefreshTransferState();

            _log.Info($"SerialStream created.");

            //if (!IsValid())
            //{
            //    UserSettings.TransferActive = false;
            //    UserSettings.ComPort = null;
            //}
        }

        private bool CheckSerialPort(string serialport)
        {
            Stop();//stop current serial stream first to avoid access denied
                   // BlockedComport.Clear();
            var available = true;
            int TestbaudRate = 1000000;

            if (serialport != null)
            {
                if (serialport == "Không có")
                {
                    System.Windows.MessageBox.Show("Serial Port " + serialport + " is just for testing effects, not the real device, please note");
                    available = true;
                    return available;

                }
                var serialPorttest = (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(serialport, TestbaudRate));

                try
                {

                    serialPorttest.Open();

                }

                catch (Exception)
                {

                    // BlockedComport.Add(serialport);
                    _log.Debug("Serial Port " + serialport + " access denied, added to Blacklist");
                    System.Windows.MessageBox.Show("Serial Port " + serialport + " is in use or unavailable, Please chose another COM Port");
                    available = false;

                    //_log.Debug(ex, "Exception catched.");
                    //to be safe, we reset the serial port
                    //  MessageBox.Show("Serial Port " + UserSettings.ComPort + " is in use or unavailable, Please chose another COM Port");




                    //allow the system some time to recover

                    // Dispose();
                }
                serialPorttest.Close();

            }

            else
            {
                available = false;
            }

            return available;


        }
        private void UserSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserSettings.TransferActive):
                case nameof(UserSettings.DevicePort):
                case nameof(UserSettings.StaticColor):


                    RefreshTransferState();
                    break;
            }
        }

        public bool IsValid() => SerialPort.GetPortNames().Contains(UserSettings.DevicePort) || UserSettings.DevicePort == "Không có";
        // public bool IsAcess() => !BlockedComport.Contains(UserSettings.ComPort);
        // public IList<string> BlockedComport = new List<string>();

        private void RefreshTransferState()
        {

            if (UserSettings.TransferActive)
            {
                if (IsValid() && CheckSerialPort(UserSettings.DevicePort))
                {

                    //start it
                    _log.Debug("starting the serial stream for device Name : " + UserSettings.DeviceName);
                    Start();
                }
                else
                {
                    UserSettings.TransferActive = false;
                    UserSettings.DevicePort = null;
                }
            }

            else if (!UserSettings.TransferActive && IsRunning)
            {
                //stop it
                _log.Debug("stopping the serial stream");
                Stop();
            }
        }

        private readonly byte[] _messagePreamble = { (byte)'a', (byte)'b', (byte)'n' };
        private readonly byte[] _messagePostamble = { 15, 12, 93 };
        private readonly byte[] _messageZoeamble = { 15, 12, 93 };
        private readonly byte[] _commandmessage = { 15, 12, 93 };



        private Thread _workerThread;
        private CancellationTokenSource _cancellationTokenSource;


        private int frameCounter;
        private int blackFrameCounter;



        public void Start()
        {
            _log.Debug("Start called.");
            if (_workerThread != null) return;

            _cancellationTokenSource = new CancellationTokenSource();
            _workerThread = new Thread(DoWork) {
                Name = "Serial sending",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            WinApi.TimeBeginPeriod(1);

            // The call has failed

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

        private IDeviceSettings UserSettings { get; }
        private ISpotSet SpotSet { get; }





        private (byte[] Buffer, int OutputLength) GetOutputStream()
        {
            byte[] outputStream;

            int counter = _messagePreamble.Length;
            lock (SpotSet.Lock)
            {
                const int colorsPerLed = 3;
                int bufferLength = _messagePreamble.Length + 3
                    + (SpotSet.Spots.Length * colorsPerLed);


                outputStream = ArrayPool<byte>.Shared.Rent(bufferLength);

                Buffer.BlockCopy(_messagePreamble, 0, outputStream, 0, _messagePreamble.Length);




                ///device param///
                ///numleds/////đây là thiết bị dạng led màn hình có số led chiều dọc và chiều ngang, tổng số led sẽ là (dọc-1)*2+(ngang-1)*2///
                //////2 byte ngay tiếp sau Preamable là để ghép lại thành 1 số 16bit (vì số led có thể lớn hơn 255 nhiều) vi điều khiển sẽ dựa vào số led này để biết cần đọc bao nhiêu byte nữa///
                byte lo = (byte)(((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2) & 0xff);
                byte hi = (byte)((((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2) >> 8) & 0xff);
                outputStream[counter++] = hi;
                outputStream[counter++] = lo;

                ///byte tiếp theo ngay bên dưới sẽ là byte quy định trạng thái thiết bị/// 1 là sáng bình thường, 2 là chế độ đèn ngủ (sáng theo màu lưu sẵn) 3 là chế độ DFU (nạp code)///
                //if(devcheck==false)
                //{
                //    outputStream[counter++] = 0;
                //}
                //else
                //{
                outputStream[counter++] = 0;
                var allBlack = true;
                //}


                foreach (Spot spot in SpotSet.Spots)
                {

                    outputStream[counter++] = spot.Red; // blue
                    outputStream[counter++] = spot.Green; // green
                    outputStream[counter++] = spot.Blue; // red

                    allBlack = allBlack && spot.Red == 0 && spot.Green == 0 && spot.Blue == 0;







                }

                if (allBlack)
                {
                    blackFrameCounter++;
                }

                return (outputStream, bufferLength);
            }





        }
        private (byte[] Buffer, int OutputLength) GetOutputStreamSleep()
        {
            byte[] outputStream;

            int counter = _messagePreamble.Length;
            lock (SpotSet.Lock)
            {
                const int colorsPerLed = 3;
                int bufferLength = _messagePreamble.Length + 3
                    + (SpotSet.Spots.Length * colorsPerLed);


                outputStream = ArrayPool<byte>.Shared.Rent(bufferLength);

                Buffer.BlockCopy(_messagePreamble, 0, outputStream, 0, _messagePreamble.Length);




                ///device param///
                ///numleds/////đây là thiết bị dạng led màn hình có số led chiều dọc và chiều ngang, tổng số led sẽ là (dọc-1)*2+(ngang-1)*2///
                //////2 byte ngay tiếp sau Preamable là để ghép lại thành 1 số 16bit (vì số led có thể lớn hơn 255 nhiều) vi điều khiển sẽ dựa vào số led này để biết cần đọc bao nhiêu byte nữa///
                byte lo = (byte)(((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2) & 0xff);
                byte hi = (byte)((((UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2) >> 8) & 0xff);
                outputStream[counter++] = hi;
                outputStream[counter++] = lo;

                ///byte tiếp theo ngay bên dưới sẽ là byte quy định trạng thái thiết bị/// 1 là sáng bình thường, 2 là chế độ đèn ngủ (sáng theo màu lưu sẵn) 3 là chế độ DFU (nạp code)///
                //if(devcheck==false)
                //{
                //    outputStream[counter++] = 0;
                //}
                //else
                //{
                outputStream[counter++] = 0;
                var allBlack = true;
                //}

                int snapshotCounter = 0;
                foreach (Spot spot in SpotSet.Spots)
                {

                    outputStream[counter++] = UserSettings.SnapShot[snapshotCounter++]; // blue
                    outputStream[counter++] = UserSettings.SnapShot[snapshotCounter++]; // green
                    outputStream[counter++] = UserSettings.SnapShot[snapshotCounter++]; // red

                    allBlack = allBlack && spot.Red == 0 && spot.Green == 0 && spot.Blue == 0;







                }

                if (allBlack)
                {
                    blackFrameCounter++;
                }

                return (outputStream, bufferLength);
            }





        }

        private void DoWork(object tokenObject)
        {
            var cancellationToken = (CancellationToken)tokenObject;
            ISerialPortWrapper serialPort = null;


            if (String.IsNullOrEmpty(UserSettings.DevicePort))
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



                    while (!cancellationToken.IsCancellationRequested)
                    {
                        //open or change the serial port
                        if (openedComPort != UserSettings.DevicePort)
                        {
                            serialPort?.Close();
                            serialPort = UserSettings.DevicePort != "Không có" ? (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.DevicePort, baudRate)) : new FakeSerialPort();
                           // serialPort.DisableDtr();
                           // serialPort.DisableRts();
                            serialPort.Open();
                            openedComPort = UserSettings.DevicePort;

                        }
                        //send frame data
                        var (outputBuffer, streamLength) = GetOutputStream();






                        //if (LedOutsideCase.DFUVal == 1)
                        //{
                        //    serialPort?.Close();
                        //    serialPort = (ISerialPortWrapper)new WrappedSerialPort(new SerialPort(UserSettings.ComPort, 1200));
                        //    serialPort.Open();
                        //    serialPort.Close();

                        //}
                        // else
                        //{
                        serialPort.Write(outputBuffer, 0, streamLength);
                        //}






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
                        var fastLedTime = ((streamLength - _messagePreamble.Length - _messagePostamble.Length) / 3.0 * 0.030d);
                        var serialTransferTime = outputBuffer.Length * 10 * 1000 / baudRate;
                        var minTimespan = (int)(fastLedTime + serialTransferTime) + 1;

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
                    // MessageBox.Show("Serial Port " + UserSettings.ComPort + " is in use or unavailable, Please chose another COM Port");


                    if (serialPort != null && serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }
                    serialPort?.Dispose();

                    //allow the system some time to recover
                    Thread.Sleep(500);
                    // Dispose();
                }
                finally
                {
                    if (serialPort != null && serialPort.IsOpen)
                    {
                        //write last frame
                        // Thread.Sleep(500);
                        // serialPort.Close();
                        //Thread.Sleep(500);
                        // serialPort.Open();
                        var (outputBuffer, streamLength) = GetOutputStreamSleep();
                        serialPort.Write(outputBuffer, 0, streamLength);
                        serialPort.Write(outputBuffer, 0, streamLength);
                        serialPort.Write(outputBuffer, 0, streamLength);
                        serialPort.Write(outputBuffer, 0, streamLength);
                        serialPort.Write(outputBuffer, 0, streamLength);
                        serialPort.Write(outputBuffer, 0, streamLength);
                        _log.Debug("Last Frame Sent!");

                        serialPort.Close();
                        serialPort.Dispose();
                        _log.Debug("SerialPort Disposed!");
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












