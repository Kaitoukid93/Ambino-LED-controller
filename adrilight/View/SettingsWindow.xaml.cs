using adrilight.ViewModel;
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
using System.Windows.Shapes;





namespace adrilight.ui
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();


            var settingsViewModel = DataContext as SettingsViewModel;
            if (settingsViewModel != null)
            {
                settingsViewModel.IsSettingsWindowOpen = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var settingsViewModel = DataContext as SettingsViewModel;            

            if (settingsViewModel != null)
            {
                settingsViewModel.IsSettingsWindowOpen = false;
            }
        }


        private void DemoItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

            pro.ShowDialog();
            if (pro.pressSave)
            {
                IList<ISelectableViewPart> ls = settingsViewModel.BackUpView
                                            .OrderBy(p => p.Order).ToList();
                if (!settingsViewModel.Settings.Pro11 && !settingsViewModel.Settings.Pro12 
                    && !settingsViewModel.Settings.Pro13)
                {
                    for (int j = 0; j < ls.Count; j++)
                    {
                        if (ls[j].Order == 24)
                            ls.RemoveAt(j);
                    }
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

                DemoItemsListBox.ItemsSource = ls.OrderBy(p => p.Order).ToList();
                settingsViewModel.SelectedViewPart = settingsViewModel.SelectableViewParts.First();

                if (settingsViewModel.Settings.hasUSB && pro.numberScreen.SelectedItem != null
                    && pro.numberScreen.SelectedItem != "1")
                    settingsViewModel.Settings.hasUSBTwo = true;
                else
                    settingsViewModel.Settings.hasUSBTwo = false;

                if (pro.numberScreen.SelectedItem != null && pro.numberScreen.SelectedItem != "1")
                    settingsViewModel.Settings.hasScreenTwo = true;
                else
                    settingsViewModel.Settings.hasScreenTwo = false;
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
    }
}
