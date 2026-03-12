using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Manufactur
{
    public int ManufacturId { get; set; }

    public string? ManufacturTitle { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
