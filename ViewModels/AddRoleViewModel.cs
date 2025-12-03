using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using gc_bot.Requests;
using gc_bot.Requests.Models;
using gc_bot.Model;

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

        private readonly IRequestService? _requestService;

        public AddRoleViewModel(IRequestService? requestService = null)
        {
            Platforms = new ObservableCollection<string>
            {
                "快玩",
                "傲世堂",
                "唐门"
            };

            SelectedPlatform = Platforms.Count > 0 ? Platforms[0] : string.Empty;

            _requestService = requestService;

            // Login command will call the injected IRequestService when available.
            LoginCommand = new RelayCommand(async _ => { await LoginAsync(); }, _ => !IsBusy);

            // Confirm: create a Role and notify subscribers, then request the window to close with 'true'.
            ConfirmCommand = new RelayCommand(_ =>
            {
                var role = new Role(
                    region: SelectedPlatform ?? "未知大区",
                    server: string.IsNullOrWhiteSpace(Server) ? "0" : Server,
                    faction: "未知势力",
                    nickname: string.IsNullOrWhiteSpace(Username) ? "未命名" : Username,
                    level: 1)
                {
                    // initial resources left at defaults (0)
                };

                RoleCreated?.Invoke(role);
                RequestClose?.Invoke(true);
            }, _ => !IsBusy);

            // Cancel: request close with false
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

        // New event: raised when ConfirmCommand creates a Role.
        public event Action<Role>? RoleCreated;

        // Expose the last login response for UI binding
        public LoginResponse? LastLoginResponse { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value!;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        private async Task LoginAsync()
        {
            if (_requestService is null) 
            {
                LoginInfo = "No request service available.";
                return;
            }

            IsBusy = true;
            try
            {
                var req = new LoginRequest
                {
                    Platform = SelectedPlatform,
                    Server = Server,
                    Username = Username,
                    Password = Password
                };

                var resp = await _requestService.LoginAsync(req);
                LastLoginResponse = resp;

                if (resp.Success && !string.IsNullOrWhiteSpace(resp.Token))
                {
                    // fetch user info with the returned token and display full raw response
                    var gameInfo = await _requestService.GetStartGameAsync(resp.Token);
                    if (gameInfo is not null)
                        //if (gameInfo is not null && gameInfo.Success)
                        {
                        //LoginInfo = gameInfo?.RawJson ?? string.Empty;
                        LoginInfo = gameInfo.ToString();
                    }
                    else
                    {
                        //LoginInfo = gameInfo?.RawJson ?? (gameInfo?.Message ?? $"Login succeeded but failed to get user info: {resp.Message}");
                    }
                }
                else
                {
                    LoginInfo = $"Error: {resp.Message}";
                }
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
    }

    // Small RelayCommand extension to raise CanExecuteChanged explicitly
    public partial class RelayCommand
    {
        public void RaiseCanExecuteChanged() => System.Windows.Input.CommandManager.InvalidateRequerySuggested();
    }
}