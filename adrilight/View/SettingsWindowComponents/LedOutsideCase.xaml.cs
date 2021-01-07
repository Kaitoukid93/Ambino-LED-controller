using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using adrilight.ViewModel;
using NAudio.CoreAudioApi;
using System.Windows.Threading;
using ColorPickerWPF;
using MaterialDesignThemes.Wpf;
using adrilight;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO.Ports;
using NAudio.Wave;
using NAudio.Dsp;
using Un4seen.Bass;
using System.Collections.ObjectModel;
using adrilight.Settings;
using Un4seen.BassWasapi;
using System.Windows.Controls.Primitives;

namespace adrilight.View.SettingsWindowComponents
{
    /// <summary>
    /// Interaction logic for LedOutsideCase.xaml
    /// </summary>
    public partial class LedOutsideCase : UserControl
    {

        private int devindex;
        public static int volume;
        private WASAPIPROC _process;
        private bool _initialized;
        private int _hanctr;
        private int _lastlevel;
        public static int[] spectrumdata = new int[16];
        public static byte[] output_spectrumdata = new byte[16];
        public static int[] order_data = new int[16];
        public static int[] custom_order_data = new int[16];
        private float[] _fft;
        public bool isEffect { get; set; }



        public LedOutsideCase()
        {
            InitializeComponent();
           
            List<string> names = ComPortNames("1A86", "7523");
            if (names.Count > 0)
            {
                foreach (String s in SerialPort.GetPortNames())
                {
                    if (names.Contains(s))
                    {
                        ambino_port = s;
                    }
                    else
                    {
                        ambino_port = null;
                    }
                }
            }


            


            if (comportbox3.SelectedItem == null)
            {
                comportbox3.SelectedItem = "Không có";
            }

            if (comportbox2.SelectedItem == null)
            {
                comportbox2.SelectedItem = "Không có";
            }

            if (comportbox4.SelectedItem == null)
            {
                comportbox4.SelectedItem = "Không có";
            }
            if (comportbox6.SelectedItem == null)
            {
                comportbox6.SelectedItem = "Không có";
            }



            BassNet.Registration("saorihara93@gmail.com", "2X2831021152222");


            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            DispatcherTimer dispatcherTimer2 = new DispatcherTimer();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(5);
            dispatcherTimer2.Tick += new EventHandler(random_Tick);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 30);
            dispatcherTimer.Start();
            dispatcherTimer2.Start();
            _process = new WASAPIPROC(Process);
            _fft = new float[1024];
            //if (Music_box_1.SelectedIndex >= 0)
            //{
            

            //}
            Init();
            //  var array = (Bassbox.Items[Bassbox.SelectedIndex] as string).Split(' ');
            //  devindex = Convert.ToInt32(array[0]);
            bool result = BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            //  BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            if (!result)
            {
                var error = Bass.BASS_ErrorGetCode();
                MessageBox.Show(error.ToString());
            }
            else
            {
                //_initialized = true;
                //  Bassbox.IsEnabled = false;
            }
            // BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);

            BassWasapi.BASS_WASAPI_Start();


            //_effects = new ObservableCollection<Effect>()
            //{
            //  new Effect(){ Id=1, Name="Sáng theo hiệu ứng"}
            //        ,new Effect(){Id=2,Name="Sáng theo màn hình"}
            //        ,new Effect(){Id=3 , Name="Sáng màu tĩnh"}
            //        ,new Effect(){Id=4 , Name="Sáng theo nhạc"}
            //        ,new Effect(){Id=5 , Name="Đồng bộ Mainboard"}
            //        ,new Effect(){Id=6 , Name="Tắt"}
            //};

        }


        //private ObservableCollection<Effect> _effects;

        //public ObservableCollection<Effect> Effects {
        //    get { return _effects; }
        //    set { _effects = value; }
        //}
        //private Effect effect;

        //public Effect AEffect {
        //    get { return effect; }
        //    set { effect = value; }
        //}


