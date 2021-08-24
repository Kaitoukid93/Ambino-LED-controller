using adrilight.Spots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel.Factories
{
    class LightingViewModelFactory //: IViewModelFactory<BaseVieTwModel>
    {
        private IDeviceSettings Device;
        private IDeviceSpotSet DeviceSpotSet;
        private IGeneralSpotSet GeneralSpotSet;
        public LightingViewModelFactory(IDeviceSettings device, IDeviceSpotSet deviceSpotSet, IGeneralSpotSet generalSpotSet)
        {
            Device = device;
            DeviceSpotSet = deviceSpotSet;
            GeneralSpotSet = generalSpotSet;
        }

        

        //public LightingViewModel CreateViewModel()
        //{
        //    return new LightingViewModel(Device, DeviceSpotSet, GeneralSpotSet);
        //}
    }
}
