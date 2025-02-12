using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class MenuItem
{
    public int ItemId { get; set; }

    public int? CategoryId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Descriptions { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public bool? Status { get; set; }

    public bool IsHot { get; set; }

    public bool IsNew { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
