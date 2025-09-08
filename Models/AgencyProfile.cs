using System;
using System.Collections.Generic;

namespace TourismApp.Models;

public partial class AgencyProfile
{
    public int Id { get; set; }

    public string AgencyName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Website { get; set; }

    public string? TourGuideInfo { get; set; }

    public string? ServicesOffered { get; set; }

    public string? ProfileImagePath { get; set; }

    public string? ContactPhone { get; set; }

    public string? Address { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = null!;

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();

    public virtual ApplicationUser User { get; set; } = null!;
}
