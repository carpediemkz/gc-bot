using System;
using System.Windows.Input;

namespace gc_bot.ViewModels
{
    // Marked partial so the small extension (RaiseCanExecuteChanged) declared
    // in another file (AddRoleViewModel.cs) compiles correctly.
    public sealed partial class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value ?? throw new ArgumentNullException(nameof(value));
            remove => CommandManager.RequerySuggested -= value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}