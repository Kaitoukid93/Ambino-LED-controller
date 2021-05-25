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
using WpfAnimatedGif;
using System.Drawing;
using System.Runtime.InteropServices;
using adrilight.Util;
using OpenRGB;
using System.Collections;

namespace adrilight.View.SettingsWindowComponents
{
    /// <summary>
    /// Interaction logic for LedOutsideCase.xaml
    /// </summary>
    public partial class LedOutsideCase : UserControl
    {

     
        
       
     

 

        public static byte[] output_spectrumdata = new byte[16];
        public static int[] order_data = new int[16];
        public static int[] custom_order_data = new int[16];
       
        public bool isEffect { get; set; }
        public bool isPlaying=false;
        public BitmapImage gifimage;
        public Stream gifStreamSource;
        public int framecount = 0;
        GifBitmapDecoder decoder;
        public int pixelcounter = 0;
        private static WriteableBitmap MatrixBitmap { get; set; }

       
        

        
        



        //new gifxelation mode//
        //private int _imX1;
        //private int _imX2;
        //private int _imY1;
        //private int _imY2;

        //private int _imXMax;
        //private int _imYMax;
        //private bool? _imLockDim = false;
        //private string _text_IM_WidthHeight;

        //private bool? _gifPlayPause = false;

        //private int _imInterpolationModeIndex = 3;

        //private BitmapSource _contentBitmap;
        //private BitmapSource _gifPlayPauseImage = MatrixFrame.CreateBitmapSourceFromBitmap(Properties.Resources.icons8_play_32);


        //DispatcherTimer GifTimer = new DispatcherTimer();
        //DispatcherTimer PixTimer = new DispatcherTimer();
        //DispatcherTimer MusicTimer = new DispatcherTimer();
        //private static double _pixframeIndex = 0;
        //private static int _gifFrameIndex = 0;
        ////new gifxelation mode//

