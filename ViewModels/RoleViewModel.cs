using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using gc_bot.Model;

namespace gc_bot.ViewModels
{
    /// <summary>
    /// ViewModel wrapper around the domain <see cref="Role"/>.
    /// Exposes properties for binding and supports runtime updates.
    /// </summary>
    public sealed class RoleViewModel : INotifyPropertyChanged
    {
        private readonly Role _role;

        public RoleViewModel(Role role)
        {
            _role = role ?? throw new ArgumentNullException(nameof(role));
        }

        public Role Model => _role;

        public string Region
        {
            get => _role.Region;
            set
            {
                if (value == _role.Region) return;
                ExecuteOnUI(() =>
                {
                    // Role properties are init but allow replacement semantics
                    typeof(Role).GetProperty(nameof(Role.Region))!.SetValue(_role, value);
                    OnPropertyChanged();
                });
            }
        }

        public string Server
        {
            get => _role.Server;
            set
            {
                if (value == _role.Server) return;
                ExecuteOnUI(() =>
                {
                    typeof(Role).GetProperty(nameof(Role.Server))!.SetValue(_role, value);
                    OnPropertyChanged();
                });
            }
        }

        public string Faction
        {
            get => _role.Faction;
            set
            {
                if (value == _role.Faction) return;
                ExecuteOnUI(() =>
                {
                    typeof(Role).GetProperty(nameof(Role.Faction))!.SetValue(_role, value);
                    OnPropertyChanged();
                });
            }
        }

        public string Nickname
        {
            get => _role.Nickname;
            set
            {
                if (value == _role.Nickname) return;
                ExecuteOnUI(() =>
                {
                    typeof(Role).GetProperty(nameof(Role.Nickname))!.SetValue(_role, value);
                    OnPropertyChanged();
                });
            }
        }

        public int Level
        {
            get => _role.Level;
            set
            {
                if (value == _role.Level) return;
                ExecuteOnUI(() =>
                {
                    typeof(Role).GetProperty(nameof(Role.Level))!.SetValue(_role, Math.Max(0, value));
                    OnPropertyChanged();
                });
            }
        }

        // Resources
        public long Gold { get => _role.Gold; set => SetResource(nameof(Role.Gold), value); }
        public long Silver { get => _role.Silver; set => SetResource(nameof(Role.Silver), value); }
        public long Wood { get => _role.Wood; set => SetResource(nameof(Role.Wood), value); }
        public long Grain { get => _role.Grain; set => SetResource(nameof(Role.Grain), value); }
        public long RefinedIron { get => _role.RefinedIron; set => SetResource(nameof(Role.RefinedIron), value); }
        public long Meteorite { get => _role.Meteorite; set => SetResource(nameof(Role.Meteorite), value); }
        public long Silk { get => _role.Silk; set => SetResource(nameof(Role.Silk), value); }
        public long Pouches { get => _role.Pouches; set => SetResource(nameof(Role.Pouches), value); }
        public long Charcoal { get => _role.Charcoal; set => SetResource(nameof(Role.Charcoal), value); }
        public long Essence { get => _role.Essence; set => SetResource(nameof(Role.Essence), value); }
        public long PreciousOre { get => _role.PreciousOre; set => SetResource(nameof(Role.PreciousOre), value); }
        public long Phantoms { get => _role.Phantoms; set => SetResource(nameof(Role.Phantoms), value); }

