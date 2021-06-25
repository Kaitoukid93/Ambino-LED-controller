using adrilight.Fakes;
using adrilight.ui;
using adrilight.View;
using adrilight.ViewModel;
using GalaSoft.MvvmLight;
using Microsoft.Win32;
using Ninject;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Ninject.Extensions.Conventions;
using adrilight.Resources;
using adrilight.Util;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Squirrel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using BO;
using adrilight.Ninject;

namespace adrilight
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    //structures to exist in the LMCSHD namespace
    public struct Pixel
    {
        public byte R, G, B;
        public Pixel(byte r, byte g, byte b)
        {
            R = r; G = g; B = b;
        }

        public Int32 GetBPP24RGB_Int32()
        {
            return R << 16 | G << 8 | B;
        }
        public Int32 GetBPP16RGB_Int32()
        {
            return ((R & 0xF8) << 16) | ((G & 0xFC) << 8) | (B & 0xF8);
        }
        public Int32 GetBPP8RGB_Int32()
        {
            return ((R & 0xE0) << 16) | ((G & 0xE0) << 8) | (B & 0xC0);
        }
        public Int32 GetBPP8Grayscale_Int32()
        {
            byte color = (byte)((R + G + B) / 3);
            return color << 16 | color << 8 | color;
        }
        public Int32 GetBPP1Monochrome_Int32()
        {
            byte color = (byte)(((R + G + B) / 3) > 127 ? 255 : 0);
            return color << 16 | color << 8 | color;
        }
    }

    public struct PixelOrder
    {
        public enum Orientation { HZ, VT }
        public enum StartCorner { TL, TR, BL, BR }
        public enum NewLine { SC, SN }
    }
    public struct LEDOrder
        {
        public int pixelcounter;
        public int column;
        public int row;

        }


    public struct MatrixTitle
    {
        public int MatrixTitleDimensionX;
        public int MatrixTitleDimensionY;
        public string MatrixTitleColormode;

        public MatrixTitle(int x, int y, string cm)
        {
            MatrixTitleDimensionX = x;
            MatrixTitleDimensionY = y;
            MatrixTitleColormode = cm;
        }

        public string GetTitle()
        {
            return MatrixTitleDimensionX.ToString() + " x " + MatrixTitleDimensionY.ToString() + " | " + MatrixTitleColormode;
        }
    }

    public sealed partial class App : Application
    {
        private static readonly ILogger _log = LogManager.GetCurrentClassLogger();
        private static System.Threading.Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs startupEvent)
        {
            //string mutexId = ((System.Runtime.InteropServices.GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).GetValue(0)).Value.ToString();
            //_mutex = new System.Threading.Mutex(true, mutexId, out bool createdNew);
            //if (!createdNew) Current.Shutdown();
            //else Exit += CloseMutexHandler;
          //  IServiceProvider serviceProvider = CreateServiceProvider();
            base.OnStartup(startupEvent);

            SetupDebugLogging();
            SetupLoggingForProcessWideEvents();

            base.OnStartup(startupEvent);

            _log.Debug($"adrilight {VersionNumber}: Main() started.");
            kernel = SetupDependencyInjection(false);

            this.Resources["Locator"] = new ViewModelLocator(kernel);


           // DeviceSettings = kernel.Get<IDeviceSettings>();
           // UserSettings = kernel.Get<IUserSettings>();
            _telemetryClient = kernel.Get<TelemetryClient>();

            SetupNotifyIcon();

          //  if (!UserSettings.StartMinimized)
           // {
                // OpenSettingsWindow();
                OpenNewUI();
          //  }


           // kernel.Get<AdrilightUpdater>().StartThread();

            SetupTrackingForProcessWideEvents(_telemetryClient);
        }


        protected void CloseMutexHandler(object sender, EventArgs startupEvent)
        {
            _mutex?.Close();
        }
        private TelemetryClient _telemetryClient;

        private static TelemetryClient SetupApplicationInsights(IDeviceSettings settings)
        {
            const string ik = "65086b50-8c52-4b13-9b05-92fbe69c7a52";
            TelemetryConfiguration.Active.InstrumentationKey = ik;
            var tc = new TelemetryClient
            {
                InstrumentationKey = ik
            };

            tc.Context.User.Id = settings.InstallationId.ToString();
            tc.Context.Session.Id = Guid.NewGuid().ToString();
            tc.Context.Device.OperatingSystem = Environment.OSVersion.ToString();

            GlobalDiagnosticsContext.Set("user_id", tc.Context.User.Id);
            GlobalDiagnosticsContext.Set("session_id", tc.Context.Session.Id);
            return tc;
        }

       

        internal static IKernel SetupDependencyInjection(bool isInDesignMode)
        {

            var kernel = new StandardKernel(new DeviceSettingsInjectModule());
            //if(isInDesignMode)
            //{
            //    //setup fakes
            //    kernel.Bind<IUserSettings>().To<UserSettingsFake>().InSingletonScope();
            //    kernel.Bind<IContext>().To<ContextFake>().InSingletonScope();
            //    kernel.Bind<ISpotSet>().To<SpotSetFake>().InSingletonScope();
            //   // kernel.Bind<ISerialStream>().To<SerialStreamFake>().InSingletonScope();
            //   // kernel.Bind<IDesktopDuplicatorReader>().To<DesktopDuplicatorReaderFake>().InSingletonScope();
            //}
            //else
            //{
            //setup real implementations
            //Load setting từ file Json//
            var settingsManager = new UserSettingsManager();
                var settings = settingsManager.LoadIfExists() ?? settingsManager.MigrateOrDefault();
                var alldevicesettings = settingsManager.LoadDeviceIfExists();

            //// tách riêng từng setting của từng device///
            //// hoặc có thể mỗi device có 1 file settin riêng cũng k sao cả ////
            ///{
            ///}
            ///
            ///bind từng setting vào IUserSettings của device tương ứng
            /// kernel.Bind<IDeviceSettings>().ToConstant(devicesettings).InThreadScope(); // mắc chỗ này, IUserSettings bây giờ phải là DeviceInfoDTO
            //kernel.Bind<IContext>().To<WpfContext>().InThreadScope();
            //kernel.Bind<ISpotSet>().To<SpotSet>().InThreadScope();
            //kernel.Bind<ISerialStream>().To<SerialStream>().InThreadScope();
            //kernel.Bind<IDesktopDuplicatorReader>().To<DesktopDuplicatorReader>().InTransientScope();
            //kernel.Bind<IStaticColor>().To<StaticColor>().InTransientScope();
            //kernel.Bind<IRainbow>().To<Rainbow>().InThreadScope();
            //kernel.Bind<IMusic>().To<Music>().InThreadScope();
            //kernel.Bind<IAtmosphere>().To<Atmosphere>().InThreadScope();
            //for (var i=0;i<alldevicesettings.Count;i++)
            //{
            //    var devicename = i.ToString();
            //    kernel.Bind<IDeviceSettings>().ToConstant(alldevicesettings.ElementAt(i)).InThreadScope().Named(devicename);
            //}
            kernel.Bind<LightingViewModel>().ToSelf().InSingletonScope();
            kernel.Bind(x => x.FromThisAssembly()
              .SelectAllClasses()
              .InheritedFrom<ISelectableViewPart>()
              .BindAllInterfaces());

            for (var i=0;i<alldevicesettings.Count;i++)
            {



                // kernel.Bind<IDeviceSettings>().ToConstant(devicesettings).InThreadScope(); // mắc chỗ này, IUserSettings bây giờ phải là DeviceInfoDTO



               // kernel.Bind<TelemetryClient>().ToConstant(SetupApplicationInsights(kernel.Get<IDeviceSettings>(i.ToString())));

                
                
               var desktopDuplicationReader = kernel.Get<IDesktopDuplicatorReader>(i.ToString());
                // var spotset = kernel.Get<ISpotSet>(i.ToString());
                var serialStream = kernel.Get<ISerialStream>(i.ToString());
                var staticColor = kernel.Get<IStaticColor>(i.ToString());
                 var rainbow = kernel.Get<IRainbow>(i.ToString());
                var music = kernel.Get<IMusic>(i.ToString());
                var atmosphere = kernel.Get<IAtmosphere>(i.ToString());


            }

            //kernel.Bind<MainViewViewModel>().ToSelf().InSingletonScope();

            //  var rainbow = kernel.Get<IRainbow>();
            // var music = kernel.Get<IMusic>();
            // var atmosphere = kernel.Get<IAtmosphere>();

            // tạo instance của từng class cho mỗi device//


            //var spotset = kernel.Get<ISpotSet>(); // mỗi device cần có spotset riêng



            //}
            // kernel.Bind<SettingsViewModel>().ToSelf().InSingletonScope();
            //kernel.Bind<BaseViewModel>().ToSelf().InSingletonScope();

            //eagerly create required singletons [could be replaced with actual pipeline]



            return kernel;
            // lighting viewmodel bây giờ chỉ có nhiệm vụ load data từ spotset và settings tương ứng với card sau đó display, không phải khởi tạo class như trước
            // sau bước này thì tất cả các service đều được khởi tạo, SerialStream, SpotSet và service tương ứng với SelectedEffect được khởi chạy//
            // điều này đảm bảo SpotSet có data để lightingviewmodel hiển thị, đồng thời SerialStream cũng lấy data đó gửi ra device
            // kể từ sau khi app khởi động, mọi event điều chỉnh giá trị ở lightingviewmodel sẽ bắn về class đang chạy (rainbow, Music...) và class
            // đó sẽ thay đổi ( những cái này trong từng class đã viết rồi), đồng thời settings được lưu


        }

        private void SetupLoggingForProcessWideEvents()
        {
            AppDomain.CurrentDomain.UnhandledException +=
    (sender, args) => ApplicationWideException(sender, args.ExceptionObject as Exception, "CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (sender, args) => ApplicationWideException(sender, args.Exception, "DispatcherUnhandledException");

            Exit += (s, e) => _log.Debug("Application exit!");

            SystemEvents.PowerModeChanged += (s, e) => _log.Debug("Changing Powermode to {0}", e.Mode);
        }




        private void SetupTrackingForProcessWideEvents(TelemetryClient tc)
        {
            if (tc == null)
            {
                throw new ArgumentNullException(nameof(tc));
            }

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => tc.TrackException(args.ExceptionObject as Exception);

            DispatcherUnhandledException += (sender, args) => tc.TrackException(args.Exception);

            Exit += (s, e) => { tc.TrackEvent("AppExit"); tc.Flush(); };

            SystemEvents.PowerModeChanged += (s, e) => tc.TrackEvent("PowerModeChanged", new Dictionary<string, string> { { "Mode", e.Mode.ToString() } });
            tc.TrackEvent("AppStart");
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void SetupDebugLogging()
        {
            var config = new LoggingConfiguration();
            var debuggerTarget = new DebuggerTarget() { Layout = "${processtime} ${message:exceptionSeparator=\n\t:withException=true}" };
            config.AddTarget("debugger", debuggerTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, debuggerTarget));

            LogManager.Configuration = config;

            _log.Info($"DEBUG logging set up!");
        }

        SettingsWindow _mainForm;
        private IKernel kernel;
        MainView _newUIForm;
        private void OpenSettingsWindow()
        {
            if (_mainForm == null)
            {
                _mainForm = new SettingsWindow();  
                _mainForm.Closed += MainForm_FormClosed;
                _mainForm.Show();
                _telemetryClient.TrackEvent("SettingsWindow opened");
            }
            else
            {
                //bring to front?
                _mainForm.Focus();
            }
        }
        private void OpenNewUI()
        {
            if (_newUIForm == null)
            {
                _newUIForm = new MainView();
               // _newUIForm.Closed += MainForm_FormClosed;
                _newUIForm.Show();
               // _telemetryClient.TrackEvent("SettingsWindow opened");
            }
            else
            {
                //bring to front?
                _newUIForm.Focus();
            }
        }
        private void MainForm_FormClosed(object sender, EventArgs e)
        {
            if (_mainForm == null) return;

            //deregister to avoid memory leak
            _mainForm.Closed -= MainForm_FormClosed;
            _mainForm = null;
            _telemetryClient.TrackEvent("SettingsWindow closed");
        }
        //private IServiceProvider CreateServiceProvider()
        //{
        //    IServiceCollection services = new ServiceCollection();

        //    services.AddScoped<Rainbow>();
        //    services.AddScoped<StaticColor>();
        //    services.AddScoped<Music>();
        //    services.AddScoped<Gifxelation>();
        //    services.AddScoped<DesktopDuplicatorReader>();

        //    return services.BuildServiceProvider();
        //}
        private void SetupNotifyIcon()

        {

            var icon = new System.Drawing.Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("adrilight.zoe.ico"));
            var contextMenu = new System.Windows.Forms.ContextMenu();
            contextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Giao  diện mới", (s, e) => OpenNewUI()));
            contextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Cài đặt...", (s, e) => OpenSettingsWindow()));
            contextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Thoát", (s, e) => Shutdown(0)));

            var notifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                Text = $"adrilight {VersionNumber}",
                Icon = icon,
                Visible = true,
                ContextMenu = contextMenu
            };
            notifyIcon.DoubleClick += (s, e) => { OpenSettingsWindow(); };
            
            Exit += (s, e) => notifyIcon.Dispose();
        }
        

        public static string VersionNumber { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);


        private IUserSettings UserSettings { get; set; }
        private IDeviceSettings DeviceSettings { get; set; }
      

            private void ApplicationWideException(object sender, Exception ex, string eventSource)
        {
            _log.Fatal(ex, $"ApplicationWideException from sender={sender}, adrilight version={VersionNumber}, eventSource={eventSource}");

            var sb = new StringBuilder();
            sb.AppendLine($"Sender: {sender}");
            sb.AppendLine($"Source: {eventSource}");
            if (sender != null)
            {
                sb.AppendLine($"Sender Type: {sender.GetType().FullName}");
            }
            sb.AppendLine("-------");
            do
            {
                sb.AppendLine($"exception type: {ex.GetType().FullName}");
                sb.AppendLine($"exception message: {ex.Message}");
                sb.AppendLine($"exception stacktrace: {ex.StackTrace}");
                sb.AppendLine("-------");
                ex = ex.InnerException;
            } while (ex != null);

            MessageBox.Show(sb.ToString(), "unhandled exception :-(");
            try
            {
                Shutdown(-1);
            }
            catch
            {
                Environment.Exit(-1);
            }
        }
    }

}
