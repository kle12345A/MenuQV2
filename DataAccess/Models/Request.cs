using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Request
{
    public int RequestId { get; set; }

    public int? TableId { get; set; }

    public int? CustomerId { get; set; }

    public int? RequestTypeId { get; set; }

    public int? RequestStatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CancellationReasonId { get; set; }

    public int? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual CancellationReason? CancellationReason { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual RequestStatus? RequestStatus { get; set; }

    public virtual RequestType? RequestType { get; set; }

    public virtual ICollection<ServiceCall> ServiceCalls { get; set; } = new List<ServiceCall>();

    public virtual Table? Table { get; set; }
}
