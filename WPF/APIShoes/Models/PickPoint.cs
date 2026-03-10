using System;
using System.Collections.Generic;

namespace APIShoes.Models;

public partial class PickPoint
{
    public int PickPointId { get; set; }

    public string? PickPointIndex { get; set; }

    public string? PickPointCity { get; set; }

    public string? PickPointStreet { get; set; }

    public int? PickPointHouse { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
