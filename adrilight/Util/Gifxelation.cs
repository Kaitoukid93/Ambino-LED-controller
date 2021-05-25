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

namespace adrilight.Util
{
    internal class Gifxelation : IGifxelation
    {


        private readonly NLog.ILogger _log = LogManager.GetCurrentClassLogger();

        private static int _gifFrameIndex = 0;
        private static WriteableBitmap MatrixBitmap { get; set; }
        public Gifxelation(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            SettingsViewModel.PropertyChanged += PropertyChanged;
            UserSettings.PropertyChanged += PropertyChanged;
            MatrixFrame.DimensionsChanged += OnMatrixDimensionsChanged;
           // MatrixFrame.FrameChanged += OnFrameChanged;
            MatrixFrame.SetDimensions(MatrixFrame.Width, MatrixFrame.Height);
            //gifxelation load default gif//



            ImageProcesser.DisposeGif();
            ImageProcesser.DisposeStill();
            ImageProcesser.LoadGifFromResource();




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
                case nameof(UserSettings.TransferActive):
                case nameof(UserSettings.StaticColor):
                case nameof(UserSettings.SelectedEffect):
                    RefreshColorState();
                    break;

            }
        }
        private IUserSettings UserSettings { get; }
        private SettingsViewModel SettingsViewModel { get; }
        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;
        private void RefreshColorState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.SelectedEffect == 5;
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
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "StaticColorCreator"
                };
                thread.Start();
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


                while (!token.IsCancellationRequested)
                {
                    //var numLED = (UserSettings.SpotsX - 1) * 2 + (UserSettings.SpotsY - 1) * 2;
                    //Color currentStaticColor = UserSettings.StaticColor;
                    //var colorOutput = new OpenRGB.NET.Models.Color[numLED];
                    //double peekBrightness = 0.0;

                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 5);
                    //bool isBreathing = UserSettings.Breathing;
                    lock (SpotSet.Lock)
                    {
                        ImageProcesser.DisposeWorkingBitmap();

                        
                            if (_gifFrameIndex >= ImageProcesser.LoadedGifFrameCount - 1)
                                _gifFrameIndex = 0;
                            else
                                _gifFrameIndex++;
                            //Console.WriteLine(_gifFrameIndex);
                            ImageProcesser.LoadedGifImage.SelectActiveFrame(ImageProcesser.LoadedGifFrameDim, _gifFrameIndex);
                            ImageProcesser.WorkingBitmap = ImageProcesser.CropBitmap(new Bitmap(ImageProcesser.LoadedGifImage), ImageProcesser.ImageRect);
                            
                            MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
                        
                        ImageProcesser.DisposeWorkingBitmap();

                        //if (isBreathing)
                        //{
                        //    if (point < 500)
                        //    {
                        //        //peekBrightness = 1.0 - Math.Abs((2.0 * (point / 500)) - 1.0);
                        //        peekBrightness = Math.Exp(-(Math.Pow(((point / 500) - 0.5) / 0.14, 2.0)) / 2.0);
                        //        point++;
                        //    }
                        //    else
                        //    {
                        //        point = 0;
                        //    }



                        //}
                        //else
                        //{
                        //    peekBrightness = UserSettings.Brightness / 100.0;
                        //}

                        int counter = 0;
                        foreach (ISpot spot in SpotSet.Spots)
                        {
                            //colorOutput[counter] = Brightness.applyBrightness(new OpenRGB.NET.Models.Color(currentStaticColor.R, currentStaticColor.G, currentStaticColor.B), peekBrightness);
                            spot.SetColor(MatrixFrame.Frame[counter].R, MatrixFrame.Frame[counter].G, MatrixFrame.Frame[counter].B, true);
                            counter++;


                        }

                        if (isPreviewRunning)
                        {
                            //copy all color data to the preview
                            var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                           SettingsViewModel.PreviewSpots = SpotSet.Spots;
                           //SettingsViewModel.ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
                        }
                        Thread.Sleep(20);
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

    }
}
