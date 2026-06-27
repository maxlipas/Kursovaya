using KursMVVM.ViewModels;
using System;
using System.Collections.Generic;

namespace KursMVVM.Services
{
    public class NavigationService
    {
        private readonly MainWindowViewModel _mainViewModel;
        private readonly Dictionary<string, object> _pages = new();
        public NavigationService(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }
        public void RegisterPage(string key, object pageViewModel)
        {
            _pages[key] = pageViewModel;
        }
        public void NavigateTo(string key)
        {
            object? page = key switch
            {
                "Home" => new HomePageViewModel(),
                "Workshops" => new WorkshopsPageViewModel(),
                "SpecialClothing" => new SpecialClothingPageViewModel(),
                "Workers" => new WorkersPageViewModel(),
                "Receipts" => new ReceiptsPageViewModel(),
                "Reports" => new ReportPageViewModel(),
                "Graphics" => new GraphPageViewModel(),
                "ImportExport" => new ImportExportPageViewModel(),
                _ => _pages.TryGetValue(key, out var p) ? p : null
            };
            if (page != null)
            {
                _mainViewModel.CurrentPage = page;
            }
        }
    }
}
