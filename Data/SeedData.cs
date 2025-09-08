// File: Data/SeedData.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TourismApp.Models;

namespace TourismApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<ApplicationDbContext>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            // Ensure DB/migrations are applied
            await context.Database.MigrateAsync();

            // 1) Roles
            string[] roles = new[] { "Tourist", "Agency" };
            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                {
                    await roleManager.CreateAsync(new IdentityRole(r));
                }
            }

            // 2) Demo users
            // Agency
            var agency = await userManager.FindByEmailAsync("agency@demo.com");
            if (agency == null)
            {
                agency = new ApplicationUser
                {
                    UserName = "agency@demo.com",
                    Email = "agency@demo.com",
                    EmailConfirmed = true
                };
                var created = await userManager.CreateAsync(agency, "Passw0rd!");
                if (created.Succeeded)
                    await userManager.AddToRoleAsync(agency, "Agency");
            }

            // Tourist
            var tourist = await userManager.FindByEmailAsync("tourist@demo.com");
            if (tourist == null)
            {
                tourist = new ApplicationUser
                {
                    UserName = "tourist@demo.com",
                    Email = "tourist@demo.com",
                    EmailConfirmed = true
                };
                var created = await userManager.CreateAsync(tourist, "Passw0rd!");
                if (created.Succeeded)
                    await userManager.AddToRoleAsync(tourist, "Tourist");
            }


            // (Optional) When you add domain models later, you can seed sample tours here.
            if (agency != null && await context.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == agency.Id) == null)
                {
                    context.AgencyProfiles.Add(new AgencyProfile
                    {
                        UserId = agency.Id,
                        AgencyName = "Sunny Trails",
                        Description = "Day hikes and city tours.",
                        Website = "https://example.com"
                    });
                    await context.SaveChangesAsync();
                }

                if (tourist != null && await context.TouristProfiles.FirstOrDefaultAsync(t => t.UserId == tourist.Id) == null)
                {
                    context.TouristProfiles.Add(new TouristProfile
                    {
                        UserId = tourist.Id,
                        FullName = "Alex Explorer",
                        Country = "Finland"
                    });
                    await context.SaveChangesAsync();
                }

                // Seed amenities
                if (!await context.Amenities.AnyAsync())
                {
                    context.Amenities.AddRange(
                        new Amenity { Name = "Hotel Pickup" },
                        new Amenity { Name = "Meals Included" },
                        new Amenity { Name = "Guide" }
                    );
                    await context.SaveChangesAsync();
                }

                // Seed one tour package with two dates
                if (!await context.TourPackages.AnyAsync())
                {
                    var agencyProfile = await context.AgencyProfiles.FirstAsync();
                    var guideAmenity  = await context.Amenities.FirstAsync();

                    var pkg = new TourPackage
                    {
                        AgencyProfileId = agencyProfile.Id,
                        Title = "Old Town Walking Tour",
                        Description = "3-hour guided walk through historic sites.",
                        DurationDays = 1,
                        Price = 49,
                        GroupSizeLimit = 20,
                        ImagePath = "/images/oldtowntour.jpg"
                    };

                    pkg.TourDates.Add(new TourDate { Date = DateTime.UtcNow.Date.AddDays(7),  Capacity = 20 });
                    pkg.TourDates.Add(new TourDate { Date = DateTime.UtcNow.Date.AddDays(14), Capacity = 20 });

                    context.TourPackages.Add(pkg);
                    await context.SaveChangesAsync();

                    // link amenity
                    context.TourPackageAmenities.Add(new TourPackageAmenity
                    {
                        TourPackageId = pkg.Id,
                        AmenityId = guideAmenity.Id
                    });

                    await context.SaveChangesAsync();
                    
                    // Create a sample completed booking for testing feedback
                    if (!await context.Bookings.AnyAsync())
                    {
                        var tourDate = pkg.TourDates.First();
                        var sampleBooking = new Booking
                        {
                            UserId = tourist.Id,
                            TourDateId = tourDate.Id,
                            Participants = 2,
                            Status = BookingStatus.Completed,
                            PaymentStatus = PaymentStatus.Paid,
                            CreatedAt = DateTime.UtcNow.AddDays(-5)
                        };
                        
                        context.Bookings.Add(sampleBooking);
                        await context.SaveChangesAsync();
                    }
                }
        }
    }
}
