using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class CancellationReason
{
    public int ReasonId { get; set; }

    public string ReasonText { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
