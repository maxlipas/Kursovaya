using System;
using System.Collections.Generic;

namespace KursMVVM.Models;

public class SpecialClothing
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public int WearPeriod { get; set; }

    public double UnitCost { get; set; }

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
