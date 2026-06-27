using CommunityToolkit.Mvvm.ComponentModel;
using KursMVVM.Models;
using KursMVVM.Services;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System;
using System.Linq;

namespace KursMVVM.ViewModels
{
    public partial class GraphPageViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ISeries[] workshopSeries;
        [ObservableProperty]
        private ISeries[] clothingCostSeries;
        [ObservableProperty]
        private ISeries[] monthlySeries;
        [ObservableProperty]
        private ISeries[] wearPeriodSeries;
        [ObservableProperty]
        private Axis[] xAxesClothing;
        [ObservableProperty]
        private Axis[] xAxesMonthly;
        [ObservableProperty]
        private Axis[] xAxesWear;

        public GraphPageViewModel()
        {
            workshopSeries = Array.Empty<ISeries>();
            clothingCostSeries = Array.Empty<ISeries>();
            monthlySeries = Array.Empty<ISeries>();
            wearPeriodSeries = Array.Empty<ISeries>();
            xAxesClothing = Array.Empty<Axis>();
            xAxesMonthly = Array.Empty<Axis>();
            xAxesWear = Array.Empty<Axis>();
            LoadCharts();
        }

        private void LoadCharts()
        {
            using (KursContext db = new KursContext())
            {
                var receipts = db.Receipts.Include(r => r.Worker).ThenInclude(w => w.Workshop).Include(r => r.Clothing).ToList();

                // 1. Распределение получений по цехам (Pie)
                var workshopData = receipts.GroupBy(r => r.Worker.Workshop.Name)
                    .Select(g => new PieSeries<double> { Name = g.Key, Values = new double[] { g.Count() }, DataLabelsPaint = new SolidColorPaint(SKColors.Black), DataLabelsSize = 14 })
                    .ToArray<ISeries>();
                WorkshopSeries = workshopData;

                // 2. Стоимость спецодежды по видам (Column)
                var clothingData = db.SpecialClothings.Select(c => new ColumnSeries<double>
                {
                    Name = c.Type,
                    Values = new double[] { c.UnitCost },
                    Fill = new SolidColorPaint(new SKColor(0, 128, 255))
                }).ToArray<ISeries>();
                ClothingCostSeries = clothingData;
                XAxesClothing = new[] { new Axis { Labels = new[] { "Стоимость" } } };

                // 3. Количество получений по месяцам (Line)
                var monthly = receipts.GroupBy(r => r.DateReceived.ToString("yyyy-MM"))
                    .OrderBy(g => g.Key)
                    .Select(g => new ObservableValue(g.Count()))
                    .ToArray();
                MonthlySeries = new ISeries[]
                {
                    new LineSeries<ObservableValue>
                    {
                        Name = "Получения",
                        Values = monthly,
                        Fill = new SolidColorPaint(new SKColor(0, 200, 100, 50)),
                        Stroke = new SolidColorPaint(new SKColor(0, 200, 100))
                    }
                };
                var monthlyLabels = receipts.GroupBy(r => r.DateReceived.ToString("yyyy-MM")).OrderBy(g => g.Key).Select(g => g.Key).ToArray();
                XAxesMonthly = new[] { new Axis { Labels = monthlyLabels } };

                // 4. Сравнение сроков носки и фактического использования (Bar)
                var today = DateTime.Now;
                var wearData = receipts.Select(r =>
                {
                    var monthsUsed = (today.Year - r.DateReceived.Year) * 12 + today.Month - r.DateReceived.Month;
                    return new { r.Clothing.Type, WearPeriod = r.Clothing.WearPeriod, MonthsUsed = monthsUsed };
                }).GroupBy(x => x.Type)
                .Select(g => new { Type = g.Key, AvgUsed = g.Average(x => x.MonthsUsed), Period = g.First().WearPeriod })
                .ToList();

                var usedValues = wearData.Select(x => new ObservableValue(x.AvgUsed)).ToArray();
                var periodValues = wearData.Select(x => new ObservableValue(x.Period)).ToArray();
                WearPeriodSeries = new ISeries[]
                {
                    new ColumnSeries<ObservableValue>
                    {
                        Name = "Факт (мес.)",
                        Values = usedValues,
                        Fill = new SolidColorPaint(new SKColor(255, 128, 0))
                    },
                    new ColumnSeries<ObservableValue>
                    {
                        Name = "Срок носки",
                        Values = periodValues,
                        Fill = new SolidColorPaint(new SKColor(128, 0, 255))
                    }
                };
                XAxesWear = new[] { new Axis { Labels = wearData.Select(x => x.Type).ToArray() } };
            }
        }
    }
}
