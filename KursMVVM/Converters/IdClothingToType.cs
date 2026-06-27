using Avalonia.Data.Converters;
using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace KursMVVM.Converters
{
    public class IdClothingToType : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return "";
            int id = (int)value;
            using (var db = new KursContext())
            {
                var c = db.SpecialClothings.FirstOrDefault(x => x.Id == id);
                return c?.Type ?? "";
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