        /// <summary>
        /// Adjust a named resource by delta (keeps non-negative).
        /// </summary>
        public void AdjustResource(string resourceName, long delta)
        {
            ExecuteOnUI(() =>
            {
                _role.AdjustResource(resourceName, delta);
                // Raise property changed for the matching property (best-effort mapping).
                switch (resourceName.Trim().ToLowerInvariant())
                {
                    case "金币":
                    case "gold":
                        OnPropertyChanged(nameof(Gold));
                        break;
                    case "银币":
                    case "silver":
                        OnPropertyChanged(nameof(Silver));
                        break;
                    case "木材":
                    case "wood":
                        OnPropertyChanged(nameof(Wood));
                        break;
                    case "粮食":
                    case "grain":
                        OnPropertyChanged(nameof(Grain));
                        break;
                    case "镔铁":
                    case "refinediron":
                        OnPropertyChanged(nameof(RefinedIron));
                        break;
                    case "陨铁":
                    case "meteorite":
                        OnPropertyChanged(nameof(Meteorite));
                        break;
                    case "丝绸":
                    case "silk":
                        OnPropertyChanged(nameof(Silk));
                        break;
                    case "锦囊":
                    case "pouches":
                        OnPropertyChanged(nameof(Pouches));
                        break;
                    case "木炭":
                    case "charcoal":
                        OnPropertyChanged(nameof(Charcoal));
                        break;
                    case "精粹":
                    case "essence":
                        OnPropertyChanged(nameof(Essence));
                        break;
                    case "精金矿":
                    case "preciousore":
                        OnPropertyChanged(nameof(PreciousOre));
                        break;
                    case "幻影":
                    case "phantoms":
                        OnPropertyChanged(nameof(Phantoms));
                        break;
                    default:
                        // no-op
                        break;
                }
            });
        }

        /// <summary>
        /// Replace state from another Role instance.
        /// </summary>
        public void UpdateFrom(Role other)
        {
            if (other is null) return;
            ExecuteOnUI(() =>
            {
                typeof(Role).GetProperty(nameof(Role.Region))!.SetValue(_role, other.Region);
                typeof(Role).GetProperty(nameof(Role.Server))!.SetValue(_role, other.Server);
                typeof(Role).GetProperty(nameof(Role.Faction))!.SetValue(_role, other.Faction);
                typeof(Role).GetProperty(nameof(Role.Nickname))!.SetValue(_role, other.Nickname);
                typeof(Role).GetProperty(nameof(Role.Level))!.SetValue(_role, other.Level);

                typeof(Role).GetProperty(nameof(Role.Gold))!.SetValue(_role, other.Gold);
                typeof(Role).GetProperty(nameof(Role.Silver))!.SetValue(_role, other.Silver);
                typeof(Role).GetProperty(nameof(Role.Wood))!.SetValue(_role, other.Wood);
                typeof(Role).GetProperty(nameof(Role.Grain))!.SetValue(_role, other.Grain);
                typeof(Role).GetProperty(nameof(Role.RefinedIron))!.SetValue(_role, other.RefinedIron);
                typeof(Role).GetProperty(nameof(Role.Meteorite))!.SetValue(_role, other.Meteorite);
                typeof(Role).GetProperty(nameof(Role.Silk))!.SetValue(_role, other.Silk);
                typeof(Role).GetProperty(nameof(Role.Pouches))!.SetValue(_role, other.Pouches);
                typeof(Role).GetProperty(nameof(Role.Charcoal))!.SetValue(_role, other.Charcoal);
                typeof(Role).GetProperty(nameof(Role.Essence))!.SetValue(_role, other.Essence);
                typeof(Role).GetProperty(nameof(Role.PreciousOre))!.SetValue(_role, other.PreciousOre);
                typeof(Role).GetProperty(nameof(Role.Phantoms))!.SetValue(_role, other.Phantoms);

                // notify all properties
                OnPropertyChanged(string.Empty);
            });
        }

        private void SetResource(string propName, long value)
        {
            ExecuteOnUI(() =>
            {
                typeof(Role).GetProperty(propName)!.SetValue(_role, Math.Max(0, value));
                OnPropertyChanged(propName);
            });
        }

        private void ExecuteOnUI(Action action)
        {
            if (action == null) return;
            var dsp = Application.Current?.Dispatcher;
            if (dsp is not null && !dsp.CheckAccess())
            {
                dsp.Invoke(action);
            }
            else
            {
                action();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}