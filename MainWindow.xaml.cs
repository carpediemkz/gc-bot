using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using gc_bot.ViewModels;

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
            // subscribe to the TopButtons CLR event from code-behind
            
            TopButtonsControl.ButtonClicked += ButtonClick;
        }

        // event handler signature must match EventHandler<int>: (object sender, int arg)
        private void ButtonClick(object? sender, int index)
        {
            // empty here
            // todo
            // ItemsPanelControl?.ScrollToEnd();
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