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

            // Resolve MainViewModel via DI if available so services can be injected.
            var svc = (Application.Current as App)?.Services;
            if (svc?.GetService(typeof(gc_bot.Requests.IRequestService)) is gc_bot.Requests.IRequestService req)
            {
                _vm = new MainViewModel();
                // leave MainViewModel unchanged for now; AddRole dialog will receive IRequestService when opened
            }
            else
            {
                _vm = new MainViewModel();
            }

            DataContext = _vm;

            // Subscribe to RoleAdded raised by the TopButtons control.
            TopButtonsControl.RoleAdded += TopButtonsControl_RoleAdded;
        }

        private void TopButtonsControl_RoleAdded(object? sender, Role e)
        {
            if (e is not null)
            {
                _vm.AddRole(e);
                ItemsPanelControl?.ScrollToEnd();
            }
        }

        // Handler wired in XAML: ButtonClicked="ButtonClick"
        private void ButtonClick(object? sender, int index)
        {
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