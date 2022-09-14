using Kuic.Wpf.Helpers;
using System.Windows;
using System.Windows.Controls;
using GridLengthConverter = Kuic.Wpf.Helpers.GridLengthConverter;

namespace Kuic.Wpf.Attached
{
    public sealed class Grid : DependencyObject
    {
        public static string GetColumnDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnDefinitionsProperty);
        }

        public static void SetColumnDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnDefinitionsProperty, value);
        }

        public static string GetRowDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(RowDefinitionsProperty);
        }

        public static void SetRowDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(RowDefinitionsProperty, value);
        }

        public static readonly DependencyProperty ColumnDefinitionsProperty = DependencyPropertyEx.RegisterAttached(typeof(string), typeof(Grid), new PropertyMetadata(string.Empty, OnColumnDefinitionsChanged));
        public static readonly DependencyProperty RowDefinitionsProperty = DependencyPropertyEx.RegisterAttached(typeof(string), typeof(Grid), new PropertyMetadata(string.Empty, OnRowDefinitionsChanged));

        private static void OnColumnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not System.Windows.Controls.Grid grid) return;
            grid.ColumnDefinitions.Clear();
            foreach (var cd in GridLengthConverter.ConvertFromString<ColumnDefinition>((string)e.NewValue))
                grid.ColumnDefinitions.Add(cd);
        }

        private static void OnRowDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not System.Windows.Controls.Grid grid) return;
            grid.RowDefinitions.Clear();
            foreach (var rd in GridLengthConverter.ConvertFromString<RowDefinition>((string)e.NewValue))
                grid.RowDefinitions.Add(rd);
        }
    }
}
