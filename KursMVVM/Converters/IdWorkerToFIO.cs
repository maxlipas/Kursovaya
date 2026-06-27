using Avalonia.Data.Converters;
using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace KursMVVM.Converters
{
    public class IdWorkerToFIO : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return "";
            int id = (int)value;
            using (var db = new KursContext())
            {
                var worker = db.Workers.FirstOrDefault(w => w.Id == id);
                return worker?.FIO ?? "";
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
