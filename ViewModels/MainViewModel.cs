using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using gc_bot.Model;

namespace gc_bot.ViewModels
{
    public sealed class MainViewModel
    {
        public ObservableCollection<RoleViewModel> Roles { get; } = new();

        // existing command used by TopButtons for non-add actions
        public ICommand ButtonCommand { get; }

        public MainViewModel()
        {
            ButtonCommand = new RelayCommand(OnButtonExecuted);

            // seed multiple roles for demonstration
            Roles.Add(new RoleViewModel(new Role("大区A", "1", "势力X", "示例一", level: 10)
            {
                Gold = 1200,
                Silver = 4500,
                Wood = 300,
                Grain = 250,
                RefinedIron = 75,
                Meteorite = 10,
                Silk = 50,
                Pouches = 5,
                Charcoal = 20,
                Essence = 3,
                PreciousOre = 1,
                Phantoms = 0
            }));

            Roles.Add(new RoleViewModel(new Role("大区B", "2", "势力Y", "示例二", level: 22)
            {
                Gold = 5000,
                Silver = 20000,
                Wood = 1200,
                Grain = 800,
                RefinedIron = 300,
                Meteorite = 25,
                Silk = 200,
                Pouches = 12,
                Charcoal = 100,
                Essence = 10,
                PreciousOre = 2,
                Phantoms = 1
            }));

            Roles.Add(new RoleViewModel(new Role("大区C", "3", "势力Z", "示例三", level: 5)
            {
                Gold = 200,
                Silver = 800,
                Wood = 50,
                Grain = 60,
                RefinedIron = 5,
                Meteorite = 0,
                Silk = 10,
                Pouches = 0,
                Charcoal = 2,
                Essence = 0,
                PreciousOre = 0,
                Phantoms = 0
            }));

            Roles.Add(new RoleViewModel(new Role("大区A", "4", "势力X", "示例四", level: 18)
            {
                Gold = 2500,
                Silver = 9000,
                Wood = 600,
                Grain = 400,
                RefinedIron = 120,
                Meteorite = 7,
                Silk = 80,
                Pouches = 6,
                Charcoal = 30,
                Essence = 5,
                PreciousOre = 1,
                Phantoms = 2
            }));

            // optionally simulate background updates
            _ = Task.Run(() => SimulateBackgroundUpdates(CancellationToken.None));
        }

        private void OnButtonExecuted(object? parameter)
        {
            // handle other buttons in a simple way for demo
            if (parameter is int i)
            {
                Roles.Add(new RoleViewModel(new Role("Region", i.ToString(), "Faction", $"Role{i}", 1)));
            }
            else if (parameter is string s && int.TryParse(s, out var j))
            {
                Roles.Add(new RoleViewModel(new Role("Region", j.ToString(), "Faction", $"Role{j}", 1)));
            }
        }

        /// <summary>
        /// Thread-safe add of a role to the collection (can be called from any thread).
        /// </summary>
        public void AddRole(Role role)
        {
            if (role is null) throw new ArgumentNullException(nameof(role));
            var vm = new RoleViewModel(role);
            var dsp = Application.Current?.Dispatcher;
            if (dsp is not null && !dsp.CheckAccess())
            {
                dsp.Invoke(() => Roles.Add(vm));
            }
            else
            {
                Roles.Add(vm);
            }
        }

        /// <summary>
        /// Remove by RoleViewModel instance (thread-safe).
        /// </summary>
        public void RemoveRole(RoleViewModel roleVm)
        {
            if (roleVm is null) return;
            var dsp = Application.Current?.Dispatcher;
            if (dsp is not null && !dsp.CheckAccess())
            {
                dsp.Invoke(() => Roles.Remove(roleVm));
            }
            else
            {
                Roles.Remove(roleVm);
            }
        }

        /// <summary>
        /// Find role by nickname.
        /// </summary>
        public RoleViewModel? FindByNickname(string nick) =>
            Roles.FirstOrDefault(r => string.Equals(r.Nickname, nick, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Example background updater to show how role data may change at runtime.
        /// Replace with real update logic (network push, polling, game engine events).
        /// </summary>
        private async Task SimulateBackgroundUpdates(CancellationToken ct)
        {
            var rnd = new Random();
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000, ct).ContinueWith(_ => { }, TaskScheduler.Default);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (ct.IsCancellationRequested) break;
                if (Roles.Count == 0) continue;

                var idx = rnd.Next(Roles.Count);
                var target = Roles[idx];

                // random adjustments
                var goldDelta = rnd.Next(-50, 151);
                target.AdjustResource("金币", goldDelta);

                // update level occasionally
                if (rnd.NextDouble() < 0.1)
                {
                    var newLevel = Math.Max(1, target.Level + rnd.Next(0, 2));
                    target.Level = newLevel;
                }
            }
        }
    }
}