using System;
using System.Collections.Generic;

namespace KursMVVM.Models;

public class Worker
{
    public int Id { get; set; }

    public string FIO { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int Discount { get; set; }

    public int WorkshopId { get; set; }

    public virtual Workshop Workshop { get; set; } = null!;

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
