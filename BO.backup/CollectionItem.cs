using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   public class CollectionItem:NotifyObject
    {
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                _value = value;
                OnPropertyChanged();
            }
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                OnPropertyChanged();
            }
        }
        private bool _isselected;
        public bool IsSelected
        {
            get { return _isselected; }
            set
            {
                if (_isselected == value) return;
                _isselected = value;
                OnPropertyChanged();
            }
        }
    }
}
