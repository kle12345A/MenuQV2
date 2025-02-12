using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? RequestId { get; set; }

    public int? ItemId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string? Note { get; set; }

    public virtual MenuItem? Item { get; set; }

    public virtual Request? Request { get; set; }
}
