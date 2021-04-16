using BO;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel
{
   public class PreviewViewModel : ViewModelBase
    {
        private ObservableCollection<LedLight> _ledLights;
        public ObservableCollection<LedLight> LedLights {
            get { return _ledLights; }
            set
            {
                if (_ledLights == value) return;
                _ledLights = value;
                RaisePropertyChanged();
            }
        }
        public PreviewViewModel()
        {
            LedLights = new ObservableCollection<LedLight>();
            for (int i = 0; i < 66; i++)
            {
                LedLight led = new LedLight();
                if (i < 30)
                {
                    led.Num = (i + 1).ToString();
                    led.IsActive = true;
                }
                LedLights.Add(led);
            }
        }
    }
}
