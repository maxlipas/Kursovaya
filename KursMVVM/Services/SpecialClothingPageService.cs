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
    public class SpecialClothingPageService : IDBService<SpecialClothing>
    {
        public async Task<List<SpecialClothing>> get()
        {
            using (KursContext db = new KursContext())
            {
                return await db.SpecialClothings.ToListAsync()!;
            }
        }
        public async Task Add(SpecialClothing clothing)
        {
            using (KursContext db = new KursContext())
            {
                await db.SpecialClothings.AddAsync(clothing);
                await db.SaveChangesAsync();
            }
        }
        public async Task Edit(SpecialClothing clothing)
        {
            using (KursContext db = new KursContext())
            {
                SpecialClothing editClothing = db.SpecialClothings.FirstOrDefault(p => p.Id == clothing.Id)!;
                editClothing.Type = clothing.Type;
                editClothing.WearPeriod = clothing.WearPeriod;
                editClothing.UnitCost = clothing.UnitCost;
                db.SpecialClothings.Update(editClothing);
                await db.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                using (KursContext db = new KursContext())
                {
                    if (db.Receipts.Any(r => r.ClothingId == id))
                    {
                        var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Невозможно удалить спецодежду: она используется в получениях", ButtonEnum.Ok);
                        await box.ShowAsync();
                        return;
                    }
                    SpecialClothing deleteClothing = db.SpecialClothings.FirstOrDefault(p => p.Id == id)!;
                    db.SpecialClothings.Remove(deleteClothing);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("Ошибка", ex.Message, ButtonEnum.Ok);
                await box.ShowAsync();
            }
        }
        public async Task<SpecialClothing?> getById(int id)
        {
            using (KursContext db = new KursContext())
            {
                return await db.SpecialClothings.FirstOrDefaultAsync(p => p.Id == id);
            }
        }
    }
}
