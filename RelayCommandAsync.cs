using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace JustMVVM
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// Can be used alongside CommandParameter when binding to any object
    /// </summary>
    public class RelayCommandAsync<T> : ICommand
    {
        readonly Predicate<T> _canExecute;
        readonly Func<T, Task> _execute;

        public bool IsExecuting { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandAsync"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommandAsync(Func<T, Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandAsync"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommandAsync(Func<T, Task> execute, Predicate<T> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Event for when method changes if it can execute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Checks if the method can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return !IsExecuting;
            else
                return !IsExecuting && _canExecute((T)parameter);
        }

        /// <summary>
        /// Execute method of command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(Object parameter)
        {
            try
            {
                Task.Run(async () => await ExecuteAsync(parameter));
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
                throw;
            }
        }

        private async Task ExecuteAsync(object parameter)
        {
            IsExecuting = true;
            await _execute((T)parameter);
            IsExecuting = false;

            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }
    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// </summary>
    public class RelayCommandAsync : ICommand
    {
        readonly Func<bool> _canExecute;
        readonly Func<Task> _execute;

        public bool IsExecuting { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandAsync"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommandAsync(Func<Task> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommandAsync"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute)
        {

            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Called when method changes if it can execute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// True if method can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public bool CanExecute(Object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        /// <summary>
        /// Execute method of a command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            try
            {
                Task.Run(async () => await ExecuteAsync());
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
                throw;
            }
        }

        private async Task ExecuteAsync()
        {
            IsExecuting = true;
            // Force CanExecute to run before actually executing so it can't run multiple times on a long running process
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());

            await _execute();

            IsExecuting = false;
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }
    }
}
