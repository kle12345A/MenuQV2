﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public partial class VnPayTransaction
    {
        public int Id { get; set; }

        public string? OrderDescription { get; set; }

        public string? TransactionId { get; set; }

        public string? OrderId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentId { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
