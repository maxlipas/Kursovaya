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
    public partial class ReceiptsPageViewModel : ViewModelBase
    {
        private WindowNotificationManager _notificationManager;
        private ReceiptsPageService pageService;
        [ObservableProperty]
        private ObservableCollection<Receipt> receipts = new();
        [ObservableProperty]
        private Receipt? selectedReceipt = null;
        public ReceiptsPageViewModel()
        {
            pageService = new ReceiptsPageService();
            _notificationManager = new WindowNotificationManager(MainWindow.Instance!)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 3
            };
            Load();
        }
        private void Load()
        {
            Receipts.Clear();
            Receipts = new ObservableCollection<Receipt>(getReceipts());
        }
        private List<Receipt> getReceipts()
        {
            Task<List<Receipt>> task = Task.Run(() => pageService.get());
            return task.Result;
        }
        [RelayCommand]
        private async Task Add()
        {
            try
            {
                var dialog = new ReceiptWindow(new Receipt());
                Receipt result = await dialog.ShowDialog<Receipt>(MainWindow.Instance!);
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
                Receipt receipt = (Receipt)param;
                ReceiptWindow dialog = new ReceiptWindow(receipt);
                Receipt result = await dialog.ShowDialog<Receipt>(MainWindow.Instance!);
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
                    Receipt receipt = (Receipt)param;
                    await pageService.Delete(receipt.Id);
                    Load();
                }
            }
        }
    }
}
