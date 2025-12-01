using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using gc_bot.ViewModels;
using gc_bot.Views;

namespace gc_bot.Controls
{
    public partial class TopButtons : UserControl
    {
        public TopButtons()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raised when any top button is clicked. The int argument is the button number (1..8).
        /// Kept for backward compatibility.
        /// </summary>
        public event EventHandler<int>? ButtonClicked;

        /// <summary>
        /// Raised when a new role is successfully created via the AddRole dialog.
        /// Subscribers (typically the host window) receive the created ItemViewModel.
        /// </summary>
        public event EventHandler<ItemViewModel>? RoleAdded;

        public static readonly DependencyProperty ButtonsCommandProperty =
            DependencyProperty.Register(
                nameof(ButtonsCommand),
                typeof(ICommand),
                typeof(TopButtons),
                new PropertyMetadata(null));

        public ICommand? ButtonsCommand
        {
            get => (ICommand?)GetValue(ButtonsCommandProperty);
            set => SetValue(ButtonsCommandProperty, value);
        }

        // Use exact RoutedEventHandler signature so XAML can wire the Click correctly.
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var index = -1;
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out var idx))
            {
                index = idx;
            }

            // For MVVM-friendly behavior, raise the CLR event first.
            ButtonClicked?.Invoke(this, index);

            // If button 1 is "????" (Add Role) handle dialog here inside the control.
            if (index == 1)
            {
                // Create and show the Add Role dialog. Use the nearest window as owner.
                var addVm = new AddRoleViewModel();
                var dlg = new AddRoleWindow
                {
                    DataContext = addVm,
                    Owner = Window.GetWindow(this)
                };

                var res = dlg.ShowDialog();
                if (res == true)
                {
                    // Create an ItemViewModel and notify host via RoleAdded event.
                    var title = $"{addVm.Username}@{addVm.SelectedPlatform}/{addVm.Server}";
                    var item = new ItemViewModel(title, addVm.LoginInfo ?? string.Empty);
                    RoleAdded?.Invoke(this, item);
                }

                // Do not continue to execute the generic ButtonsCommand for this special action,
                // unless you want both behaviors. Return to avoid duplicate handling.
                return;
            }

            // Fallback: execute bound ICommand if available for other buttons.
            if (ButtonsCommand is ICommand cmd && cmd.CanExecute(index))
            {
                cmd.Execute(index);
            }
        }
    }
}