        List<string> ComPortNames(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        private static string[] lines = new string[28];
        private static string[] lines2 = new string[24];
        public static int musicvalue;
        public static string ambino_port;
        public static string temp;
        private IUserSettings UserSettings { get; }
        public static byte DFUVal = 0;


        private void Init()
        {
            bool result = false;
            int devicecount = BassWasapi.BASS_WASAPI_GetDeviceCount();
            for (int i = 0; i < devicecount; i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    Bassbox.Items.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            //  Bassbox.SelectedIndex =0;
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }

        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            /*  if(musicvisualize.IsChecked==true)
              { 
              MMDevice defaultDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
              if(defaultDevice!=null)
              { 
              //var device = (MMDevice)audio.SelectedItem;
              zoebar.Value = (int)(Math.Round(defaultDevice.AudioMeterInformation.MasterPeakValue * 255));
                  musicvalue = (int)(Math.Round(defaultDevice.AudioMeterInformation.MasterPeakValue * 255));

              }
              }
              */

            int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
            if (ret < -1) return;
            int x, y;
            int b0 = 0;


            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x = 0; x < 16; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (16 - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 250 - 4);
                if (y > 255) y = 255;
                if (y < 10) y = 0;
                spectrumdata[x] = (spectrumdata[x] * 6 + y * 2 + 7) / 8; //Smoothing out the value (take 5/8 of old value and 3/8 of new value to make finnal value)
                if (spectrumdata[x] > 255)
                    spectrumdata[x] = 255;

                //  Console.Write("{0, 3} ", y);




            }
            int i;
            //output_spectrumdata = spectrumdata;
           
            output_spectrumdata[0] = Convert.ToByte(spectrumdata[Music_box_1.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[1] = Convert.ToByte(spectrumdata[Music_box_2.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[2] = Convert.ToByte(spectrumdata[Music_box_3.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[3] = Convert.ToByte(spectrumdata[Music_box_4.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[4] = Convert.ToByte(spectrumdata[Music_box_5.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[5] = Convert.ToByte(spectrumdata[Music_box_6.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[6] = Convert.ToByte(spectrumdata[Music_box_7.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[7] = Convert.ToByte(spectrumdata[Music_box_8.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[8] = Convert.ToByte(spectrumdata[Music_box_9.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[9] = Convert.ToByte(spectrumdata[Music_box_10.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[10] = Convert.ToByte(spectrumdata[Music_box_11.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[11] = Convert.ToByte(spectrumdata[Music_box_12.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[12] = Convert.ToByte(spectrumdata[Music_box_13.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[13] = Convert.ToByte(spectrumdata[Music_box_14.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[14] = Convert.ToByte(spectrumdata[Music_box_15.SelectedIndex]); // Re-Arrange the value to match the order of LEDs
            output_spectrumdata[15] = Convert.ToByte(spectrumdata[Music_box_16.SelectedIndex]); // Re-Arrange the value to match the order of LEDs


            zoebar1.Value = spectrumdata[Music_box_1.SelectedIndex];
                zoebar2.Value = spectrumdata[Music_box_2.SelectedIndex];
                zoebar3.Value = spectrumdata[Music_box_3.SelectedIndex];
                zoebar4.Value = spectrumdata[Music_box_4.SelectedIndex];
                zoebar5.Value = spectrumdata[Music_box_5.SelectedIndex];
                zoebar6.Value = spectrumdata[Music_box_6.SelectedIndex];
                zoebar7.Value = spectrumdata[Music_box_7.SelectedIndex];
                zoebar8.Value = spectrumdata[Music_box_8.SelectedIndex];
                zoebar9.Value = spectrumdata[Music_box_9.SelectedIndex];
                zoebar10.Value = spectrumdata[Music_box_10.SelectedIndex];
                zoebar11.Value = spectrumdata[Music_box_11.SelectedIndex];
                zoebar12.Value = spectrumdata[Music_box_12.SelectedIndex];
                zoebar13.Value = spectrumdata[Music_box_13.SelectedIndex];
                zoebar14.Value = spectrumdata[Music_box_14.SelectedIndex];
                zoebar15.Value = spectrumdata[Music_box_15.SelectedIndex];
                zoebar16.Value = spectrumdata[Music_box_16.SelectedIndex];

           



            //  Array.Clear(spectrumdata, 0, 16);

            int level = BassWasapi.BASS_WASAPI_GetLevel(); // Get level (VU metter) for Old AMBINO Device (remove in the future)
            volume = (byte)level;


            // _l.Value = Utils.LowWord32(level);
            //  _r.Value = Utils.HighWord32(level);
            if (level == _lastlevel && level != 0) _hanctr++;
            _lastlevel = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            /*  if (_hanctr > 3)
              {
                  _hanctr = 0;
                //  _l.Value = 0;
                 // _r.Value = 0;
                  Free();
                  Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                  _initialized = false;
                //  Enable = true;
              }
              */
        }
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }


        private async Task DFU_func()
        {
            DFUVal = 1;
            await Task.Delay(1000);
            HUBV2Connect.IsChecked = false;
            DFUVal = 0;

        }


        public void random_Tick(object sender, EventArgs e)
        {
            if (Shuffle.IsChecked == true)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, 30);
                effectbox.SelectedIndex = index;
            }
            if(shuffle.IsChecked==true)
            {
                Random rnd2 = new Random();
                int index = rnd2.Next(0, 7);
                method.SelectedIndex = index;
            }


        }



        public class LedOutsideCaseSelectableViewPart : ISelectableViewPart
        {
            private readonly Lazy<LedOutsideCase> lazyContent;

            public LedOutsideCaseSelectableViewPart(Lazy<LedOutsideCase> lazyContent)
            {
                this.lazyContent = lazyContent ?? throw new ArgumentNullException(nameof(lazyContent));
            }

            public int Order => 26;

            public string ViewPartName => "LED màn hình";

            public object Content { get => lazyContent.Value; }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (ambino_port != null)
            {

                comportbox3.SelectedValue = ambino_port;
            }
        }


        private void Comportbox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void effectbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ChangeRunningMode(object sender, SelectionChangedEventArgs e)
        {
            var i = (Label)(sender as ComboBox).SelectedItem;
            var item = (String)screenefectbox.SelectedItem;

            if (i != null && item != null)
            {
                if (i.Content.ToString() == "Rainbow Custom Zone")
                {
                    if (item == "Sáng theo hiệu ứng")
                    {
                        isEffect = false;
                        //txtTitle.Visibility = Visibility.Visible;
                        //cuszoneIcon.Visibility = Visibility.Visible;
                        Bassbox.Visibility = Visibility.Collapsed;
                        filemaubox.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_1.Visibility = Visibility.Visible;
                        ClrPcker_Background_2.Visibility = Visibility.Visible;
                        ClrPcker_Background_3.Visibility = Visibility.Visible;
                        ClrPcker_Background_4.Visibility = Visibility.Visible;
                        ClrPcker_Background_5.Visibility = Visibility.Visible;
                        ClrPcker_Background_6.Visibility = Visibility.Visible;
                        ClrPcker_Background_7.Visibility = Visibility.Visible;
                        ClrPcker_Background_8.Visibility = Visibility.Visible;
                        bt4.Visibility = Visibility.Visible;
                        bt5.Visibility = Visibility.Visible;


                        //btPlay.Visibility = Visibility.Collapsed;
                        zoebar1.Visibility = Visibility.Collapsed;
                        zoebar2.Visibility = Visibility.Collapsed;
                        zoebar3.Visibility = Visibility.Collapsed;
                        zoebar4.Visibility = Visibility.Collapsed;
                        zoebar5.Visibility = Visibility.Collapsed;
                        zoebar6.Visibility = Visibility.Collapsed;
                        zoebar7.Visibility = Visibility.Collapsed;
                        zoebar8.Visibility = Visibility.Collapsed;
                        zoebar9.Visibility = Visibility.Collapsed;
                        zoebar10.Visibility = Visibility.Collapsed;
                        zoebar11.Visibility = Visibility.Collapsed;
                        zoebar12.Visibility = Visibility.Collapsed;
                        zoebar13.Visibility = Visibility.Collapsed;
                        zoebar14.Visibility = Visibility.Collapsed;
                        zoebar15.Visibility = Visibility.Collapsed;
                        zoebar16.Visibility = Visibility.Collapsed;

                        //txtLed.Visibility = Visibility.Collapsed;
                        //btColor1.Visibility = Visibility.Collapsed;
                        //btColor2.Visibility = Visibility.Collapsed;
                        //btColor3.Visibility = Visibility.Collapsed;
                        //btColor4.Visibility = Visibility.Collapsed;
                        //btColor5.Visibility = Visibility.Collapsed;
                        //btColor6.Visibility = Visibility.Collapsed;
                        //btColor7.Visibility = Visibility.Collapsed;
                        //btColor8.Visibility = Visibility.Collapsed;
                        //btColor9.Visibility = Visibility.Collapsed;
                        //btColor10.Visibility = Visibility.Collapsed;
                        //btColor11.Visibility = Visibility.Collapsed;
                        //btColor12.Visibility = Visibility.Collapsed;
                        //btColor13.Visibility = Visibility.Collapsed;
                        //btColor14.Visibility = Visibility.Collapsed;
                        //btColor15.Visibility = Visibility.Collapsed;
                        //btColor16.Visibility = Visibility.Collapsed;

                        //txtFreq.Visibility = Visibility.Collapsed;
                        //Music_box_1.Visibility = Visibility.Collapsed;
                        //Music_box_2.Visibility = Visibility.Collapsed;
                        //Music_box_3.Visibility = Visibility.Collapsed;
                        //Music_box_4.Visibility = Visibility.Collapsed;
                        //Music_box_5.Visibility = Visibility.Collapsed;
                        //Music_box_6.Visibility = Visibility.Collapsed;
                        //Music_box_7.Visibility = Visibility.Collapsed;
                        //Music_box_8.Visibility = Visibility.Collapsed;
                        //Music_box_9.Visibility = Visibility.Collapsed;
                        //Music_box_10.Visibility = Visibility.Collapsed;
                        //Music_box_11.Visibility = Visibility.Collapsed;
                        //Music_box_12.Visibility = Visibility.Collapsed;
                        //Music_box_13.Visibility = Visibility.Collapsed;
                        //Music_box_14.Visibility = Visibility.Collapsed;
                        //Music_box_15.Visibility = Visibility.Collapsed;
                        //Music_box_16.Visibility = Visibility.Collapsed;

                        this.customZone.Visibility = Visibility.Visible;
                        this.customZone.Height = 150;
                        filemauchip.Content = filemaubox.Text;
                    }
                    else if (item == "Sáng theo nhạc")
                    {
                        isEffect = true;
                        //txtTitle.Visibility = Visibility.Visible;
                        //cuszoneIcon.Visibility = Visibility.Visible;
                        Bassbox.Visibility = Visibility.Visible;
                        filemaubox.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_1.Visibility = Visibility.Visible;
                        ClrPcker_Background_2.Visibility = Visibility.Visible;
                        ClrPcker_Background_3.Visibility = Visibility.Visible;
                        ClrPcker_Background_4.Visibility = Visibility.Visible;
                        ClrPcker_Background_5.Visibility = Visibility.Visible;
                        ClrPcker_Background_6.Visibility = Visibility.Visible;
                        ClrPcker_Background_7.Visibility = Visibility.Visible;
                        ClrPcker_Background_8.Visibility = Visibility.Visible;
                        bt4.Visibility = Visibility.Visible;
                        bt5.Visibility = Visibility.Visible;


                        //btPlay.Visibility = Visibility.Visible;
                        zoebar1.Visibility = Visibility.Visible;
                        zoebar2.Visibility = Visibility.Visible;
                        zoebar3.Visibility = Visibility.Visible;
                        zoebar4.Visibility = Visibility.Visible;
                        zoebar5.Visibility = Visibility.Visible;
                        zoebar6.Visibility = Visibility.Visible;
                        zoebar7.Visibility = Visibility.Visible;
                        zoebar8.Visibility = Visibility.Visible;
                        zoebar9.Visibility = Visibility.Visible;
                        zoebar10.Visibility = Visibility.Visible;
                        zoebar11.Visibility = Visibility.Visible;
                        zoebar12.Visibility = Visibility.Visible;
                        zoebar13.Visibility = Visibility.Visible;
                        zoebar14.Visibility = Visibility.Visible;
                        zoebar15.Visibility = Visibility.Visible;
                        zoebar16.Visibility = Visibility.Visible;

                        //txtLed.Visibility = Visibility.Visible;
                        //btColor1.Visibility = Visibility.Visible;
                        //btColor2.Visibility = Visibility.Visible;
                        //btColor3.Visibility = Visibility.Visible;
                        //btColor4.Visibility = Visibility.Visible;
                        //btColor5.Visibility = Visibility.Visible;
                        //btColor6.Visibility = Visibility.Visible;
                        //btColor7.Visibility = Visibility.Visible;
                        //btColor8.Visibility = Visibility.Visible;
                        //btColor9.Visibility = Visibility.Visible;
                        //btColor10.Visibility = Visibility.Visible;
                        //btColor11.Visibility = Visibility.Visible;
                        //btColor12.Visibility = Visibility.Visible;
                        //btColor13.Visibility = Visibility.Visible;
                        //btColor14.Visibility = Visibility.Visible;
                        //btColor15.Visibility = Visibility.Visible;
                        //btColor16.Visibility = Visibility.Visible;

                        //txtFreq.Visibility = Visibility.Visible;
                        //Music_box_1.Visibility = Visibility.Visible;
                        //Music_box_2.Visibility = Visibility.Visible;
                        //Music_box_3.Visibility = Visibility.Visible;
                        //Music_box_4.Visibility = Visibility.Visible;
                        //Music_box_5.Visibility = Visibility.Visible;
                        //Music_box_6.Visibility = Visibility.Visible;
                        //Music_box_7.Visibility = Visibility.Visible;
                        //Music_box_8.Visibility = Visibility.Visible;
                        //Music_box_9.Visibility = Visibility.Visible;
                        //Music_box_10.Visibility = Visibility.Visible;
                        //Music_box_11.Visibility = Visibility.Visible;
                        //Music_box_12.Visibility = Visibility.Visible;
                        //Music_box_13.Visibility = Visibility.Visible;
                        //Music_box_14.Visibility = Visibility.Visible;
                        //Music_box_15.Visibility = Visibility.Visible;
                        //Music_box_16.Visibility = Visibility.Visible;


                        //zoebar1.Height = 40;
                        //zoebar2.Height = 40;
                        //zoebar3.Height = 40;
                        //zoebar4.Height = 40;
                        //zoebar5.Height = 40;
                        //zoebar6.Height = 40;
                        //zoebar7.Height = 40;
                        //zoebar8.Height = 40;
                        //zoebar9.Height = 40;
                        //zoebar10.Height = 40;
                        //zoebar11.Height = 40;
                        //zoebar12.Height = 40;
                        //zoebar13.Height = 40;
                        //zoebar14.Height = 40;
                        //zoebar15.Height = 40;
                        //zoebar16.Height = 40;


                        this.customZone.Visibility = Visibility.Visible;
                        this.customZone.Height = 349;
                        filemauchip.Content = filemaubox.Text;

                    }
                }
                else
                {
                    if (item == "Sáng theo hiệu ứng")
                    {
                        this.customZone.Visibility = Visibility.Collapsed;
                    }
                    else if (item== "Sáng theo nhạc")
                    {
                        //txtTitle.Visibility = Visibility.Visible;
                        //cuszoneIcon.Visibility = Visibility.Visible;
                        Bassbox.Visibility = Visibility.Visible;
                        filemaubox.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_1.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_2.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_3.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_4.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_5.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_6.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_7.Visibility = Visibility.Collapsed;
                        ClrPcker_Background_8.Visibility = Visibility.Collapsed;
                        bt4.Visibility = Visibility.Visible;
                        bt5.Visibility = Visibility.Visible;


                        //btPlay.Visibility = Visibility.Visible;
                        zoebar1.Visibility = Visibility.Visible;
                        zoebar2.Visibility = Visibility.Visible;
                        zoebar3.Visibility = Visibility.Visible;
                        zoebar4.Visibility = Visibility.Visible;
                        zoebar5.Visibility = Visibility.Visible;
                        zoebar6.Visibility = Visibility.Visible;
                        zoebar7.Visibility = Visibility.Visible;
                        zoebar8.Visibility = Visibility.Visible;
                        zoebar9.Visibility = Visibility.Visible;
                        zoebar10.Visibility = Visibility.Visible;
                        zoebar11.Visibility = Visibility.Visible;
                        zoebar12.Visibility = Visibility.Visible;
                        zoebar13.Visibility = Visibility.Visible;
                        zoebar14.Visibility = Visibility.Visible;
                        zoebar15.Visibility = Visibility.Visible;
                        zoebar16.Visibility = Visibility.Visible;

                        //txtLed.Visibility = Visibility.Visible;
                        //btColor1.Visibility = Visibility.Visible;
                        //btColor2.Visibility = Visibility.Visible;
                        //btColor3.Visibility = Visibility.Visible;
                        //btColor4.Visibility = Visibility.Visible;
                        //btColor5.Visibility = Visibility.Visible;
                        //btColor6.Visibility = Visibility.Visible;
                        //btColor7.Visibility = Visibility.Visible;
                        //btColor8.Visibility = Visibility.Visible;
                        //btColor9.Visibility = Visibility.Visible;
                        //btColor10.Visibility = Visibility.Visible;
                        //btColor11.Visibility = Visibility.Visible;
                        //btColor12.Visibility = Visibility.Visible;
                        //btColor13.Visibility = Visibility.Visible;
                        //btColor14.Visibility = Visibility.Visible;
                        //btColor15.Visibility = Visibility.Visible;
                        //btColor16.Visibility = Visibility.Visible;

                        //txtFreq.Visibility = Visibility.Visible;
                        //Music_box_1.Visibility = Visibility.Visible;
                        //Music_box_2.Visibility = Visibility.Visible;
                        //Music_box_3.Visibility = Visibility.Visible;
                        //Music_box_4.Visibility = Visibility.Visible;
                        //Music_box_5.Visibility = Visibility.Visible;
                        //Music_box_6.Visibility = Visibility.Visible;
                        //Music_box_7.Visibility = Visibility.Visible;
                        //Music_box_8.Visibility = Visibility.Visible;
                        //Music_box_9.Visibility = Visibility.Visible;
                        //Music_box_10.Visibility = Visibility.Visible;
                        //Music_box_11.Visibility = Visibility.Visible;
                        //Music_box_12.Visibility = Visibility.Visible;
                        //Music_box_13.Visibility = Visibility.Visible;
                        //Music_box_14.Visibility = Visibility.Visible;
                        //Music_box_15.Visibility = Visibility.Visible;
                        //Music_box_16.Visibility = Visibility.Visible;

                        this.customZone.Visibility = Visibility.Visible;
                        this.customZone.Height = 349;
                        filemauchip.Content = i.Content.ToString();
                    }
                }
            }
        }

        private void ChangeEffect(object sender, SelectionChangedEventArgs e)
        {
            var item = (String)(sender as ComboBox).SelectedItem;
            var i = (Label)effectbox.SelectedItem;
            
           
            //if (numberScreen.SelectedItem == null)
            //    numberScreen.SelectedItem = "Linear Lighting";

            if (item != null)
            {

                previewCard.Visibility = Visibility.Collapsed;
                //balanceCard.Visibility = Visibility.Collapsed;
                //ledCard.Visibility = Visibility.Collapsed;
                ambilightCard.Visibility = Visibility.Collapsed;
                //sizescreenCard.Visibility = Visibility.Collapsed;
                btnReset.Visibility = Visibility.Collapsed;

                switch (item)
                {
                    case "Sáng theo hiệu ứng":
                        this.effectCard.Visibility = Visibility.Visible;
                        this.staticCard.Visibility = Visibility.Collapsed;

                        ambilightCard.Visibility = Visibility.Collapsed;
                        Ambilightdesk_card.Visibility = Visibility.Collapsed;
                        Ambilightcase.Visibility = Visibility.Collapsed;

                        this.offcard.Visibility = Visibility.Collapsed;
                        this.Auracard.Visibility = Visibility.Collapsed;
                        if (i != null)
                        {
                            if (i.Content.ToString() == "Rainbow Custom Zone")
                            {
                                isEffect = false;
                                //txtTitle.Visibility = Visibility.Visible;
                                //cuszoneIcon.Visibility = Visibility.Visible;
                                Bassbox.Visibility = Visibility.Collapsed;
                                filemaubox.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_1.Visibility = Visibility.Visible;
                                ClrPcker_Background_2.Visibility = Visibility.Visible;
                                ClrPcker_Background_3.Visibility = Visibility.Visible;
                                ClrPcker_Background_4.Visibility = Visibility.Visible;
                                ClrPcker_Background_5.Visibility = Visibility.Visible;
                                ClrPcker_Background_6.Visibility = Visibility.Visible;
                                ClrPcker_Background_7.Visibility = Visibility.Visible;
                                ClrPcker_Background_8.Visibility = Visibility.Visible;
                                bt4.Visibility = Visibility.Visible;
                                bt5.Visibility = Visibility.Visible;


                                //btPlay.Visibility = Visibility.Collapsed;
                                zoebar1.Visibility = Visibility.Collapsed;
                                zoebar2.Visibility = Visibility.Collapsed;
                                zoebar3.Visibility = Visibility.Collapsed;
                                zoebar4.Visibility = Visibility.Collapsed;
                                zoebar5.Visibility = Visibility.Collapsed;
                                zoebar6.Visibility = Visibility.Collapsed;
                                zoebar7.Visibility = Visibility.Collapsed;
                                zoebar8.Visibility = Visibility.Collapsed;
                                zoebar9.Visibility = Visibility.Collapsed;
                                zoebar10.Visibility = Visibility.Collapsed;
                                zoebar11.Visibility = Visibility.Collapsed;
                                zoebar12.Visibility = Visibility.Collapsed;
                                zoebar13.Visibility = Visibility.Collapsed;
                                zoebar14.Visibility = Visibility.Collapsed;
                                zoebar15.Visibility = Visibility.Collapsed;
                                zoebar16.Visibility = Visibility.Collapsed;

                                //txtLed.Visibility = Visibility.Collapsed;
                                //btColor1.Visibility = Visibility.Collapsed;
                                //btColor2.Visibility = Visibility.Collapsed;
                                //btColor3.Visibility = Visibility.Collapsed;
                                //btColor4.Visibility = Visibility.Collapsed;
                                //btColor5.Visibility = Visibility.Collapsed;
                                //btColor6.Visibility = Visibility.Collapsed;
                                //btColor7.Visibility = Visibility.Collapsed;
                                //btColor8.Visibility = Visibility.Collapsed;
                                //btColor9.Visibility = Visibility.Collapsed;
                                //btColor10.Visibility = Visibility.Collapsed;
                                //btColor11.Visibility = Visibility.Collapsed;
                                //btColor12.Visibility = Visibility.Collapsed;
                                //btColor13.Visibility = Visibility.Collapsed;
                                //btColor14.Visibility = Visibility.Collapsed;
                                //btColor15.Visibility = Visibility.Collapsed;
                                //btColor16.Visibility = Visibility.Collapsed;

                                //txtFreq.Visibility = Visibility.Collapsed;
                                //Music_box_1.Visibility = Visibility.Collapsed;
                                //Music_box_2.Visibility = Visibility.Collapsed;
                                //Music_box_3.Visibility = Visibility.Collapsed;
                                //Music_box_4.Visibility = Visibility.Collapsed;
                                //Music_box_5.Visibility = Visibility.Collapsed;
                                //Music_box_6.Visibility = Visibility.Collapsed;
                                //Music_box_7.Visibility = Visibility.Collapsed;
                                //Music_box_8.Visibility = Visibility.Collapsed;
                                //Music_box_9.Visibility = Visibility.Collapsed;
                                //Music_box_10.Visibility = Visibility.Collapsed;
                                //Music_box_11.Visibility = Visibility.Collapsed;
                                //Music_box_12.Visibility = Visibility.Collapsed;
                                //Music_box_13.Visibility = Visibility.Collapsed;
                                //Music_box_14.Visibility = Visibility.Collapsed;
                                //Music_box_15.Visibility = Visibility.Collapsed;
                                //Music_box_16.Visibility = Visibility.Collapsed;

                                this.customZone.Visibility = Visibility.Visible;
                                this.customZone.Height = 150;
                                filemauchip.Content = filemaubox.Text;
                            }
                            else
                            {
                                this.customZone.Visibility = Visibility.Collapsed;
                                filemauchip.Content = i.Content.ToString();
                            }
                        }
                        break;
                    case "Sáng theo màn hình":
                        this.effectCard.Visibility = Visibility.Collapsed;
                        this.staticCard.Visibility = Visibility.Collapsed;
                        this.customZone.Visibility = Visibility.Collapsed;
                        this.offcard.Visibility = Visibility.Collapsed;
                        this.Auracard.Visibility = Visibility.Collapsed;


                        previewCard.Visibility = Visibility.Visible;
                        if(NavigationChip.SelectedIndex==0)//screen tab selected
                        {
                            ambilightCard.Visibility = Visibility.Visible;
                            Ambilightdesk_card.Visibility = Visibility.Collapsed;
                            Ambilightcase.Visibility = Visibility.Collapsed;
                        }
                        else if(NavigationChip.SelectedIndex==1)// case tab selected
                        {
                            ambilightCard.Visibility = Visibility.Collapsed;
                            Ambilightdesk_card.Visibility = Visibility.Collapsed;
                            Ambilightcase.Visibility = Visibility.Visible;

                        }
                        else if (NavigationChip.SelectedIndex==2)//desk tab selected
                        {
                            ambilightCard.Visibility = Visibility.Collapsed;
                          

                            Ambilightdesk_card.Visibility = Visibility.Visible;
                            Ambilightcase.Visibility = Visibility.Collapsed;
                        }
                        //balanceCard.Visibility = Visibility.Visible;
                        //ledCard.Visibility = Visibility.Visible;
                        
                        //sizescreenCard.Visibility = Visibility.Visible;
                        btnReset.Visibility = Visibility.Visible;
                        break;
                    case "Sáng màu tĩnh":
                        this.effectCard.Visibility = Visibility.Collapsed;
                        this.staticCard.Visibility = Visibility.Visible;
                        this.customZone.Visibility = Visibility.Collapsed;
                        this.offcard.Visibility = Visibility.Collapsed;
                        this.Auracard.Visibility = Visibility.Collapsed;
                        ambilightCard.Visibility = Visibility.Collapsed;
                        Ambilightdesk_card.Visibility = Visibility.Collapsed;
                        Ambilightcase.Visibility = Visibility.Collapsed;
                        break;
                    case "Sáng theo nhạc":
                        this.effectCard.Visibility = Visibility.Visible;
                        this.staticCard.Visibility = Visibility.Collapsed;
                        this.offcard.Visibility = Visibility.Collapsed;
                        this.Auracard.Visibility = Visibility.Collapsed;
                        ambilightCard.Visibility = Visibility.Collapsed;
                        Ambilightdesk_card.Visibility = Visibility.Collapsed;
                        Ambilightcase.Visibility = Visibility.Collapsed;
                        if (i != null)
                        {
                            if (i.Content.ToString() == "Rainbow Custom Zone")
                            {
                                isEffect = true;
                                //txtTitle.Visibility = Visibility.Visible;
                                //cuszoneIcon.Visibility = Visibility.Visible;
                                Bassbox.Visibility = Visibility.Visible;
                                filemaubox.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_1.Visibility = Visibility.Visible;
                                ClrPcker_Background_2.Visibility = Visibility.Visible;
                                ClrPcker_Background_3.Visibility = Visibility.Visible;
                                ClrPcker_Background_4.Visibility = Visibility.Visible;
                                ClrPcker_Background_5.Visibility = Visibility.Visible;
                                ClrPcker_Background_6.Visibility = Visibility.Visible;
                                ClrPcker_Background_7.Visibility = Visibility.Visible;
                                ClrPcker_Background_8.Visibility = Visibility.Visible;
                                bt4.Visibility = Visibility.Visible;
                                bt5.Visibility = Visibility.Visible;


                                //btPlay.Visibility = Visibility.Visible;
                                zoebar1.Visibility = Visibility.Visible;
                                zoebar2.Visibility = Visibility.Visible;
                                zoebar3.Visibility = Visibility.Visible;
                                zoebar4.Visibility = Visibility.Visible;
                                zoebar5.Visibility = Visibility.Visible;
                                zoebar6.Visibility = Visibility.Visible;
                                zoebar7.Visibility = Visibility.Visible;
                                zoebar8.Visibility = Visibility.Visible;
                                zoebar9.Visibility = Visibility.Visible;
                                zoebar10.Visibility = Visibility.Visible;
                                zoebar11.Visibility = Visibility.Visible;
                                zoebar12.Visibility = Visibility.Visible;
                                zoebar13.Visibility = Visibility.Visible;
                                zoebar14.Visibility = Visibility.Visible;
                                zoebar15.Visibility = Visibility.Visible;
                                zoebar16.Visibility = Visibility.Visible;

                                //txtLed.Visibility = Visibility.Visible;
                                //btColor1.Visibility = Visibility.Visible;
                                //btColor2.Visibility = Visibility.Visible;
                                //btColor3.Visibility = Visibility.Visible;
                                //btColor4.Visibility = Visibility.Visible;
                                //btColor5.Visibility = Visibility.Visible;
                                //btColor6.Visibility = Visibility.Visible;
                                //btColor7.Visibility = Visibility.Visible;
                                //btColor8.Visibility = Visibility.Visible;
                                //btColor9.Visibility = Visibility.Visible;
                                //btColor10.Visibility = Visibility.Visible;
                                //btColor11.Visibility = Visibility.Visible;
                                //btColor12.Visibility = Visibility.Visible;
                                //btColor13.Visibility = Visibility.Visible;
                                //btColor14.Visibility = Visibility.Visible;
                                //btColor15.Visibility = Visibility.Visible;
                                //btColor16.Visibility = Visibility.Visible;

                                //txtFreq.Visibility = Visibility.Visible;
                                //Music_box_1.Visibility = Visibility.Visible;
                                //Music_box_2.Visibility = Visibility.Visible;
                                //Music_box_3.Visibility = Visibility.Visible;
                                //Music_box_4.Visibility = Visibility.Visible;
                                //Music_box_5.Visibility = Visibility.Visible;
                                //Music_box_6.Visibility = Visibility.Visible;
                                //Music_box_7.Visibility = Visibility.Visible;
                                //Music_box_8.Visibility = Visibility.Visible;
                                //Music_box_9.Visibility = Visibility.Visible;
                                //Music_box_10.Visibility = Visibility.Visible;
                                //Music_box_11.Visibility = Visibility.Visible;
                                //Music_box_12.Visibility = Visibility.Visible;
                                //Music_box_13.Visibility = Visibility.Visible;
                                //Music_box_14.Visibility = Visibility.Visible;
                                //Music_box_15.Visibility = Visibility.Visible;
                                //Music_box_16.Visibility = Visibility.Visible;





                                this.customZone.Visibility = Visibility.Visible;
                                this.customZone.Height = 349;
                                filemauchip.Content = filemaubox.Text;
                            }
                            else
                            {
                                //txtTitle.Visibility = Visibility.Visible;
                                //cuszoneIcon.Visibility = Visibility.Visible;
                                Bassbox.Visibility = Visibility.Visible;
                                filemaubox.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_1.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_2.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_3.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_4.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_5.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_6.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_7.Visibility = Visibility.Collapsed;
                                ClrPcker_Background_8.Visibility = Visibility.Collapsed;
                                bt4.Visibility = Visibility.Visible;
                                bt5.Visibility = Visibility.Visible;


                                //btPlay.Visibility = Visibility.Visible;
                                zoebar1.Visibility = Visibility.Visible;
                                zoebar2.Visibility = Visibility.Visible;
                                zoebar3.Visibility = Visibility.Visible;
                                zoebar4.Visibility = Visibility.Visible;
                                zoebar5.Visibility = Visibility.Visible;
                                zoebar6.Visibility = Visibility.Visible;
                                zoebar7.Visibility = Visibility.Visible;
                                zoebar8.Visibility = Visibility.Visible;
                                zoebar9.Visibility = Visibility.Visible;
                                zoebar10.Visibility = Visibility.Visible;
                                zoebar11.Visibility = Visibility.Visible;
                                zoebar12.Visibility = Visibility.Visible;
                                zoebar13.Visibility = Visibility.Visible;
                                zoebar14.Visibility = Visibility.Visible;
                                zoebar15.Visibility = Visibility.Visible;
                                zoebar16.Visibility = Visibility.Visible;

                                //txtLed.Visibility = Visibility.Visible;
                                //btColor1.Visibility = Visibility.Visible;
                                //btColor2.Visibility = Visibility.Visible;
                                //btColor3.Visibility = Visibility.Visible;
                                //btColor4.Visibility = Visibility.Visible;
                                //btColor5.Visibility = Visibility.Visible;
                                //btColor6.Visibility = Visibility.Visible;
                                //btColor7.Visibility = Visibility.Visible;
                                //btColor8.Visibility = Visibility.Visible;
                                //btColor9.Visibility = Visibility.Visible;
                                //btColor10.Visibility = Visibility.Visible;
                                //btColor11.Visibility = Visibility.Visible;
                                //btColor12.Visibility = Visibility.Visible;
                                //btColor13.Visibility = Visibility.Visible;
                                //btColor14.Visibility = Visibility.Visible;
                                //btColor15.Visibility = Visibility.Visible;
                                //btColor16.Visibility = Visibility.Visible;

                                //txtFreq.Visibility = Visibility.Visible;
                                //Music_box_1.Visibility = Visibility.Visible;
                                //Music_box_2.Visibility = Visibility.Visible;
                                //Music_box_3.Visibility = Visibility.Visible;
                                //Music_box_4.Visibility = Visibility.Visible;
                                //Music_box_5.Visibility = Visibility.Visible;
                                //Music_box_6.Visibility = Visibility.Visible;
                                //Music_box_7.Visibility = Visibility.Visible;
                                //Music_box_8.Visibility = Visibility.Visible;
                                //Music_box_9.Visibility = Visibility.Visible;
                                //Music_box_10.Visibility = Visibility.Visible;
                                //Music_box_11.Visibility = Visibility.Visible;
                                //Music_box_12.Visibility = Visibility.Visible;
                                //Music_box_13.Visibility = Visibility.Visible;
                                //Music_box_14.Visibility = Visibility.Visible;
                                //Music_box_15.Visibility = Visibility.Visible;
                                //Music_box_16.Visibility = Visibility.Visible;

                                this.customZone.Visibility = Visibility.Visible;
                                this.customZone.Height = 349;
                                filemauchip.Content = i.Content.ToString();
                            }
                        }
                        break;
                    case "Đồng bộ Mainboard":
                        this.effectCard.Visibility = Visibility.Collapsed;
                        this.staticCard.Visibility = Visibility.Collapsed;
                        this.customZone.Visibility = Visibility.Collapsed;
                        this.offcard.Visibility = Visibility.Collapsed;
                        this.Auracard.Visibility = Visibility.Visible;
                        ambilightCard.Visibility = Visibility.Collapsed;
                        Ambilightdesk_card.Visibility = Visibility.Collapsed;
                        Ambilightcase.Visibility = Visibility.Collapsed;


                        break;
                    case "Tắt":
                        this.effectCard.Visibility = Visibility.Collapsed;
                        this.staticCard.Visibility = Visibility.Collapsed;
                        this.customZone.Visibility = Visibility.Collapsed;
                        this.offcard.Visibility = Visibility.Visible;
                        this.Auracard.Visibility = Visibility.Collapsed;
                        ambilightCard.Visibility = Visibility.Collapsed;
                        Ambilightdesk_card.Visibility = Visibility.Collapsed;
                        Ambilightcase.Visibility = Visibility.Collapsed;
                        break;
                }
            }

        }

       

        //private void ChangeLighting(object sender, SelectionChangedEventArgs e)
        //{
        //    var settingsViewModel = DataContext as SettingsViewModel;
        //    var item = (Label)numberScreen.SelectedItem;
        //    if (settingsViewModel != null)
        //        if (item.Content.ToString() == "Non Lighting")
        //            settingsViewModel.Settings.UseLinearLighting = false;
        //        else
        //            settingsViewModel.Settings.UseLinearLighting = true;
        //}

        private void ClrPcker_Background_1_SelectedColorChanged(object sender, RoutedEventArgs e)
        {

            lines2[0] = ClrPcker_Background_1.SelectedColor.ToString();


        }
        private void ClrPcker_Background_2_SelectedColorChanged(object sender, RoutedEventArgs e)
        {


            lines2[1] = ClrPcker_Background_2.SelectedColor.ToString();


        }
        private void ClrPcker_Background_3_SelectedColorChanged(object sender, RoutedEventArgs e)
        {


            lines2[2] = ClrPcker_Background_3.SelectedColor.ToString();


        }
        private void ClrPcker_Background_4_SelectedColorChanged(object sender, RoutedEventArgs e)
        {

            lines2[3] = ClrPcker_Background_4.SelectedColor.ToString();

        }
        private void ClrPcker_Background_5_SelectedColorChanged(object sender, RoutedEventArgs e)
        {




            lines2[4] = ClrPcker_Background_5.SelectedColor.ToString();



        }
        private void ClrPcker_Background_6_SelectedColorChanged(object sender, RoutedEventArgs e)
        {


            lines2[5] = ClrPcker_Background_6.SelectedColor.ToString();



        }
        private void ClrPcker_Background_7_SelectedColorChanged(object sender, RoutedEventArgs e)
        {


            lines2[6] = ClrPcker_Background_7.SelectedColor.ToString();


        }
        private void ClrPcker_Background_8_SelectedColorChanged(object sender, RoutedEventArgs e)
        {


            lines2[7] = ClrPcker_Background_8.SelectedColor.ToString();


        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            SaveFileDialog FileMau = new SaveFileDialog();
            FileMau.CreatePrompt = true;
            FileMau.OverwritePrompt = true;

            FileMau.Title = "Lưu file màu";
            FileMau.FileName = "Ambino_Color_Palette";
            FileMau.CheckFileExists = false;
            FileMau.CheckPathExists = true;
            FileMau.DefaultExt = "txt";
            FileMau.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            FileMau.InitialDirectory =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FileMau.RestoreDirectory = true;
            if (FileMau.ShowDialog() == true)
            {
                
           
                
                   
                lines2[8] = Convert.ToString(Music_box_1.SelectedIndex);
                lines2[9] = Convert.ToString(Music_box_2.SelectedIndex);
                lines2[10] = Convert.ToString(Music_box_3.SelectedIndex);
                lines2[11] = Convert.ToString(Music_box_4.SelectedIndex);
                lines2[12] = Convert.ToString(Music_box_5.SelectedIndex);
                lines2[13] = Convert.ToString(Music_box_6.SelectedIndex);
                lines2[14] = Convert.ToString(Music_box_7.SelectedIndex);
                lines2[15] = Convert.ToString(Music_box_8.SelectedIndex);
                lines2[16] = Convert.ToString(Music_box_9.SelectedIndex);
                lines2[17] = Convert.ToString(Music_box_10.SelectedIndex);
                lines2[18] = Convert.ToString(Music_box_11.SelectedIndex);
                lines2[19] = Convert.ToString(Music_box_12.SelectedIndex);
                lines2[20] = Convert.ToString(Music_box_13.SelectedIndex);
                lines2[21] = Convert.ToString(Music_box_14.SelectedIndex);
                lines2[22] = Convert.ToString(Music_box_15.SelectedIndex);
                lines2[23] = Convert.ToString(Music_box_16.SelectedIndex);


                System.IO.File.WriteAllLines(FileMau.FileName, lines2);

                string[] deviceInfo = new string[4];
                deviceInfo[0] = effectbox.SelectedIndex.ToString();
                deviceInfo[1] = method.SelectedIndex.ToString();
                deviceInfo[2] = speed.Value.ToString();
                deviceInfo[3] = sin.Value.ToString();
                System.IO.File.AppendAllLines(FileMau.FileName, deviceInfo);
            
                filemaubox.Text = FileMau.SafeFileName;
                filemauchip.Content = FileMau.SafeFileName;
                //  UserSettings.filemau = filemaubox.Text;

            }
        }

        private void CountingButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (DFU.Badge == null || Equals(DFU.Badge, string.Empty))
                DFU.Badge = 0;

            var next = int.Parse(DFU.Badge.ToString() ?? "0") + 1;
            if(next==15)
            {
                _ = DFU_func();
            }

            DFU.Badge = next < 21 ? (object)next : null;

            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            OpenFileDialog filemau = new OpenFileDialog();
            filemau.Title = "Chọn file màu";
            filemau.CheckFileExists = true;
            filemau.CheckPathExists = true;
            filemau.DefaultExt = "txt";
            filemau.Filter = "Text files (*.txt)|*.txt";
            filemau.FilterIndex = 2;
            filemau.ShowDialog();

            if (!string.IsNullOrEmpty(filemau.FileName) && File.Exists(filemau.FileName))
            {
                lines = System.IO.File.ReadAllLines(filemau.FileName);
                int z = System.IO.File.ReadAllLines(filemau.FileName).Count();
                if (z >= 24)
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            ClrPcker_Background_1.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[0]);
                            ClrPcker_Background_2.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[1]);
                            ClrPcker_Background_3.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[2]);
                            ClrPcker_Background_4.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[3]);
                            ClrPcker_Background_5.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[4]);
                            ClrPcker_Background_6.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[5]);
                            ClrPcker_Background_7.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[6]);
                            ClrPcker_Background_8.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[7]);
                            filemaubox.Text = filemau.SafeFileName;
                            filemauchip.Content = filemau.SafeFileName;
                            //  UserSettings.filemau = filemaubox.Text;

                        }
                    }
                    for (int j = 8; j < 24; j++)
                    {
                        if (!string.IsNullOrEmpty(lines[j]))
                        {
                            if(musicchip.SelectedItem==Custom)
                            {
                           
                            
                                custom_order_data[0]= Convert.ToInt16(lines[8]);
                                custom_order_data[1] = Convert.ToInt16(lines[9]);
                                custom_order_data[2] = Convert.ToInt16(lines[10]);
                                custom_order_data[3] = Convert.ToInt16(lines[11]);
                                custom_order_data[4] = Convert.ToInt16(lines[12]);
                                custom_order_data[5] = Convert.ToInt16(lines[13]);
                                custom_order_data[6] = Convert.ToInt16(lines[14]);
                                custom_order_data[7] = Convert.ToInt16(lines[15]);
                                custom_order_data[8] = Convert.ToInt16(lines[16]);
                                custom_order_data[9] = Convert.ToInt16(lines[17]);
                                custom_order_data[10] = Convert.ToInt16(lines[18]);
                                custom_order_data[11] = Convert.ToInt16(lines[19]);
                                custom_order_data[12] = Convert.ToInt16(lines[20]);
                                custom_order_data[13] = Convert.ToInt16(lines[21]);
                                custom_order_data[14] = Convert.ToInt16(lines[22]);
                                custom_order_data[15] = Convert.ToInt16(lines[23]);

                                Music_box_1.SelectedIndex = custom_order_data[0];
                                Music_box_2.SelectedIndex = custom_order_data[1];
                                Music_box_3.SelectedIndex = custom_order_data[2];
                                Music_box_4.SelectedIndex = custom_order_data[3];
                                Music_box_5.SelectedIndex = custom_order_data[4];
                                Music_box_6.SelectedIndex = custom_order_data[5];
                                Music_box_7.SelectedIndex = custom_order_data[6];
                                Music_box_8.SelectedIndex = custom_order_data[7];
                                Music_box_9.SelectedIndex = custom_order_data[8];
                                Music_box_10.SelectedIndex = custom_order_data[9];
                                Music_box_11.SelectedIndex = custom_order_data[10];
                                Music_box_12.SelectedIndex = custom_order_data[11];
                                Music_box_13.SelectedIndex = custom_order_data[13];
                                Music_box_15.SelectedIndex = custom_order_data[14];
                                Music_box_16.SelectedIndex = custom_order_data[15];



                            }
                        }
                    }
                    if (z == 28)
                    {
                        effectbox.SelectedIndex = Convert.ToInt16(lines[24]);
                        method.SelectedIndex = Convert.ToInt16(lines[25]);
                        speed.Value = Convert.ToInt16(lines[26]);
                        sin.Value = Convert.ToInt16(lines[27]);
                    }
                }
            }
        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Music_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (Music_box_1.SelectedIndex >= 0)
            //{
            //    custom_order_data[0] = Music_box_1.SelectedIndex;
            //    custom_order_data[1] = Music_box_2.SelectedIndex;
            //    custom_order_data[2] = Music_box_3.SelectedIndex;
            //    custom_order_data[3] = Music_box_4.SelectedIndex;
            //    custom_order_data[4] = Music_box_5.SelectedIndex;
            //    custom_order_data[5] = Music_box_6.SelectedIndex;
            //    custom_order_data[6] = Music_box_7.SelectedIndex;
            //    custom_order_data[7] = Music_box_8.SelectedIndex;
            //    custom_order_data[8] = Music_box_9.SelectedIndex;
            //    custom_order_data[9] = Music_box_10.SelectedIndex;
            //    custom_order_data[10] = Music_box_11.SelectedIndex;
            //    custom_order_data[11] = Music_box_12.SelectedIndex;
            //    custom_order_data[12] = Music_box_13.SelectedIndex;
            //    custom_order_data[13] = Music_box_14.SelectedIndex;
            //    custom_order_data[14] = Music_box_15.SelectedIndex;
            //    custom_order_data[15] = Music_box_16.SelectedIndex;
            //}


        }

        private void Devicebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BassWasapi.BASS_WASAPI_Free();  // every time we change the audio device, Bass wasapi need to init again with default value
            Bass.BASS_Free();
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (Bassbox.SelectedIndex >= 0)
            {
                var array = (Bassbox.Items[Bassbox.SelectedIndex] as string).Split(' ');
                devindex = Convert.ToInt32(array[0]);
            }
            else
            { devindex = -1; }

            //  UserSettings.devindex = devindex;
            bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero); // this is the function to init the device according to device index
            //  BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            if (!result)
            {
                var error = Bass.BASS_ErrorGetCode();
                MessageBox.Show(error.ToString());
            }
            else
            {
                //_initialized = true;
                //  Bassbox.IsEnabled = false;
            }
            //BassWasapi.BASS_WASAPI_Init(-3, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);

            BassWasapi.BASS_WASAPI_Start();


        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            //Music_box_1.SelectedIndex = 0;
            //Music_box_2.SelectedIndex = 1;
            //Music_box_3.SelectedIndex = 2;
            //Music_box_4.SelectedIndex = 3;
            //Music_box_5.SelectedIndex = 4;
            //Music_box_6.SelectedIndex = 5;
            //Music_box_7.SelectedIndex = 6;
            //Music_box_8.SelectedIndex = 7;
            //Music_box_9.SelectedIndex = 8;
            //Music_box_10.SelectedIndex = 9;
            //Music_box_11.SelectedIndex = 10;
            //Music_box_12.SelectedIndex = 11;
            //Music_box_13.SelectedIndex = 12;
            //Music_box_14.SelectedIndex = 13;
            //Music_box_15.SelectedIndex = 14;
            //Music_box_16.SelectedIndex = 15;
        }

        private void ClrPcker_Background_1_Copy2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ClrPcker_Background_1_Copy1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void ClrPcker_Background_1_Copy3_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Screenbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screenbox.SelectedIndex == 0)
            {

                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                //widthslide.IsEnabled = false;
                //heightslide.IsEnabled = false;
                //offsetslide.IsEnabled = false;
                ///  if (D3vicE == 0)
                // {
                width.Text = "10";
                height.Text = "6";
                offset.Text = "9";
                //  }
                //  else if (D3vicE == 1)
                // {
                //   width.Text = "10";
                //  height.Text = "6";
                //  offset.Text = "9";




            }
            else if (screenbox.SelectedIndex == 1)
            {

                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                //widthslide.IsEnabled = false;
                //heightslide.IsEnabled = false;
                //offsetslide.IsEnabled = false;
                ///  if (D3vicE == 0)
                // {
                width.Text = "11";
                height.Text = "6";
                offset.Text = "10";
                //  }
                //  else if (D3vicE == 1)
                // {
                //   width.Text = "10";
                //  height.Text = "6";
                //  offset.Text = "9";




            }
            else if (screenbox.SelectedIndex == 2)
            {
                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                //widthslide.IsEnabled = false;
                //heightslide.IsEnabled = false;
                //offsetslide.IsEnabled = false;
                width.Text = "14";
                height.Text = "6";
                offset.Text = "13";
            }
            else if (screenbox.SelectedIndex == 3)
            {
                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                //widthslide.IsEnabled = false;
                //heightslide.IsEnabled = false;
                //offsetslide.IsEnabled = false;
                width.Text = "14";
                height.Text = "7";
                offset.Text = "13";
            }
            else if (screenbox.SelectedIndex == 4)
            {
                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                //widthslide.IsEnabled = false;
                //heightslide.IsEnabled = false;
                //offsetslide.IsEnabled = false;
                width.Text = "15";
                height.Text = "7";
                offset.Text = "14";
            }
            else if (screenbox.SelectedIndex == 5)
            {
                width.IsEnabled = true;
                height.IsEnabled = true;
                offset.IsEnabled = true;
                //widthslide.IsEnabled = true;
                //heightslide.IsEnabled = true;
                //offsetslide.IsEnabled = true;
            }

        }

        private void Screenbox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screenbox2.SelectedIndex == 0)
            {

                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;



                width2.Text = "10";
                height2.Text = "6";
                //offset.Text = "9";





            }
            else if (screenbox2.SelectedIndex == 1)
            {

                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;



                width2.Text = "11";
                height2.Text = "6";
                offset.Text = "9";





            }
            else if (screenbox2.SelectedIndex == 2)
            {
                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;
                //    offsetslide.IsEnabled = false;
                width2.Text = "14";
                height2.Text = "6";
                //offset.Text = "13";


            }
            else if (screenbox2.SelectedIndex == 3)
            {
                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;
                //    offsetslide.IsEnabled = false;

                width2.Text = "14";
                height2.Text = "7";
                //offset.Text = "13";

            }
            else if (screenbox2.SelectedIndex == 4)
            {
                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;
                //    offsetslide.IsEnabled = false;
                width2.Text = "15";
                height2.Text = "7";


            }
            else if (screenbox2.SelectedIndex == 5)
            {
                width2.IsEnabled = true;
                height2.IsEnabled = true;
                offset.IsEnabled = true;
                //    widthslide.IsEnabled = true;
                //    heightslide.IsEnabled = true;
                //    offsetslide.IsEnabled = true;
            }

        }

        private void SettingScreen_Click(object sender, RoutedEventArgs e)
        {
            var status = (sender as ToggleButton).IsChecked;
            if (status != null)
            {
                if (status == true)
                {
                    // Code for Checked state
                    effectCard.Visibility = Visibility.Collapsed;
                    staticCard.Visibility = Visibility.Collapsed;
                    customZone.Visibility = Visibility.Collapsed;


                    previewCard.Visibility = Visibility.Visible;
                    //balanceCard.Visibility = Visibility.Visible;
                    //ledCard.Visibility = Visibility.Visible;
                    ambilightCard.Visibility = Visibility.Visible;
                    //sizescreenCard.Visibility = Visibility.Visible;


                    btnReset.IsEnabled = true;
                }
                else
                {
                    // Code for Un-Checked state
                    previewCard.Visibility = Visibility.Collapsed;
                    //balanceCard.Visibility = Visibility.Collapsed;
                    //ledCard.Visibility = Visibility.Collapsed;
                    ambilightCard.Visibility = Visibility.Collapsed;
                    //sizescreenCard.Visibility = Visibility.Collapsed;


                    btnReset.IsEnabled = false;


                    var item = (Label)screenefectbox.SelectedItem;
                    var i = (Label)effectbox.SelectedItem;
                    if (item != null)
                    {
                        switch (item.Content.ToString())
                        {
                            case "Sáng theo hiệu ứng":
                                this.effectCard.Visibility = Visibility.Visible;
                                this.staticCard.Visibility = Visibility.Collapsed;
                                if (i != null)
                                {
                                    if (i.Content.ToString() == "Rainbow Custom Zone")
                                    {
                                        isEffect = false;
                                        //txtTitle.Visibility = Visibility.Visible;
                                        //cuszoneIcon.Visibility = Visibility.Visible;
                                        Bassbox.Visibility = Visibility.Collapsed;
                                        filemaubox.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_1.Visibility = Visibility.Visible;
                                        ClrPcker_Background_2.Visibility = Visibility.Visible;
                                        ClrPcker_Background_3.Visibility = Visibility.Visible;
                                        ClrPcker_Background_4.Visibility = Visibility.Visible;
                                        ClrPcker_Background_5.Visibility = Visibility.Visible;
                                        ClrPcker_Background_6.Visibility = Visibility.Visible;
                                        ClrPcker_Background_7.Visibility = Visibility.Visible;
                                        ClrPcker_Background_8.Visibility = Visibility.Visible;
                                        bt4.Visibility = Visibility.Visible;
                                        bt5.Visibility = Visibility.Visible;


                                        //btPlay.Visibility = Visibility.Collapsed;
                                        zoebar1.Visibility = Visibility.Collapsed;
                                        zoebar2.Visibility = Visibility.Collapsed;
                                        zoebar3.Visibility = Visibility.Collapsed;
                                        zoebar4.Visibility = Visibility.Collapsed;
                                        zoebar5.Visibility = Visibility.Collapsed;
                                        zoebar6.Visibility = Visibility.Collapsed;
                                        zoebar7.Visibility = Visibility.Collapsed;
                                        zoebar8.Visibility = Visibility.Collapsed;
                                        zoebar9.Visibility = Visibility.Collapsed;
                                        zoebar10.Visibility = Visibility.Collapsed;
                                        zoebar11.Visibility = Visibility.Collapsed;
                                        zoebar12.Visibility = Visibility.Collapsed;
                                        zoebar13.Visibility = Visibility.Collapsed;
                                        zoebar14.Visibility = Visibility.Collapsed;
                                        zoebar15.Visibility = Visibility.Collapsed;
                                        zoebar16.Visibility = Visibility.Collapsed;

                                        //txtLed.Visibility = Visibility.Collapsed;
                                        //btColor1.Visibility = Visibility.Collapsed;
                                        //btColor2.Visibility = Visibility.Collapsed;
                                        //btColor3.Visibility = Visibility.Collapsed;
                                        //btColor4.Visibility = Visibility.Collapsed;
                                        //btColor5.Visibility = Visibility.Collapsed;
                                        //btColor6.Visibility = Visibility.Collapsed;
                                        //btColor7.Visibility = Visibility.Collapsed;
                                        //btColor8.Visibility = Visibility.Collapsed;
                                        //btColor9.Visibility = Visibility.Collapsed;
                                        //btColor10.Visibility = Visibility.Collapsed;
                                        //btColor11.Visibility = Visibility.Collapsed;
                                        //btColor12.Visibility = Visibility.Collapsed;
                                        //btColor13.Visibility = Visibility.Collapsed;
                                        //btColor14.Visibility = Visibility.Collapsed;
                                        //btColor15.Visibility = Visibility.Collapsed;
                                        //btColor16.Visibility = Visibility.Collapsed;

                                        //txtFreq.Visibility = Visibility.Collapsed;
                                        //Music_box_1.Visibility = Visibility.Collapsed;
                                        //Music_box_2.Visibility = Visibility.Collapsed;
                                        //Music_box_3.Visibility = Visibility.Collapsed;
                                        //Music_box_4.Visibility = Visibility.Collapsed;
                                        //Music_box_5.Visibility = Visibility.Collapsed;
                                        //Music_box_6.Visibility = Visibility.Collapsed;
                                        //Music_box_7.Visibility = Visibility.Collapsed;
                                        //Music_box_8.Visibility = Visibility.Collapsed;
                                        //Music_box_9.Visibility = Visibility.Collapsed;
                                        //Music_box_10.Visibility = Visibility.Collapsed;
                                        //Music_box_11.Visibility = Visibility.Collapsed;
                                        //Music_box_12.Visibility = Visibility.Collapsed;
                                        //Music_box_13.Visibility = Visibility.Collapsed;
                                        //Music_box_14.Visibility = Visibility.Collapsed;
                                        //Music_box_15.Visibility = Visibility.Collapsed;
                                        //Music_box_16.Visibility = Visibility.Collapsed;

                                        this.customZone.Visibility = Visibility.Visible;
                                        this.customZone.Height = 150;
                                    }
                                    else
                                    {
                                        this.customZone.Visibility = Visibility.Collapsed;
                                    }
                                }
                                break;
                            case "Sáng theo màn hình":
                                this.effectCard.Visibility = Visibility.Collapsed;
                                this.staticCard.Visibility = Visibility.Collapsed;
                                this.customZone.Visibility = Visibility.Collapsed;
                                break;
                            case "Sáng màu tĩnh":
                                this.effectCard.Visibility = Visibility.Collapsed;
                                this.staticCard.Visibility = Visibility.Visible;
                                this.customZone.Visibility = Visibility.Collapsed;
                                break;
                            case "Sáng theo nhạc":
                                this.effectCard.Visibility = Visibility.Visible;
                                this.staticCard.Visibility = Visibility.Collapsed;
                                if (i != null)
                                {
                                    if (i.Content.ToString() == "Rainbow Custom Zone")
                                    {
                                        isEffect = true;
                                        //txtTitle.Visibility = Visibility.Visible;
                                        //cuszoneIcon.Visibility = Visibility.Visible;
                                        Bassbox.Visibility = Visibility.Visible;
                                        filemaubox.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_1.Visibility = Visibility.Visible;
                                        ClrPcker_Background_2.Visibility = Visibility.Visible;
                                        ClrPcker_Background_3.Visibility = Visibility.Visible;
                                        ClrPcker_Background_4.Visibility = Visibility.Visible;
                                        ClrPcker_Background_5.Visibility = Visibility.Visible;
                                        ClrPcker_Background_6.Visibility = Visibility.Visible;
                                        ClrPcker_Background_7.Visibility = Visibility.Visible;
                                        ClrPcker_Background_8.Visibility = Visibility.Visible;
                                        bt4.Visibility = Visibility.Visible;
                                        bt5.Visibility = Visibility.Visible;
                                        //equalizer.Visibility = Visibility.Visible;
                                        //presetcombobox.Visibility = Visibility.Visible;
                                        //presettext.Visibility = Visibility.Visible;


                                        //btPlay.Visibility = Visibility.Visible;
                                        zoebar1.Visibility = Visibility.Visible;
                                        zoebar2.Visibility = Visibility.Visible;
                                        zoebar3.Visibility = Visibility.Visible;
                                        zoebar4.Visibility = Visibility.Visible;
                                        zoebar5.Visibility = Visibility.Visible;
                                        zoebar6.Visibility = Visibility.Visible;
                                        zoebar7.Visibility = Visibility.Visible;
                                        zoebar8.Visibility = Visibility.Visible;
                                        zoebar9.Visibility = Visibility.Visible;
                                        zoebar10.Visibility = Visibility.Visible;
                                        zoebar11.Visibility = Visibility.Visible;
                                        zoebar12.Visibility = Visibility.Visible;
                                        zoebar13.Visibility = Visibility.Visible;
                                        zoebar14.Visibility = Visibility.Visible;
                                        zoebar15.Visibility = Visibility.Visible;
                                        zoebar16.Visibility = Visibility.Visible;

                                        //txtLed.Visibility = Visibility.Visible;
                                        //btColor1.Visibility = Visibility.Visible;
                                        //btColor2.Visibility = Visibility.Visible;
                                        //btColor3.Visibility = Visibility.Visible;
                                        //btColor4.Visibility = Visibility.Visible;
                                        //btColor5.Visibility = Visibility.Visible;
                                        //btColor6.Visibility = Visibility.Visible;
                                        //btColor7.Visibility = Visibility.Visible;
                                        //btColor8.Visibility = Visibility.Visible;
                                        //btColor9.Visibility = Visibility.Visible;
                                        //btColor10.Visibility = Visibility.Visible;
                                        //btColor11.Visibility = Visibility.Visible;
                                        //btColor12.Visibility = Visibility.Visible;
                                        //btColor13.Visibility = Visibility.Visible;
                                        //btColor14.Visibility = Visibility.Visible;
                                        //btColor15.Visibility = Visibility.Visible;
                                        //btColor16.Visibility = Visibility.Visible;

                                        //txtFreq.Visibility = Visibility.Visible;
                                        //Music_box_1.Visibility = Visibility.Visible;
                                        //Music_box_2.Visibility = Visibility.Visible;
                                        //Music_box_3.Visibility = Visibility.Visible;
                                        //Music_box_4.Visibility = Visibility.Visible;
                                        //Music_box_5.Visibility = Visibility.Visible;
                                        //Music_box_6.Visibility = Visibility.Visible;
                                        //Music_box_7.Visibility = Visibility.Visible;
                                        //Music_box_8.Visibility = Visibility.Visible;
                                        //Music_box_9.Visibility = Visibility.Visible;
                                        //Music_box_10.Visibility = Visibility.Visible;
                                        //Music_box_11.Visibility = Visibility.Visible;
                                        //Music_box_12.Visibility = Visibility.Visible;
                                        //Music_box_13.Visibility = Visibility.Visible;
                                        //Music_box_14.Visibility = Visibility.Visible;
                                        //Music_box_15.Visibility = Visibility.Visible;
                                        //Music_box_16.Visibility = Visibility.Visible;





                                        this.customZone.Visibility = Visibility.Visible;
                                        this.customZone.Height = 349;
                                    }
                                    else
                                    {
                                        //txtTitle.Visibility = Visibility.Visible;
                                        //cuszoneIcon.Visibility = Visibility.Visible;
                                        Bassbox.Visibility = Visibility.Visible;
                                        filemaubox.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_1.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_2.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_3.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_4.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_5.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_6.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_7.Visibility = Visibility.Collapsed;
                                        ClrPcker_Background_8.Visibility = Visibility.Collapsed;
                                        bt4.Visibility = Visibility.Visible;
                                        bt5.Visibility = Visibility.Visible;


                                        //btPlay.Visibility = Visibility.Visible;
                                        zoebar1.Visibility = Visibility.Visible;
                                        zoebar2.Visibility = Visibility.Visible;
                                        zoebar3.Visibility = Visibility.Visible;
                                        zoebar4.Visibility = Visibility.Visible;
                                        zoebar5.Visibility = Visibility.Visible;
                                        zoebar6.Visibility = Visibility.Visible;
                                        zoebar7.Visibility = Visibility.Visible;
                                        zoebar8.Visibility = Visibility.Visible;
                                        zoebar9.Visibility = Visibility.Visible;
                                        zoebar10.Visibility = Visibility.Visible;
                                        zoebar11.Visibility = Visibility.Visible;
                                        zoebar12.Visibility = Visibility.Visible;
                                        zoebar13.Visibility = Visibility.Visible;
                                        zoebar14.Visibility = Visibility.Visible;
                                        zoebar15.Visibility = Visibility.Visible;
                                        zoebar16.Visibility = Visibility.Visible;

                                        //txtLed.Visibility = Visibility.Visible;
                                        //btColor1.Visibility = Visibility.Visible;
                                        //btColor2.Visibility = Visibility.Visible;
                                        //btColor3.Visibility = Visibility.Visible;
                                        //btColor4.Visibility = Visibility.Visible;
                                        //btColor5.Visibility = Visibility.Visible;
                                        //btColor6.Visibility = Visibility.Visible;
                                        //btColor7.Visibility = Visibility.Visible;
                                        //btColor8.Visibility = Visibility.Visible;
                                        //btColor9.Visibility = Visibility.Visible;
                                        //btColor10.Visibility = Visibility.Visible;
                                        //btColor11.Visibility = Visibility.Visible;
                                        //btColor12.Visibility = Visibility.Visible;
                                        //btColor13.Visibility = Visibility.Visible;
                                        //btColor14.Visibility = Visibility.Visible;
                                        //btColor15.Visibility = Visibility.Visible;
                                        //btColor16.Visibility = Visibility.Visible;

                                        //txtFreq.Visibility = Visibility.Visible;
                                        //Music_box_1.Visibility = Visibility.Visible;
                                        //Music_box_2.Visibility = Visibility.Visible;
                                        //Music_box_3.Visibility = Visibility.Visible;
                                        //Music_box_4.Visibility = Visibility.Visible;
                                        //Music_box_5.Visibility = Visibility.Visible;
                                        //Music_box_6.Visibility = Visibility.Visible;
                                        //Music_box_7.Visibility = Visibility.Visible;
                                        //Music_box_8.Visibility = Visibility.Visible;
                                        //Music_box_9.Visibility = Visibility.Visible;
                                        //Music_box_10.Visibility = Visibility.Visible;
                                        //Music_box_11.Visibility = Visibility.Visible;
                                        //Music_box_12.Visibility = Visibility.Visible;
                                        //Music_box_13.Visibility = Visibility.Visible;
                                        //Music_box_14.Visibility = Visibility.Visible;
                                        //Music_box_15.Visibility = Visibility.Visible;
                                        //Music_box_16.Visibility = Visibility.Visible;

                                        this.customZone.Visibility = Visibility.Visible;
                                        this.customZone.Height = 349;
                                    }
                                }
                                break;
                            case "Đồng bộ Mainboard":
                                this.effectCard.Visibility = Visibility.Collapsed;
                                this.staticCard.Visibility = Visibility.Collapsed;
                                this.customZone.Visibility = Visibility.Collapsed;
                                break;
                            case "Tắt":
                                this.effectCard.Visibility = Visibility.Collapsed;
                                this.staticCard.Visibility = Visibility.Collapsed;
                                this.customZone.Visibility = Visibility.Collapsed;
                                break;
                        }
                    }
                }
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            sliBlue.Value = 100;
            sliGreen.Value = 100;
            sliRed.Value = 100;
            //numberScreen.SelectedIndex = 0;
            sliBlack.Value = 10;
            txtHeight.Text = "150";
            txtWidth.Text = "150";
            txtLeftRight.Text = "0";
            txtTopBottom.Text = "100";
        }

        private static readonly Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !regex.IsMatch(text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Comportbox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox4.SelectedItem == comportbox2.SelectedItem)
            {
                comportbox2.SelectedItem = "Không có";
            }
            else if (comportbox4.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
        }

        private void Comportbox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox3.SelectedItem == comportbox2.SelectedItem)
            {
                comportbox2.SelectedItem = "Không có";
            }
            else if (comportbox3.SelectedItem == comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }

        }

        private void Comportbox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox2.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
            else if (comportbox2.SelectedItem == comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }
        }

        private void ColorPicker_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

        }

        private void ColorZone_MouseEnter(object sender, MouseEventArgs e)
        {



        }

        private void orange_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picker.Color = ((SolidColorBrush)orange.Background).Color;
        }

        private void colortext_TextChanged(object sender, TextChangedEventArgs e)
        {

            //picker.Color = (Color)ColorConverter.ConvertFromString(colortext.Text);
        }

        private void green_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picker.Color = ((SolidColorBrush)green.Background).Color;
        }

        private void aura_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picker.Color = ((SolidColorBrush)aura.Background).Color;
        }

        private void cyan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picker.Color = ((SolidColorBrush)cyan.Background).Color;
        }

