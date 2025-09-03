using Microsoft.AspNetCore.Mvc.Rendering;
using TourismApp.Models.ViewModels; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourismApp.Data;
using TourismApp.Models;


namespace TourismApp.Controllers
{
    [Authorize]
    public partial class BookingsController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public BookingsController(ApplicationDbContext ctx) => _ctx = ctx;

         [Authorize(Roles = "Agency")]
    public async Task<IActionResult> Manage(int? packageId)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
        if (agency == null) return Unauthorized();

        var packages = await _ctx.TourPackages
            .Where(p => p.AgencyProfileId == agency.Id)
            .OrderBy(p => p.Title)
            .ToListAsync();

        var q = _ctx.Bookings
            .Include(b => b.User)
            .Include(b => b.TourDate)
                .ThenInclude(td => td.TourPackage)
            .Where(b => b.TourDate.TourPackage.AgencyProfileId == agency.Id)
            .AsQueryable();

        if (packageId.HasValue && packageId.Value > 0)
            q = q.Where(b => b.TourDate.TourPackageId == packageId.Value);

        var list = await q
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new ManageBookingRow
            {
                Id = b.Id,
                TouristEmail = b.User.Email!,
                PackageTitle = b.TourDate.TourPackage.Title,
                Date = b.TourDate.Date,
                Participants = b.Participants,
                Status = b.Status,
                PaymentStatus = b.PaymentStatus
            })
            .ToListAsync();

        var options = packages.Select(p => new SelectListItem
        {
            Value = p.Id.ToString(),
            Text = p.Title,
            Selected = packageId.HasValue && p.Id == packageId.Value
        }).ToList();

        var vm = new ManageBookingsVM
        {
            PackageId = packageId,
            Packages = packages,
            PackageOptions = options,
            Items = list,
            TotalParticipants = list.Sum(x => x.Participants),
            TotalRevenuePaid = list
                .Where(x => x.PaymentStatus == PaymentStatus.Paid)
                .Sum(x => x.Participants * _ctx.TourPackages
                    .Where(p => p.Title == x.PackageTitle && p.AgencyProfileId == agency.Id)
                    .Select(p => p.Price)
                    .FirstOrDefault())
        };

        return View(vm);
    }

    [HttpPost, Authorize(Roles = "Agency")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(int id, BookingStatus status)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var booking = await _ctx.Bookings
            .Include(b => b.TourDate).ThenInclude(td => td.TourPackage)
            .FirstOrDefaultAsync(b => b.Id == id && b.TourDate.TourPackage.AgencyProfileId == _ctx.AgencyProfiles
                .Where(a => a.UserId == uid).Select(a => a.Id).FirstOrDefault());

        if (booking == null) return NotFound();

        // Allowed transitions
        bool allowed =
            (booking.Status == BookingStatus.Pending   && (status == BookingStatus.Confirmed || status == BookingStatus.Cancelled)) ||
            (booking.Status == BookingStatus.Confirmed && (status == BookingStatus.Completed || status == BookingStatus.Cancelled));

        if (!allowed)
        {
            TempData["Error"] = $"Cannot change status from {booking.Status} to {status}.";
            return RedirectToAction(nameof(Manage), new { packageId = booking.TourDate.TourPackageId });
        }

        booking.Status = status;
        await _ctx.SaveChangesAsync();
        TempData["Msg"] = $"Booking #{booking.Id} status set to {status}.";
        return RedirectToAction(nameof(Manage), new { packageId = booking.TourDate.TourPackageId });
    }

    [HttpPost, Authorize(Roles = "Agency")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPayment(int id, PaymentStatus paymentStatus)
    {
        var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var booking = await _ctx.Bookings
            .Include(b => b.TourDate).ThenInclude(td => td.TourPackage)
            .FirstOrDefaultAsync(b => b.Id == id && b.TourDate.TourPackage.AgencyProfileId == _ctx.AgencyProfiles
                .Where(a => a.UserId == uid).Select(a => a.Id).FirstOrDefault());

        if (booking == null) return NotFound();

        if (booking.Status == BookingStatus.Cancelled)
        {
            TempData["Error"] = "Cannot set payment for a cancelled booking.";
            return RedirectToAction(nameof(Manage), new { packageId = booking.TourDate.TourPackageId });
        }

        booking.PaymentStatus = paymentStatus;
        await _ctx.SaveChangesAsync();
        TempData["Msg"] = $"Booking #{booking.Id} payment set to {paymentStatus}.";
        return RedirectToAction(nameof(Manage), new { packageId = booking.TourDate.TourPackageId });
    }

        // Tourist: see own bookings
        [Authorize(Roles = "Tourist")]
        public async Task<IActionResult> Index()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var items = await _ctx.Bookings
                .Where(b => b.UserId == uid)
                .Include(b => b.TourDate)
                    .ThenInclude(td => td.TourPackage)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(items);
        }

        // Tourist: create a booking from the Details page
[HttpPost, Authorize(Roles = "Tourist")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(int tourDateId, int participants)
{
    if (participants < 1)
    {
        TempData["Error"] = "Participants must be at least 1.";
        return RedirectToAction("Details", "Tours", new { id = tourDateId });
    }

    var tourDate = await _ctx.TourDates
        .Include(td => td.TourPackage)
        .FirstOrDefaultAsync(td => td.Id == tourDateId);

    if (tourDate == null) return NotFound();

    // Capacity check: ensure there's enough space
    var booked = await _ctx.Bookings
        .Where(b => b.TourDateId == tourDateId && b.Status != BookingStatus.Cancelled)
        .SumAsync(b => (int?)b.Participants) ?? 0;

    if (booked + participants > tourDate.Capacity)
    {
        TempData["Error"] = $"Not enough capacity. Available: {tourDate.Capacity - booked}.";
        return RedirectToAction("Details", "Tours", new { id = tourDateId });
    }

    var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Create a new booking
    _ctx.Bookings.Add(new Booking
    {
        UserId = uid,
        TourDateId = tourDateId,
        Participants = participants,
        Status = BookingStatus.Pending,
        PaymentStatus = PaymentStatus.Unpaid,
        CreatedAt = DateTime.UtcNow
    });

    await _ctx.SaveChangesAsync();

    TempData["Msg"] = "Booking created successfully!";
    return RedirectToAction("Index", "Bookings");
}



        // (Optional) Tourist: cancel a booking that isn't completed yet
        [HttpPost, Authorize(Roles = "Tourist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var b = await _ctx.Bookings
                .Include(x => x.TourDate).ThenInclude(td => td.TourPackage)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == uid);

            if (b == null) return NotFound();

            if (b.Status == BookingStatus.Completed || b.Status == BookingStatus.Cancelled)
            {
                TempData["Error"] = "This booking cannot be cancelled.";
                return RedirectToAction(nameof(Index));
            }

            b.Status = BookingStatus.Cancelled;
            await _ctx.SaveChangesAsync();
            TempData["Msg"] = "Booking cancelled.";
            return RedirectToAction(nameof(Index));
        }

        // ===== helpers =====
        private async Task<IActionResult> RedirectBackToDetails(int tourDateId)
        {
            var pkgId = await _ctx.TourDates
                .Where(td => td.Id == tourDateId)
                .Select(td => td.TourPackageId)
                .FirstOrDefaultAsync();

            return RedirectToAction("Details", "Tours", new { id = pkgId });
        }
    }
}
