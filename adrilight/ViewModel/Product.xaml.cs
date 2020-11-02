using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace adrilight
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : Window
    {

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);


        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;

        const uint SC_CLOSE = 0xF060;

        public bool pressSave = false;

        public Product()
        {
            InitializeComponent();
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            pressSave = false;
            this.Close();
        }

        private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if ((bool)CheckBox21.IsChecked && !(bool)CheckBox200.IsChecked && !(bool)CheckBox201.IsChecked)
            {
                MessageBox.Show("Vui lòng chọn loại kết nối PCI hoặc USB!");
            }
            else if ((bool)CheckBox22.IsChecked && !(bool)CheckBox210.IsChecked && !(bool)CheckBox211.IsChecked)
            {
                MessageBox.Show("Vui lòng chọn loại kết nối PCI hoặc USB!");
            }
            else
            {
                pressSave = true;
                this.Close();
            }

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Disable close button
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
        }
    }
}
