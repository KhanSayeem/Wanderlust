using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourismApp.Data;

namespace TourismApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public HomeController(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index()
        {
            // Get popular tours (most booked)
            var popularToursQuery = from tp in _ctx.TourPackages
                                    join td in _ctx.TourDates on tp.Id equals td.TourPackageId into tourDates
                                    from td in tourDates.DefaultIfEmpty()
                                    join b in _ctx.Bookings on td.Id equals b.TourDateId into bookings
                                    from b in bookings.DefaultIfEmpty()
                                    where b == null || b.Status != TourismApp.Models.BookingStatus.Cancelled
                                    group new { tp, b } by new { tp.Id, tp.Title, tp.Description, tp.Price, tp.DurationDays, tp.ImagePath, tp.AgencyProfile } into g
                                    orderby g.Count(x => x.b != null) descending, g.Key.Id descending
                                    select new TourismApp.Models.TourPackage
                                    {
                                        Id = g.Key.Id,
                                        Title = g.Key.Title,
                                        Description = g.Key.Description,
                                        Price = g.Key.Price,
                                        DurationDays = g.Key.DurationDays,
                                        ImagePath = g.Key.ImagePath,
                                        AgencyProfile = g.Key.AgencyProfile
                                    };

            var tours = await _ctx.TourPackages
                .Include(t => t.AgencyProfile)
                .OrderByDescending(t => t.Id)
                .Take(6)
                .ToListAsync();

            ViewBag.TotalTours = await _ctx.TourPackages.CountAsync();
            ViewBag.TotalAgencies = await _ctx.AgencyProfiles.CountAsync();
            ViewBag.TotalBookings = await _ctx.Bookings.CountAsync();

            return View(tours);
        }

        public IActionResult Privacy() => View();

    }
}
