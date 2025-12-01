using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using gc_bot.ViewModels;

namespace gc_bot.Views
{
    public partial class AddRoleWindow : Window
    {
        private AddRoleViewModel? _attachedVm;

        public AddRoleWindow()
        {
            InitializeComponent();

            // Watch for DataContext changes so the window attaches to the actual VM
            // whether the caller sets it via object initializer or not.
            DataContextChanged += AddRoleWindow_DataContextChanged;

            // Attach to the current DataContext if it is already set.
            AttachVm(DataContext as AddRoleViewModel);

            // If no DataContext provided by caller, create a default VM.
            if (DataContext is not AddRoleViewModel)
            {
                DataContext = new AddRoleViewModel();
            }
        }

        private void AddRoleWindow_DataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
        {
            AttachVm(e.NewValue as AddRoleViewModel);
        }

        private void AttachVm(AddRoleViewModel? vm)
        {
            if (_attachedVm is not null)
            {
                _attachedVm.RequestClose -= Vm_RequestClose;
            }

            _attachedVm = vm;

            if (_attachedVm is not null)
            {
                _attachedVm.RequestClose += Vm_RequestClose;

#if DEBUG
                // Prefill values only in DEBUG builds to ease debugging.
                // Update both VM and view controls because PasswordBox cannot be updated via binding.
                if (string.IsNullOrWhiteSpace(_attachedVm.SelectedPlatform))
                {
                    _attachedVm.SelectedPlatform = "快玩";
                }

                if (string.IsNullOrWhiteSpace(_attachedVm.Server))
                {
                    _attachedVm.Server = "396";
                }

                if (string.IsNullOrWhiteSpace(_attachedVm.Username))
                {
                    _attachedVm.Username = "testuserone";
                }

                if (string.IsNullOrWhiteSpace(_attachedVm.Password))
                {
                    _attachedVm.Password = "testone";
                }

                // Ensure PasswordBox visual reflects the prefilled VM password (PasswordBox won't update from VM).
                if (PasswordBox is not null)
                {
                    // Only set when different to avoid extra events.
                    if (PasswordBox.Password != _attachedVm.Password)
                    {
                        PasswordBox.Password = _attachedVm.Password ?? string.Empty;
                    }
                }
#endif
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