// File: Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourismApp.Models;

namespace TourismApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<AgencyProfile> AgencyProfiles => Set<AgencyProfile>();
        public DbSet<TouristProfile> TouristProfiles => Set<TouristProfile>();
        public DbSet<TourPackage> TourPackages => Set<TourPackage>();
        public DbSet<TourDate> TourDates => Set<TourDate>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<TourPackageAmenity> TourPackageAmenities => Set<TourPackageAmenity>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Remove duplicate identity table configurations - they're handled by base.OnModelCreating()

            // 1–1: User ↔ AgencyProfile (cascade is fine)
            builder.Entity<AgencyProfile>()
                .HasOne(a => a.User)
                .WithOne(u => u.AgencyProfile)
                .HasForeignKey<AgencyProfile>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1–1: User ↔ TouristProfile (cascade is fine)
            builder.Entity<TouristProfile>()
                .HasOne(t => t.User)
                .WithOne(u => u.TouristProfile)
                .HasForeignKey<TouristProfile>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // TourPackage -> AgencyProfile (cascade ok)
            builder.Entity<TourPackage>()
                .HasOne(tp => tp.AgencyProfile)
                .WithMany(a => a.TourPackages)
                .HasForeignKey(tp => tp.AgencyProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // TourDate -> TourPackage (cascade ok)
            builder.Entity<TourDate>()
                .HasOne(td => td.TourPackage)
                .WithMany(tp => tp.TourDates)
                .HasForeignKey(td => td.TourPackageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> TourDate (keep cascade, so deleting a date cleans bookings)
            builder.Entity<Booking>()
                .HasOne(b => b.TourDate)
                .WithMany(td => td.Bookings)
                .HasForeignKey(b => b.TourDateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking -> User (DISABLE cascade to break multiple path)
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings!)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction); // or Restrict

            // Feedback -> Booking (1–1, cascade is fine)
            builder.Entity<Feedback>()
                .HasOne(f => f.Booking)
                .WithOne(b => b.Feedback!)
                .HasForeignKey<Feedback>(f => f.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many–many join
            builder.Entity<TourPackageAmenity>()
                .HasKey(x => new { x.TourPackageId, x.AmenityId });
            builder.Entity<TourPackageAmenity>()
                .HasOne(x => x.TourPackage)
                .WithMany(p => p.TourPackageAmenities)
                .HasForeignKey(x => x.TourPackageId);
            builder.Entity<TourPackageAmenity>()
                .HasOne(x => x.Amenity)
                .WithMany(a => a.TourPackageAmenities)
                .HasForeignKey(x => x.AmenityId);

            // Configure decimal precision for Price property
            builder.Entity<TourPackage>()
                .Property(tp => tp.Price)
                .HasPrecision(18, 2);
        }
    }
}
