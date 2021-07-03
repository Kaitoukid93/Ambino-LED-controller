using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel.Factories
{
   public class AllDeviceViewModelFactory : IViewModelFactory<AllDeviceViewModel>
    {
        private List<IDeviceSettings> AllDevices;
        public AllDeviceViewModelFactory(List<IDeviceSettings> allDevices)
        {
            AllDevices = allDevices;
        }
        public AllDeviceViewModel CreateViewModel()
        {
            return new AllDeviceViewModel(AllDevices);
        }
    }
}
