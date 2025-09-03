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
            var tours = await _ctx.TourPackages
                .Include(t => t.AgencyProfile)
                .OrderByDescending(t => t.Id)
                .Take(6)
                .ToListAsync();

            return View(tours);
        }

        public IActionResult Privacy() => View();

    }
}
