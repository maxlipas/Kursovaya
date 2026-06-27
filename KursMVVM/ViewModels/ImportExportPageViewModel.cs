using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KursMVVM.Services;
using KursMVVM.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KursMVVM.ViewModels
{
    public partial class ImportExportPageViewModel : ViewModelBase
    {
        private ImportExportService _service;
        private WindowNotificationManager _notificationManager;
        [ObservableProperty]
        private ObservableCollection<string> logs = new();
        [ObservableProperty]
        private string selectedEntity = "Workshops";
        public string[] Entities { get; } = new[] { "Workshops", "SpecialClothing", "Workers", "Receipts" };

        public ImportExportPageViewModel()
        {
            _service = new ImportExportService();
            _notificationManager = new WindowNotificationManager(MainWindow.Instance!)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 3
            };
        }

        [RelayCommand]
        private async Task Import()
        {
            var topLevel = TopLevel.GetTopLevel(MainWindow.Instance!);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(
                    new FilePickerOpenOptions
                    {
                        Title = "Выберите файл для импорта",
                        AllowMultiple = false,
                        FileTypeFilter = new[]
                        {
                            new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
                            new FilePickerFileType("Excel") { Patterns = new[] { "*.xlsx" } },
                            new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } },
                            new FilePickerFileType("XML") { Patterns = new[] { "*.xml" } },
                        }
                    });
                if (files.Count > 0)
                {
                    var path = files[0].Path.AbsolutePath;
                    var ext = System.IO.Path.GetExtension(path).ToLower();
                    try
                    {
                        Logs.Add($"Путь: {path}");
                        Logs.Add($"Сущность: {SelectedEntity}");
                        switch (ext)
                        {
                            case ".csv":
                                if (SelectedEntity == "Workshops") await _service.ImportWorkshopsCSV(path);
                                else if (SelectedEntity == "SpecialClothing") await _service.ImportSpecialClothingCSV(path);
                                else if (SelectedEntity == "Workers") await _service.ImportWorkersCSV(path);
                                else if (SelectedEntity == "Receipts") await _service.ImportReceiptsCSV(path);
                                else Logs.Add("Неизвестная сущность для CSV");
                                break;
                            case ".xlsx":
                                if (SelectedEntity == "Workers") await _service.ImportWorkersExcel(path);
                                else if (SelectedEntity == "Receipts") await _service.ImportReceiptsExcel(path);
                                else Logs.Add("Для Excel доступны только Workers и Receipts");
                                break;
                            case ".json":
                                await _service.ImportJSON(path);
                                break;
                            case ".xml":
                                await _service.ImportXML(path);
                                break;
                            default:
                                Logs.Add($"Неизвестное расширение: {ext}");
                                break;
                        }
                        foreach (var log in _service.Logs)
                            Logs.Add(log);
                        _notificationManager.Show(new Notification("Импорт", "Импорт завершён", NotificationType.Success, TimeSpan.FromSeconds(2)));
                    }
                    catch (Exception ex)
                    {
                        Logs.Add(ex.Message);
                    }
                }
            }
        }

        [RelayCommand]
        private async Task Export()
        {
            var topLevel = TopLevel.GetTopLevel(MainWindow.Instance!);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.SaveFilePickerAsync(
                    new FilePickerSaveOptions
                    {
                        Title = "Выберите файл для экспорта",
                        FileTypeChoices = new[]
                        {
                            new FilePickerFileType("CSV") { Patterns = new[] { "*.csv" } },
                            new FilePickerFileType("Excel") { Patterns = new[] { "*.xlsx" } },
                            new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } },
                        }
                    });
                if (files != null)
                {
                    var path = files.Path.AbsolutePath;
                    var ext = System.IO.Path.GetExtension(path).ToLower();
                    try
                    {
                        switch (ext)
                        {
                            case ".csv":
                                if (SelectedEntity == "Workshops") await _service.ExportWorkshopsCSV(path);
                                else if (SelectedEntity == "SpecialClothing") await _service.ExportSpecialClothingCSV(path);
                                else if (SelectedEntity == "Workers") await _service.ExportWorkersCSV(path);
                                else if (SelectedEntity == "Receipts") await _service.ExportReceiptsCSV(path);
                                break;
                            case ".xlsx":
                                using (var db = new KursMVVM.Models.KursContext())
                                {
                                    var workers = db.Workers.ToList();
                                    var headers = new List<string> { "ФИО", "Должность", "Скидка", "Цех" };
                                    var rows = workers.Select(w => new List<object> { w.FIO, w.Position, w.Discount, w.WorkshopId }).ToList();
                                    await _service.ExportReportExcel(path, "Работники", headers, rows);
                                }
                                break;
                            case ".pdf":
                                using (var db = new KursMVVM.Models.KursContext())
                                {
                                    var workers = db.Workers.ToList();
                                    var headers = new List<string> { "ФИО", "Должность", "Скидка", "Цех" };
                                    var rows = workers.Select(w => new List<string> { w.FIO, w.Position, w.Discount.ToString(), w.WorkshopId.ToString() }).ToList();
                                    await _service.ExportReportPDF(path, "Отчёт", headers, rows);
                                }
                                break;
                            default:
                                Logs.Add($"Неизвестное расширение: {ext}");
                                break;
                        }
                        _notificationManager.Show(new Notification("Экспорт", "Файл сохранён", NotificationType.Success, TimeSpan.FromSeconds(2)));
                    }
                    catch (Exception ex)
                    {
                        Logs.Add(ex.Message);
                    }
                }
            }
        }
    }
}
