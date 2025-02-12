using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Area
{
    public int AreaId { get; set; }

    public string AreaName { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
