using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? OrderItem { get; set; }

    public DateOnly? OrderDate { get; set; }

    public DateOnly? OrderDateDelivery { get; set; }

    public int? OrderPoint { get; set; }

    public int? OrderUser { get; set; }

    public int? OrderCode { get; set; }

    public string? OrderStatus { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual Point? OrderPointNavigation { get; set; }

    public virtual User? OrderUserNavigation { get; set; }
}
