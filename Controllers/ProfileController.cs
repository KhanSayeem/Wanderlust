using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourismApp.Data;
using TourismApp.Models;

namespace TourismApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ProfileController(ApplicationDbContext ctx) => _ctx = ctx;

        // Tourist Profile
        [Authorize(Roles = "Tourist")]
        public async Task<IActionResult> Tourist()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _ctx.TouristProfiles.FirstOrDefaultAsync(p => p.UserId == uid);
            
            if (profile == null)
            {
                profile = new TouristProfile { UserId = uid };
            }

            return View(profile);
        }

        [HttpPost, Authorize(Roles = "Tourist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tourist(TouristProfile profile)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var existing = await _ctx.TouristProfiles.FirstOrDefaultAsync(p => p.UserId == uid);
            
            if (existing == null)
            {
                profile.UserId = uid;
                _ctx.TouristProfiles.Add(profile);
            }
            else
            {
                existing.FullName = profile.FullName;
                existing.Country = profile.Country;
            }

            await _ctx.SaveChangesAsync();
            TempData["Msg"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Tourist));
        }

        // Agency Profile
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> Agency()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _ctx.AgencyProfiles.FirstOrDefaultAsync(p => p.UserId == uid);
            
            if (profile == null)
            {
                profile = new AgencyProfile { UserId = uid };
            }

            return View(profile);
        }

        [HttpPost, Authorize(Roles = "Agency")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agency(AgencyProfile profile)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var existing = await _ctx.AgencyProfiles.FirstOrDefaultAsync(p => p.UserId == uid);
            
            if (existing == null)
            {
                profile.UserId = uid;
                _ctx.AgencyProfiles.Add(profile);
            }
            else
            {
                existing.AgencyName = profile.AgencyName;
                existing.Description = profile.Description;
                existing.Website = profile.Website;
            }

            await _ctx.SaveChangesAsync();
            TempData["Msg"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Agency));
        }
    }
}