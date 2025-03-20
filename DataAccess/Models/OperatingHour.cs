using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class OperatingHour
{
    public int OperatingHourId { get; set; }

    public String RestaurantName { get; set; }

    public String ImageURL { get; set; }

    public TimeOnly OpeningTime { get; set; }

    public TimeOnly ClosingTime { get; set; }

    public bool? IsOpen { get; set; }
}
