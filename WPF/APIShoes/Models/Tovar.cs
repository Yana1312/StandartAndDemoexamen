using System;
using System.Collections.Generic;

namespace APIShoes.Models;

public partial class Tovar
{
    public string TovarArticul { get; set; } = null!;

    public string? TovarTitle { get; set; }

    public string? TovarUnit { get; set; }

    public decimal? TovarCost { get; set; }

    public int? TovarSupplier { get; set; }

    public int? TovarManufactor { get; set; }

    public int? TovarCategory { get; set; }

    public int? TovarSale { get; set; }

    public int? TovarCount { get; set; }

    public string? TovarDescription { get; set; }

    public string? TovarPhoto { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual Category? TovarCategoryNavigation { get; set; }

    public virtual Manufactur? TovarManufactorNavigation { get; set; }

    public virtual Supplier? TovarSupplierNavigation { get; set; }
}
