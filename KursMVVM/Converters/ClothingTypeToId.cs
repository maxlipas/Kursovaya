using Avalonia.Data.Converters;
using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace KursMVVM.Converters
{
    public class ClothingTypeToId : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            string type = value.ToString()!;
            using (var db = new KursContext())
            {
                var clothing = db.SpecialClothings.FirstOrDefault(c => c.Type == type);
                return clothing?.Id ?? 0;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
