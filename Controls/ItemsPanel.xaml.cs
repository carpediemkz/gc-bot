using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace gc_bot.Controls
{
    public partial class ItemsPanel : UserControl
    {
        public ItemsPanel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                nameof(ItemsSource),
                typeof(IEnumerable),
                typeof(ItemsPanel),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public IEnumerable? ItemsSource
        {
            get => (IEnumerable?)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsPanel panel)
            {
                panel.PART_ListView.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        public void ScrollToEnd()
        {
            if (PART_ListView.Items.Count > 0)
            {
                PART_ListView.ScrollIntoView(PART_ListView.Items[PART_ListView.Items.Count - 1]);
            }
        }
    }
}