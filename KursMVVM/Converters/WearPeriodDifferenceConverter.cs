using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class WearPeriodDifferenceConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateReceived && parameter is int wearPeriod)
            {
                var today = DateTime.Now;
                var monthsUsed = (today.Year - dateReceived.Year) * 12 + today.Month - dateReceived.Month;
                return wearPeriod - monthsUsed;
            }
            return 0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
