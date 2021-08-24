using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.Settings
{
    internal class AllDeviceSettings<T> : ViewModelBase, IAllDeviceSettings<T> where T : IDeviceSettings
    {
        private IList<T> _allDevices;
        public IList<T> AllDevices { get => _allDevices; set { Set(() => AllDevices, ref _allDevices, value); } }
    }
}
