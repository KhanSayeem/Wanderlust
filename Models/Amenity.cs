using System;
using System.Collections.Generic;

namespace TourismApp.Models;

public partial class Amenity
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TourPackage> TourPackages { get; set; } = new List<TourPackage>();
    
    public virtual ICollection<TourPackageAmenity> TourPackageAmenities { get; set; } = new List<TourPackageAmenity>();
}
