using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using Castle.Core.Logging;
using NLog;
using adrilight.ViewModel;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using System.Runtime.InteropServices;
using System.Drawing;
using BO;

namespace adrilight.Util
{
    internal class Gifxelation : IGifxelation, IDisposable
    {


        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();
        private Thread _workerThread;

        private static WriteableBitmap MatrixBitmap { get; set; }
        public Gifxelation(IDeviceSettings device, ISpotSet spotSet, MainViewViewModel viewModel, SettingInfoDTO setting)
        {
            deviceInfo = device ?? throw new ArgumentNullException(nameof(device));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            settingInfo = setting ?? throw new ArgumentNullException(nameof(setting));
            deviceInfo.PropertyChanged += PropertyChanged;
            settingInfo.PropertyChanged += SettingInfo_PropertyChanged;
            MatrixFrame.DimensionsChanged += OnMatrixDimensionsChanged;
           // MatrixFrame.FrameChanged += OnFrameChanged;
            MatrixFrame.SetDimensions(MatrixFrame.Width, MatrixFrame.Height);
            //gifxelation load default gif//



            ImageProcesser.DisposeGif();
            ImageProcesser.DisposeStill();
            if(!ImageProcesser.LoadGifFromDisk(deviceInfo.GifFilePath))
            {
                ImageProcesser.LoadGifFromResource();
            }
            




            //settingsViewModel.ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
            MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
            //EndX.Maximum = ImageProcesser.LoadedGifImage.Width;
            //startX.Maximum = ImageProcesser.LoadedGifImage.Width;
            //EndX.Value = ImageProcesser.LoadedGifImage.Width;
            //EndY.Maximum = ImageProcesser.LoadedGifImage.Height;
            //StartY.Maximum = ImageProcesser.LoadedGifImage.Height;
            //EndY.Value = ImageProcesser.LoadedGifImage.Height;
            //startX.Value = 0;
            //StartY.Value = 0;
            ImageProcesser.ImageRect = new System.Drawing.Rectangle(Convert.ToInt32(0), Convert.ToInt32(0), Convert.ToInt32(ImageProcesser.LoadedGifImage.Width ), Convert.ToInt32(ImageProcesser.LoadedGifImage.Height));
           // FrameToPreview();
            //SerialManager.PushFrame();
            ImageProcesser.ImageLoadState = ImageProcesser.LoadState.Gif;
            RefreshColorState();
            _log.Info($"Static Color Created");

        }


        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(settingInfo.TransferActive):
                case nameof(deviceInfo.StaticColor):
                case nameof(deviceInfo.SelectedEffect):
                case nameof(deviceInfo.GifFilePath):
                    RefreshColorState();
                    break;

            }
        }
        private void SettingInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private IDeviceSettings deviceInfo { get; }
        private MainViewViewModel ViewModel { get; }
        private SettingInfoDTO settingInfo { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private void RefreshColorState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = settingInfo.TransferActive && deviceInfo.SelectedEffect == 5;
            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the StaticColor");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }


            else if (!isRunning && shouldBeRunning)
            {
                
                //start it
                _log.Debug("starting the StaticColor");
                _cancellationTokenSource = new CancellationTokenSource();
                _workerThread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "StaticColorCreator"
                };
                _workerThread.Start();
            }
            else if (isRunning && shouldBeRunning)//refresh called
            {
                ImageProcesser.DisposeWorkingBitmap();
                ImageProcesser.DisposeGif();
                //start it
               IsRunning = false;
                _log.Debug("starting the StaticColor");
                _cancellationTokenSource = new CancellationTokenSource();
                _workerThread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "StaticColorCreator"
                };
                _workerThread.Start();
            }
        }

        private ISpotSet SpotSet { get; }
        

        public void Run(CancellationToken token)//static color creator
        {
            if (IsRunning) throw new Exception(" Static Color is already running!");

            IsRunning = true;

            _log.Debug("Started Static Color.");


            MatrixBitmap = new WriteableBitmap(MatrixFrame.Width, MatrixFrame.Height, 96, 96, PixelFormats.Bgr32, null);
            

            try
            {
                // BitmapData bitmapData = new BitmapData();
                //  BitmapData bitmapData2 = new BitmapData();
                //  int colorcount = 0;

                int _gifFrameIndex = 0;
                while (!token.IsCancellationRequested)
                {
                    //var numLED = (UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2;
                    //Color currentStaticColor = UserSettings.StaticColor;
                    //var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                    //double peekBrightness = 0.0;

                    bool isPreviewRunning = (deviceInfo.SelectedEffect ==5);
                    //bool isBreathing = UserSettings.Breathing;
                    lock (SpotSet.Lock)
                    {
                        
                        

                        if (_gifFrameIndex >= ImageProcesser.LoadedGifFrameCount - 1)
                                _gifFrameIndex = 0;
                            else
                                _gifFrameIndex++;
                            //Console.WriteLine(_gifFrameIndex);
                            ImageProcesser.LoadedGifImage.SelectActiveFrame(ImageProcesser.LoadedGifFrameDim, _gifFrameIndex);
                            ImageProcesser.WorkingBitmap = ImageProcesser.CropBitmap(new Bitmap(ImageProcesser.LoadedGifImage), ImageProcesser.ImageRect);
                            
                            MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
                        
                            ImageProcesser.DisposeWorkingBitmap();

                       
                        int counter = 0;
                        foreach (ISpot spot in SpotSet.Spots)
                        {
                            //colorOutput[counter] = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(currentStaticColor.R, currentStaticColor.G, currentStaticColor.B), peekBrightness);
                            spot.SetColor(MatrixFrame.Frame[counter].R, MatrixFrame.Frame[counter].G, MatrixFrame.Frame[counter].B, true);
                            counter++;


                        }
                        counter = 0;
                        foreach (ISpot spot in SpotSet.Spots2)
                        {
                            //colorOutput[counter] = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(currentStaticColor.R, currentStaticColor.G, currentStaticColor.B), peekBrightness);
                            spot.SetColor(MatrixFrame.Frame[counter].R, MatrixFrame.Frame[counter].G, MatrixFrame.Frame[counter].B, true);
                            counter++;


                        }


                        if (isPreviewRunning)
                        {
                            //copy all color data to the preview
                            var needsNewArray = ViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                           // ViewModel.PreviewSpots = SpotSet.Spots;

                            ViewModel.PreviewGif = SpotSet.Spots2;
                        }
                       
                        Thread.Sleep(33);
                    }







                }
                //motion speed

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
                //if (currentStaticColor == null)
                //{
                //    for (int i = 0; i < numLED; i++) //fill all LED with default static color
                //    {
                //        paletteOutput[i] = defaultColor;
                //    }



                //allow the system some time to recover
                Thread.Sleep(500);
            }
            finally
            {

                ImageProcesser.DisposeWorkingBitmap();
                ImageProcesser.DisposeGif();
                _log.Debug("Stopped Static Color Creator.");
                IsRunning = false;
            }

        }

        private void OnMatrixDimensionsChanged()
        {
            MatrixBitmap = new WriteableBitmap(MatrixFrame.Width, MatrixFrame.Height, 96, 96, PixelFormats.Bgr32, null);

           // MatrixImage.Source = MatrixBitmap;


        }
        private void OnFrameChanged()
        {
            Application.Current.Dispatcher.Invoke(() => { FrameToPreview(); });
        }
        private void FrameToPreview()
        {
            MatrixBitmap.Lock();
            IntPtr pixelAddress = MatrixBitmap.BackBuffer;

            Marshal.Copy(MatrixFrame.FrameToInt32(), 0, pixelAddress, (MatrixFrame.Width * MatrixFrame.Height));

            MatrixBitmap.AddDirtyRect(new Int32Rect(0, 0, MatrixFrame.Width, MatrixFrame.Height));
            MatrixBitmap.Unlock();
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
        public void Stop()
        {
            _log.Debug("Stop called.");
            if (_workerThread == null) return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            _workerThread?.Join();
            _workerThread = null;
        }
    }
}
