using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace gc_bot.ViewModels
{
    public sealed class ItemViewModel : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _description = string.Empty;
        private DateTime _timestamp = DateTime.Now;

        public ItemViewModel() { }

        public ItemViewModel(string title, string description)
        {
            Title = title;
            Description = description;
            Timestamp = DateTime.Now;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}