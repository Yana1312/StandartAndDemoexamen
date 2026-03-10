using System;
using System.Collections.Generic;

namespace AppShoes.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string? SupplierTitle { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
