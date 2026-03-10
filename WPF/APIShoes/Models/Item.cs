using System;
using System.Collections.Generic;

namespace APIShoes.Models;

public partial class Item
{
    public int OrderId { get; set; }

    public string ItemArticul { get; set; } = null!;

    public int? ItemCount { get; set; }

    public virtual Tovar ItemArticulNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
