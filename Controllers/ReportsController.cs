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

            var summaryData = await _ctx.Bookings
                .Include(b => b.TourDate).ThenInclude(td => td.TourPackage)
                .Where(b => b.TourDate.TourPackage.AgencyProfileId == agency.Id)
                .GroupBy(b => new { b.TourDate.TourPackage.Title, b.Status })
                .Select(g => new ReportSummaryRow
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

            return View(summaryData);
        }

        public async Task<IActionResult> Detailed()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
            if (agency == null) return Unauthorized();

            var detailedData = await _ctx.Bookings
                .Include(b => b.User)
                .Include(b => b.TourDate).ThenInclude(td => td.TourPackage)
                .Include(b => b.Feedback)
                .Where(b => b.TourDate.TourPackage.AgencyProfileId == agency.Id)
                .Select(b => new DetailedReportRow
                {
                    BookingId = b.Id,
                    PackageTitle = b.TourDate.TourPackage.Title,
                    TourDate = b.TourDate.Date,
                    CustomerEmail = b.User.Email!,
                    Participants = b.Participants,
                    Status = b.Status,
                    PaymentStatus = b.PaymentStatus,
                    BookingDate = b.CreatedAt,
                    TotalAmount = b.Participants * b.TourDate.TourPackage.Price,
                    HasFeedback = b.Feedback != null,
                    Rating = b.Feedback != null ? b.Feedback.Rating : (int?)null
                })
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(detailedData);
        }
    }

    public class ReportSummaryRow
    {
        public string PackageTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Bookings { get; set; }
        public int Participants { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DetailedReportRow
    {
        public int BookingId { get; set; }
        public string PackageTitle { get; set; } = string.Empty;
        public DateTime TourDate { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public int Participants { get; set; }
        public TourismApp.Models.BookingStatus Status { get; set; }
        public TourismApp.Models.PaymentStatus PaymentStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool HasFeedback { get; set; }
        public int? Rating { get; set; }
    }
}
