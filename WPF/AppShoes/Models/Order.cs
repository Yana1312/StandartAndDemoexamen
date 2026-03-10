using System;
using System.Collections.Generic;

namespace AppShoes.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? OrderItem { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? OrderDeliveryDate { get; set; }

    public int? OrderAdress { get; set; }

    public int? OrderUser { get; set; }

    public int? OrderCode { get; set; }

    public string? OrderStatus { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual PickPoint? OrderAdressNavigation { get; set; }

    public virtual User? OrderUserNavigation { get; set; }
}