        public LedOutsideCase()
        {
            InitializeComponent();
            //var settingsViewModel = DataContext as SettingsViewModel;

         
            //MatrixFrame.DimensionsChanged += OnMatrixDimensionsChanged;
            //MatrixFrame.FrameChanged += OnFrameChanged;
            //MatrixFrame.SetDimensions(MatrixFrame.Width, MatrixFrame.Height);
            ////gifxelation load default gif//



            //ImageProcesser.DisposeGif();
            //ImageProcesser.DisposeStill();
            //ImageProcesser.LoadGifFromResource();


            

            //settingsViewModel.ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
            //MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
            //    EndX.Maximum = ImageProcesser.LoadedGifImage.Width;
            //    startX.Maximum = ImageProcesser.LoadedGifImage.Width;
            //    EndX.Value = ImageProcesser.LoadedGifImage.Width;
            //    EndY.Maximum = ImageProcesser.LoadedGifImage.Height;
            //    StartY.Maximum = ImageProcesser.LoadedGifImage.Height;
            //    EndY.Value = ImageProcesser.LoadedGifImage.Height;
            //    startX.Value = 0;
            //    StartY.Value = 0;
            //ImageProcesser.ImageRect = new System.Drawing.Rectangle(Convert.ToInt32(startX.Value), Convert.ToInt32(StartY.Value), Convert.ToInt32(EndX.Value - startX.Value),Convert.ToInt32(EndY.Value-StartY.Value));
            ////FrameToPreview();
            ////SerialManager.PushFrame();
            //ImageProcesser.ImageLoadState = ImageProcesser.LoadState.Gif;
            ////ResetSliders();

            //gifxelation//

            
                
            





       


           // DispatcherTimer dispatcherTimer = new DispatcherTimer();
         
            

           // dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
          //  dispatcherTimer.Interval = TimeSpan.FromMilliseconds(5);
            //dispatcherTimer2.Tick += new EventHandler(random_Tick);
            //dispatcherTimer2.Interval = new TimeSpan(0, 0, 30);
            
            
           // dispatcherTimer.Start();
           
            
            //StartMusic();




   
            //if (Music_box_1.SelectedIndex >= 0)
            //{
            

            //}
            //Init();
            //  var array = (Bassbox.Items[Bassbox.SelectedIndex] as string).Split(' ');
       
            // BassWasapi.BASS_WASAPI_Init(-1, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);

    


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
     

      
      
      
      
        

       
     
      
        //public BitmapSource GifPlayPauseImage {
        //    get { return _gifPlayPauseImage; }
        //    set
        //    {
        //        if (value != _gifPlayPauseImage)
        //        {
        //            _gifPlayPauseImage = value;
        //            //OnPropertyChanged();
        //        }
        //    }
        //}


        

        


      


  
        //public void StartMusic()
        //{

        //    MusicTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
        //    MusicTimer.Tick += new EventHandler(dispatcherTimer_Tick);
        //    MusicTimer.Start();

        //}
       
        //public void StopPix()
        //{
        //    PixTimer.Stop();
        //}

        //public void Gif_Timer_Tick(object sender, EventArgs e)
        //{
        //    var settingsViewModel = DataContext as SettingsViewModel;
        //    ImageProcesser.DisposeWorkingBitmap();
           
        //    if(EffectSelection.SelectedIndex==6)
        //    {
        //        if (_gifFrameIndex >= ImageProcesser.LoadedGifFrameCount - 1)
        //            _gifFrameIndex = 0;
        //        else
        //            _gifFrameIndex++;
        //        //Console.WriteLine(_gifFrameIndex);
        //        ImageProcesser.LoadedGifImage.SelectActiveFrame(ImageProcesser.LoadedGifFrameDim, _gifFrameIndex);
        //        ImageProcesser.WorkingBitmap = ImageProcesser.CropBitmap(new Bitmap(ImageProcesser.LoadedGifImage), ImageProcesser.ImageRect);
        //        settingsViewModel.ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
        //        MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
        //    }
            
        //    ImageProcesser.DisposeWorkingBitmap();
        //    //SerialManager.PushFrame();

        //    //this section is for RGB processing mode
        //    //User input start color hue and we create a hue gradient or rainbow smooth color or whatever


           

        //    //create first frame
        //    //push to view

        //    //frame moving(index ++)


        //    //end frame??


        //}




   
     
      



        //private static string[] lines = new string[28];
        //private static string[] lines2 = new string[24];
        
        
        //public static string temp;
        //public static string gifilepath;
        //private IUserSettings UserSettings { get; }
        //public static byte DFUVal = 0;
        
        //private const int size = 30;
        private const int space = 3;




       

        //cleanup
     


      

       

        //public void random_Tick(object sender, EventArgs e)
        //{
        //    if (Shuffle.IsChecked == true)
        //    {
        //        Random rnd = new Random();
        //        int index = rnd.Next(0, 30);
        //        effectbox.SelectedIndex = index;
        //    }
        //    if (shuffle.IsChecked == true)
        //    {
        //        Random rnd2 = new Random();
        //        int index = rnd2.Next(0, 7);
        //        method.SelectedIndex = index;
        //    }


        //}

       







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

    

     

    

        private void ColorPicker_ColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
        {

        }

        private void ColorZone_MouseEnter(object sender, MouseEventArgs e)
        {



        }

        

       

     

      
        


      
        




      

        

        

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            pixelcounter = 0;
            playground.Children.Clear();
            if (ledx.Text != null && ledy.Text != null)
            {
                int width = Convert.ToInt32(ledx.Text);
                int height = Convert.ToInt32(ledy.Text);

                //  MatrixFrame.DrawRectangles(width, height,playground);
                System.Windows.Media.Color color1, color2, color3, color4, color5, color6, color7, color8;
             





                MatrixFrame.SetDimensions(Convert.ToInt32(ledx.Text), Convert.ToInt32(ledy.Text));









                //Draw new rectangle matrix//

















            }
        }



    

        public int[] customorder = new int[MatrixFrame.Width*MatrixFrame.Height];
        public void CanvasTouch(object sender, MouseButtonEventArgs e)
        {
            var settingsViewModel = DataContext as SettingsViewModel;
            if (e.OriginalSource is System.Windows.Shapes.Rectangle)//rectangle clicked
            {
                int width = Convert.ToInt32(ledx.Text);
                int height = Convert.ToInt32(ledy.Text);
                System.Windows.Shapes.Rectangle activeRectangel = (System.Windows.Shapes.Rectangle)e.OriginalSource;
                if (pickxeltext.Text != null)
                {

                
                    if (Convert.ToString(activeRectangel.Fill) == "#FFFFFFFF")
                    {
                        
                            activeRectangel.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(pickxeltext.Text);
                        int i = Convert.ToInt32(Canvas.GetLeft(activeRectangel) / (activeRectangel.Width + space));
                        int j = Convert.ToInt32(Canvas.GetTop(activeRectangel) / (activeRectangel.Width + space));

                        pixelcounter++;
                        //customorder[pixelcounter] = width * i + j;
                        TextBlock order = new TextBlock();
                        order.Text = Convert.ToString(pixelcounter);
                        playground.Children.Add(order);
                        order.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF7344C3");
                        Canvas.SetLeft(order, Canvas.GetLeft(activeRectangel) + 3);
                        Canvas.SetTop(order, Canvas.GetTop(activeRectangel) + 3);
                        //MatrixFrame.SetRectanglesOrder(settingsViewModel.Settings.LEDorder[pixelcounter], playground);
                        //need to save order right here//

                        }
                //else if (Convert.ToString(activeRectangel.Fill) == pickxeltext.Text)
                //        {
                //            pixelcounter--;
                //        activeRectangel.Fill= (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFFFFFF");

                //    }





                    }
                }
                else
                    {

                    }
            }
        
