using Avalonia.Data.Converters;
using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;

namespace KursMVVM.Converters
{
    public class IdReceiptToClothing : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null) return "";
            int id = (int)value;
            using (var db = new KursContext())
            {
                var receipt = db.Receipts.FirstOrDefault(r => r.Id == id);
                if (receipt == null) return "";
                var clothing = db.SpecialClothings.FirstOrDefault(c => c.Id == receipt.ClothingId);
                return clothing?.Type ?? "";
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
