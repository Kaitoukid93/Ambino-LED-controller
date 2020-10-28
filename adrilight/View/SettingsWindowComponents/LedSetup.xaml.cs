using adrilight.ViewModel;
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

namespace adrilight.View.SettingsWindowComponents
{
    /// <summary>
    /// Interaction logic for LedSetup.xaml
    /// </summary>
    public partial class LedSetup : UserControl
    {
        public LedSetup()
        {
            InitializeComponent();
        }

        public class LedSetupSelectableViewPart : ISelectableViewPart
        {
            private readonly Lazy<LedSetup> lazyContent;

            public LedSetupSelectableViewPart(Lazy<LedSetup> lazyContent)
            {
                this.lazyContent = lazyContent ?? throw new ArgumentNullException(nameof(lazyContent));
            }
            public int Order => 10;

            public string ViewPartName => "Chọn cỡ màn hình";

            public object Content { get => lazyContent.Value; }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        public int D3vicE;

        private void Screenbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if (screenbox.SelectedIndex == 0)
            {

                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide.IsEnabled = false;
                heightslide.IsEnabled = false;
                offsetslide.IsEnabled = false;
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
                widthslide.IsEnabled = false;
                heightslide.IsEnabled = false;
                offsetslide.IsEnabled = false;
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
                widthslide.IsEnabled = false;
                heightslide.IsEnabled = false;
                offsetslide.IsEnabled = false;
               
                    width.Text = "14";
                    height.Text = "6";
                    offset.Text = "13";
                
             

              
            }
            else if (screenbox.SelectedIndex == 3)
            {
                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide.IsEnabled = false;
                heightslide.IsEnabled = false;
                offsetslide.IsEnabled = false;
                
                    width.Text = "14";
                    height.Text = "7";
                    offset.Text = "13";
              

               
            }
            else if (screenbox.SelectedIndex == 4)
            {
                width.IsEnabled = false;
                height.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide.IsEnabled = false;
                heightslide.IsEnabled = false;
                offsetslide.IsEnabled = false;
                
                    width.Text = "15";
                    height.Text = "7";
                    offset.Text = "14";
               
                   
                

                
            }
           
           
            else if (screenbox.SelectedIndex == 5)
            {
                width.IsEnabled = true;
                height.IsEnabled = true;
                offset.IsEnabled = true;
                widthslide.IsEnabled = true;
                heightslide.IsEnabled = true;
                offsetslide.IsEnabled = true;


            }
          
        }


        private void Devicebox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // screenbox.SelectedItem = "Kích thước khác";
            //to reset selected value
            //screenbox.SelectedIndex = 0;
           // if (D3vicE != devicebox.SelectedIndex)
           // {

          //  }
            D3vicE = devicebox.SelectedIndex;
            //Screenbox_SelectionChanged(sender,e);
            //Application.Current.Shutdown();
           // System.Windows.Forms.Application.Restart();


        }

        private void Screenbox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (screenbox2.SelectedIndex == 0)
            {

                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide2.IsEnabled = false;
                heightslide2.IsEnabled = false;



                width2.Text = "10";
                height2.Text = "6";
                //offset.Text = "9";





            }
           else if (screenbox2.SelectedIndex == 1)
            {

                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide2.IsEnabled = false;
                heightslide2.IsEnabled = false;
                
               
               
                    width2.Text = "11";
                    height2.Text = "6";
                    //offset.Text = "9";
                
                



            }
            else if (screenbox2.SelectedIndex == 2)
            {
                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide2.IsEnabled = false;
                heightslide2.IsEnabled = false;
                offsetslide.IsEnabled = false;
                width2.Text = "14";
                height2.Text = "6";
                   // offset.Text = "13";
              

            }
            else if (screenbox2.SelectedIndex == 3)
            {
                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide2.IsEnabled = false;
                heightslide2.IsEnabled = false;
                offsetslide.IsEnabled = false;
                
                    width2.Text = "14";
                    height2.Text = "7";
                   // offset.Text = "13";
               
            }
            else if (screenbox2.SelectedIndex == 4)
            {
                width2.IsEnabled = false;
                height2.IsEnabled = false;
                offset.IsEnabled = false;
                widthslide2.IsEnabled = false;
                heightslide2.IsEnabled = false;
                offsetslide.IsEnabled = false;
                  width2.Text = "15";
                   height2.Text = "7";
                
            
            }
            else if (screenbox2.SelectedIndex == 5)
            {
                width.IsEnabled = true;
                height.IsEnabled = true;
                offset.IsEnabled = true;
                widthslide.IsEnabled = true;
                heightslide.IsEnabled = true;
                offsetslide.IsEnabled = true;
            }
           
        }
    }

}
