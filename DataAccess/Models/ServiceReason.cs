using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ServiceReason
{
    public int ReasonId { get; set; }

    public string ReasonText { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<ServiceCall> ServiceCalls { get; set; } = new List<ServiceCall>();
}
