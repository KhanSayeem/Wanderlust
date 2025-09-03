using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourismApp.Data;

namespace TourismApp.Controllers
{
    [Authorize(Roles = "Agency")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ReportsController(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
            if (agency == null) return Unauthorized();

            var data = await _ctx.Bookings
                .Include(b => b.TourDate).ThenInclude(td => td.TourPackage)
                .Where(b => b.TourDate.TourPackage.AgencyProfileId == agency.Id)
                .GroupBy(b => new { b.TourDate.TourPackage.Title, b.Status })
                .Select(g => new ReportRow
                {
                    PackageTitle = g.Key.Title,
                    Status = g.Key.Status.ToString(),
                    Bookings = g.Count(),
                    Participants = g.Sum(x => x.Participants),
                    Revenue = g.Where(x => x.PaymentStatus == TourismApp.Models.PaymentStatus.Paid)
                                .Sum(x => x.Participants * x.TourDate.TourPackage.Price)
                })
                .OrderBy(r => r.PackageTitle)
                .ToListAsync();

            return View(data);
        }
    }

    public class ReportRow
    {
        public string PackageTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public int Participants { get; set; }
        public decimal Revenue { get; set; }
    }
}
