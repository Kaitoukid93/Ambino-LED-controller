using adrilight.ViewModel;
using NAudio.CoreAudioApi;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ColorPickerWPF;
using MaterialDesignThemes.Wpf;
using adrilight;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.IO.Ports;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace adrilight.View.SettingsWindowComponents
{
    /// <summary>
    /// Interaction logic for ComPortSetup.xaml
    /// </summary>
    public partial class ComPortSetup : UserControl
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
        public ComPortSetup()
        {
            
            InitializeComponent();
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
            if(Music_box_1.SelectedIndex>=0)
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

            if (comportbox.SelectedItem==null)
            {
                comportbox.SelectedItem = "Không có";
            }
            if (comportbox2.SelectedItem == null)
            {
                comportbox2.SelectedItem = "Không có";
            }
            if (comportbox3.SelectedItem == null)
            {
                comportbox3.SelectedItem = "Không có";
            }
            if (comportbox4.SelectedItem == null)
            {
                comportbox4.SelectedItem = "Không có";
            }
            if (comportbox5.SelectedItem == null)
            {
                comportbox5.SelectedItem = "Không có";
            }
        }
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


        private IUserSettings UserSettings { get; }

        //Rainbow custom zone create new holder//
        private static string[] lines = new string[24];
        //Rainbow custom zone save value holder//
        private static string[] lines2 = new string[24];
        //Music value using Naudio (volume Level)
        public static int musicvalue;
        //Ambino VID PID auto detect(unused for now)//
        public static string ambino_port;
        //For automatic DFU mode Ambino HUBV2//
        public static byte DFU=0;
        //Bass library value//
        //private List<byte> _spectrumdata;
       // private float[] _fft;


        // public static string temp;
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
                spectrumdata[x] = (spectrumdata[x] * 6 + y*2) / 8; //Smoothing out the value (take 5/8 of old value and 3/8 of new value to make finnal value)
                if (spectrumdata[x] > 255)
                    spectrumdata[x] = 255;

                //  Console.Write("{0, 3} ", y);


               

            }
            int i;
            //output_spectrumdata = spectrumdata;
            for (i = 0; i < 16; i++)
            {
                if(order_data[i]>=0)
                    
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
           /* if (_hanctr > 300)
            {
                _hanctr = 0;
              //  _l.Value = 0;
               // _r.Value = 0;
                Free();
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
              //  _initialized = false;
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
            Connect_button.IsChecked = false;
            DFU = 0;
        }
       

        public void random_Tick(object sender, EventArgs e)
        {
            if(Shuffle.IsChecked == true)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, 30);
                effectbox.SelectedIndex = index;
            }
           
            
        }

        

        public class ComPortSetupSelectableViewPart : ISelectableViewPart
        {
            private readonly Lazy<ComPortSetup> lazyContent;

            public ComPortSetupSelectableViewPart(Lazy<ComPortSetup> lazyContent)
            {
                this.lazyContent = lazyContent ?? throw new ArgumentNullException(nameof(lazyContent));
            }

            public int Order => 50;

            public string ViewPartName => "Kết nối với USB";

            public object Content { get => lazyContent.Value; }
        }


        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void audio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ToggleButton_Checked_1(object sender, RoutedEventArgs e)
        {

        }


        private void CountingButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
        {
            













            }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void zoebar_ValueChanged()
        {

        }







        private void Button_Click_1(object sender, RoutedEventArgs e)
        {


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





        private void Button_Click(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {


        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {




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
                if (z == 24)
                {
                    for(int i=0;i<=23;i++)
                    {
                        if(!string.IsNullOrEmpty(lines[i]))
                        {
                            ClrPcker_Background_1.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[0]);
                            ClrPcker_Background_2.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[1]);
                            ClrPcker_Background_3.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[2]);
                            ClrPcker_Background_4.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[3]);
                            ClrPcker_Background_5.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[4]);
                            ClrPcker_Background_6.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[5]);
                            ClrPcker_Background_7.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[6]);
                            ClrPcker_Background_8.SelectedColor = (Color)ColorConverter.ConvertFromString(lines[7]);
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
                            filemaubox.Text = filemau.SafeFileName;
                        }
                    }
                 
                }


            }
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

                System.IO.File.WriteAllLines(FileMau.FileName, lines2);

            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (ambino_port != null)
            {

                comportbox.SelectedValue = ambino_port;
            }



        }

        private void CommentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (method.SelectedIndex == 0 || method.SelectedIndex == 1 || method.SelectedIndex == 2 || method.SelectedIndex == 3|| method.SelectedIndex == 4 || method.SelectedIndex == 5 || method.SelectedIndex == 8)
            {
                speed.IsEnabled = true;
                sin.IsEnabled = true;
            }
            else
            {
                speed.IsEnabled = false;
                sin.IsEnabled = false;
            }
           /* if (method.SelectedIndex==8)
            {
                staticcolor.IsEnabled = true;

            }
            else
            {
                staticcolor.IsEnabled = false;
            }
            */

            if(method.SelectedIndex==6|| method.SelectedIndex == 7  )
            {
                effectbox.IsEnabled = false;

            }
            else
            {
                effectbox.IsEnabled = true;
            }
        }

        private void Caseeffectbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (effectbox_Copy2.SelectedIndex == 1)
            {
                screenbutton.IsChecked = false;
            }
        }

        private void Screen_Effect_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          /*  if(Screen_Effect_box.SelectedIndex==0)
            {
                screenbutton.IsChecked = false;
            }
            if (Screen_Effect_box.SelectedIndex == 1)
            {
                screenbutton.IsChecked = true;
                rainbowbutton.IsChecked = true;
                staticbutton.IsChecked = false;
                if (edge.SelectedIndex==1)
                edge.SelectedIndex = 0;

            }
            if (Screen_Effect_box.SelectedIndex == 2)
            {
                screenbutton.IsChecked = true;
                rainbowbutton.IsChecked = false;
                staticbutton.IsChecked = true;
            }
            */
        }

        private void Musicvisualize_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Effect_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Time_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void butcolor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {

        }

        private void Comportbox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox2.SelectedItem==comportbox.SelectedItem)
            {
                comportbox.SelectedItem = "Không có";
            }
            else if (comportbox2.SelectedItem==comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
            else if (comportbox2.SelectedItem==comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }
            else if (comportbox2.SelectedItem == comportbox5.SelectedItem)
            {
                comportbox5.SelectedItem = "Không có";
            }


        }

        private void Comportbox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox3.SelectedItem == comportbox.SelectedItem)
            {
                comportbox.SelectedItem = "Không có";
            }
            else if (comportbox3.SelectedItem == comportbox2.SelectedItem)
            {
                comportbox2.SelectedItem = "Không có";
            }
            else if (comportbox3.SelectedItem == comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }
            else if (comportbox3.SelectedItem == comportbox5.SelectedItem)
            {
                comportbox5.SelectedItem = "Không có";
            }

        }

        private void Comportbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox.SelectedItem == comportbox2.SelectedItem)
            {
                comportbox2.SelectedItem = "Không có";
            }
            else if (comportbox.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
            else if (comportbox.SelectedItem == comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }
            else if (comportbox.SelectedItem == comportbox5.SelectedItem)
            {
                comportbox5.SelectedItem = "Không có";
            }
            // comportbox.Items.Remove("COM1");
        }

        private void Comportbox4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox4.SelectedItem == comportbox.SelectedItem)
            {
                comportbox.SelectedItem = "Không có";
            }
            else if (comportbox4.SelectedItem == comportbox2.SelectedItem)
            {
                comportbox2.SelectedItem = "Không có";
            }
            else if (comportbox4.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
            else if (comportbox4.SelectedItem == comportbox5.SelectedItem)
            {
                comportbox5.SelectedItem = "Không có";
            }

        }

        private void ToggleButton_Checked_3(object sender, RoutedEventArgs e)
        {

        }

        private void Edge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*if (edge.SelectedIndex==1)
            {
                Screen_Effect_box.SelectedIndex = 0; this was created for linking 3 options of LED Screen, Old hardware required this to operate properly, 
                Since HUBv2 using new Hardware, this is no longer needed
            }
            */
        }

        private void Shuffle_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void Comportbox5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comportbox5.SelectedItem == comportbox2.SelectedItem)
            {
                comportbox2.SelectedItem = "Không có";
            }
            else if (comportbox5.SelectedItem == comportbox3.SelectedItem)
            {
                comportbox3.SelectedItem = "Không có";
            }
            else if (comportbox5.SelectedItem == comportbox4.SelectedItem)
            {
                comportbox4.SelectedItem = "Không có";
            }
            else if (comportbox.SelectedItem == comportbox.SelectedItem)
            {
                comportbox.SelectedItem = "Không có";
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
           
        }

        private void Comportbox5_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = DFU_func();
        }

        private void Devicebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BassWasapi.BASS_WASAPI_Free();  // every time we change the audio device, Bass wasapi need to init again with default value
            Bass.BASS_Free();
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if(Bassbox.SelectedIndex>=0)
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

        private void Music_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            if(Music_box_1.SelectedIndex>=0)
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

            for (int i=8;i<=23;i++)
            {
                lines2[i] = Convert.ToString(order_data[i - 8]);
            }
           


           
        }

        private void effectbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
           Music_box_1.SelectedIndex=0;
            Music_box_2.SelectedIndex=1;
             Music_box_3.SelectedIndex=2;
             Music_box_4.SelectedIndex=3;
            Music_box_5.SelectedIndex=4;
             Music_box_6.SelectedIndex=5;
          Music_box_7.SelectedIndex=6;
             Music_box_8.SelectedIndex=7;
            Music_box_9.SelectedIndex=8;
           Music_box_10.SelectedIndex=9;
            Music_box_11.SelectedIndex = 10;
            Music_box_12.SelectedIndex=11;
             Music_box_13.SelectedIndex=12;
             Music_box_14.SelectedIndex=13;
             Music_box_15.SelectedIndex=14;
             Music_box_16.SelectedIndex=15;
        }
    }
}
