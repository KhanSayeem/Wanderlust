using System;
using System.Collections.Generic;

namespace TourismApp.Models;

public partial class TouristProfile
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Country { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;
}
