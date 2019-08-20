/*
*---------------------------------
*|		All rights reserved.
*|		author: lizhanping
*|		version:1.0
*|		File: RelayCommand.cs
*|		Summary: 
*|		Date: 2019/8/19 14:55:46
*---------------------------------
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace UpdateService
{
    public class RelayCommand : ICommand
    {
        private Action _execute;
        private Func<bool> _canExecute;
        public RelayCommand(Action action,Func<bool> func)
        {
            if (action == null)
                throw new ArgumentNullException("Execute");
            this._execute = action;
            this._canExecute = func;
        }

        public RelayCommand(Action action) : this(action, null) { }


        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            else
            {
                return _canExecute.Invoke();
            }
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }

        public void RaiseExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
