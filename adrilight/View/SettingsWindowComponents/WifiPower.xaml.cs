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
    /// Interaction logic for LightingMode.xaml
    /// </summary>
    public partial class WifiPower : UserControl
    {

        private static string[] lines = new string[28];
        private static string[] lines2 = new string[24];
        public static int[] order_data = new int[16];

        public WifiPower()
        {
            InitializeComponent();
        }

        private void effectbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (effectbox_Copy1.SelectedItem != null)
            {
                var i = (Label)effectbox_Copy1.SelectedItem;
                if (i.Content.ToString() == "Sáng theo hiệu ứng")
                {
                    if (effectbox.SelectedItem != null)
                    {
                        var j = (Label)effectbox.SelectedItem;
                        if (j.Content.ToString() == "Rainbow Custom Zone")
                        {
                            Bassbox.Visibility = Visibility.Collapsed;
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

                            txtLed.Visibility = Visibility.Collapsed;
                            txtFreq.Visibility = Visibility.Collapsed;

                            staticCard.Visibility = Visibility.Collapsed;
                            effectCard.Visibility = Visibility.Visible;
                            customZone.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            staticCard.Visibility = Visibility.Collapsed;
                            effectCard.Visibility = Visibility.Visible;
                            customZone.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else if (i.Content.ToString() == "Sáng màu tĩnh")
                {
                    staticCard.Visibility = Visibility.Visible;
                    effectCard.Visibility = Visibility.Collapsed;
                    customZone.Visibility = Visibility.Collapsed;
                }
                else
                {
                    staticCard.Visibility = Visibility.Collapsed;
                    effectCard.Visibility = Visibility.Collapsed;
                    customZone.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Button_Effect_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Time_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Comportbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ChangeRunningMode(object sender, SelectionChangedEventArgs e)
        {
            if (effectbox.SelectedItem != null)
            {
                var i = (Label)effectbox.SelectedItem;
                if (i.Content.ToString() == "Rainbow Custom Zone")
                {
                    Bassbox.Visibility = Visibility.Collapsed;
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

                    txtLed.Visibility = Visibility.Collapsed;
                    txtFreq.Visibility = Visibility.Collapsed;

                    staticCard.Visibility = Visibility.Collapsed;
                    effectCard.Visibility = Visibility.Visible;
                    customZone.Visibility = Visibility.Visible;
                }
                else
                {
                    staticCard.Visibility = Visibility.Collapsed;
                    effectCard.Visibility = Visibility.Visible;
                    customZone.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }
        private void ToggleButton_Checked_2(object sender, RoutedEventArgs e)
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
                for (int i = 0; i < 16; i++)
                {
                    lines2[i + 8] = Convert.ToString(i);
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

        public class WifiPowerSelectableViewPart : ISelectableViewPart
        {
            private readonly Lazy<WifiPower> lazyContent;

            public WifiPowerSelectableViewPart(Lazy<WifiPower> lazyContent)
            {
                this.lazyContent = lazyContent ?? throw new ArgumentNullException(nameof(lazyContent));
            }

            public int Order => 28;

            public string ViewPartName => "Nút nguồn Wifi";

            public object Content { get => lazyContent.Value; }
        }
    }
}
