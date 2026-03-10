using System;
using System.Collections.Generic;

namespace AppShoes.Models;

public partial class Manufactur
{
    public int ManufacturId { get; set; }

    public string? ManufacturName { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