        private void blue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picker.Color = ((SolidColorBrush)blue.Background).Color;
        }

        private void pink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            picker.Color = ((SolidColorBrush)pink.Background).Color;
        }

        private void screenSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screenSelect.SelectedIndex == 0)
            {
                this.screen1setup.Visibility = Visibility.Visible;
                this.screen2setup.Visibility = Visibility.Collapsed;
                this.screen3setup.Visibility = Visibility.Collapsed;

            }
            else if (screenSelect.SelectedIndex == 1)
            {
                this.screen1setup.Visibility = Visibility.Collapsed;
                this.screen2setup.Visibility = Visibility.Visible;
                this.screen3setup.Visibility = Visibility.Collapsed;

            }
            else if(screenSelect.SelectedIndex==2)
            {
                this.screen1setup.Visibility = Visibility.Collapsed;
                this.screen2setup.Visibility = Visibility.Collapsed;
                this.screen3setup.Visibility = Visibility.Visible;
            }
        }

        private void txtWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var width = 150;
            //if (String.IsNullOrEmpty(txtWidth.Text))
            //{
            //    width = Convert.ToByte(txtWidth.Text);
            //}
            //if (width >= 20)
            //{
            //    previewRec.Width = width / 5;
            //}
            //else
            //{
            //    width = 150;

            //    previewRec.Width = width / 5;

            ////}
            //if (UserSettings.SpotWidth >= 50)
            //    previewRec.Width = UserSettings.SpotWidth/5;
        }

        private void txtHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            //var height = 150;
            //if (String.IsNullOrEmpty(txtHeight.Text))
            //{
            //    height = Convert.ToByte(txtHeight.Text);
            //}
            //if (height >= 20)
            //{
            //    previewRec.Height = height / 5;
            //}
            //else
            //{
            //    height = 150;

            //    previewRec.Height = height / 5;

            ////}
            //if( UserSettings.SpotHeight>=50)
            //previewRec.Width = UserSettings.SpotHeight/5;
        }

        private void filemauchip_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog filemau = new OpenFileDialog();
            filemau.Title = "Chọn file màu";
            filemau.CheckFileExists = true;
            filemau.CheckPathExists = true;
            filemau.DefaultExt = "txt";
            filemau.Filter = "Text files (*.txt)|*.txt";
            filemau.FilterIndex = 2;
            filemau.ShowDialog();

            if (!string.IsNullOrEmpty(filemau.FileName) && File.Exists(filemau.FileName))
            {
                lines = System.IO.File.ReadAllLines(filemau.FileName);
                int z = System.IO.File.ReadAllLines(filemau.FileName).Count();
                if (z >= 24)
                {
                    for (int i = 0; i <= 7; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i]))
                        {
                            ClrPcker_Background_1.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[0]);
                            ClrPcker_Background_2.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[1]);
                            ClrPcker_Background_3.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[2]);
                            ClrPcker_Background_4.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[3]);
                            ClrPcker_Background_5.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[4]);
                            ClrPcker_Background_6.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[5]);
                            ClrPcker_Background_7.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[6]);
                            ClrPcker_Background_8.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[7]);
                            filemaubox.Text = filemau.SafeFileName;
                            filemauchip.Content = filemau.SafeFileName;
                            //  UserSettings.filemau = filemaubox.Text;

                        }
                    }
                    for (int j = 8; j < 24; j++)
                    {
                        if (!string.IsNullOrEmpty(lines[j]))
                        {
                            //if (musicchip.SelectedItem == Custom)
                            //{
                            //    Music_box_1.SelectedIndex = Convert.ToInt16(lines[8]);
                            //    Music_box_2.SelectedIndex = Convert.ToInt16(lines[9]);
                            //    Music_box_3.SelectedIndex = Convert.ToInt16(lines[10]);
                            //    Music_box_4.SelectedIndex = Convert.ToInt16(lines[11]);
                            //    Music_box_5.SelectedIndex = Convert.ToInt16(lines[12]);
                            //    Music_box_6.SelectedIndex = Convert.ToInt16(lines[13]);
                            //    Music_box_7.SelectedIndex = Convert.ToInt16(lines[14]);
                            //    Music_box_8.SelectedIndex = Convert.ToInt16(lines[15]);
                            //    Music_box_9.SelectedIndex = Convert.ToInt16(lines[16]);
                            //    Music_box_10.SelectedIndex = Convert.ToInt16(lines[17]);
                            //    Music_box_11.SelectedIndex = Convert.ToInt16(lines[18]);
                            //    Music_box_12.SelectedIndex = Convert.ToInt16(lines[19]);
                            //    Music_box_13.SelectedIndex = Convert.ToInt16(lines[20]);
                            //    Music_box_14.SelectedIndex = Convert.ToInt16(lines[21]);
                            //    Music_box_15.SelectedIndex = Convert.ToInt16(lines[22]);
                            //    Music_box_16.SelectedIndex = Convert.ToInt16(lines[23]);
                            //}
                        }
                    }
                    if (z == 28)
                    {
                        effectbox.SelectedIndex = Convert.ToInt16(lines[24]);
                        method.SelectedIndex = Convert.ToInt16(lines[25]);
                        speed.Value = Convert.ToInt16(lines[26]);
                        sin.Value = Convert.ToInt16(lines[27]);
                    }
                }
            } 
        }

        private void navigachip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            
        }

        private void navichip_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            //int index = screenefectbox.SelectedIndex;
            //int index2 = faneffectbox.SelectedIndex;
            //screenefectbox.SelectedIndex = 0;
            //faneffectbox.SelectedIndex = 0;
            //screenefectbox.SelectedIndex = index;
            //faneffectbox.SelectedIndex = index2;


        }
        string screenNo;

        private void AddProduct(object sender, RoutedEventArgs e)
        {
            Product pro = new Product();
            var settingsViewModel = DataContext as SettingsViewModel;
            bool[] lsBefore = new bool[12];
            lsBefore[0] = settingsViewModel.Settings.Pro11;
            lsBefore[1] = settingsViewModel.Settings.Pro12;
            lsBefore[2] = settingsViewModel.Settings.Pro13;
            lsBefore[3] = settingsViewModel.Settings.Pro14;
            lsBefore[4] = settingsViewModel.Settings.Pro21;


            lsBefore[5] = settingsViewModel.Settings.hasPCI;
            lsBefore[6] = settingsViewModel.Settings.hasUSB;

            pro.SUSB.IsChecked = lsBefore[6];
            pro.SPCI.IsChecked = lsBefore[5];


            if (settingsViewModel.Settings.hasUSBTwo)
            {
                pro.numberScreen.SelectedItem = "2";
                screenNo = "2";
            }
            else
            {
                if (pro.numberScreen.SelectedItem != null)
                    screenNo = pro.numberScreen.SelectedItem.ToString();
                else
                {
                    pro.numberScreen.SelectedItem = "1";
                    screenNo = "1";
                }
            }
            lsBefore[7] = settingsViewModel.Settings.hasUSBTwo;
            lsBefore[8] = settingsViewModel.Settings.hasScreenTwo;

            lsBefore[9] = settingsViewModel.Settings.hasPCISecond;
            lsBefore[10] = settingsViewModel.Settings.hasUSBSecond;

            lsBefore[11] = settingsViewModel.Settings.Pro31;
            pro.DUSB.IsChecked = lsBefore[10];
            pro.DPCI.IsChecked = lsBefore[9];

            pro.ShowDialog();
            if (pro.pressSave)
            {
                IList<ISelectableViewPart> ls = settingsViewModel.BackUpView
                                            .OrderBy(p => p.Order).ToList();
                if (!settingsViewModel.Settings.Pro11 && !settingsViewModel.Settings.Pro12
                    && !settingsViewModel.Settings.Pro13)
                {
                    settingsViewModel.Settings.caseenable = false;
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (ls[j].Order == 24)
                            ls.RemoveAt(j);
                    }
                }
                else
                {
                    settingsViewModel.Settings.caseenable = true;
                }
                if (!settingsViewModel.Settings.Pro21)
                {
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (ls[j].Order == 26)
                            ls.RemoveAt(j);
                    }

                }
                if (!settingsViewModel.Settings.Pro22)
                {
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (ls[j].Order == 27)
                            ls.RemoveAt(j);
                    }

                }
                if (!settingsViewModel.Settings.Pro31)
                {
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (ls[j].Order == 28)
                            ls.RemoveAt(j);
                    }
                }

                //DemoItemsListBox.ItemsSource = ls.OrderBy(p => p.Order).ToList();
                settingsViewModel.SelectedViewPart = settingsViewModel.SelectableViewParts.First();

                //if (settingsViewModel.Settings.hasUSB && pro.numberScreen.SelectedItem != null
                //    && pro.numberScreen.SelectedItem != "1")
                //    settingsViewModel.Settings.hasUSBTwo = true;
                //else
                //    settingsViewModel.Settings.hasUSBTwo = false;
                if (pro.SUSB.IsChecked == true)
                {
                    //pro.SPCI.IsChecked = false;
                    settingsViewModel.Settings.hasUSB = true;
                    settingsViewModel.Settings.hasPCI = false;

                }
                else if (pro.SUSB.IsChecked == false)
                {
                    //pro.SPCI.IsChecked = true;
                    settingsViewModel.Settings.hasUSB = false;
                    settingsViewModel.Settings.hasPCI = true;
                }

                if (pro.DUSB.IsChecked == true)
                {
                    //pro.SPCI.IsChecked = false;
                    settingsViewModel.Settings.hasUSBSecond = true;
                    settingsViewModel.Settings.hasPCISecond = false;

                }
                else if (pro.DUSB.IsChecked == false)
                {
                    //pro.SPCI.IsChecked = true;
                    settingsViewModel.Settings.hasUSBSecond = false;
                    settingsViewModel.Settings.hasPCISecond = true;
                }




                if (settingsViewModel.Settings.hasUSB == true)
                {
                    settingsViewModel.Settings.hasPCI = false;
                    if (settingsViewModel.Settings.screencounter == 0)
                    {
                        settingsViewModel.Settings.hasUSB = true;
                        settingsViewModel.Settings.hasUSBTwo = false;

                    }
                    else if (settingsViewModel.Settings.screencounter == 1)
                    {
                        settingsViewModel.Settings.hasUSB = true;
                        settingsViewModel.Settings.hasUSBTwo = true;
                    }
                    else if (settingsViewModel.Settings.screencounter == 2)
                    {
                        settingsViewModel.Settings.hasUSB = true;
                        settingsViewModel.Settings.hasUSBTwo = true;
                    }
                }
                else
                {
                    settingsViewModel.Settings.hasPCI = true;
                    if (settingsViewModel.Settings.screencounter == 0)
                    {
                        settingsViewModel.Settings.hasUSB = false;
                        settingsViewModel.Settings.hasUSBTwo = false;
                    }
                    else if (settingsViewModel.Settings.screencounter == 1)
                    {
                        settingsViewModel.Settings.hasUSB = false;
                        settingsViewModel.Settings.hasUSBTwo = false;
                    }
                    else if (settingsViewModel.Settings.screencounter == 2)
                    {
                        settingsViewModel.Settings.hasUSB = false;
                        settingsViewModel.Settings.hasUSBTwo = false;
                    }
                }
                if (settingsViewModel.Settings.hasUSBSecond == true)
                {
                    settingsViewModel.Settings.hasPCISecond = false;

                }
                else
                {
                    settingsViewModel.Settings.hasPCISecond = true;

                }




                //if (pro.numberScreen.SelectedItem != null && pro.numberScreen.SelectedItem != "1")
                //    settingsViewModel.Settings.hasScreenTwo = true;
                //else
                //    settingsViewModel.Settings.hasScreenTwo = false;
            }
            else
            {
                settingsViewModel.Settings.Pro11 = lsBefore[0];
                settingsViewModel.Settings.Pro12 = lsBefore[1];
                settingsViewModel.Settings.Pro13 = lsBefore[2];
                settingsViewModel.Settings.Pro14 = lsBefore[3];
                settingsViewModel.Settings.Pro21 = lsBefore[4];

                settingsViewModel.Settings.hasPCI = lsBefore[5];
                settingsViewModel.Settings.hasUSB = lsBefore[6];
                settingsViewModel.Settings.hasUSBTwo = lsBefore[7];
                settingsViewModel.Settings.hasScreenTwo = lsBefore[8];
                settingsViewModel.Settings.hasPCISecond = lsBefore[9];
                settingsViewModel.Settings.hasUSBSecond = lsBefore[10];
                pro.numberScreen.SelectedItem = screenNo;
                settingsViewModel.Settings.Pro31 = lsBefore[11];
            }

        }

        

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void minimize_click(object sender, RoutedEventArgs e)
        {
            
        }

        private void musicchip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (musicchip.SelectedIndex == 0)
            //{
            //    order_data[0] = 2;
            //    order_data[1] = 2;
            //    order_data[2] = 4;
            //    order_data[3] = 4;
            //    order_data[4] = 6;
            //    order_data[5] = 6;
            //    order_data[6] = 8;
            //    order_data[7] = 8;
            //    order_data[8] = 8;
            //    order_data[9] = 8;
            //    order_data[10] = 6;
            //    order_data[11] = 6;
            //    order_data[12] = 4;
            //    order_data[13] = 4;
            //    order_data[14] = 2;
            //    order_data[15] = 2;


            //    Music_box_1.SelectedIndex = order_data[0]-1;
            //    Music_box_2.SelectedIndex = order_data[1]-1 ;
            //    Music_box_3.SelectedIndex = order_data[2]-1 ;
            //    Music_box_4.SelectedIndex = order_data[3]-1 ;
            //    Music_box_5.SelectedIndex = order_data[4]-1 ;
            //    Music_box_6.SelectedIndex = order_data[5]-1 ;
            //    Music_box_7.SelectedIndex = order_data[6]-1 ;
            //    Music_box_8.SelectedIndex = order_data[7]-1 ;
            //    Music_box_9.SelectedIndex = order_data[8]-1 ;
            //    Music_box_10.SelectedIndex = order_data[9]-1 ;
            //    Music_box_11.SelectedIndex = order_data[10]-1 ;
            //    Music_box_12.SelectedIndex = order_data[11]-1 ;
            //    Music_box_13.SelectedIndex = order_data[12]-1 ;
            //    Music_box_14.SelectedIndex = order_data[13]-1 ;
            //    Music_box_15.SelectedIndex = order_data[14]-1 ;
            //    Music_box_16.SelectedIndex = order_data[15]-1 ;
            //}
            //else if (musicchip.SelectedIndex == 1)
            //{
            //    order_data[0] = 2;
            //    order_data[1] = 2;
            //    order_data[2] = 8;
            //    order_data[3] = 8;
            //    order_data[4] = 14;
            //    order_data[5] = 14;
            //    order_data[6] = 15;
            //    order_data[7] = 15;
            //    order_data[8] = 14;
            //    order_data[9] = 14;
            //    order_data[10] = 8;
            //    order_data[11] = 8;
            //    order_data[12] = 2;
            //    order_data[13] = 2;
            //    order_data[14] = 2;
            //    order_data[15] = 2;

            //    Music_box_1.SelectedIndex = order_data[0] - 1;
            //    Music_box_2.SelectedIndex = order_data[1] - 1;
            //    Music_box_3.SelectedIndex = order_data[2] - 1;
            //    Music_box_4.SelectedIndex = order_data[3] - 1;
            //    Music_box_5.SelectedIndex = order_data[4] - 1;
            //    Music_box_6.SelectedIndex = order_data[5] - 1;
            //    Music_box_7.SelectedIndex = order_data[6] - 1;
            //    Music_box_8.SelectedIndex = order_data[7] - 1;
            //    Music_box_9.SelectedIndex = order_data[8] - 1;
            //    Music_box_10.SelectedIndex = order_data[9] - 1;
            //    Music_box_11.SelectedIndex = order_data[10] - 1;
            //    Music_box_12.SelectedIndex = order_data[11] - 1;
            //    Music_box_13.SelectedIndex = order_data[12] - 1;
            //    Music_box_14.SelectedIndex = order_data[13] - 1;
            //    Music_box_15.SelectedIndex = order_data[14] - 1;
            //    Music_box_16.SelectedIndex = order_data[15] - 1;
            //}
            //else if (musicchip.SelectedIndex == 2)
            //{
            //    order_data[0] = 2;
            //    order_data[1] = 2;
            //    order_data[2] = 2;
            //    order_data[3] = 2;
            //    order_data[4] = 2;
            //    order_data[5] = 2;
            //    order_data[6] = 9;
            //    order_data[7] = 9;
            //    order_data[8] = 9;
            //    order_data[9] = 9;
            //    order_data[10] = 2;
            //    order_data[11] = 2;
            //    order_data[12] = 2;
            //    order_data[13] = 2;
            //    order_data[14] = 2;
            //    order_data[15] = 2;

            //    Music_box_1.SelectedIndex = order_data[0] - 1;
            //    Music_box_2.SelectedIndex = order_data[1] - 1;
            //    Music_box_3.SelectedIndex = order_data[2] - 1;
            //    Music_box_4.SelectedIndex = order_data[3] - 1;
            //    Music_box_5.SelectedIndex = order_data[4] - 1;
            //    Music_box_6.SelectedIndex = order_data[5] - 1;
            //    Music_box_7.SelectedIndex = order_data[6] - 1;
            //    Music_box_8.SelectedIndex = order_data[7] - 1;
            //    Music_box_9.SelectedIndex = order_data[8] - 1;
            //    Music_box_10.SelectedIndex = order_data[9] - 1;
            //    Music_box_11.SelectedIndex = order_data[10] - 1;
            //    Music_box_12.SelectedIndex = order_data[11] - 1;
            //    Music_box_13.SelectedIndex = order_data[12] - 1;
            //    Music_box_14.SelectedIndex = order_data[13] - 1;
            //    Music_box_15.SelectedIndex = order_data[14] - 1;
            //    Music_box_16.SelectedIndex = order_data[15] - 1;
            //}
            //else if (musicchip.SelectedIndex == 3)
            //{
            //    order_data[0] = 8;
            //    order_data[1] = 8;
            //    order_data[2] = 8;
            //    order_data[3] = 8;
            //    order_data[4] = 11;
            //    order_data[5] = 11;
            //    order_data[6] = 11;
            //    order_data[7] = 11;
            //    order_data[8] = 13;
            //    order_data[9] = 13;
            //    order_data[10] = 13;
            //    order_data[11] = 13;
            //    order_data[12] = 2;
            //    order_data[13] = 2;
            //    order_data[14] = 2;
            //    order_data[15] = 2;

            //    Music_box_1.SelectedIndex = order_data[0] - 1;
            //    Music_box_2.SelectedIndex = order_data[1] - 1;
            //    Music_box_3.SelectedIndex = order_data[2] - 1;
            //    Music_box_4.SelectedIndex = order_data[3] - 1;
            //    Music_box_5.SelectedIndex = order_data[4] - 1;
            //    Music_box_6.SelectedIndex = order_data[5] - 1;
            //    Music_box_7.SelectedIndex = order_data[6] - 1;
            //    Music_box_8.SelectedIndex = order_data[7] - 1;
            //    Music_box_9.SelectedIndex = order_data[8] - 1;
            //    Music_box_10.SelectedIndex = order_data[9] - 1;
            //    Music_box_11.SelectedIndex = order_data[10] - 1;
            //    Music_box_12.SelectedIndex = order_data[11] - 1;
            //    Music_box_13.SelectedIndex = order_data[12] - 1;
            //    Music_box_14.SelectedIndex = order_data[13] - 1;
            //    Music_box_15.SelectedIndex = order_data[14] - 1;
            //    Music_box_16.SelectedIndex = order_data[15] - 1;
            //}
            //if (musicchip.SelectedIndex == 4)
            //{
            //    order_data[0] = 1;
            //    order_data[1] = 3;
            //    order_data[2] = 5;
            //    order_data[3] = 7;
            //    order_data[4] = 9;
            //    order_data[5] = 11;
            //    order_data[6] = 13;
            //    order_data[7] = 15;
            //    order_data[8] = 15;
            //    order_data[9] = 13;
            //    order_data[10] = 11;
            //    order_data[11] = 9;
            //    order_data[12] = 7;
            //    order_data[13] = 5;
            //    order_data[14] = 3;
            //    order_data[15] = 3;

            //    Music_box_1.SelectedIndex = order_data[0] - 1;
            //    Music_box_2.SelectedIndex = order_data[1] - 1;
            //    Music_box_3.SelectedIndex = order_data[2] - 1;
            //    Music_box_4.SelectedIndex = order_data[3] - 1;
            //    Music_box_5.SelectedIndex = order_data[4] - 1;
            //    Music_box_6.SelectedIndex = order_data[5] - 1;
            //    Music_box_7.SelectedIndex = order_data[6] - 1;
            //    Music_box_8.SelectedIndex = order_data[7] - 1;
            //    Music_box_9.SelectedIndex = order_data[8] - 1;
            //    Music_box_10.SelectedIndex = order_data[9] - 1;
            //    Music_box_11.SelectedIndex = order_data[10] - 1;
            //    Music_box_12.SelectedIndex = order_data[11] - 1;
            //    Music_box_13.SelectedIndex = order_data[12] - 1;
            //    Music_box_14.SelectedIndex = order_data[13] - 1;
            //    Music_box_15.SelectedIndex = order_data[14] - 1;
            //    Music_box_16.SelectedIndex = order_data[15] - 1;
            //}

            //else if(musicchip.SelectedIndex == 5)
            //    {

            //    //Music_box_1.SelectedIndex = custom_order_data[0];
            //    //Music_box_2.SelectedIndex = custom_order_data[1];
            //    //Music_box_3.SelectedIndex = custom_order_data[2];
            //    //Music_box_4.SelectedIndex = custom_order_data[3];
            //    //Music_box_5.SelectedIndex = custom_order_data[4];
            //    //Music_box_6.SelectedIndex = custom_order_data[5];
            //    //Music_box_7.SelectedIndex = custom_order_data[6];
            //    //Music_box_8.SelectedIndex = custom_order_data[7];
            //    //Music_box_9.SelectedIndex = custom_order_data[8];
            //    //Music_box_10.SelectedIndex = custom_order_data[9];
            //    //Music_box_11.SelectedIndex = custom_order_data[10];
            //    //Music_box_12.SelectedIndex = custom_order_data[11];
            //    //Music_box_13.SelectedIndex = custom_order_data[12];
            //    //Music_box_14.SelectedIndex = custom_order_data[13];
            //    //Music_box_15.SelectedIndex = custom_order_data[14];  
            //    //Music_box_16.SelectedIndex = custom_order_data[15];
            //}

            for (int i = 8; i <= 23; i++)
            {
                lines2[i] = Convert.ToString(order_data[i - 8]);
            }
        }

        private void Screen_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if(Screen.Visibility==Visibility.Collapsed)
            //{
            //    Screen.IsSelected = false;
            //}
        }

        private void Case_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if(Case.Visibility==Visibility.Collapsed)
            //{
            //    Case.IsSelected = false;
            //}
        }

        private void Desk_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if(Desk.Visibility==Visibility.Collapsed)
            //{
            //    Desk.IsSelected = false;
            //}
        }

        private void Sample1_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {

        }

        private void OK1_Click(object sender, RoutedEventArgs e)
        {
            order_data[0] = Music_box_1.SelectedIndex;   
        }

        private void screenbox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screenbox3.SelectedIndex == 0)
            {

                width3.IsEnabled = false;
                height3.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;



                width3.Text = "10";
                height3.Text = "6";
                //offset.Text = "9";





            }
            else if (screenbox3.SelectedIndex == 1)
            {

                width3.IsEnabled = false;
                height3.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;



                width3.Text = "11";
                height3.Text = "6";
                offset.Text = "9";





            }
            else if (screenbox3.SelectedIndex == 2)
            {
                width3.IsEnabled = false;
                height3.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;
                //    offsetslide.IsEnabled = false;
                width3.Text = "14";
                height3.Text = "6";
                //offset.Text = "13";


            }
            else if (screenbox3.SelectedIndex == 3)
            {
                width3.IsEnabled = false;
                height3.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;
                //    offsetslide.IsEnabled = false;

                width3.Text = "14";
                height3.Text = "7";
                //offset.Text = "13";

            }
            else if (screenbox3.SelectedIndex == 4)
            {
                width3.IsEnabled = false;
                height3.IsEnabled = false;
                offset.IsEnabled = false;
                //    widthslide2.IsEnabled = false;
                //    heightslide2.IsEnabled = false;
                //    offsetslide.IsEnabled = false;
                width3.Text = "15";
                height3.Text = "7";


            }
            else if (screenbox3.SelectedIndex == 5)
            {
                width3.IsEnabled = true;
                height3.IsEnabled = true;
                offset.IsEnabled = true;
                //    widthslide.IsEnabled = true;
                //    heightslide.IsEnabled = true;
                //    offsetslide.IsEnabled = true;
            }
        }

        private void OK2_Click(object sender, RoutedEventArgs e)
        {
            order_data[1] = Music_box_2.SelectedIndex;
        }

        private void OK3_Click(object sender, RoutedEventArgs e)
        {
            order_data[2] = Music_box_3.SelectedIndex;
        }

        private void OK4_Click(object sender, RoutedEventArgs e)
        {
            order_data[3] = Music_box_4.SelectedIndex;
        }

        private void OK5_Click(object sender, RoutedEventArgs e)
        {
            order_data[4] = Music_box_5.SelectedIndex;
        }

        private void OK6_Click(object sender, RoutedEventArgs e)
        {
            order_data[5] = Music_box_6.SelectedIndex;
        }

        private void OK7_Click(object sender, RoutedEventArgs e)
        {
            order_data[6] = Music_box_7.SelectedIndex;
        }

        private void OK8_Click(object sender, RoutedEventArgs e)
        {
            order_data[7] = Music_box_8.SelectedIndex;
        }

        private void OK9_Click(object sender, RoutedEventArgs e)
        {
            order_data[8] = Music_box_9.SelectedIndex;
        }

        private void OK10_Click(object sender, RoutedEventArgs e)
        {
            order_data[9] = Music_box_10.SelectedIndex;
        }

        private void OK11_Click(object sender, RoutedEventArgs e)
        {
            order_data[10] = Music_box_11.SelectedIndex;
        }

        private void OK12_Click(object sender, RoutedEventArgs e)
        {
            order_data[11] = Music_box_12.SelectedIndex;
        }

        private void OK13_Click(object sender, RoutedEventArgs e)
        {
            order_data[12] = Music_box_13.SelectedIndex;
        }

        private void OK14_Click(object sender, RoutedEventArgs e)
        {
            order_data[13] = Music_box_14.SelectedIndex;
        }

        private void OK15_Click(object sender, RoutedEventArgs e)
        {
            order_data[14] = Music_box_15.SelectedIndex;
        }

        private void OK16_Click(object sender, RoutedEventArgs e)
        {
            order_data[15] = Music_box_16.SelectedIndex;
        }

        private void OKPort_Click(object sender, RoutedEventArgs e)
        {

        }










        //private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    var value = Convert.ToByte(Bluance.Value);
        //    Brush Balance = new SolidColorBrush(Color.FromRgb(255, 255, value));
        //    previewRec.Fill = Balance;
        //}
    }
}
