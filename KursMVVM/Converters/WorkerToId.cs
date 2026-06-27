using Avalonia.Data.Converters;
using KursMVVM.Models;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class WorkerToId : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (int)value!;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null) return (value as Worker)!.Id;
            return null;
        }
    }
}
