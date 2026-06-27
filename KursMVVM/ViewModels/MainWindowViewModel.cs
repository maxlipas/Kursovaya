using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KursMVVM.Services;
using KursMVVM.Views;

namespace KursMVVM.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly NavigationService _navigationService;
    [ObservableProperty]
    private object? currentPage;
    [ObservableProperty]
    private string title;
    public MainWindowViewModel()
    {
        _navigationService = new NavigationService(this);
        _navigationService.RegisterPage("Home", new HomePageViewModel());
        _navigationService.RegisterPage("Workshops", new WorkshopsPageViewModel());
        _navigationService.RegisterPage("SpecialClothing", new SpecialClothingPageViewModel());
        _navigationService.RegisterPage("Workers", new WorkersPageViewModel());
        _navigationService.RegisterPage("Receipts", new ReceiptsPageViewModel());
        _navigationService.RegisterPage("Reports", new ReportPageViewModel());
        _navigationService.RegisterPage("Graphics", new GraphPageViewModel());
        _navigationService.RegisterPage("ImportExport", new ImportExportPageViewModel());
        _navigationService.NavigateTo("Home");
        title = "Учёт спецодежды";
    }
    [RelayCommand]
    public void Navigate(string pageKey)
    {
        _navigationService.NavigateTo(pageKey);
    }
}
