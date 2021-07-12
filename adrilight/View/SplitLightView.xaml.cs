using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace adrilight.View
{
    /// <summary>
    /// Interaction logic for SplitLightView.xaml
    /// </summary>
    public partial class SplitLightView : UserControl
    {
        public SplitLightView()
        {
            InitializeComponent();
        }

        private void zone0_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone0.Background = new SolidColorBrush(color);
        }

        private void zone1_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone1.Background = new SolidColorBrush(color);
        }

        private void zone2_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone2.Background = new SolidColorBrush(color);
        }

        private void zone3_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone3.Background = new SolidColorBrush(color);
        }

        private void zone4_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone4.Background = new SolidColorBrush(color);
        }

        private void zone5_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone5.Background = new SolidColorBrush(color);
        }

        private void zone6_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone6.Background = new SolidColorBrush(color);
        }

        private void zone7_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone7.Background = new SolidColorBrush(color);
        }

        private void zone8_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone8.Background = new SolidColorBrush(color);
        }

        private void zone9_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone9.Background = new SolidColorBrush(color);
        }

        private void zone10_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone10.Background = new SolidColorBrush(color);
        }

        private void zone11_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone11.Background = new SolidColorBrush(color);
        }

        private void zone12_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone12.Background = new SolidColorBrush(color);
        }

        private void zone13_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone13.Background = new SolidColorBrush(color);
        }

        private void zone14_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb( CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone14.Background = new SolidColorBrush(color);
        }

        private void zone15_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(CustomZonePicker.Color.R, CustomZonePicker.Color.G, CustomZonePicker.Color.B);
            zone15.Background = new SolidColorBrush(color);
        }

        private void mzone0_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone0.Background = new SolidColorBrush(color);
        }

        private void mzone1_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone1.Background = new SolidColorBrush(color);
        }

        private void mzone2_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone2.Background = new SolidColorBrush(color);

        }

        private void mzone3_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone3.Background = new SolidColorBrush(color);
        }

        private void mzone4_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone4.Background = new SolidColorBrush(color);

        }

        private void mzone5_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone5.Background = new SolidColorBrush(color);
        }

        private void mzone6_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone6.Background = new SolidColorBrush(color);
        }

        private void mzone7_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone7.Background = new SolidColorBrush(color);
        }

        private void mzone8_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone8.Background = new SolidColorBrush(color);
        }

        private void mzone9_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone9.Background = new SolidColorBrush(color);
        }

        private void mzone10_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone10.Background = new SolidColorBrush(color);
        }

        private void mzone11_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone11.Background = new SolidColorBrush(color);
        }

        private void mzone12_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone12.Background = new SolidColorBrush(color);
        }

        private void mzone13_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone13.Background = new SolidColorBrush(color);
        }

        private void mzone14_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone14.Background = new SolidColorBrush(color);
        }

        private void mzone15_Click(object sender, RoutedEventArgs e)
        {
            var color = Color.FromRgb(mCustomZonePicker.Color.R, mCustomZonePicker.Color.G, mCustomZonePicker.Color.B);
            mzone15.Background = new SolidColorBrush(color);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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


                var lines = File.ReadAllLines(filemau.FileName);


                try
                {

                    mzone0.Background= new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[0]));
                    mzone1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[1]));
                    mzone2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[2]));
                    mzone3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[3]));
                    mzone4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[4]));
                    mzone5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[5]));
                    mzone6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[6]));
                    mzone7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[7]));
                    mzone8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[8]));
                    mzone9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[9]));
                    mzone10.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[10]));
                    mzone11.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[11]));
                    mzone12.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[12]));
                    mzone13.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[13]));
                    mzone14.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[14]));
                    mzone15.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[15]));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Corrupted Color File!!!");
                }

            }




        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // save music palete
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
            string[] colorData =
                {
             mzone0.Background.ToString(),
              mzone1.Background.ToString(),
               mzone2.Background.ToString(),
                mzone3.Background.ToString(),
                 mzone4.Background.ToString(),
                  mzone5.Background.ToString(),
                   mzone6.Background.ToString(),
                    mzone7.Background.ToString(),
                     mzone8.Background.ToString(),
                      mzone9.Background.ToString(),
                       mzone10.Background.ToString(),
                        mzone11.Background.ToString(),
                         mzone12.Background.ToString(),
                          mzone13.Background.ToString(),
                           mzone14.Background.ToString(),
                            mzone15.Background.ToString(),
                   

            };
            if (FileMau.ShowDialog() == true)
            {

                System.IO.File.WriteAllLines(FileMau.FileName, colorData);

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
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


                var lines = File.ReadAllLines(filemau.FileName);


                try
                {

                    zone0.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[0]));
                    zone1.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[1]));
                    zone2.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[2]));
                    zone3.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[3]));
                    zone4.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[4]));
                    zone5.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[5]));
                    zone6.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[6]));
                    zone7.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[7]));
                    zone8.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[8]));
                    zone9.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[9]));
                    zone10.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[10]));
                    zone11.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[11]));
                    zone12.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[12]));
                    zone13.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[13]));
                    zone14.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[14]));
                    zone15.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(lines[15]));
                }
                catch (FormatException)
                {
                    MessageBox.Show("Corrupted Color File!!!");
                }

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
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
            string[] colorData =
                {
             zone0.Background.ToString(),
              zone1.Background.ToString(),
               zone2.Background.ToString(),
                zone3.Background.ToString(),
                 zone4.Background.ToString(),
                  zone5.Background.ToString(),
                   zone6.Background.ToString(),
                    zone7.Background.ToString(),
                     zone8.Background.ToString(),
                      zone9.Background.ToString(),
                       zone10.Background.ToString(),
                        zone11.Background.ToString(),
                         zone12.Background.ToString(),
                          zone13.Background.ToString(),
                           zone14.Background.ToString(),
                            zone15.Background.ToString(),


            };
            if (FileMau.ShowDialog() == true)
            {

                System.IO.File.WriteAllLines(FileMau.FileName, colorData);

            }

        }

        private void neutral_Checked(object sender, RoutedEventArgs e)
        { 
          
        }

        private void WBWarm_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void WBCold_Checked(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
