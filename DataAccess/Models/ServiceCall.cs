using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class ServiceCall
{
    public int ServiceCallId { get; set; }

    public int? RequestId { get; set; }

    public int? ReasonId { get; set; }

    public string? Note { get; set; }

    public virtual ServiceReason? Reason { get; set; }

    public virtual Request? Request { get; set; }
}
