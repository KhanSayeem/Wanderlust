// File: Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TourismApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public AgencyProfile? AgencyProfile { get; set; }   // 1–1
        public TouristProfile? TouristProfile { get; set; } // 1–1

        public ICollection<Booking>? Bookings { get; set; } // 1–many (as Tourist)
    }
}
