using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adrilight.ViewModel.Factories
{
    public class DashboardViewModelFactory : IViewModelFactory<DashboardViewModel>
    {
        public DashboardViewModel CreateViewModel()  // this dashboard contain call device cards that load the settings from device settings
        {
            return new DashboardViewModel();
        }
    }
}
