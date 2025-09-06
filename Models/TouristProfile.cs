using System;
using System.Collections.Generic;

namespace TourismApp.Models;

public partial class TouristProfile
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Country { get; set; }

    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? ProfileImagePath { get; set; }

    public string? EmergencyContact { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = null!;

    public virtual ApplicationUser User { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
