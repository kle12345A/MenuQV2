﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.DTOs
{
    public class InvoiceDTO
    {
        public int RequestId { get; set; }
        public string InvoiceCode { get; set; }
        public int? TableId { get; set; } 
        public String TableName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public string InvoiceStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }


}
