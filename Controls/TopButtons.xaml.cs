using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace gc_bot.Controls
{
    public partial class TopButtons : UserControl
    {
        public TopButtons()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raised when any top button is clicked. The int argument is the button number (1..8).
        /// Kept for backward compatibility.
        /// </summary>
        public event EventHandler<int>? ButtonClicked;

        public static readonly DependencyProperty ButtonsCommandProperty =
            DependencyProperty.Register(
                nameof(ButtonsCommand),
                typeof(ICommand),
                typeof(TopButtons),
                new PropertyMetadata(null));

        public ICommand? ButtonsCommand
        {
            get => (ICommand?)GetValue(ButtonsCommandProperty);
            set => SetValue(ButtonsCommandProperty, value);
        }

        // Use exact RoutedEventHandler signature so XAML can wire the Click correctly.
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var index = -1;
            if (sender is Button btn && int.TryParse(btn.Tag?.ToString(), out var idx))
            {
                index = idx;
            }

            MessageBox.Show($"{index}New command invoked.");


            ButtonClicked?.Invoke(this, index);

            if (ButtonsCommand is ICommand cmd && cmd.CanExecute(index))
            {
                cmd.Execute(index);
            }
        }
    }
}