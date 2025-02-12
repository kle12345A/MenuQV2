using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int RoleId { get; set; }

    public bool? Active { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual Role Role { get; set; } = null!;
}
