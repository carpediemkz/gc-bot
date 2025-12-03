using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using gc_bot.ViewModels;
using gc_bot.Views;
using gc_bot.Model;

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
        /// Subscribers receive the created Role.
        /// </summary>
        public event EventHandler<Role>? RoleAdded;

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

            // Special-case: index == 1 is "新增角色" (open AddRole dialog).
            if (index == 1)
            {
                // When opening the AddRole dialog, inject a real IRequestService implementation if available.
                var requestService = new gc_bot.Requests.ApiClient();
                var addVm = new AddRoleViewModel(requestService);
                EventHandler<Role>? createdHandler = null;

                // Subscribe to RoleCreated so we can re-raise RoleAdded to hosts.
                createdHandler = (s, role) =>
                {
                    RoleAdded?.Invoke(this, role);
                };

                // AddRoleViewModel raises RoleCreated via Action<Role>, adapt to EventHandler<Role>
                void RoleCreatedAdapter(Role role) => createdHandler?.Invoke(this, role);
                addVm.RoleCreated += RoleCreatedAdapter;

                var dlg = new AddRoleWindow
                {
                    DataContext = addVm,
                    Owner = Window.GetWindow(this)
                };

                var res = dlg.ShowDialog();

                // Unsubscribe adapter to avoid leaks
                addVm.RoleCreated -= RoleCreatedAdapter;

                // Do not raise generic ButtonClicked or execute ButtonsCommand for this action.
                return;
            }

            // Non-add actions: raise CLR event for backward compatibility, then execute ICommand (MVVM).
            ButtonClicked?.Invoke(this, index);

            if (ButtonsCommand is ICommand cmd && cmd.CanExecute(index))
            {
                cmd.Execute(index);
            }
        }
    }
}