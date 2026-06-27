using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class ExpirationColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int monthsLeft)
            {
                return monthsLeft <= 1 ? new SolidColorBrush(Colors.Red) :
                       monthsLeft <= 3 ? new SolidColorBrush(Colors.Orange) :
                       new SolidColorBrush(Colors.Green);
            }
            return new SolidColorBrush(Colors.Black);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
