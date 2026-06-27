using System;
using System.Collections.Generic;

namespace KursMVVM.Models;

public class Workshop
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Manager { get; set; } = null!;

    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
