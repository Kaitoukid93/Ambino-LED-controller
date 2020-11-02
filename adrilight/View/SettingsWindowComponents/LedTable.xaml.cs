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
    public partial class LedTable : UserControl
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
        private float[] _fft;
        public bool isEffect { get; set; }

        public LedTable()
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

            if (comportbox4.SelectedItem == null)
            {
                comportbox4.SelectedItem = "Không có";
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
            if (Music_box_1.SelectedIndex >= 0)
            {
                order_data[0] = Music_box_1.SelectedIndex;
                order_data[1] = Music_box_2.SelectedIndex;
                order_data[2] = Music_box_3.SelectedIndex;
                order_data[3] = Music_box_4.SelectedIndex;
                order_data[4] = Music_box_5.SelectedIndex;
                order_data[5] = Music_box_6.SelectedIndex;
                order_data[6] = Music_box_7.SelectedIndex;
                order_data[7] = Music_box_8.SelectedIndex;
                order_data[8] = Music_box_9.SelectedIndex;
                order_data[9] = Music_box_10.SelectedIndex;
                order_data[10] = Music_box_11.SelectedIndex;
                order_data[11] = Music_box_12.SelectedIndex;
                order_data[12] = Music_box_13.SelectedIndex;
                order_data[13] = Music_box_14.SelectedIndex;
                order_data[14] = Music_box_15.SelectedIndex;
                order_data[15] = Music_box_16.SelectedIndex;
            }
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
        public static byte DFU = 0;


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
            for (i = 0; i < 16; i++)
            {
                if (order_data[i] >= 0)

                    output_spectrumdata[i] = Convert.ToByte(spectrumdata[order_data[i]]); // Re-Arrange the value to match the order of LEDs
                zoebar1.Value = spectrumdata[order_data[0]];
                zoebar2.Value = spectrumdata[order_data[1]];
                zoebar3.Value = spectrumdata[order_data[2]];
                zoebar4.Value = spectrumdata[order_data[3]];
                zoebar5.Value = spectrumdata[order_data[4]];
                zoebar6.Value = spectrumdata[order_data[5]];
                zoebar7.Value = spectrumdata[order_data[6]];
                zoebar8.Value = spectrumdata[order_data[7]];
                zoebar9.Value = spectrumdata[order_data[8]];
                zoebar10.Value = spectrumdata[order_data[9]];
                zoebar11.Value = spectrumdata[order_data[10]];
                zoebar12.Value = spectrumdata[order_data[11]];
                zoebar13.Value = spectrumdata[order_data[12]];
                zoebar14.Value = spectrumdata[order_data[13]];
                zoebar15.Value = spectrumdata[order_data[14]];
                zoebar16.Value = spectrumdata[order_data[15]];

            }



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
            DFU = 1;
            await Task.Delay(1000);
            DFU = 0;
        }


        public void random_Tick(object sender, EventArgs e)
        {
            if (Shuffle.IsChecked == true)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, 30);
                effectbox.SelectedIndex = index;
            }


        }



        public class LedTableSelectableViewPart : ISelectableViewPart
        {
            private readonly Lazy<LedTable> lazyContent;

            public LedTableSelectableViewPart(Lazy<LedTable> lazyContent)
            {
                this.lazyContent = lazyContent ?? throw new ArgumentNullException(nameof(lazyContent));
            }

            public int Order => 27;

            public string ViewPartName => "LED cạnh bàn";

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
            if (comportbox3.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
        }

        private void effectbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void ChangeRunningMode(object sender, SelectionChangedEventArgs e)
        {
            var i = (Label)(sender as ComboBox).SelectedItem;
            var item = (IEffect)effectbox_Copy2.SelectedItem;

            if (i != null && item != null)
            {
                if (i.Content.ToString() == "Rainbow Custom Zone")
                {
                    if (item.Name == "Sáng theo hiệu ứng")
                    {
                        isEffect = false;
                        txtTitle.Visibility = Visibility.Visible;
                        cuszoneIcon.Visibility = Visibility.Visible;
                        Bassbox.Visibility = Visibility.Collapsed;
                        filemaubox.Visibility = Visibility.Visible;
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


                        btPlay.Visibility = Visibility.Collapsed;
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

                        txtLed.Visibility = Visibility.Collapsed;
                        btColor1.Visibility = Visibility.Collapsed;
                        btColor2.Visibility = Visibility.Collapsed;
                        btColor3.Visibility = Visibility.Collapsed;
                        btColor4.Visibility = Visibility.Collapsed;
                        btColor5.Visibility = Visibility.Collapsed;
                        btColor6.Visibility = Visibility.Collapsed;
                        btColor7.Visibility = Visibility.Collapsed;
                        btColor8.Visibility = Visibility.Collapsed;
                        btColor9.Visibility = Visibility.Collapsed;
                        btColor10.Visibility = Visibility.Collapsed;
                        btColor11.Visibility = Visibility.Collapsed;
                        btColor12.Visibility = Visibility.Collapsed;
                        btColor13.Visibility = Visibility.Collapsed;
                        btColor14.Visibility = Visibility.Collapsed;
                        btColor15.Visibility = Visibility.Collapsed;
                        btColor16.Visibility = Visibility.Collapsed;

                        txtFreq.Visibility = Visibility.Collapsed;
                        Music_box_1.Visibility = Visibility.Collapsed;
                        Music_box_2.Visibility = Visibility.Collapsed;
                        Music_box_3.Visibility = Visibility.Collapsed;
                        Music_box_4.Visibility = Visibility.Collapsed;
                        Music_box_5.Visibility = Visibility.Collapsed;
                        Music_box_6.Visibility = Visibility.Collapsed;
                        Music_box_7.Visibility = Visibility.Collapsed;
                        Music_box_8.Visibility = Visibility.Collapsed;
                        Music_box_9.Visibility = Visibility.Collapsed;
                        Music_box_10.Visibility = Visibility.Collapsed;
                        Music_box_11.Visibility = Visibility.Collapsed;
                        Music_box_12.Visibility = Visibility.Collapsed;
                        Music_box_13.Visibility = Visibility.Collapsed;
                        Music_box_14.Visibility = Visibility.Collapsed;
                        Music_box_15.Visibility = Visibility.Collapsed;
                        Music_box_16.Visibility = Visibility.Collapsed;

                        this.customZone.Visibility = Visibility.Visible;
                        this.customZone.Height = 150;
                    }
                    else if (item.Name == "Sáng theo nhạc")
                    {
                        isEffect = true;
                        txtTitle.Visibility = Visibility.Visible;
                        cuszoneIcon.Visibility = Visibility.Visible;
                        Bassbox.Visibility = Visibility.Visible;
                        filemaubox.Visibility = Visibility.Visible;
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


                        btPlay.Visibility = Visibility.Visible;
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

                        txtLed.Visibility = Visibility.Visible;
                        btColor1.Visibility = Visibility.Visible;
                        btColor2.Visibility = Visibility.Visible;
                        btColor3.Visibility = Visibility.Visible;
                        btColor4.Visibility = Visibility.Visible;
                        btColor5.Visibility = Visibility.Visible;
                        btColor6.Visibility = Visibility.Visible;
                        btColor7.Visibility = Visibility.Visible;
                        btColor8.Visibility = Visibility.Visible;
                        btColor9.Visibility = Visibility.Visible;
                        btColor10.Visibility = Visibility.Visible;
                        btColor11.Visibility = Visibility.Visible;
                        btColor12.Visibility = Visibility.Visible;
                        btColor13.Visibility = Visibility.Visible;
                        btColor14.Visibility = Visibility.Visible;
                        btColor15.Visibility = Visibility.Visible;
                        btColor16.Visibility = Visibility.Visible;

                        txtFreq.Visibility = Visibility.Visible;
                        Music_box_1.Visibility = Visibility.Visible;
                        Music_box_2.Visibility = Visibility.Visible;
                        Music_box_3.Visibility = Visibility.Visible;
                        Music_box_4.Visibility = Visibility.Visible;
                        Music_box_5.Visibility = Visibility.Visible;
                        Music_box_6.Visibility = Visibility.Visible;
                        Music_box_7.Visibility = Visibility.Visible;
                        Music_box_8.Visibility = Visibility.Visible;
                        Music_box_9.Visibility = Visibility.Visible;
                        Music_box_10.Visibility = Visibility.Visible;
                        Music_box_11.Visibility = Visibility.Visible;
                        Music_box_12.Visibility = Visibility.Visible;
                        Music_box_13.Visibility = Visibility.Visible;
                        Music_box_14.Visibility = Visibility.Visible;
                        Music_box_15.Visibility = Visibility.Visible;
                        Music_box_16.Visibility = Visibility.Visible;


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
                        this.customZone.Height = 262;
                    }
                }
                else
                {
                    if (item.Name == "Sáng theo hiệu ứng")
                    {
                        this.customZone.Visibility = Visibility.Collapsed;
                    }
                    else if (item.Name == "Sáng theo nhạc")
                    {
                        txtTitle.Visibility = Visibility.Visible;
                        cuszoneIcon.Visibility = Visibility.Visible;
                        Bassbox.Visibility = Visibility.Visible;
                        filemaubox.Visibility = Visibility.Visible;
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


                        btPlay.Visibility = Visibility.Visible;
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

                        txtLed.Visibility = Visibility.Visible;
                        btColor1.Visibility = Visibility.Visible;
                        btColor2.Visibility = Visibility.Visible;
                        btColor3.Visibility = Visibility.Visible;
                        btColor4.Visibility = Visibility.Visible;
                        btColor5.Visibility = Visibility.Visible;
                        btColor6.Visibility = Visibility.Visible;
                        btColor7.Visibility = Visibility.Visible;
                        btColor8.Visibility = Visibility.Visible;
                        btColor9.Visibility = Visibility.Visible;
                        btColor10.Visibility = Visibility.Visible;
                        btColor11.Visibility = Visibility.Visible;
                        btColor12.Visibility = Visibility.Visible;
                        btColor13.Visibility = Visibility.Visible;
                        btColor14.Visibility = Visibility.Visible;
                        btColor15.Visibility = Visibility.Visible;
                        btColor16.Visibility = Visibility.Visible;

                        txtFreq.Visibility = Visibility.Visible;
                        Music_box_1.Visibility = Visibility.Visible;
                        Music_box_2.Visibility = Visibility.Visible;
                        Music_box_3.Visibility = Visibility.Visible;
                        Music_box_4.Visibility = Visibility.Visible;
                        Music_box_5.Visibility = Visibility.Visible;
                        Music_box_6.Visibility = Visibility.Visible;
                        Music_box_7.Visibility = Visibility.Visible;
                        Music_box_8.Visibility = Visibility.Visible;
                        Music_box_9.Visibility = Visibility.Visible;
                        Music_box_10.Visibility = Visibility.Visible;
                        Music_box_11.Visibility = Visibility.Visible;
                        Music_box_12.Visibility = Visibility.Visible;
                        Music_box_13.Visibility = Visibility.Visible;
                        Music_box_14.Visibility = Visibility.Visible;
                        Music_box_15.Visibility = Visibility.Visible;
                        Music_box_16.Visibility = Visibility.Visible;

                        this.customZone.Visibility = Visibility.Visible;
                        this.customZone.Height = 262;
                    }
                }
            }
        }

        private void ChangeEffect(object sender, SelectionChangedEventArgs e)
        {
            var item = (IEffect)(sender as ComboBox).SelectedItem;
            var i = (Label)effectbox.SelectedItem;
            if (numberScreen.SelectedItem == null)
                numberScreen.SelectedItem = "Linear Lighting";

            if (item != null)
            {

                previewCard.Visibility = Visibility.Collapsed;
                balanceCard.Visibility = Visibility.Collapsed;
                ledCard.Visibility = Visibility.Collapsed;
                ambilightCard.Visibility = Visibility.Collapsed;
                sizescreenCard.Visibility = Visibility.Collapsed;
                btnReset.Visibility = Visibility.Collapsed;

                switch (item.Name)
                {
                    case "Sáng theo hiệu ứng":
                        this.effectCard.Visibility = Visibility.Visible;
                        this.staticCard.Visibility = Visibility.Collapsed;
                        if (i != null)
                        {
                            if (i.Content.ToString() == "Rainbow Custom Zone")
                            {
                                isEffect = false;
                                txtTitle.Visibility = Visibility.Visible;
                                cuszoneIcon.Visibility = Visibility.Visible;
                                Bassbox.Visibility = Visibility.Collapsed;
                                filemaubox.Visibility = Visibility.Visible;
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


                                btPlay.Visibility = Visibility.Collapsed;
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

                                txtLed.Visibility = Visibility.Collapsed;
                                btColor1.Visibility = Visibility.Collapsed;
                                btColor2.Visibility = Visibility.Collapsed;
                                btColor3.Visibility = Visibility.Collapsed;
                                btColor4.Visibility = Visibility.Collapsed;
                                btColor5.Visibility = Visibility.Collapsed;
                                btColor6.Visibility = Visibility.Collapsed;
                                btColor7.Visibility = Visibility.Collapsed;
                                btColor8.Visibility = Visibility.Collapsed;
                                btColor9.Visibility = Visibility.Collapsed;
                                btColor10.Visibility = Visibility.Collapsed;
                                btColor11.Visibility = Visibility.Collapsed;
                                btColor12.Visibility = Visibility.Collapsed;
                                btColor13.Visibility = Visibility.Collapsed;
                                btColor14.Visibility = Visibility.Collapsed;
                                btColor15.Visibility = Visibility.Collapsed;
                                btColor16.Visibility = Visibility.Collapsed;

                                txtFreq.Visibility = Visibility.Collapsed;
                                Music_box_1.Visibility = Visibility.Collapsed;
                                Music_box_2.Visibility = Visibility.Collapsed;
                                Music_box_3.Visibility = Visibility.Collapsed;
                                Music_box_4.Visibility = Visibility.Collapsed;
                                Music_box_5.Visibility = Visibility.Collapsed;
                                Music_box_6.Visibility = Visibility.Collapsed;
                                Music_box_7.Visibility = Visibility.Collapsed;
                                Music_box_8.Visibility = Visibility.Collapsed;
                                Music_box_9.Visibility = Visibility.Collapsed;
                                Music_box_10.Visibility = Visibility.Collapsed;
                                Music_box_11.Visibility = Visibility.Collapsed;
                                Music_box_12.Visibility = Visibility.Collapsed;
                                Music_box_13.Visibility = Visibility.Collapsed;
                                Music_box_14.Visibility = Visibility.Collapsed;
                                Music_box_15.Visibility = Visibility.Collapsed;
                                Music_box_16.Visibility = Visibility.Collapsed;

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


                        previewCard.Visibility = Visibility.Visible;
                        balanceCard.Visibility = Visibility.Visible;
                        ledCard.Visibility = Visibility.Visible;
                        ambilightCard.Visibility = Visibility.Visible;
                        sizescreenCard.Visibility = Visibility.Visible;
                        btnReset.Visibility = Visibility.Visible;
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
                                txtTitle.Visibility = Visibility.Visible;
                                cuszoneIcon.Visibility = Visibility.Visible;
                                Bassbox.Visibility = Visibility.Visible;
                                filemaubox.Visibility = Visibility.Visible;
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


                                btPlay.Visibility = Visibility.Visible;
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

                                txtLed.Visibility = Visibility.Visible;
                                btColor1.Visibility = Visibility.Visible;
                                btColor2.Visibility = Visibility.Visible;
                                btColor3.Visibility = Visibility.Visible;
                                btColor4.Visibility = Visibility.Visible;
                                btColor5.Visibility = Visibility.Visible;
                                btColor6.Visibility = Visibility.Visible;
                                btColor7.Visibility = Visibility.Visible;
                                btColor8.Visibility = Visibility.Visible;
                                btColor9.Visibility = Visibility.Visible;
                                btColor10.Visibility = Visibility.Visible;
                                btColor11.Visibility = Visibility.Visible;
                                btColor12.Visibility = Visibility.Visible;
                                btColor13.Visibility = Visibility.Visible;
                                btColor14.Visibility = Visibility.Visible;
                                btColor15.Visibility = Visibility.Visible;
                                btColor16.Visibility = Visibility.Visible;

                                txtFreq.Visibility = Visibility.Visible;
                                Music_box_1.Visibility = Visibility.Visible;
                                Music_box_2.Visibility = Visibility.Visible;
                                Music_box_3.Visibility = Visibility.Visible;
                                Music_box_4.Visibility = Visibility.Visible;
                                Music_box_5.Visibility = Visibility.Visible;
                                Music_box_6.Visibility = Visibility.Visible;
                                Music_box_7.Visibility = Visibility.Visible;
                                Music_box_8.Visibility = Visibility.Visible;
                                Music_box_9.Visibility = Visibility.Visible;
                                Music_box_10.Visibility = Visibility.Visible;
                                Music_box_11.Visibility = Visibility.Visible;
                                Music_box_12.Visibility = Visibility.Visible;
                                Music_box_13.Visibility = Visibility.Visible;
                                Music_box_14.Visibility = Visibility.Visible;
                                Music_box_15.Visibility = Visibility.Visible;
                                Music_box_16.Visibility = Visibility.Visible;





                                this.customZone.Visibility = Visibility.Visible;
                                this.customZone.Height = 262;
                            }
                            else
                            {
                                txtTitle.Visibility = Visibility.Visible;
                                cuszoneIcon.Visibility = Visibility.Visible;
                                Bassbox.Visibility = Visibility.Visible;
                                filemaubox.Visibility = Visibility.Visible;
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


                                btPlay.Visibility = Visibility.Visible;
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

                                txtLed.Visibility = Visibility.Visible;
                                btColor1.Visibility = Visibility.Visible;
                                btColor2.Visibility = Visibility.Visible;
                                btColor3.Visibility = Visibility.Visible;
                                btColor4.Visibility = Visibility.Visible;
                                btColor5.Visibility = Visibility.Visible;
                                btColor6.Visibility = Visibility.Visible;
                                btColor7.Visibility = Visibility.Visible;
                                btColor8.Visibility = Visibility.Visible;
                                btColor9.Visibility = Visibility.Visible;
                                btColor10.Visibility = Visibility.Visible;
                                btColor11.Visibility = Visibility.Visible;
                                btColor12.Visibility = Visibility.Visible;
                                btColor13.Visibility = Visibility.Visible;
                                btColor14.Visibility = Visibility.Visible;
                                btColor15.Visibility = Visibility.Visible;
                                btColor16.Visibility = Visibility.Visible;

                                txtFreq.Visibility = Visibility.Visible;
                                Music_box_1.Visibility = Visibility.Visible;
                                Music_box_2.Visibility = Visibility.Visible;
                                Music_box_3.Visibility = Visibility.Visible;
                                Music_box_4.Visibility = Visibility.Visible;
                                Music_box_5.Visibility = Visibility.Visible;
                                Music_box_6.Visibility = Visibility.Visible;
                                Music_box_7.Visibility = Visibility.Visible;
                                Music_box_8.Visibility = Visibility.Visible;
                                Music_box_9.Visibility = Visibility.Visible;
                                Music_box_10.Visibility = Visibility.Visible;
                                Music_box_11.Visibility = Visibility.Visible;
                                Music_box_12.Visibility = Visibility.Visible;
                                Music_box_13.Visibility = Visibility.Visible;
                                Music_box_14.Visibility = Visibility.Visible;
                                Music_box_15.Visibility = Visibility.Visible;
                                Music_box_16.Visibility = Visibility.Visible;

                                this.customZone.Visibility = Visibility.Visible;
                                this.customZone.Height = 262;
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

        private void ChangeLighting(object sender, SelectionChangedEventArgs e)
        {
            var settingsViewModel = DataContext as SettingsViewModel;
            if (settingsViewModel != null)
                if (numberScreen.SelectedIndex == 1)
                    settingsViewModel.Settings.UseLinearLighting = false;
                else
                    settingsViewModel.Settings.UseLinearLighting = true;
        }

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
                var item = (IEffect)effectbox_Copy2.SelectedItem;
                if (item.Name == "Sáng theo hiệu ứng")
                {
                    for (int i = 0; i < 16; i++)
                    {
                        lines2[i + 8] = Convert.ToString(i);
                    }
                }


                System.IO.File.WriteAllLines(FileMau.FileName, lines2);

                string[] deviceInfo = new string[4];
                deviceInfo[0] = effectbox.SelectedIndex.ToString();
                deviceInfo[1] = method.SelectedIndex.ToString();
                deviceInfo[2] = speed.Value.ToString();
                deviceInfo[3] = sin.Value.ToString();
                System.IO.File.AppendAllLines(FileMau.FileName, deviceInfo);
            }
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
                        }
                    }
                    for (int j = 8; j < 24; j++)
                    {
                        if (!string.IsNullOrEmpty(lines[j]))
                        {
                            Music_box_1.SelectedIndex = Convert.ToInt16(lines[8]);
                            Music_box_2.SelectedIndex = Convert.ToInt16(lines[9]);
                            Music_box_3.SelectedIndex = Convert.ToInt16(lines[10]);
                            Music_box_4.SelectedIndex = Convert.ToInt16(lines[11]);
                            Music_box_5.SelectedIndex = Convert.ToInt16(lines[12]);
                            Music_box_6.SelectedIndex = Convert.ToInt16(lines[13]);
                            Music_box_7.SelectedIndex = Convert.ToInt16(lines[14]);
                            Music_box_8.SelectedIndex = Convert.ToInt16(lines[15]);
                            Music_box_9.SelectedIndex = Convert.ToInt16(lines[16]);
                            Music_box_10.SelectedIndex = Convert.ToInt16(lines[17]);
                            Music_box_11.SelectedIndex = Convert.ToInt16(lines[18]);
                            Music_box_12.SelectedIndex = Convert.ToInt16(lines[19]);
                            Music_box_13.SelectedIndex = Convert.ToInt16(lines[20]);
                            Music_box_14.SelectedIndex = Convert.ToInt16(lines[21]);
                            Music_box_15.SelectedIndex = Convert.ToInt16(lines[22]);
                            Music_box_16.SelectedIndex = Convert.ToInt16(lines[23]);
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
            if (Music_box_1.SelectedIndex >= 0)
            {
                order_data[0] = Music_box_1.SelectedIndex;
                order_data[1] = Music_box_2.SelectedIndex;
                order_data[2] = Music_box_3.SelectedIndex;
                order_data[3] = Music_box_4.SelectedIndex;
                order_data[4] = Music_box_5.SelectedIndex;
                order_data[5] = Music_box_6.SelectedIndex;
                order_data[6] = Music_box_7.SelectedIndex;
                order_data[7] = Music_box_8.SelectedIndex;
                order_data[8] = Music_box_9.SelectedIndex;
                order_data[9] = Music_box_10.SelectedIndex;
                order_data[10] = Music_box_11.SelectedIndex;
                order_data[11] = Music_box_12.SelectedIndex;
                order_data[12] = Music_box_13.SelectedIndex;
                order_data[13] = Music_box_14.SelectedIndex;
                order_data[14] = Music_box_15.SelectedIndex;
                order_data[15] = Music_box_16.SelectedIndex;
            }

            for (int i = 8; i <= 23; i++)
            {
                lines2[i] = Convert.ToString(order_data[i - 8]);
            }
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
            Music_box_1.SelectedIndex = 0;
            Music_box_2.SelectedIndex = 1;
            Music_box_3.SelectedIndex = 2;
            Music_box_4.SelectedIndex = 3;
            Music_box_5.SelectedIndex = 4;
            Music_box_6.SelectedIndex = 5;
            Music_box_7.SelectedIndex = 6;
            Music_box_8.SelectedIndex = 7;
            Music_box_9.SelectedIndex = 8;
            Music_box_10.SelectedIndex = 9;
            Music_box_11.SelectedIndex = 10;
            Music_box_12.SelectedIndex = 11;
            Music_box_13.SelectedIndex = 12;
            Music_box_14.SelectedIndex = 13;
            Music_box_15.SelectedIndex = 14;
            Music_box_16.SelectedIndex = 15;
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
                    balanceCard.Visibility = Visibility.Visible;
                    ledCard.Visibility = Visibility.Visible;
                    ambilightCard.Visibility = Visibility.Visible;
                    sizescreenCard.Visibility = Visibility.Visible;


                    btnReset.IsEnabled = true;
                }
                else
                {
                    // Code for Un-Checked state
                    previewCard.Visibility = Visibility.Collapsed;
                    balanceCard.Visibility = Visibility.Collapsed;
                    ledCard.Visibility = Visibility.Collapsed;
                    ambilightCard.Visibility = Visibility.Collapsed;
                    sizescreenCard.Visibility = Visibility.Collapsed;


                    btnReset.IsEnabled = false;


                    var item = (IEffect)effectbox_Copy2.SelectedItem;
                    var i = (Label)effectbox.SelectedItem;
                    if (item != null)
                    {
                        switch (item.Name)
                        {
                            case "Sáng theo hiệu ứng":
                                this.effectCard.Visibility = Visibility.Visible;
                                this.staticCard.Visibility = Visibility.Collapsed;
                                if (i != null)
                                {
                                    if (i.Content.ToString() == "Rainbow Custom Zone")
                                    {
                                        isEffect = false;
                                        txtTitle.Visibility = Visibility.Visible;
                                        cuszoneIcon.Visibility = Visibility.Visible;
                                        Bassbox.Visibility = Visibility.Collapsed;
                                        filemaubox.Visibility = Visibility.Visible;
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


                                        btPlay.Visibility = Visibility.Collapsed;
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

                                        txtLed.Visibility = Visibility.Collapsed;
                                        btColor1.Visibility = Visibility.Collapsed;
                                        btColor2.Visibility = Visibility.Collapsed;
                                        btColor3.Visibility = Visibility.Collapsed;
                                        btColor4.Visibility = Visibility.Collapsed;
                                        btColor5.Visibility = Visibility.Collapsed;
                                        btColor6.Visibility = Visibility.Collapsed;
                                        btColor7.Visibility = Visibility.Collapsed;
                                        btColor8.Visibility = Visibility.Collapsed;
                                        btColor9.Visibility = Visibility.Collapsed;
                                        btColor10.Visibility = Visibility.Collapsed;
                                        btColor11.Visibility = Visibility.Collapsed;
                                        btColor12.Visibility = Visibility.Collapsed;
                                        btColor13.Visibility = Visibility.Collapsed;
                                        btColor14.Visibility = Visibility.Collapsed;
                                        btColor15.Visibility = Visibility.Collapsed;
                                        btColor16.Visibility = Visibility.Collapsed;

                                        txtFreq.Visibility = Visibility.Collapsed;
                                        Music_box_1.Visibility = Visibility.Collapsed;
                                        Music_box_2.Visibility = Visibility.Collapsed;
                                        Music_box_3.Visibility = Visibility.Collapsed;
                                        Music_box_4.Visibility = Visibility.Collapsed;
                                        Music_box_5.Visibility = Visibility.Collapsed;
                                        Music_box_6.Visibility = Visibility.Collapsed;
                                        Music_box_7.Visibility = Visibility.Collapsed;
                                        Music_box_8.Visibility = Visibility.Collapsed;
                                        Music_box_9.Visibility = Visibility.Collapsed;
                                        Music_box_10.Visibility = Visibility.Collapsed;
                                        Music_box_11.Visibility = Visibility.Collapsed;
                                        Music_box_12.Visibility = Visibility.Collapsed;
                                        Music_box_13.Visibility = Visibility.Collapsed;
                                        Music_box_14.Visibility = Visibility.Collapsed;
                                        Music_box_15.Visibility = Visibility.Collapsed;
                                        Music_box_16.Visibility = Visibility.Collapsed;

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
                                        txtTitle.Visibility = Visibility.Visible;
                                        cuszoneIcon.Visibility = Visibility.Visible;
                                        Bassbox.Visibility = Visibility.Visible;
                                        filemaubox.Visibility = Visibility.Visible;
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


                                        btPlay.Visibility = Visibility.Visible;
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

                                        txtLed.Visibility = Visibility.Visible;
                                        btColor1.Visibility = Visibility.Visible;
                                        btColor2.Visibility = Visibility.Visible;
                                        btColor3.Visibility = Visibility.Visible;
                                        btColor4.Visibility = Visibility.Visible;
                                        btColor5.Visibility = Visibility.Visible;
                                        btColor6.Visibility = Visibility.Visible;
                                        btColor7.Visibility = Visibility.Visible;
                                        btColor8.Visibility = Visibility.Visible;
                                        btColor9.Visibility = Visibility.Visible;
                                        btColor10.Visibility = Visibility.Visible;
                                        btColor11.Visibility = Visibility.Visible;
                                        btColor12.Visibility = Visibility.Visible;
                                        btColor13.Visibility = Visibility.Visible;
                                        btColor14.Visibility = Visibility.Visible;
                                        btColor15.Visibility = Visibility.Visible;
                                        btColor16.Visibility = Visibility.Visible;

                                        txtFreq.Visibility = Visibility.Visible;
                                        Music_box_1.Visibility = Visibility.Visible;
                                        Music_box_2.Visibility = Visibility.Visible;
                                        Music_box_3.Visibility = Visibility.Visible;
                                        Music_box_4.Visibility = Visibility.Visible;
                                        Music_box_5.Visibility = Visibility.Visible;
                                        Music_box_6.Visibility = Visibility.Visible;
                                        Music_box_7.Visibility = Visibility.Visible;
                                        Music_box_8.Visibility = Visibility.Visible;
                                        Music_box_9.Visibility = Visibility.Visible;
                                        Music_box_10.Visibility = Visibility.Visible;
                                        Music_box_11.Visibility = Visibility.Visible;
                                        Music_box_12.Visibility = Visibility.Visible;
                                        Music_box_13.Visibility = Visibility.Visible;
                                        Music_box_14.Visibility = Visibility.Visible;
                                        Music_box_15.Visibility = Visibility.Visible;
                                        Music_box_16.Visibility = Visibility.Visible;





                                        this.customZone.Visibility = Visibility.Visible;
                                        this.customZone.Height = 262;
                                    }
                                    else
                                    {
                                        txtTitle.Visibility = Visibility.Visible;
                                        cuszoneIcon.Visibility = Visibility.Visible;
                                        Bassbox.Visibility = Visibility.Visible;
                                        filemaubox.Visibility = Visibility.Visible;
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


                                        btPlay.Visibility = Visibility.Visible;
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

                                        txtLed.Visibility = Visibility.Visible;
                                        btColor1.Visibility = Visibility.Visible;
                                        btColor2.Visibility = Visibility.Visible;
                                        btColor3.Visibility = Visibility.Visible;
                                        btColor4.Visibility = Visibility.Visible;
                                        btColor5.Visibility = Visibility.Visible;
                                        btColor6.Visibility = Visibility.Visible;
                                        btColor7.Visibility = Visibility.Visible;
                                        btColor8.Visibility = Visibility.Visible;
                                        btColor9.Visibility = Visibility.Visible;
                                        btColor10.Visibility = Visibility.Visible;
                                        btColor11.Visibility = Visibility.Visible;
                                        btColor12.Visibility = Visibility.Visible;
                                        btColor13.Visibility = Visibility.Visible;
                                        btColor14.Visibility = Visibility.Visible;
                                        btColor15.Visibility = Visibility.Visible;
                                        btColor16.Visibility = Visibility.Visible;

                                        txtFreq.Visibility = Visibility.Visible;
                                        Music_box_1.Visibility = Visibility.Visible;
                                        Music_box_2.Visibility = Visibility.Visible;
                                        Music_box_3.Visibility = Visibility.Visible;
                                        Music_box_4.Visibility = Visibility.Visible;
                                        Music_box_5.Visibility = Visibility.Visible;
                                        Music_box_6.Visibility = Visibility.Visible;
                                        Music_box_7.Visibility = Visibility.Visible;
                                        Music_box_8.Visibility = Visibility.Visible;
                                        Music_box_9.Visibility = Visibility.Visible;
                                        Music_box_10.Visibility = Visibility.Visible;
                                        Music_box_11.Visibility = Visibility.Visible;
                                        Music_box_12.Visibility = Visibility.Visible;
                                        Music_box_13.Visibility = Visibility.Visible;
                                        Music_box_14.Visibility = Visibility.Visible;
                                        Music_box_15.Visibility = Visibility.Visible;
                                        Music_box_16.Visibility = Visibility.Visible;

                                        this.customZone.Visibility = Visibility.Visible;
                                        this.customZone.Height = 262;
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
            numberScreen.SelectedIndex = 0;
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
            if (comportbox4.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
        }

        private void Comportbox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox3.SelectedItem == comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }

        }

        //private void Comportbox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (comportbox2.SelectedItem == comportbox3.SelectedItem)
        //    {
        //        comportbox3.SelectedItem = "Không có";
        //    }
        //    else if (comportbox2.SelectedItem == comportbox4.SelectedItem)
        //    {
        //        comportbox4.SelectedItem = "Không có";
        //    }
        //}

    }
}
