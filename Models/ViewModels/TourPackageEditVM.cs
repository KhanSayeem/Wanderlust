using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TourismApp.Models.ViewModels
{
    public class TourPackageEditVM
    {
        public int Id { get; set; }
        public int AgencyProfileId { get; set; }   // keep FK intact (hidden)

        [Required, StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Range(1, 60)]
        public int DurationDays { get; set; }

        [Precision(18,2)]
        public decimal Price { get; set; }

        [Range(1, 1000)]
        public int GroupSizeLimit { get; set; }

        public string? ImagePath { get; set; }

        // Existing dates (can edit/remove)
        public List<TourDateInput> ExistingDates { get; set; } = new();

        // New dates to add
        public List<TourDateInput> NewDates { get; set; } = new() { new() };
    }
}
