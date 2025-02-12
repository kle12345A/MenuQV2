using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int RequestId { get; set; }

    public string InvoiceCode { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public bool PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Request Request { get; set; } = null!;
}
