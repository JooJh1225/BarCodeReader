using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BarcodeReader
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public Func<object, bool> CanExecuteFunc;
        public Action<object> ExecuteAct;

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            ExecuteAct?.Invoke(parameter);
        }
    }
}
