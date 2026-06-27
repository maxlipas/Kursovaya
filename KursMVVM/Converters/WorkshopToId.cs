using Avalonia.Data.Converters;
using KursMVVM.Models;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class WorkshopToId : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (int)value!;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null) return (value as Workshop)!.Id;
            return null;
        }
    }
}
