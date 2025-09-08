namespace TourismApp.Models
{
    public class TourPackageAmenity
    {
        public int TourPackageId { get; set; }
        public TourPackage TourPackage { get; set; } = null!;

        public int AmenityId { get; set; }
        public Amenity Amenity { get; set; } = null!;
    }
}
