using System.Windows;
using System.Windows.Controls;

namespace Kuic.Wpf.Helpers
{
    public static class GridLengthConverter
    {
        public static IEnumerable<T> ConvertFromString<T>(string value) where T : DefinitionBase, new()
        {
            ArgumentNullException.ThrowIfNull(value);
            var values = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var elementValue in values.Select(v => v.Trim()))
            {
                var definition = new T();
                GridLength length;
                if (elementValue.Equals("Auto", StringComparison.OrdinalIgnoreCase))
                    length = new GridLength();
                else if (elementValue.EndsWith('*'))
                {
                    if (elementValue.Length == 1)
                        length = new GridLength(1D, GridUnitType.Star);
                    else if (double.TryParse(elementValue[..^1], out double size))
                        length = new GridLength(size, GridUnitType.Star);
                    else
                        throw new FormatException($"The provided grid length '{elementValue}' is invalid.");
                }
                else if (double.TryParse(elementValue, out double size))
                    length = new GridLength(size, GridUnitType.Pixel);
                else if (elementValue.Length > 2 && elementValue[0] == '[' && elementValue[^1] == ']')
                {
                    definition.SharedSizeGroup = elementValue[1..^1];
                    length = new GridLength();
                }
                else throw new FormatException($"The provided grid length '{elementValue}' is invalid.");

                if (definition is ColumnDefinition c) c.Width = length;
                else if (definition is RowDefinition r) r.Height = length;
                else throw new NotSupportedException($"Only '{nameof(ColumnDefinition)}' and '{nameof(RowDefinition)}' are supported.");

                yield return definition;
            }
        }
    }
}
