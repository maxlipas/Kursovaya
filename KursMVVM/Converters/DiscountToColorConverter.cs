using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class DiscountToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int discount)
            {
                return discount == 0 ? "Черный" :
                       discount <= 10 ? "Синий" :
                       discount <= 20 ? "Зелёный" : "Красный";
            }
            return "Черный";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
