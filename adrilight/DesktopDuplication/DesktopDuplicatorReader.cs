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
using adrilight.Resources;
using adrilight.Util;

namespace adrilight
{
    internal class DesktopDuplicatorReader : IDesktopDuplicatorReader
    {
        private readonly ILogger _log = LogManager.GetCurrentClassLogger();

        public DesktopDuplicatorReader(IGeneralSettings userSettings, IGeneralSpotSet spotSet)
        {
            UserSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            SpotSet = spotSet ?? throw new ArgumentNullException(nameof(spotSet));
           

           // SettingsViewModel = settingsViewModel ?? throw new ArgumentNullException(nameof(settingsViewModel));
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryForever(ProvideDelayDuration);

            UserSettings.PropertyChanged += PropertyChanged;
           // SettingsViewModel.PropertyChanged += PropertyChanged;
            RefreshCapturingState();

            _log.Info($"DesktopDuplicatorReader created.");
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                
                case nameof(UserSettings.ShouldbeRunning):

                    RefreshCapturingState();
                    break;

                case nameof(UserSettings.SelectedDisplay):
                case nameof(UserSettings.SelectedAdapter):
                    RefreshCaptureSource();
                    break;
            }
        }

        public bool IsRunning { get; private set; } = false;
        public bool NeededRefreshing { get; private set; } = false;
        private CancellationTokenSource _cancellationTokenSource;

        private void RefreshCaptureSource()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.ShouldbeRunning;
            //  var shouldBeRefreshing = NeededRefreshing;
            if (isRunning && shouldBeRunning)
            {
                //start it

                IsRunning = false;
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
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
        private void RefreshCapturingState()
        {
            var isRunning = _cancellationTokenSource != null && IsRunning;
            var shouldBeRunning = UserSettings.ShouldbeRunning;
            //  var shouldBeRefreshing = NeededRefreshing;



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


  

        private IGeneralSettings UserSettings { get; }
        private IGeneralSpotSet SpotSet { get; }
       
  

        private readonly Policy _retryPolicy;

        private TimeSpan ProvideDelayDuration(int index)
        {
            if (index < 10)
            {
                
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



        public void Run(CancellationToken token)
        {
            if (IsRunning) throw new Exception(nameof(DesktopDuplicatorReader) + " is already running!");

            IsRunning = true;
            NeededRefreshing = false;
            _log.Debug("Started Desktop Duplication Reader.");
            Bitmap image = null;
            BitmapData bitmapData = new BitmapData();

            try
            {



                while (!token.IsCancellationRequested)
                {
                    var frameTime = Stopwatch.StartNew();
                    var newImage = _retryPolicy.Execute(() => GetNextFrame(image,0));
                    TraceFrameDetails(newImage);
                 
                    if (newImage == null)
                    {
                        //there was a timeout before there was the next frame, simply retry!
                        continue;
                    }
                    image = newImage;


                    image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb, bitmapData);

                    lock (SpotSet.Lock)
                    {
                        var useLinearLighting = UserSettings.UseLinearLighting;

                        var imageRectangle = new Rectangle(0, 0, image.Width, image.Height);

                        if (imageRectangle.Width != SpotSet.ExpectedScreenWidth || imageRectangle.Height != SpotSet.ExpectedScreenHeight)
                        {
                            //the screen was resized or this is some kind of powersaving state
                            SpotSet.IndicateMissingValues();

                            continue;
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
                                    byte FinalR = (byte)(sumR * countInverse);
                                    byte FinalG = (byte)(sumG * countInverse);
                                    byte FinalB = (byte)(sumB * countInverse);

                                    ApplyColorCorrections(FinalR, FinalG, FinalB
                                        , out byte finalR, out byte finalG, out byte finalB, useLinearLighting
                                        , UserSettings.SaturationTreshold, spot.Red, spot.Green, spot.Blue);

                                    //var spotColor = new OpenRGB.NET.Models.Color(finalR, finalG, finalB);

                                    //var semifinalSpotColor = Brightness.applyBrightness(spotColor, brightness);
                                    //ApplySmoothing(semifinalSpotColor.R, semifinalSpotColor.G, semifinalSpotColor.B
                                    //    , out byte RealfinalR, out byte RealfinalG, out byte RealfinalB,
                                    // spot.Red, spot.Green, spot.Blue);
                                    spot.SetColor(finalR, finalG, finalB, true);

                                });
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
                GC.Collect();
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

        private void ApplyColorCorrections(byte r, byte g, byte b, out byte finalR, out byte finalG, out byte finalB, int useLinearLighting, byte saturationTreshold
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
            //r *= UserSettings.WhitebalanceRed / 100f;
            //g *= UserSettings.WhitebalanceGreen / 100f;
            //b *= UserSettings.WhitebalanceBlue / 100f;

            if (useLinearLighting==1)
            {
                //apply non linear LED fading ( http://www.mikrocontroller.net/articles/LED-Fading )
                finalR = FadeNonLinear(r);
                finalG = FadeNonLinear(g);
                finalB = FadeNonLinear(b);
            }
            else
            {
                //output
                finalR = r;
                finalG = g;
                finalB = b;
            }
        }
        //private void ApplySmoothing(float r, float g, float b, out byte semifinalR, out byte semifinalG, out byte semifinalB,
        //   byte lastColorR, byte lastColorG, byte lastColorB)
        //{
        //    int smoothingFactor = 3;
        //    if (UserSettings.InstantMode)
        //    {
        //        smoothingFactor = 0;
        //    }
        //    else if (UserSettings.NormalMode)
        //    {
        //        smoothingFactor = 3;
        //    }
        //    else if (UserSettings.SmoothMode)
        //    {
        //        smoothingFactor = 5;
        //    }

        //    semifinalR = (byte)((r + smoothingFactor * lastColorR) / (smoothingFactor + 1));
        //    semifinalG = (byte)((g + smoothingFactor * lastColorG) / (smoothingFactor + 1));
        //    semifinalB = (byte)((b + smoothingFactor * lastColorB) / (smoothingFactor + 1));
        //}

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

        private Bitmap GetNextFrame(Bitmap reusableBitmap, int outputAddress)
        {
            var selectedDisplay = outputAddress;
            var selectedAdapter = 0; //UserSettings.SelectedAdapter;

            if (_desktopDuplicator == null)
            {
                try
                {
                    _desktopDuplicator = new DesktopDuplicator(selectedAdapter, selectedDisplay);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Unknown, just retry")
                    {
                        _log.Error(ex, "could be secure desktop");
                    }
                    // _desktopDuplicator.Dispose();
                    // _desktopDuplicator = null;
                    GC.Collect();
                    return null;

                }

            }

            try
            {
                return _desktopDuplicator.GetLatestFrame(reusableBitmap);
            }
            catch (Exception ex)
            {
                if (ex.Message != "_outputDuplication is null" && ex.Message != "Access Lost, resolution might be changed" && ex.Message != "Invalid call, might be retrying" && ex.Message != "Failed to release frame.")
                {
                    _log.Error(ex, "GetNextFrame() failed.");

                    // throw;
                }
                else if (ex.Message == "Access Lost, resolution might be changed")
                {
                    _log.Error(ex, "Access Lost, retrying");

                }
                else if (ex.Message == "Invalid call, might be retrying")
                {
                    _log.Error(ex, "Invalid Call Lost, retrying");
                }
                else if (ex.Message == "Failed to release frame.")
                {
                    _log.Error(ex, "Failed to release frame.");
                }

                _desktopDuplicator.Dispose();
                _desktopDuplicator = null;
                GC.Collect();
                return null;
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