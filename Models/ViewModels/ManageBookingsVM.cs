using System.Collections.Generic;
using TourismApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace TourismApp.Models.ViewModels
{
    public class ManageBookingRow
    {
        public int Id { get; set; }
        public string TouristEmail { get; set; } = string.Empty;
        public string PackageTitle { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int Participants { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }

    public class ManageBookingsVM
    {
        public int? PackageId { get; set; }                // current filter
        public List<TourPackage> Packages { get; set; } = new();
        // agency packages for the filter dropdown

        public IEnumerable<SelectListItem> PackageOptions { get; set; } = Array.Empty<SelectListItem>();

        public List<ManageBookingRow> Items { get; set; } = new();

        // quick totals (Paid revenue only)
        public int TotalParticipants { get; set; }
        public decimal TotalRevenuePaid { get; set; }
    }
}
