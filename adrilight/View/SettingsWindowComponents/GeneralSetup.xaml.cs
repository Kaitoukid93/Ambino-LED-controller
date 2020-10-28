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
using System.Configuration;
using System.Diagnostics;


namespace adrilight.View.SettingsWindowComponents
{
    /// <summary>
    /// Interaction logic for LightingMode.xaml
    /// </summary>
    public partial class GeneralSetup : UserControl
    {
        public GeneralSetup()
        {
            InitializeComponent();
        }



        public class GeneralSetupSelectableViewPart : ISelectableViewPart
        {
            private readonly Lazy<GeneralSetup> lazyContent;

            public GeneralSetupSelectableViewPart(Lazy<GeneralSetup> lazyContent)
            {
                this.lazyContent = lazyContent ?? throw new ArgumentNullException(nameof(lazyContent));
            }

            public int Order => 900;

            public string ViewPartName => "Cài đặt chung";

            public object Content { get => lazyContent.Value; }
        }
        private void EmailButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto://lienhe@ambino.net");
        }
        private void GitHubButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/fabsenet/adrilight");

        }
        private void ChatButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://m.me/109869992932970");
        }
        private void QuestionButton_onclick(object sender,RoutedEventArgs e)
        {
            Process.Start("https://ambino.net/huong-dan-lap-dat-ambino");
        }
        private void CorldButton_onclick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://colrd.com/palette/");
        }


    }
}
