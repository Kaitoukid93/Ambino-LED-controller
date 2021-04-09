using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
   public class DeviceCard: INotifyPropertyChanged
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
        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected == value) return;
                _isConnected = value;
                OnPropertyChanged();
            }
        }
        private bool _isNew = false;
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                if (_isNew == value) return;
                _isNew = value;
                OnPropertyChanged();
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _typeName;
        public string TypeName
        {
            get { return _typeName; }
            set
            {
                if (_typeName == value) return;
                _typeName = value;
                OnPropertyChanged();
            }
        }
        private string _comPort;
        public string ComPort
        {
            get { return _comPort; }
            set
            {
                if (_comPort == value) return;
                _comPort = value;
                OnPropertyChanged();
            }
        }
        private string _description;
        public string Description
        {
            get { return _comPort; }
            set
            {
                if (_comPort == value) return;
                _comPort = value;
                OnPropertyChanged();
            }
        }
        private double _rightness;
        public double Brightness
        {
            get { return _rightness; }
            set
            {
                if (_rightness == value) return;
                _rightness = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
