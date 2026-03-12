using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Tovar
{
    public int TovarId { get; set; }

    public string? TovarTitle { get; set; }

    public decimal? TovarCost { get; set; }

    public int? TovarSupplier { get; set; }

    public int? TovarManufactur { get; set; }

    public int? TovarCategory { get; set; }

    public string? TovarDescription { get; set; }

    public string? TovarPhoto { get; set; }

    public int? TovarCount { get; set; }

    public int? TovarSale { get; set; }

    public virtual ICollection<Basket> Baskets { get; set; } = new List<Basket>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual Category? TovarCategoryNavigation { get; set; }

    public virtual Manufactur? TovarManufacturNavigation { get; set; }

    public virtual Supplier? TovarSupplierNavigation { get; set; }
}
