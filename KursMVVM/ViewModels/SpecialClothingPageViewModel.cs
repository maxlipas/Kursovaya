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
    public partial class SpecialClothingPageViewModel : ViewModelBase
    {
        private WindowNotificationManager _notificationManager;
        private SpecialClothingPageService pageService;
        [ObservableProperty]
        private ObservableCollection<SpecialClothing> clothings = new();
        [ObservableProperty]
        private SpecialClothing? selectedClothing = null;
        public SpecialClothingPageViewModel()
        {
            pageService = new SpecialClothingPageService();
            _notificationManager = new WindowNotificationManager(MainWindow.Instance!)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 3
            };
            Load();
        }
        private void Load()
        {
            Clothings.Clear();
            Clothings = new ObservableCollection<SpecialClothing>(getClothings());
        }
        private List<SpecialClothing> getClothings()
        {
            Task<List<SpecialClothing>> task = Task.Run(() => pageService.get());
            return task.Result;
        }
        [RelayCommand]
        private async Task Add()
        {
            try
            {
                var dialog = new SpecialClothingWindow(new SpecialClothing());
                SpecialClothing result = await dialog.ShowDialog<SpecialClothing>(MainWindow.Instance!);
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
                SpecialClothing clothing = (SpecialClothing)param;
                SpecialClothingWindow dialog = new SpecialClothingWindow(clothing);
                SpecialClothing result = await dialog.ShowDialog<SpecialClothing>(MainWindow.Instance!);
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
                    SpecialClothing clothing = (SpecialClothing)param;
                    await pageService.Delete(clothing.Id);
                    Load();
                }
            }
        }
    }
}
