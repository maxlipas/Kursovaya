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
    public partial class WorkersPageViewModel : ViewModelBase
    {
        private WindowNotificationManager _notificationManager;
        private WorkersPageService pageService;
        [ObservableProperty]
        private ObservableCollection<Worker> workers = new();
        [ObservableProperty]
        private Worker? selectedWorker = null;
        public WorkersPageViewModel()
        {
            pageService = new WorkersPageService();
            _notificationManager = new WindowNotificationManager(MainWindow.Instance!)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 3
            };
            Load();
        }
        private void Load()
        {
            Workers.Clear();
            Workers = new ObservableCollection<Worker>(getWorkers());
        }
        private List<Worker> getWorkers()
        {
            Task<List<Worker>> task = Task.Run(() => pageService.get());
            return task.Result;
        }
        [RelayCommand]
        private async Task Add()
        {
            try
            {
                var dialog = new WorkerWindow(new Worker());
                Worker result = await dialog.ShowDialog<Worker>(MainWindow.Instance!);
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
                Worker worker = (Worker)param;
                WorkerWindow dialog = new WorkerWindow(worker);
                Worker result = await dialog.ShowDialog<Worker>(MainWindow.Instance!);
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
                    Worker worker = (Worker)param;
                    await pageService.Delete(worker.Id);
                    Load();
                }
            }
        }
    }
}
