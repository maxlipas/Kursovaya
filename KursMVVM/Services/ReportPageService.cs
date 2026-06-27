using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KursMVVM.Services
{
    public class ReportPageService
    {
        // Отчет 1. Сводка по цехам
        public async Task<List<WorkshopSummaryReport>> GetWorkshopSummary()
        {
            using (KursContext db = new KursContext())
            {
                var query = from w in db.Workshops
                            join worker in db.Workers on w.Id equals worker.WorkshopId into workersGroup
                            from wg in workersGroup.DefaultIfEmpty()
                            join r in db.Receipts on wg.Id equals r.WorkerId into receiptsGroup
                            from rg in receiptsGroup.DefaultIfEmpty()
                            join c in db.SpecialClothings on rg.ClothingId equals c.Id into clothingsGroup
                            from cg in clothingsGroup.DefaultIfEmpty()
                            select new { Workshop = w, Worker = wg, Receipt = rg, Clothing = cg };

                var result = query.ToList()
                    .GroupBy(x => x.Workshop)
                    .Select(g => new WorkshopSummaryReport
                    {
                        WorkshopName = g.Key.Name,
                        WorkerCount = g.Where(x => x.Worker != null).Select(x => x.Worker.Id).Distinct().Count(),
                        ReceiptCount = g.Where(x => x.Receipt != null).Select(x => x.Receipt.Id).Distinct().Count(),
                        TotalCost = g.Where(x => x.Receipt != null && x.Clothing != null && x.Worker != null)
                            .Sum(x => x.Clothing.UnitCost * (1 - x.Worker.Discount / 100.0))
                    }).ToList();
                return result;
            }
        }

        // Отчет 2. Обеспеченность работников спецодеждой
        public async Task<List<WorkerProvisionReport>> GetWorkerProvision()
        {
            using (KursContext db = new KursContext())
            {
                var workers = (await db.Workers.Include(w => w.Workshop).ToListAsync())!;
                var receipts = (await db.Receipts.ToListAsync())!;
                var result = new List<WorkerProvisionReport>();
                foreach (var worker in workers)
                {
                    var workerReceipts = receipts.Where(r => r.WorkerId == worker.Id).ToList();
                    result.Add(new WorkerProvisionReport
                    {
                        FIO = worker.FIO,
                        Position = worker.Position,
                        WorkshopName = worker.Workshop?.Name ?? "",
                        ClothingTypesCount = workerReceipts.Select(r => r.ClothingId).Distinct().Count(),
                        LastReceiptDate = workerReceipts.Count > 0 ? workerReceipts.Max(r => r.DateReceived).ToString("dd.MM.yyyy") : "-"
                    });
                }
                return result;
            }
        }

        // Отчет 3. Спецодежда с истекающим сроком носки
        public async Task<List<ExpiringClothingReport>> GetExpiringClothing()
        {
            using (KursContext db = new KursContext())
            {
                var today = DateTime.Now;
                var receipts = (await db.Receipts.Include(r => r.Worker).Include(r => r.Clothing).ToListAsync())!;
                var result = new List<ExpiringClothingReport>();
                foreach (var r in receipts)
                {
                    var monthsUsed = (today.Year - r.DateReceived.Year) * 12 + today.Month - r.DateReceived.Month;
                    var monthsLeft = r.Clothing.WearPeriod - monthsUsed;
                    if (monthsLeft <= 3 && monthsLeft >= 0)
                    {
                        result.Add(new ExpiringClothingReport
                        {
                            ClothingType = r.Clothing.Type,
                            DateReceived = r.DateReceived.ToString("dd.MM.yyyy"),
                            WorkerFIO = r.Worker.FIO,
                            MonthsLeft = monthsLeft
                        });
                    }
                }
                return result.OrderBy(r => r.MonthsLeft).ToList();
            }
        }

        // Отчет 4. Невостребованная спецодежда
        public async Task<List<UnusedClothingReport>> GetUnusedClothing()
        {
            using (KursContext db = new KursContext())
            {
                var usedIds = (await db.Receipts.Select(r => r.ClothingId).Distinct().ToListAsync())!;
                var unused = (await db.SpecialClothings.Where(c => !usedIds.Contains(c.Id)).ToListAsync())!;
                return unused.Select(c => new UnusedClothingReport
                {
                    ClothingType = c.Type,
                    WearPeriod = c.WearPeriod,
                    UnitCost = c.UnitCost
                }).ToList();
            }
        }

        // Отчет 5. Стоимость обеспечения по цехам
        public async Task<List<WorkshopCostReport>> GetWorkshopCost()
        {
            using (KursContext db = new KursContext())
            {
                var query = from w in db.Workshops
                            join worker in db.Workers on w.Id equals worker.WorkshopId
                            join r in db.Receipts on worker.Id equals r.WorkerId
                            join c in db.SpecialClothings on r.ClothingId equals c.Id
                            select new { w.Name, Cost = c.UnitCost * (1 - worker.Discount / 100.0) };

                var result = query
                    .GroupBy(x => x.Name)
                    .Select(g => new WorkshopCostReport
                    {
                        WorkshopName = g.Key,
                        TotalCost = g.Sum(x => x.Cost)
                    }).ToList();
                return result;
            }
        }
    }

    public class WorkshopSummaryReport
    {
        public string WorkshopName { get; set; } = null!;
        public int WorkerCount { get; set; }
        public int ReceiptCount { get; set; }
        public double TotalCost { get; set; }
    }

    public class WorkerProvisionReport
    {
        public string FIO { get; set; } = null!;
        public string Position { get; set; } = null!;
        public string WorkshopName { get; set; } = null!;
        public int ClothingTypesCount { get; set; }
        public string LastReceiptDate { get; set; } = null!;
    }

    public class ExpiringClothingReport
    {
        public string ClothingType { get; set; } = null!;
        public string DateReceived { get; set; } = null!;
        public string WorkerFIO { get; set; } = null!;
        public int MonthsLeft { get; set; }
    }

    public class UnusedClothingReport
    {
        public string ClothingType { get; set; } = null!;
        public int WearPeriod { get; set; }
        public double UnitCost { get; set; }
    }

    public class WorkshopCostReport
    {
        public string WorkshopName { get; set; } = null!;
        public double TotalCost { get; set; }
    }
}
