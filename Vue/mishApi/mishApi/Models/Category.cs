using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryTitle { get; set; }

    public virtual ICollection<Tovar> Tovars { get; set; } = new List<Tovar>();
}
