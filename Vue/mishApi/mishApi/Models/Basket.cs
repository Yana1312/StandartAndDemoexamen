using System;
using System.Collections.Generic;

namespace mishApi.Models;

public partial class Basket
{
    public int UserId { get; set; }

    public int TovarId { get; set; }

    public int? BasketCount { get; set; }

    public virtual Tovar Tovar { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
