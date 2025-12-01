using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using gc_bot.ViewModels;

namespace gc_bot.Views
{
    public partial class AddRoleWindow : Window
    {
        public AddRoleWindow()
        {
            InitializeComponent();

            // Create VM if not provided by caller.
            if (DataContext is not AddRoleViewModel)
            {
                DataContext = new AddRoleViewModel();
            }

            if (DataContext is AddRoleViewModel vm)
            {
                vm.RequestClose += Vm_RequestClose;
            }
        }

        private void Vm_RequestClose(bool? result)
        {
            // set DialogResult and close window
            DialogResult = result;
            Close();
        }

        // Keep the PasswordBox value flowing to the VM (PasswordBox can't bind directly to a secure string easily here).
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddRoleViewModel vm)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        // Allow server textbox to accept only digits
        private static readonly Regex _digitsRegex = new(@"^\d*$");

        private void Server_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_digitsRegex.IsMatch(e.Text);
        }
    }
}