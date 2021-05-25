﻿using System;
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
using adrilight.Resources;
using adrilight.Util;

namespace adrilight
{
    internal class DesktopDuplicatorReader : IDesktopDuplicatorReader
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public DesktopDuplicatorReader(IUserSettings userSettings, ISpotSet spotSet, SettingsViewModel settingsViewModel, IContext context)
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

            _log.Info($"DesktopDuplicatorReader created.");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(UserSettings.SelectedEffect):
                case nameof(UserSettings.TransferActive):
                case nameof(UserSettings.IsPreviewEnabled):
                case nameof(UserSettings.CaptureActive):
                
                case nameof(SettingsViewModel.IsSettingsWindowOpen):
                
               

                    RefreshCapturingState();
                    break;
                 
            }
        }

        public bool IsRunning { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;


        private void RefreshCapturingState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.TransferActive && UserSettings.SelectedEffect==0;

            

            if (isRunning && !shouldBeRunning)
            {
                //stop it!
                _log.Debug("stopping the capturing");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }
            else if (!isRunning && shouldBeRunning)
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
        
            try
            {
                BitmapData bitmapData = new BitmapData();
            

                while (!token.IsCancellationRequested)
                {
                    var frameTime = Stopwatch.StartNew();
                    var newImage = _retryPolicy.Execute(() => GetNextFrame(image));
                    TraceFrameDetails(newImage);
                    var brightness = UserSettings.Brightness/100d;

                    if (newImage == null )
                    {
                        //there was a timeout before there was the next frame, simply retry!
                        continue;
                    }
                    image = newImage;
             
                    bool isPreviewRunning = (SettingsViewModel.IsSettingsWindowOpen && UserSettings.SelectedEffect == 0);
                    if (isPreviewRunning)
                    {
                      // SettingsViewModel.SetPreviewImage(image); remove this, using grey gradient background for better visual
                    }
            
  
                    image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);
               
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
                                    var spotColor = new OpenRGB.NET.Models.Color(finalR,finalG,finalB);
                                    var finalSpotColor= Brightness.applyBrightness(spotColor, brightness);
                                    spot.SetColor(finalSpotColor.R, finalSpotColor.G, finalSpotColor.B, isPreviewRunning);

                                });
                        }

                        if (isPreviewRunning)
                        {
                            //copy all color data to the preview
                            var needsNewArray = SettingsViewModel.PreviewSpots?.Length != SpotSet.Spots.Length;

                            SettingsViewModel.PreviewSpots = SpotSet.Spots;
                        }
                    }
                


                    image.UnlockBits(bitmapData);

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
                _desktopDuplicator?.Dispose();
                _desktopDuplicator = null;                
                _log.Debug("Stopped Desktop Duplication Reader.");
                IsRunning = false;
            }
        }
        

        


       







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
