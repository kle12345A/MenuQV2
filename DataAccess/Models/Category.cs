using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
