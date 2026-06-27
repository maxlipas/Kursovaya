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
    public class WorkshopsPageService : IDBService<Workshop>
    {
        public async Task<List<Workshop>> get()
        {
            using (KursContext db = new KursContext())
            {
                return await db.Workshops.ToListAsync()!;
            }
        }
        public async Task Add(Workshop workshop)
        {
            using (KursContext db = new KursContext())
            {
                await db.Workshops.AddAsync(workshop);
                await db.SaveChangesAsync();
            }
        }
        public async Task Edit(Workshop workshop)
        {
            using (KursContext db = new KursContext())
            {
                Workshop editWorkshop = db.Workshops.FirstOrDefault(p => p.Id == workshop.Id)!;
                editWorkshop.Name = workshop.Name;
                editWorkshop.Manager = workshop.Manager;
                db.Workshops.Update(editWorkshop);
                await db.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                using (KursContext db = new KursContext())
                {
                    if (db.Workers.Any(w => w.WorkshopId == id))
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Невозможно удалить цех: в нём есть работники", ButtonEnum.Ok);
                        await box.ShowAsync();
                        return;
                    }
                    Workshop deleteWorkshop = db.Workshops.FirstOrDefault(p => p.Id == id)!;
                    db.Workshops.Remove(deleteWorkshop);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message, ButtonEnum.Ok);
                await box.ShowAsync();
            }
        }
        public async Task<Workshop?> getById(int id)
        {
            using (KursContext db = new KursContext())
            {
                return await db.Workshops.FirstOrDefaultAsync(p => p.Id == id);
            }
        }
    }
}
