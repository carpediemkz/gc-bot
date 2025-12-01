using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                panel.PART_DataGrid.ItemsSource = e.NewValue as IEnumerable;
            }
        }

        public void ScrollToEnd()
        {
            if (PART_DataGrid.Items.Count > 0)
            {
                var last = PART_DataGrid.Items[PART_DataGrid.Items.Count - 1];
                PART_DataGrid.ScrollIntoView(last);
                PART_DataGrid.SelectedItem = last;
            }
        }

        // Per-row remove button handler. Removes the row's data item from the underlying collection if possible.
        private void RemoveRow_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            var item = btn.DataContext;
            if (item is null) return;

            // Try to obtain an IList reference to remove the item.
            IList? list = ItemsSource as IList;
            if (list == null)
            {
                var view = CollectionViewSource.GetDefaultView(ItemsSource);
                if (view?.SourceCollection is IList srcList)
                {
                    list = srcList;
                }
            }

            if (list is not null)
            {
                // Ensure removal runs on UI thread
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => { if (list.Contains(item)) list.Remove(item); });
                }
                else
                {
                    if (list.Contains(item)) list.Remove(item);
                }
            }
            else
            {
                // Fallback: try to remove via DataGrid.Items (this won't update source collection)
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(() => PART_DataGrid.Items.Remove(item));
                }
                else
                {
                    PART_DataGrid.Items.Remove(item);
                }
            }
        }
    }
}