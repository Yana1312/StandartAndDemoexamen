using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Point
{
    public int PointId { get; set; }

    public string? PointIndex { get; set; }

    public string? PointCity { get; set; }

    public string? PointStreet { get; set; }

    public string? PointHouse { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
