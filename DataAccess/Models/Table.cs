using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Table
{
    public int TableId { get; set; }

    public int? AreaId { get; set; }

    public string TableNumber { get; set; } = null!;

    public bool? Status { get; set; }

    public int SeatCapacity { get; set; }

    public string TableStatus { get; set; } = null!;

    public virtual Area? Area { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
