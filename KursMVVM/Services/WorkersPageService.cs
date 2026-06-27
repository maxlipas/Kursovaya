using KursMVVM.Models;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KursMVVM.Services
{
    public class WorkersPageService : IDBService<Worker>
    {
        public async Task<List<Worker>> get()
        {
            using (KursContext db = new KursContext())
            {
                return await db.Workers.Include(w => w.Workshop).ToListAsync()!;
            }
        }
        public async Task Add(Worker worker)
        {
            using (KursContext db = new KursContext())
            {
                await db.Workers.AddAsync(worker);
                await db.SaveChangesAsync();
            }
        }
        public async Task Edit(Worker worker)
        {
            using (KursContext db = new KursContext())
            {
                Worker editWorker = db.Workers.FirstOrDefault(p => p.Id == worker.Id)!;
                editWorker.FIO = worker.FIO;
                editWorker.Position = worker.Position;
                editWorker.Discount = worker.Discount;
                editWorker.WorkshopId = worker.WorkshopId;
                db.Workers.Update(editWorker);
                await db.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                using (KursContext db = new KursContext())
                {
                    if (db.Receipts.Any(r => r.WorkerId == id))
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Невозможно удалить работника: у него есть получения спецодежды", ButtonEnum.Ok);
                        await box.ShowAsync();
                        return;
                    }
                    Worker deleteWorker = db.Workers.FirstOrDefault(p => p.Id == id)!;
                    db.Workers.Remove(deleteWorker);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message, ButtonEnum.Ok);
                await box.ShowAsync();
            }
        }
        public async Task<Worker?> getById(int id)
        {
            using (KursContext db = new KursContext())
            {
                return await db.Workers.Include(w => w.Workshop).FirstOrDefaultAsync(p => p.Id == id);
            }
        }
    }
}
