using DataAccess.Enum;
using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int RequestId { get; set; }

    public int CustomerId { get; set; }

    public string InvoiceCode { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public bool PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TableId { get; set; }

    public virtual Request Request { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;

    public InvoiceStatus InvoiceStatus { get; set; } = InvoiceStatus.Serving; // Mặc định đang phục vụ
}