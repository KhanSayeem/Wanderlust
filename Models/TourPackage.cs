using System;
using System.Collections.Generic;

namespace TourismApp.Models;

public partial class TourPackage
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int DurationDays { get; set; }

    public decimal Price { get; set; }

    public int GroupSizeLimit { get; set; }

    public string? ImagePath { get; set; }

    public int AgencyProfileId { get; set; }

    public virtual AgencyProfile AgencyProfile { get; set; } = null!;

    public virtual ICollection<TourDate> TourDates { get; set; } = new List<TourDate>();

    public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
    
    public virtual ICollection<TourPackageAmenity> TourPackageAmenities { get; set; } = new List<TourPackageAmenity>();
}
