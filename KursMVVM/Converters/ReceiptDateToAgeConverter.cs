using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class ReceiptDateToAgeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateReceived)
            {
                var today = DateTime.Now;
                var months = (today.Year - dateReceived.Year) * 12 + today.Month - dateReceived.Month;
                return months;
            }
            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
