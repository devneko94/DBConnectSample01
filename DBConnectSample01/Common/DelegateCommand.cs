using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DBConnectSample01.Common
{
    public class DelegateCommand : ICommand
    {
        private Action _action = null;
        private Func<bool> _func = null;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action) : this(action, () => true) { }

        public DelegateCommand(Action action, Func<bool> func)
        {
            _action = action;
            _func = func;
        }

        public bool CanExecute(object parameter)
        {
            return _func();
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public class DelegateCommand<T> : ICommand
    {
        private Action<T> _action = null;
        private Func<T, bool> _func = null;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> action) : this(action, (T) => true) { }

        public DelegateCommand(Action<T> action, Func<T, bool> func)
        {
            _action = action;
            _func = func;
        }

        public bool CanExecute(object parameter)
        {
            return _func((T)parameter);
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
