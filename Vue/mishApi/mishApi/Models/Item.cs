using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Item
{
    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public int? ItemCount { get; set; }

    public virtual Tovar ItemNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
