using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace gc_bot.ViewModels
{
    public sealed class AddRoleViewModel : INotifyPropertyChanged
    {
        private string _selectedPlatform = string.Empty;
        private string _server = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _loginInfo = string.Empty;
        private bool _isBusy;

        public AddRoleViewModel()
        {
            Platforms = new ObservableCollection<string>
            {
                "快玩",
                "傲世堂",
                "唐门"
            };

            SelectedPlatform = Platforms.Count > 0 ? Platforms[0] : string.Empty;

            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => !IsBusy);
            ConfirmCommand = new RelayCommand(_ => RequestClose?.Invoke(true), _ => !IsBusy);
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(false));
        }

        public ObservableCollection<string> Platforms { get; }

        public string SelectedPlatform
        {
            get => _selectedPlatform;
            set => SetProperty(ref _selectedPlatform, value);
        }

        public string Server
        {
            get => _server;
            set => SetProperty(ref _server, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        // Note: PasswordBox cannot be two-way bound directly. Window code-behind will update this.
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string LoginInfo
        {
            get => _loginInfo;
            set => SetProperty(ref _loginInfo, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    // notify commands can-execute changed
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (ConfirmCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        // The Window subscribes to this to close the dialog with the boolean result.
        public event Action<bool?>? RequestClose;

        private async Task LoginAsync()
        {
            IsBusy = true;
            try
            {
                // Simulate network request. Replace with real IRequestService call later.
                await Task.Delay(700);
                // Example fake response:
                LoginInfo = $"Mock login succeeded for '{Username}' on {SelectedPlatform} (server {Server}).";
            }
            catch (Exception ex)
            {
                LoginInfo = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value!;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }

    // Small RelayCommand extension to raise CanExecuteChanged explicitly
    public partial class RelayCommand
    {
        public void RaiseCanExecuteChanged() => System.Windows.Input.CommandManager.InvalidateRequerySuggested();
    }
}