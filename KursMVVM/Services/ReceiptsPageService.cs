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
    public class ReceiptsPageService : IDBService<Receipt>
    {
        public async Task<List<Receipt>> get()
        {
            using (KursContext db = new KursContext())
            {
                return await db.Receipts.Include(r => r.Worker).Include(r => r.Clothing).ToListAsync()!;
            }
        }
        public async Task Add(Receipt receipt)
        {
            using (KursContext db = new KursContext())
            {
                await db.Receipts.AddAsync(receipt);
                await db.SaveChangesAsync();
            }
        }
        public async Task Edit(Receipt receipt)
        {
            using (KursContext db = new KursContext())
            {
                Receipt editReceipt = db.Receipts.FirstOrDefault(p => p.Id == receipt.Id)!;
                editReceipt.WorkerId = receipt.WorkerId;
                editReceipt.ClothingId = receipt.ClothingId;
                editReceipt.DateReceived = receipt.DateReceived;
                editReceipt.Signature = receipt.Signature;
                db.Receipts.Update(editReceipt);
                await db.SaveChangesAsync();
            }
        }
        public async Task Delete(int id)
        {
            try
            {
                using (KursContext db = new KursContext())
                {
                    Receipt deleteReceipt = db.Receipts.FirstOrDefault(p => p.Id == id)!;
                    db.Receipts.Remove(deleteReceipt);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager
                .GetMessageBoxStandard("Ошибка", ex.Message, ButtonEnum.Ok);
                await box.ShowAsync();
            }
        }
        public async Task<Receipt?> getById(int id)
        {
            using (KursContext db = new KursContext())
            {
                return await db.Receipts.Include(r => r.Worker).Include(r => r.Clothing).FirstOrDefaultAsync(p => p.Id == id);
            }
        }
    }
}
