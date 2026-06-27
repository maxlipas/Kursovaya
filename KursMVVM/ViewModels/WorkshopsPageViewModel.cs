using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KursMVVM.Models;
using KursMVVM.Services;
using KursMVVM.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KursMVVM.ViewModels
{
    public partial class WorkshopsPageViewModel : ViewModelBase
    {
        private WindowNotificationManager _notificationManager;
        private WorkshopsPageService pageService;
        [ObservableProperty]
        private ObservableCollection<Workshop> workshops = new();
        [ObservableProperty]
        private Workshop? selectedWorkshop = null;
        public WorkshopsPageViewModel()
        {
            pageService = new WorkshopsPageService();
            _notificationManager = new WindowNotificationManager(MainWindow.Instance!)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 3
            };
            Load();
        }
        private void Load()
        {
            Workshops.Clear();
            Workshops = new ObservableCollection<Workshop>(getWorkshops());
        }
        private List<Workshop> getWorkshops()
        {
            Task<List<Workshop>> task = Task.Run(() => pageService.get());
            return task.Result;
        }
        [RelayCommand]
        private async Task Add()
        {
            try
            {
                var dialog = new WorkshopWindow(new Workshop());
                Workshop result = await dialog.ShowDialog<Workshop>(MainWindow.Instance!);
                if (result != null)
                {
                    await pageService.Add(result);
                }
            }
            catch (Exception) { }
            finally { Load(); }
        }
        [RelayCommand]
        private async Task Edit(object param)
        {
            if (param != null)
            {
                Workshop workshop = (Workshop)param;
                WorkshopWindow dialog = new WorkshopWindow(workshop);
                Workshop result = await dialog.ShowDialog<Workshop>(MainWindow.Instance!);
                if (result != null)
                {
                    await pageService.Edit(result);
                    Load();
                }
            }
        }
        [RelayCommand]
        private async Task Delete(object param)
        {
            if (param != null)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Внимание", "Вы действительно хотите удалить объект?", ButtonEnum.OkCancel);
                ButtonResult result = await box.ShowAsync();
                if (result == ButtonResult.Ok)
                {
                    Workshop workshop = (Workshop)param;
                    await pageService.Delete(workshop.Id);
                    Load();
                }
            }
        }
    }
}
