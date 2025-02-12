using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class RequestType
{
    public int RequestTypeId { get; set; }

    public string RequestTypeName { get; set; } = null!;

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
