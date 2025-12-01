using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace gc_bot.ViewModels
{
    public sealed class MainViewModel
    {
        public ObservableCollection<ItemViewModel> Items { get; } = new();

        public ICommand ButtonCommand { get; }

        public MainViewModel()
        {
            // sample data
            for (int i = 1; i <= 30; i++)
            {
                Items.Add(new ItemViewModel($"Item {i}", "Sample description demonstrating MVVM binding and wrapping."));
            }

            ButtonCommand = new RelayCommand(OnButtonExecuted);
        }

        private void OnButtonExecuted(object? parameter)
        {
            // parameter is expected to be int or string/parsable int (button index)
            var index = ParseIndex(parameter);
            var title = index > 0 ? $"Button {index}" : "Button";
            Items.Add(new ItemViewModel(title, $"Clicked at {DateTime.Now:T}"));
        }

        private static int ParseIndex(object? parameter)
        {
            if (parameter is int i) return i;
            if (parameter is string s && int.TryParse(s, out var j)) return j;
            if (parameter is null) return -1;
            try
            {
                return Convert.ToInt32(parameter);
            }
            catch
            {
                return -1;
            }
        }
    }
}