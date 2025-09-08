using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourismApp.Data;
using TourismApp.Models;

namespace TourismApp.Controllers
{
    [Authorize]
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public FeedbacksController(ApplicationDbContext ctx) => _ctx = ctx;

        // GET: Feedback/Create
        public async Task<IActionResult> Create(int bookingId)
{
    var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Ensure the booking is completed for the current user
    var booking = await _ctx.Bookings
        .Include(b => b.TourDate).ThenInclude(td => td.TourPackage)
        .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == uid && b.Status == BookingStatus.Completed);

    if (booking == null)
        return RedirectToAction("Index", "Bookings"); // If no completed booking found, redirect to booking list.

    var feedback = new Feedback
    {
        BookingId = bookingId
    };

    ViewData["BookingId"] = bookingId; // Set the BookingId to be used in the form

    return View(feedback);
}



     [HttpPost, Authorize(Roles = "Tourist")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Feedback feedback)
{
    Console.WriteLine($"Received Feedback - BookingId: {feedback.BookingId}, Rating: {feedback.Rating}, Comment: {feedback.Comment}");

    if (!ModelState.IsValid)
    {
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine($"Model error: {error.ErrorMessage}");
        }
        TempData["Error"] = "Invalid feedback submission.";
        return View(feedback);
    }

    var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var booking = await _ctx.Bookings
        .FirstOrDefaultAsync(b => b.Id == feedback.BookingId && b.UserId == uid);

    if (booking == null || booking.Status != BookingStatus.Completed)
    {
        TempData["Error"] = "You can only leave feedback for completed bookings.";
        return RedirectToAction("Index", "Bookings");
    }

    // Check if feedback already exists
    var existingFeedback = await _ctx.Feedbacks
        .FirstOrDefaultAsync(f => f.BookingId == feedback.BookingId);
    
    if (existingFeedback != null)
    {
        TempData["Error"] = "You have already submitted feedback for this booking.";
        return RedirectToAction("Index", "Bookings");
    }

    feedback.CreatedAt = DateTime.UtcNow;
    _ctx.Feedbacks.Add(feedback);
    await _ctx.SaveChangesAsync();

    Console.WriteLine($"Feedback saved for BookingId: {feedback.BookingId}");

    TempData["Msg"] = "Thank you for your feedback!";
    return RedirectToAction("Index", "Bookings");
}

        // GET: My Feedbacks (Tourist)
        [Authorize(Roles = "Tourist")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var feedbacks = await _ctx.Feedbacks
                    .Include(f => f.Booking)
                        .ThenInclude(b => b.TourDate)
                        .ThenInclude(td => td.TourPackage)
                    .Where(f => f.Booking.UserId == uid)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();

                return View(feedbacks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving feedbacks: {ex.Message}");
                TempData["Error"] = "An error occurred while retrieving your feedbacks.";
                return View(new List<Feedback>());
            }
        }

        // GET: Agency Feedbacks
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> Agency(int? packageId)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
            if (agency == null) return Unauthorized();

            var packages = await _ctx.TourPackages
                .Where(p => p.AgencyProfileId == agency.Id)
                .OrderBy(p => p.Title)
                .ToListAsync();

            var q = _ctx.Feedbacks
                .Include(f => f.Booking)
                    .ThenInclude(b => b.User)
                .Include(f => f.Booking)
                    .ThenInclude(b => b.TourDate)
                    .ThenInclude(td => td.TourPackage)
                .Where(f => f.Booking.TourDate.TourPackage.AgencyProfileId == agency.Id)
                .AsQueryable();

            if (packageId.HasValue && packageId.Value > 0)
                q = q.Where(f => f.Booking.TourDate.TourPackageId == packageId.Value);

            var feedbacks = await q
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            ViewBag.Packages = packages;
            ViewBag.SelectedPackageId = packageId;
            return View(feedbacks);
        }

    }
}
