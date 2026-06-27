using Avalonia.Data.Converters;
using KursMVVM;
using KursMVVM.ViewModels;
using System;
using System.Globalization;

namespace KursMVVM.Converters
{
    public class ViewModelToViewConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                HomePageViewModel => new HomePageView { DataContext = value },
                WorkshopsPageViewModel => new WorkshopsPageView { DataContext = value },
                SpecialClothingPageViewModel => new SpecialClothingPageView { DataContext = value },
                WorkersPageViewModel => new WorkersPageView { DataContext = value },
                ReceiptsPageViewModel => new ReceiptsPageView { DataContext = value },
                GraphPageViewModel => new GraphPageView { DataContext = value },
                ReportPageViewModel => new ReportPageView { DataContext = value },
                ImportExportPageViewModel => new ImportExportPageView { DataContext = value },
                _ =>null
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
