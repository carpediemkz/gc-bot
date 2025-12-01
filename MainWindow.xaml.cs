using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using gc_bot.ViewModels;
using gc_bot.Views;
using gc_bot.Model;

namespace gc_bot
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel();
            DataContext = _vm;

            // Subscribe to RoleAdded raised by the TopButtons control.
            TopButtonsControl.RoleAdded += TopButtonsControl_RoleAdded;
        }

        private void TopButtonsControl_RoleAdded(object? sender, ItemViewModel e)
        {
            if (e is not null)
            {
                // Convert the incoming ItemViewModel (title/description) into a Role model
                // and add it to the main ViewModel using its thread-safe AddRole method.
                var nickname = string.IsNullOrWhiteSpace(e.Title) ? "Unnamed" : e.Title;
                var role = new Role(region: "未知大区", server: "0", faction: "未知势力", nickname: nickname, level: 1)
                {
                    // put the description into a resource or use as LoginInfo -> map to Gold as placeholder if needed
                    // keep numeric resources at 0 by default; adapt mapping as required.
                };

                _vm.AddRole(role);
                ItemsPanelControl?.ScrollToEnd();
            }
        }

        // Handler wired in XAML: ButtonClicked="ButtonClick"
        private void ButtonClick(object? sender, int index)
        {
            // Removed AddRole dialog logic from here — TopButtons handles Add Role internally.
            // Delegate other indices to the ViewModel command if available.
            if (_vm.ButtonCommand is ICommand cmd && cmd.CanExecute(index))
            {
                cmd.Execute(index);
                ItemsPanelControl?.ScrollToEnd();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe to avoid leaks.
            TopButtonsControl.RoleAdded -= TopButtonsControl_RoleAdded;
            base.OnClosed(e);
        }

        private void New_Click(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "New command invoked.", "New", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Open_Click(object? sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open File",
                Filter = "All files (*.*)|*.*"
            };

            if (dlg.ShowDialog(this) == true)
            {
                MessageBox.Show(this, $"Selected file: {dlg.FileName}", "Open", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Exit_Click(object? sender, RoutedEventArgs e) => Close();

        private void Preferences_Click(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "Preferences not implemented yet.", "Preferences", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void About_Click(object? sender, RoutedEventArgs e)
        {
            MessageBox.Show(this, "gc_bot\nVersion 0.1\n\u00A9 Your Company", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}