using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SintezWpfUiLib
{
    class RelayCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate
        { get; set; }

        public Action<object> ExecuteDelegate
        { get; set; }

        public RelayCommand(Action<object> action)
        {
            this.ExecuteDelegate = action;
            this.CanExecuteDelegate = null;
        }

        public RelayCommand(Action<object> action, Predicate<object> canExecute)
        {
            this.ExecuteDelegate = action;
            this.CanExecuteDelegate = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return this.CanExecuteDelegate == null ? true : this.CanExecuteDelegate.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            this.ExecuteDelegate?.Invoke(parameter);
        }
    }
}
