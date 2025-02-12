using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public int AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public virtual Account Account { get; set; } = null!;
}
