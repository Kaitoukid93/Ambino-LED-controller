using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace adrilight.ViewModel
{
    public class BaseViewModel : ViewModelBase
    {
        public BaseViewModel()
        {
            ReadData();
        }
        
        public virtual void ReadData() { }
    }
    public class RelayCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
           : this(null, execute)
        {
            _execute = execute;
        }

        public RelayCommand(Predicate<T> canExecute,Action<T> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    //public class RelayCommand : ICommand
    //{
    //    private readonly Predicate<object> _canExecute;
    //    private readonly Action<object> _execute;

    //    public RelayCommand(Action<object> execute)
    //       : this(execute, null)
    //    {
    //        _execute = execute;
    //    }

    //    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    //    {
    //        if (execute == null)
    //        {
    //            throw new ArgumentNullException("execute");
    //        }
    //        _execute = execute;
    //        _canExecute = canExecute;
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return _canExecute == null || _canExecute(parameter);
    //    }

    //    public void Execute(object parameter)
    //    {
    //        _execute(parameter);
    //    }

    //    // Ensures WPF commanding infrastructure asks all RelayCommand objects whether their
    //    // associated views should be enabled whenever a command is invoked 
    //    public event EventHandler CanExecuteChanged {
    //        add
    //        {
    //            CommandManager.RequerySuggested += value;
    //            CanExecuteChangedInternal += value;
    //        }
    //        remove
    //        {
    //            CommandManager.RequerySuggested -= value;
    //            CanExecuteChangedInternal -= value;
    //        }
    //    }

    //    private event EventHandler CanExecuteChangedInternal;

    //    public void RaiseCanExecuteChanged()
    //    {
    //        CanExecuteChangedInternal.Raise(this);
    //    }
    //}
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
    public static class EventRaiser
    {
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public static void Raise<T>(this EventHandler<EventArgs<T>> handler, object sender, T value)
        {
            if (handler != null)
            {
                handler(sender, new EventArgs<T>(value));
            }
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T value) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, value);
            }
        }

        public static void Raise<T>(this EventHandler<EventArgs<T>> handler, object sender, EventArgs<T> value)
        {
            if (handler != null)
            {
                handler(sender, value);
            }
        }
    }

}
