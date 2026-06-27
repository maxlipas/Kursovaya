using CommunityToolkit.Mvvm.ComponentModel;
using KursMVVM.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KursMVVM.ViewModels
{
    public partial class ReportPageViewModel : ViewModelBase
    {
        private ReportPageService reportService;
        [ObservableProperty]
        private ObservableCollection<WorkshopSummaryReport> workshopSummary = new();
        [ObservableProperty]
        private ObservableCollection<WorkerProvisionReport> workerProvision = new();
        [ObservableProperty]
        private ObservableCollection<ExpiringClothingReport> expiringClothing = new();
        [ObservableProperty]
        private ObservableCollection<UnusedClothingReport> unusedClothing = new();
        [ObservableProperty]
        private ObservableCollection<WorkshopCostReport> workshopCost = new();

        public ReportPageViewModel()
        {
            reportService = new ReportPageService();
            Load();
        }

        private async void Load()
        {
            WorkshopSummary = new ObservableCollection<WorkshopSummaryReport>(await reportService.GetWorkshopSummary());
            WorkerProvision = new ObservableCollection<WorkerProvisionReport>(await reportService.GetWorkerProvision());
            ExpiringClothing = new ObservableCollection<ExpiringClothingReport>(await reportService.GetExpiringClothing());
            UnusedClothing = new ObservableCollection<UnusedClothingReport>(await reportService.GetUnusedClothing());
            WorkshopCost = new ObservableCollection<WorkshopCostReport>(await reportService.GetWorkshopCost());
        }
    }
}
