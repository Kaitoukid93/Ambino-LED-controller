using ColorPickerWPF;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace adrilight.ViewModel
{
  public  class AppSettingViewModel: ViewModelBase
    {
        public ICommand SelectColorCommand { get; set; }
        private Color _selectedColor;
        public Color SelectedColor {
            get { return _selectedColor; }
            set
            {
                if (_selectedColor == value) return;
                _selectedColor = value;
                RaisePropertyChanged();
            }
        }
        public AppSettingViewModel()
        {
            ReadData();
        }
        public void ReadData()
        {
            SelectedColor = Colors.White;
            SelectColorCommand = new RelayCommand<string>((p) => {
                return true;
            }, (p) =>
            {
                Color color;

                bool ok = ColorPickerWindow.ShowDialog(out color);
                if (ok)
                {
                    SelectedColor = color;
                }
            });
        }
    }
}
