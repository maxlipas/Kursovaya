using Avalonia.Data.Converters;
using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace KursMVVM.Converters
{
    public class IdReceiptToReceipt : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return null!;
            int id = (int)value;
            using (var db = new KursContext())
            {
                return db.Receipts.FirstOrDefault(r => r.Id == id);
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
