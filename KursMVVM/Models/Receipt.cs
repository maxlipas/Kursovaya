using System;
using System.Collections.Generic;

namespace KursMVVM.Models;

public class Receipt
{
    public int Id { get; set; }

    public int WorkerId { get; set; }

    public int ClothingId { get; set; }

    public DateTime DateReceived { get; set; }

    public string Signature { get; set; } = null!;

    public virtual Worker Worker { get; set; } = null!;

    public virtual SpecialClothing Clothing { get; set; } = null!;
}
