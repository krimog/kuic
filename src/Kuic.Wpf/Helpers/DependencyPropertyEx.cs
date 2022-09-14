using System.Runtime.CompilerServices;
using System.Windows;

namespace Kuic.Wpf.Helpers
{
    public static class DependencyPropertyEx
    {
        private const string PropertyNameSuffix = "Property";

        public static DependencyProperty RegisterAttached(Type propertyType, Type ownerType, [CallerMemberName] string? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(propertyName);
            return DependencyProperty.RegisterAttached(propertyName[..^PropertyNameSuffix.Length], propertyType, ownerType);
        }

        public static DependencyProperty RegisterAttached(Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, [CallerMemberName] string? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(propertyName);
            return DependencyProperty.RegisterAttached(propertyName[..^PropertyNameSuffix.Length], propertyType, ownerType, defaultMetadata);
        }

        public static DependencyProperty RegisterAttached(Type propertyType, Type ownerType, PropertyMetadata defaultMetadata, ValidateValueCallback validateValueCallback, [CallerMemberName] string? propertyName = null)
        {
            ArgumentNullException.ThrowIfNull(propertyName);
            return DependencyProperty.RegisterAttached(propertyName[..^PropertyNameSuffix.Length], propertyType, ownerType, defaultMetadata, validateValueCallback);
        }
    }
}
