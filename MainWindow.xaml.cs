using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace gc_bot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void New_Click(object? sender, RoutedEventArgs e)
        {
            // Placeholder: create new document or reset UI state
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
                // TODO: load the file into the app
            }
        }

        private void Exit_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }

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