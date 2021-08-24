using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   public class LedLight : NotifyObject
    {
        private bool _isActice = false;
        public bool IsActive
        {
            get { return _isActice; }
            set
            {
                if (_isActice == value) return;
                _isActice = value;
                OnPropertyChanged();
            }
        }
        private string _num ;
        public string Num
        {
            get { return _num; }
            set
            {
                if (_num == value) return;
                _num = value;
                OnPropertyChanged();
            }
        }
    }
}
