using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int AccountId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public DateOnly? HireDate { get; set; }

    public virtual Account Account { get; set; } = null!;
}
