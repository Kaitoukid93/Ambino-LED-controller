using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using adrilight.DesktopDuplication;
using NLog;
using Polly;
using System.Linq;
using System.Windows.Media.Imaging;
using adrilight.ViewModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace adrilight
{
    internal class DesktopDuplicatorReader : IDesktopDuplicatorReader
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public DesktopDuplicatorReader(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SpotSet2 = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
            SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryForever(ProvideDelayDuration);

            UserSettings.PropertyChanged += PropertyChanged;
            SettingsViewModel.PropertyChanged += PropertyChanged;
            RefreshCapturingState();
            //RefreshCapturingStateSecond();

            _log.Info($"DesktopDuplicatorReader created.");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserSettings.TransferActive):
                case nameof(UserSettings.IsPreviewEnabled):
                case nameof(UserSettings.CaptureActive):
                case nameof(SettingsViewModel.IsSettingsWindowOpen):
                case nameof(SettingsViewModel.IsPreviewTabOpen):
                
                    RefreshCapturingState();
                    break;
                case nameof(SettingsViewModel.IsPreviewTabOpenSecond):
                   // RefreshCapturingState();
                    break;
                    //RefreshCapturingStateSecond();
                    //break;
            }
        }

        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunningSecond { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSourceSecond;


        private void RefreshCapturingState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive;

            var shouldBeCapturing = UserSettings.CaptureActive || SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpen|| SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpenSecond;            

            if (isRunning && !shouldBeCapturing && shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the capturing");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }
            else if (!isRunning && shouldBeCapturing && shouldBeRunning)
            {
                //start it
                _log.Debug("starting the capturing");
                _cancellationTokenSource = new CancellationTokenSource();
                var thread = new Thread(() => Run(_cancellationTokenSource.Token)) {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal,
                    Name = "DesktopDuplicatorReader"
                };
                thread.Start();
                if (Screen.AllScreens.Length == 2)
                {
                    var thread2 = new Thread(() => Run2(_cancellationTokenSource.Token)) {
                        IsBackground = true,
                        Priority = ThreadPriority.BelowNormal,
                        Name = "DesktopDuplicatorReader2"
                    };
                    thread2.Start();
                }
                if(Screen.AllScreens.Length==3)
                {
                    var thread3 = new Thread(() => Run3(_cancellationTokenSource.Token)) {
                        IsBackground = true,
                        Priority = ThreadPriority.BelowNormal,
                        Name = "DesktopDuplicatorReader3"
                    };
                    thread3.Start();
                }
                    
                
            }

        }


        //private void RefreshCapturingStateSecond()
        //{
        //    var isRunningSecond = _cancellationTokenSourceSecond != null && IsRunningSecond;
        //    var shouldBeRunningSecond = UserSettings.TransferActive;

        //    var shouldBeCapturingSecond = UserSettings.CaptureActive || SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpenSecond;

        //    if (isRunningSecond && !shouldBeCapturingSecond && shouldBeRunningSecond)
        //    {
        //        //stop it!
        //        _log.Debug("stopping the capturing");
        //        _cancellationTokenSourceSecond.Cancel();
        //        _cancellationTokenSourceSecond = null;
        //    }
        //    else if (!isRunningSecond && shouldBeCapturingSecond && shouldBeRunningSecond)
        //    {
        //        //start it
        //        _log.Debug("starting the capturing");
        //        _cancellationTokenSourceSecond = new CancellationTokenSource();
        //        var thread = new Thread(() => RunSecond(_cancellationTokenSourceSecond.Token)) {
        //            IsBackground = true,
        //            Priority = ThreadPriority.BelowNormal,
        //            Name = "DesktopDuplicatorReader"
        //        };
        //        thread.Start();
        //        if (Screen.AllScreens.Length == 2)
        //        {
        //            var thread2 = new Thread(() => Run2Second(_cancellationTokenSourceSecond.Token)) {
        //                IsBackground = true,
        //                Priority = ThreadPriority.BelowNormal,
        //                Name = "DesktopDuplicatorReader2"
        //            };
        //            thread2.Start();
        //        }


        //    }

        //}


        private IUserSettings UserSettings { get; }
        private ISpotSet SpotSet { get; }
        private ISpotSet SpotSet2 { get; }
        private ISpotSet SpotSet3 { get; }
        private SettingsViewModel SettingsViewModel { get; }

        private readonly Policy _retryPolicy;

        private TimeSpan ProvideDelayDuration(int index)
        {
            if (index < 10)
            {
                //first second
                return TimeSpan.FromMilliseconds(100);
            }

            if (index < 10 + 256)
            {
                //steps where there is also led dimming
                SpotSet.IndicateMissingValues();
                return TimeSpan.FromMilliseconds(5000d / 256);
            }
            return TimeSpan.FromMilliseconds(1000);
        }

        private DesktopDuplicator _desktopDuplicator;
        private DesktopDuplicator _desktopDuplicator2;
        private DesktopDuplicator _desktopDuplicator3;


        public void Run(CancellationToken token)
        {
            if (IsRunning) throw new Exception(nameof(DesktopDuplicatorReader) + " is already running!");

            IsRunning = true;
            _log.Debug("Started Desktop Duplication Reader.");
            Bitmap image = null;
           // Bitmap image2 = null;
            try
            {
                BitmapData bitmapData = new BitmapData();
              //  BitmapData bitmapData2 = new BitmapData();

                while (!token.IsCancellationRequested)
                {
                    var frameTime = Stopwatch.StartNew();
                    var newImage = _retryPolicy.Execute(() => GetNextFrame(image));
                    TraceFrameDetails(newImage);
                   // var newImage2 = _retryPolicy.Execute(() => GetNextFrame2(image2));
                   // TraceFrameDetails(newImage2);

                    if (newImage == null )
                    {
                        //there was a timeout before there was the next frame, simply retry!
                        continue;
                    }
                    image = newImage;
                  //  image2 = newImage2;
                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpen);
                    if (isPreviewRunning)
                    {
                        SettingsViewModel.SetPreviewImage(image);
                    }


                    image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
                  //  image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData2);
                    lock (SpotSet.Lock)
                    {
                        var useLinearLighting = UserSettings.UseLinearLighting;

                        var imageRectangle = new Rectangle(0, 0, image.Width, image.Height);

                        if (imageRectangle.Width != SpotSet.ExpectedScreenWidth || imageRectangle.Height != SpotSet.ExpectedScreenHeight)
                        {
                            //the screen was resized or this is some kind of powersaving state
                            SpotSet.IndicateMissingValues();
                            return;
                        }
                        else
                        {
                            Parallel.ForEach(SpotSet.Spots
                                , spot =>
                                {
                                    const int numberOfSteps = 15;
                                    int stepx = Math.Max(1, spot.Rectangle.Width / numberOfSteps);
                                    int stepy = Math.Max(1, spot.Rectangle.Height / numberOfSteps);

                                    GetAverageColorOfRectangularRegion(spot.Rectangle, stepy, stepx, bitmapData,
                                        out int sumR, out int sumG, out int sumB, out int count);

                                    var countInverse = 1f / count;

                                    ApplyColorCorrections(sumR * countInverse, sumG * countInverse, sumB * countInverse
                                        , out byte finalR, out byte finalG, out byte finalB, useLinearLighting
                                        , UserSettings.SaturationTreshold, spot.Red, spot.Green, spot.Blue);

                                    spot.SetColor(finalR, finalG, finalB, isPreviewRunning);

                                });
                        }

                        if (isPreviewRunning)
                        {
                            //copy all color data to the preview
                            var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                            SettingsViewModel.PreviewSpots = SpotSet.Spots;
                        }
                    }
                    /*  lock (SpotSet2.Lock)
                      {
                          var useLinearLighting = UserSettings.UseLinearLighting;

                          var imageRectangle2 = new Rectangle(0, 0, image2.Width, image2.Height);

                          if (imageRectangle2.Width != SpotSet2.ExpectedScreenWidth || imageRectangle2.Height != SpotSet2.ExpectedScreenHeight)
                          {
                              //the screen was resized or this is some kind of powersaving state
                              SpotSet2.IndicateMissingValues();
                              return;
                          }
                          else
                          {
                              Parallel.ForEach(SpotSet2.Spots
                                  , spot2 =>
                                  {
                                      const int numberOfSteps = 15;
                                      int stepx = Math.Max(1, spot2.Rectangle.Width / numberOfSteps);
                                      int stepy = Math.Max(1, spot2.Rectangle.Height / numberOfSteps);

                                      GetAverageColorOfRectangularRegion(spot2.Rectangle, stepy, stepx, bitmapData2,
                                          out int sumR2, out int sumG2, out int sumB2, out int count2);

                                      var countInverse = 1f / count2;

                                      ApplyColorCorrections(sumR2 * countInverse, sumG2 * countInverse, sumB2 * countInverse
                                          , out byte finalR2, out byte finalG2, out byte finalB2, useLinearLighting
                                          , UserSettings.SaturationTreshold, spot2.Red, spot2.Green, spot2.Blue);

                                      spot2.SetColor(finalR2, finalG2, finalB2, isPreviewRunning);

                                  });
                          }


                      }*/


                    image.UnlockBits(bitmapData);
                  // image2.UnlockBits(bitmapData2);

                    int minFrameTimeInMs = 1000 / UserSettings.LimitFps;
                    var elapsedMs = (int)frameTime.ElapsedMilliseconds;
                    if (elapsedMs < minFrameTimeInMs)
                    {
                        Thread.Sleep(minFrameTimeInMs - elapsedMs);
                    }
                }
            }
            finally
            {
                image?.Dispose();
                //image2?.Dispose();

                _desktopDuplicator?.Dispose();
              // _desktopDuplicator2?.Dispose();
                _desktopDuplicator = null;                
              //  _desktopDuplicator2 = null;

                _log.Debug("Stopped Desktop Duplication Reader.");
                IsRunning = false;
            }
        }
        public void Run2(CancellationToken token)
        {
          //  if (IsRunning) throw new Exception(nameof(DesktopDuplicatorReader) + " is already running!");

           // IsRunning = true;
            _log.Debug("Started Desktop Duplication Reader.");
           // Bitmap image = null;
            Bitmap image2 = null;
            try
            {
               // BitmapData bitmapData = new BitmapData();
                BitmapData bitmapData2 = new BitmapData();

                while (!token.IsCancellationRequested)
                {
                    var frameTime = Stopwatch.StartNew();
                   // var newImage = _retryPolicy.Execute(() => GetNextFrame(image));
                  //  TraceFrameDetails(newImage);
                    var newImage2 = _retryPolicy.Execute(() => GetNextFrame2(image2));
                    TraceFrameDetails(newImage2);

                    if ( newImage2 == null)
                    {
                        //there was a timeout before there was the next frame, simply retry!
                        continue;
                    }
                  //  image = newImage;
                    image2 = newImage2;
                   



                  //  image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
                    image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData2);
                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpen);
                    lock (SpotSet2.Lock2)
                      {
                          var useLinearLighting = UserSettings.UseLinearLighting;

                          var imageRectangle2 = new Rectangle(0, 0, image2.Width, image2.Height);

                          if (imageRectangle2.Width != SpotSet2.ExpectedScreenWidth2 || imageRectangle2.Height != SpotSet2.ExpectedScreenHeight2)
                          {
                              //the screen was resized or this is some kind of powersaving state
                              SpotSet2.IndicateMissingValues();
                              return;
                          }
                          else
                          {
                              Parallel.ForEach(SpotSet2.Spots2
                                  , spot2 =>
                                  {
                                      const int numberOfSteps = 15;
                                      int stepx = Math.Max(1, spot2.Rectangle.Width / numberOfSteps);
                                      int stepy = Math.Max(1, spot2.Rectangle.Height / numberOfSteps);

                                     GetAverageColorOfRectangularRegion(spot2.Rectangle, stepy, stepx, bitmapData2,
                                          out int sumR2, out int sumG2, out int sumB2, out int count2);

                                      var countInverse = 1f / count2;

                                      ApplyColorCorrections(sumR2 * countInverse, sumG2 * countInverse, sumB2 * countInverse
                                          , out byte finalR2, out byte finalG2, out byte finalB2, useLinearLighting
                                          , UserSettings.SaturationTreshold, spot2.Red, spot2.Green, spot2.Blue);

                                      spot2.SetColor(finalR2, finalG2, finalB2, isPreviewRunning);

                                  });
                          }


                      }


                  //  image.UnlockBits(bitmapData);
                    image2.UnlockBits(bitmapData2);

                    int minFrameTimeInMs = 1000 / UserSettings.LimitFps;
                    var elapsedMs = (int)frameTime.ElapsedMilliseconds;
                    if (elapsedMs < minFrameTimeInMs)
                    {
                        Thread.Sleep(minFrameTimeInMs - elapsedMs);
                    }
                }
            }
            finally
            {
               // image?.Dispose();
                image2?.Dispose();

               // _desktopDuplicator?.Dispose();
                _desktopDuplicator2?.Dispose();
               // _desktopDuplicator = null;
                _desktopDuplicator2 = null;

                _log.Debug("Stopped Desktop Duplication Reader.");
                IsRunning = false;
            }
        }

        public void Run3(CancellationToken token)
        {
            //  if (IsRunning) throw new Exception(nameof(DesktopDuplicatorReader) + " is already running!");

            // IsRunning = true;
            _log.Debug("Started Desktop Duplication Reader.");
            // Bitmap image = null;
            Bitmap image3= null;
            try
            {
                // BitmapData bitmapData = new BitmapData();
                BitmapData bitmapData3 = new BitmapData();

                while (!token.IsCancellationRequested)
                {
                    var frameTime = Stopwatch.StartNew();
                    // var newImage = _retryPolicy.Execute(() => GetNextFrame(image));
                    //  TraceFrameDetails(newImage);
                    var newImage3 = _retryPolicy.Execute(() => GetNextFrame3(image3));
                    TraceFrameDetails(newImage3);

                    if (newImage3== null)
                    {
                        //there was a timeout before there was the next frame, simply retry!
                        continue;
                    }
                    //  image = newImage;
                    image3 = newImage3;




                    //  image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
                    image3.LockBits(new Rectangle(0, 0, image3.Width, image3.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData3);
                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpen);
                    lock (SpotSet3.Lock)
                    {
                        var useLinearLighting = UserSettings.UseLinearLighting;

                        var imageRectangle3 = new Rectangle(0, 0, image3.Width, image3.Height);

                        if (imageRectangle3.Width != SpotSet3.ExpectedScreenWidth3 || imageRectangle3.Height != SpotSet2.ExpectedScreenHeight3)
                        {
                            //the screen was resized or this is some kind of powersaving state
                            SpotSet3.IndicateMissingValues();
                            return;
                        }
                        else
                        {
                            Parallel.ForEach(SpotSet3.Spots3
                                , spot3 =>
                                {
                                    const int numberOfSteps = 15;
                                    int stepx = Math.Max(1, spot3.Rectangle.Width / numberOfSteps);
                                    int stepy = Math.Max(1, spot3.Rectangle.Height / numberOfSteps);

                                    GetAverageColorOfRectangularRegion(spot3.Rectangle, stepy, stepx, bitmapData3,
                                           out int sumR3, out int sumG3, out int sumB3, out int count3);

                                    var countInverse = 1f / count3;

                                    ApplyColorCorrections(sumR3 * countInverse, sumG3 * countInverse, sumB3 * countInverse
                                        , out byte finalR3, out byte finalG3, out byte finalB3, useLinearLighting
                                        , UserSettings.SaturationTreshold, spot3.Red, spot3.Green, spot3.Blue);

                                    spot3.SetColor(finalR3, finalG3, finalB3, isPreviewRunning);

                                });
                        }


                    }


                    //  image.UnlockBits(bitmapData);
                    image3.UnlockBits(bitmapData3);

                    int minFrameTimeInMs = 1000 / UserSettings.LimitFps;
                    var elapsedMs = (int)frameTime.ElapsedMilliseconds;
                    if (elapsedMs < minFrameTimeInMs)
                    {
                        Thread.Sleep(minFrameTimeInMs - elapsedMs);
                    }
                }
            }
            finally
            {
                // image?.Dispose();
                image3.Dispose();

                // _desktopDuplicator?.Dispose();
                _desktopDuplicator3?.Dispose();
                // _desktopDuplicator = null;
                _desktopDuplicator3 = null;

                _log.Debug("Stopped Desktop Duplication Reader.");
                IsRunning = false;
            }
        }


        //public void RunSecond(CancellationToken token)
        //{
        //    if (IsRunningSecond) throw new Exception(nameof(DesktopDuplicatorReader) + " is already running!");

        //    IsRunningSecond = true;
        //    _log.Debug("Started Desktop Duplication Reader.");
        //    Bitmap image = null;
        //    // Bitmap image2 = null;
        //    try
        //    {
        //        BitmapData bitmapData = new BitmapData();
        //        BitmapData bitmapData2 = new BitmapData();

        //        while (!token.IsCancellationRequested)
        //        {
        //            var frameTime = Stopwatch.StartNew();
        //            var newImage = _retryPolicy.Execute(() => GetNextFrame(image));
        //            TraceFrameDetails(newImage);
        //            // var newImage2 = _retryPolicy.Execute(() => GetNextFrame2(image2));
        //            // TraceFrameDetails(newImage2);

        //            if (newImage == null)
        //            {
        //                //there was a timeout before there was the next frame, simply retry!
        //                continue;
        //            }
        //            image = newImage;
        //            //  image2 = newImage2;
        //            bool isPreviewRunningSecond = (SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpenSecond);
        //            if (isPreviewRunningSecond)
        //            {
        //                SettingsViewModel.SetPreviewImage(image);
        //            }


        //            image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
        //            //  image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData2);
        //            lock (SpotSet.Lock)
        //            {
        //                var useLinearLighting = UserSettings.UseLinearLighting;

        //                var imageRectangle = new Rectangle(0, 0, image.Width, image.Height);

        //                if (imageRectangle.Width != SpotSet.ExpectedScreenWidth || imageRectangle.Height != SpotSet.ExpectedScreenHeight)
        //                {
        //                    //the screen was resized or this is some kind of powersaving state
        //                    SpotSet.IndicateMissingValues();
        //                    return;
        //                }
        //                else
        //                {
        //                    Parallel.ForEach(SpotSet.Spots
        //                        , spot =>
        //                        {
        //                            const int numberOfSteps = 15;
        //                            int stepx = Math.Max(1, spot.Rectangle.Width / numberOfSteps);
        //                            int stepy = Math.Max(1, spot.Rectangle.Height / numberOfSteps);

        //                            GetAverageColorOfRectangularRegion(spot.Rectangle, stepy, stepx, bitmapData,
        //                                out int sumR, out int sumG, out int sumB, out int count);

        //                            var countInverse = 1f / count;

        //                            ApplyColorCorrections(sumR * countInverse, sumG * countInverse, sumB * countInverse
        //                                , out byte finalR, out byte finalG, out byte finalB, useLinearLighting
        //                                , UserSettings.SaturationTreshold, spot.Red, spot.Green, spot.Blue);

        //                            spot.SetColor(finalR, finalG, finalB, isPreviewRunningSecond);

        //                        });
        //                }

        //                if (isPreviewRunningSecond)
        //                {
        //                    //copy all color data to the preview
        //                    var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

        //                    SettingsViewModel.PreviewSpots = SpotSet.Spots;
        //                }
        //            }
        //            /*  lock (SpotSet2.Lock)
        //              {
        //                  var useLinearLighting = UserSettings.UseLinearLighting;

        //                  var imageRectangle2 = new Rectangle(0, 0, image2.Width, image2.Height);

        //                  if (imageRectangle2.Width != SpotSet2.ExpectedScreenWidth || imageRectangle2.Height != SpotSet2.ExpectedScreenHeight)
        //                  {
        //                      //the screen was resized or this is some kind of powersaving state
        //                      SpotSet2.IndicateMissingValues();
        //                      return;
        //                  }
        //                  else
        //                  {
        //                      Parallel.ForEach(SpotSet2.Spots
        //                          , spot2 =>
        //                          {
        //                              const int numberOfSteps = 15;
        //                              int stepx = Math.Max(1, spot2.Rectangle.Width / numberOfSteps);
        //                              int stepy = Math.Max(1, spot2.Rectangle.Height / numberOfSteps);

        //                              GetAverageColorOfRectangularRegion(spot2.Rectangle, stepy, stepx, bitmapData2,
        //                                  out int sumR2, out int sumG2, out int sumB2, out int count2);

        //                              var countInverse = 1f / count2;

        //                              ApplyColorCorrections(sumR2 * countInverse, sumG2 * countInverse, sumB2 * countInverse
        //                                  , out byte finalR2, out byte finalG2, out byte finalB2, useLinearLighting
        //                                  , UserSettings.SaturationTreshold, spot2.Red, spot2.Green, spot2.Blue);

        //                              spot2.SetColor(finalR2, finalG2, finalB2, isPreviewRunning);

        //                          });
        //                  }


        //              }*/


        //            image.UnlockBits(bitmapData);
        //            // image2.UnlockBits(bitmapData2);

        //            int minFrameTimeInMs = 1000 / UserSettings.LimitFps;
        //            var elapsedMs = (int)frameTime.ElapsedMilliseconds;
        //            if (elapsedMs < minFrameTimeInMs)
        //            {
        //                Thread.Sleep(minFrameTimeInMs - elapsedMs);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        image?.Dispose();
        //        //image2?.Dispose();

        //        _desktopDuplicator?.Dispose();
        //        // _desktopDuplicator2?.Dispose();
        //        _desktopDuplicator = null;
        //        //  _desktopDuplicator2 = null;

        //        _log.Debug("Stopped Desktop Duplication Reader.");
        //        IsRunningSecond = false;
        //    }
        //}
        //public void Run2Second(CancellationToken token)
        //{
        //    //  if (IsRunning) throw new Exception(nameof(DesktopDuplicatorReader) + " is already running!");

        //    // IsRunning = true;
        //    _log.Debug("Started Desktop Duplication Reader.");
        //    // Bitmap image = null;
        //    Bitmap image2 = null;
        //    try
        //    {
        //        // BitmapData bitmapData = new BitmapData();
        //        BitmapData bitmapData2 = new BitmapData();

        //        while (!token.IsCancellationRequested)
        //        {
        //            var frameTime = Stopwatch.StartNew();
        //            // var newImage = _retryPolicy.Execute(() => GetNextFrame(image));
        //            //  TraceFrameDetails(newImage);
        //            var newImage2 = _retryPolicy.Execute(() => GetNextFrame2(image2));
        //            TraceFrameDetails(newImage2);

        //            if (newImage2 == null)
        //            {
        //                //there was a timeout before there was the next frame, simply retry!
        //                continue;
        //            }
        //            //  image = newImage;
        //            image2 = newImage2;




        //            //  image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
        //            image2.LockBits(new Rectangle(0, 0, image2.Width, image2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData2);
        //            bool isPreviewRunningSecond = (SettingsViewModel.IsSettingsWindowOpen && SettingsViewModel.IsPreviewTabOpenSecond);
        //            lock (SpotSet2.Lock2)
        //            {
        //                var useLinearLighting = UserSettings.UseLinearLighting;

        //                var imageRectangle2 = new Rectangle(0, 0, image2.Width, image2.Height);

        //                if (imageRectangle2.Width != SpotSet2.ExpectedScreenWidth2 || imageRectangle2.Height != SpotSet2.ExpectedScreenHeight2)
        //                {
        //                    //the screen was resized or this is some kind of powersaving state
        //                    SpotSet2.IndicateMissingValues();
        //                    return;
        //                }
        //                else
        //                {
        //                    Parallel.ForEach(SpotSet2.Spots2
        //                        , spot2 =>
        //                        {
        //                            const int numberOfSteps = 15;
        //                            int stepx = Math.Max(1, spot2.Rectangle.Width / numberOfSteps);
        //                            int stepy = Math.Max(1, spot2.Rectangle.Height / numberOfSteps);

        //                            GetAverageColorOfRectangularRegion(spot2.Rectangle, stepy, stepx, bitmapData2,
        //                                   out int sumR2, out int sumG2, out int sumB2, out int count2);

        //                            var countInverse = 1f / count2;

        //                            ApplyColorCorrections(sumR2 * countInverse, sumG2 * countInverse, sumB2 * countInverse
        //                                , out byte finalR2, out byte finalG2, out byte finalB2, useLinearLighting
        //                                , UserSettings.SaturationTreshold, spot2.Red, spot2.Green, spot2.Blue);

        //                            spot2.SetColor(finalR2, finalG2, finalB2, isPreviewRunningSecond);

        //                        });
        //                }


        //            }


        //            //  image.UnlockBits(bitmapData);
        //            image2.UnlockBits(bitmapData2);

        //            int minFrameTimeInMs = 1000 / UserSettings.LimitFps;
        //            var elapsedMs = (int)frameTime.ElapsedMilliseconds;
        //            if (elapsedMs < minFrameTimeInMs)
        //            {
        //                Thread.Sleep(minFrameTimeInMs - elapsedMs);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        // image?.Dispose();
        //        image2?.Dispose();

        //        // _desktopDuplicator?.Dispose();
        //        _desktopDuplicator2?.Dispose();
        //        // _desktopDuplicator = null;
        //        _desktopDuplicator2 = null;

        //        _log.Debug("Stopped Desktop Duplication Reader.");
        //        IsRunningSecond = false;
        //    }
        //}







        private int? _lastObservedHeight;
        private int? _lastObservedWidth;

        private void TraceFrameDetails(Bitmap image)
        {
            //there are many frames per second and we need to extract useful information and only log those!
            if (image == null)
            {
                //if the frame is null, this can mean two things. the timeout from the desktop duplication api was reached
                //before the monitor content changed or there was some other error.
            }
            else
            {
                if (_lastObservedHeight != null && _lastObservedWidth != null
                    && (_lastObservedHeight != image.Height || _lastObservedWidth != image.Width))
                {
                    _log.Debug("The frame size changed from {0}x{1} to {2}x{3}"
                        , _lastObservedWidth, _lastObservedHeight
                        , image.Width, image.Height);

                }
                _lastObservedWidth = image.Width;
                _lastObservedHeight = image.Height;
            }
        }

        private void ApplyColorCorrections(float r, float g, float b, out byte finalR, out byte finalG, out byte finalB, bool useLinearLighting, byte saturationTreshold
            , byte lastColorR, byte lastColorG, byte lastColorB)
        {
            if (lastColorR == 0 && lastColorG == 0 && lastColorB == 0)
            {
                //if the color was black the last time, we increase the saturationThreshold to make flickering more unlikely
                saturationTreshold += 2;
            }
            if (r <= saturationTreshold && g <= saturationTreshold && b <= saturationTreshold)
            {
                //black
                finalR = finalG = finalB = 0;
                return;
            }

            //"white" on wall was 66,68,77 without white balance
            //white balance
            //todo: introduce settings for white balance adjustments
            r *= UserSettings.WhitebalanceRed / 100f;
            g *= UserSettings.WhitebalanceGreen / 100f;
            b *= UserSettings.WhitebalanceBlue / 100f;

            if (!useLinearLighting)
            {
                //apply non linear LED fading ( http://www.mikrocontroller.net/articles/LED-Fading )
                finalR = FadeNonLinear(r);
                finalG = FadeNonLinear(g);
                finalB = FadeNonLinear(b);
            }
            else
            {
                //output
                finalR = (byte)r;
                finalG = (byte)g;
                finalB = (byte)b;
            }
        }

        private readonly byte[] _nonLinearFadingCache = Enumerable.Range(0, 2560)
            .Select(n => FadeNonLinearUncached(n / 10f))
            .ToArray();

        private byte FadeNonLinear(float color)
        {
            var cacheIndex = (int)(color * 10);
            return _nonLinearFadingCache[Math.Min(2560 - 1, Math.Max(0, cacheIndex))];
        }

        private static byte FadeNonLinearUncached(float color)
        {
            const float factor = 80f;
            return (byte)(256f * ((float)Math.Pow(factor, color / 256f) - 1f) / (factor - 1));
        }

        private Bitmap GetNextFrame(Bitmap reusableBitmap)
        {
            if (_desktopDuplicator == null)
            {
                _desktopDuplicator = new DesktopDuplicator(0, 0);
            }

            try
            {
                return _desktopDuplicator.GetLatestFrame(reusableBitmap);
            }
            catch (Exception ex)
            {
                if (ex.Message != "_outputDuplication is null")
                {
                    _log.Error(ex, "GetNextFrame() failed.");
                }

                _desktopDuplicator?.Dispose();
                _desktopDuplicator = null;
                throw;
            }
        }
        private Bitmap GetNextFrame2(Bitmap reusableBitmap2)
        {
            if (_desktopDuplicator2 == null)
            {
                _desktopDuplicator2 = new DesktopDuplicator(0, 1);
            }

            try
            {
                return _desktopDuplicator2.GetLatestFrame(reusableBitmap2);
            }
            catch (Exception ex)
            {
                if (ex.Message != "_outputDuplication2 is null")
                {
                    _log.Error(ex, "GetNextFrame2() failed.");
                }

                _desktopDuplicator2?.Dispose();
                _desktopDuplicator2 = null;
                throw;
            }
        }
        private Bitmap GetNextFrame3(Bitmap reusableBitmap3)
        {
            if (_desktopDuplicator3 == null)
            {
                _desktopDuplicator3 = new DesktopDuplicator(0, 2);
            }

            try
            {
                return _desktopDuplicator3.GetLatestFrame(reusableBitmap3);
            }
            catch (Exception ex)
            {
                if (ex.Message != "_outputDuplication3 is null")
                {
                    _log.Error(ex, "GetNextFrame3() failed.");
                }

                _desktopDuplicator3?.Dispose();
                _desktopDuplicator3 = null;
                throw;
            }
        }

        private unsafe void GetAverageColorOfRectangularRegion(Rectangle spotRectangle, int stepy, int stepx, BitmapData bitmapData, out int sumR, out int sumG,
            out int sumB, out int count)
        {
            sumR = 0;
            sumG = 0;
            sumB = 0;
            count = 0;

            var stepCount = spotRectangle.Width / stepx;
            var stepxTimes4 = stepx * 4;
            for (var y = spotRectangle.Top; y < spotRectangle.Bottom; y += stepy)
            {
                byte* pointer = (byte*)bitmapData.Scan0 + bitmapData.Stride * y + 4 * spotRectangle.Left;
                for (int i = 0; i < stepCount; i++)
                {
                    sumB += pointer[0];
                    sumG += pointer[1];
                    sumR += pointer[2];

                    pointer += stepxTimes4;
                }
                count += stepCount;
            }
        }

    }
}