        private ImageAnimationController _controller;

        //private void gifchip_Click(object sender, RoutedEventArgs e)
        //{
        //    var settingsViewModel = DataContext as SettingsViewModel;



        //    OpenFileDialog gifile = new OpenFileDialog();
        //    gifile.Title = "Chọn file gif";
        //    gifile.CheckFileExists = true;
        //    gifile.CheckPathExists = true;
        //    gifile.DefaultExt = "gif";
        //    gifile.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
        //    gifile.FilterIndex = 2;
        //    gifile.ShowDialog();

        //    if (!string.IsNullOrEmpty(gifile.FileName) && File.Exists(gifile.FileName))
        //    {
        //        var image = new BitmapImage();
        //        image.BeginInit();
        //        image.UriSource = new Uri(gifile.FileName);
        //        image.EndInit();
        //        gifimage = image;
        //        gifilepath = gifile.FileName;
        //        gifStreamSource = new FileStream(gifilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        decoder = new GifBitmapDecoder(gifStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
        //        ImageBehavior.SetAnimatedSource(gifxel, image);



        //        _controller = ImageBehavior.GetAnimationController(gifxel);

        //        _controller.
        //        image.CopyPixels

        //            GifPlayPause = false;
        //        ImageProcesser.DisposeGif();
        //        ImageProcesser.DisposeStill();
        //        if (ImageProcesser.LoadGifFromDisk(gifile.FileName))
        //        {
        //            settingsViewModel.ContentBitmap = MatrixFrame.CreateBitmapSourceFromBitmap(ImageProcesser.WorkingBitmap);
        //            MatrixFrame.BitmapToFrame(ImageProcesser.WorkingBitmap, ImageProcesser.InterpMode);
        //            EndX.Maximum = ImageProcesser.LoadedGifImage.Width;
        //            startX.Maximum = ImageProcesser.LoadedGifImage.Width;
        //            EndX.Value = ImageProcesser.LoadedGifImage.Width;
        //            EndY.Maximum = ImageProcesser.LoadedGifImage.Height;
        //            StartY.Maximum = ImageProcesser.LoadedGifImage.Height;
        //            EndY.Value = ImageProcesser.LoadedGifImage.Height;
        //            startX.Value = 0;
        //            StartY.Value = 0;
        //            FrameToPreview();
        //            SerialManager.PushFrame();
        //            ImageProcesser.ImageLoadState = ImageProcesser.LoadState.Gif;
        //            ResetSliders();
        //        }
        //        else
        //        {
        //            System.Windows.MessageBox.Show("Cannot load image.");
        //        }

        //    }

        //}

        private void gifplaypausebutton_Checked(object sender, RoutedEventArgs e)
        {
            
      
        }

        //private void OnMatrixDimensionsChanged()
        //{
        //    MatrixBitmap = new WriteableBitmap(MatrixFrame.Width, MatrixFrame.Height, 96, 96, PixelFormats.Bgr32, null);

        //    MatrixImage.Source = MatrixBitmap;

        
        //}
        //private void OnFrameChanged()
        //{
        //    Dispatcher.Invoke(() => { FrameToPreview(); });
        //}
        //private void FrameToPreview()
        //{
        //    MatrixBitmap.Lock();
        //    IntPtr pixelAddress = MatrixBitmap.BackBuffer;

        //    Marshal.Copy(MatrixFrame.FrameToInt32(), 0, pixelAddress, (MatrixFrame.Width * MatrixFrame.Height));

        //    MatrixBitmap.AddDirtyRect(new Int32Rect(0, 0, MatrixFrame.Width, MatrixFrame.Height));
        //    MatrixBitmap.Unlock();
        //}
    }



    }

