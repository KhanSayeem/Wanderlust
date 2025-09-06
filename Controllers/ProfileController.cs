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
        public async Task<IActionResult> Tourist(TouristProfile profile, IFormFile? profileImage)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var existing = await _ctx.TouristProfiles.FirstOrDefaultAsync(p => p.UserId == uid);
            
            // Handle image upload
            string? imagePath = null;
            if (profileImage != null && profileImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
                
                if (allowedExtensions.Contains(extension))
                {
                    var fileName = $"{uid}_tourist_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                    var uploadPath = Path.Combine("wwwroot", "uploads", "profiles");
                    Directory.CreateDirectory(uploadPath);
                    var fullPath = Path.Combine(uploadPath, fileName);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }
                    
                    imagePath = $"/uploads/profiles/{fileName}";
                }
            }
            
            if (existing == null)
            {
                profile.UserId = uid;
                if (imagePath != null) profile.ProfileImagePath = imagePath;
                _ctx.TouristProfiles.Add(profile);
            }
            else
            {
                existing.FullName = profile.FullName;
                existing.Country = profile.Country;
                existing.Phone = profile.Phone;
                existing.DateOfBirth = profile.DateOfBirth;
                existing.EmergencyContact = profile.EmergencyContact;
                if (imagePath != null) existing.ProfileImagePath = imagePath;
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
        public async Task<IActionResult> Agency(AgencyProfile profile, IFormFile? profileImage)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            if (!ModelState.IsValid)
            {
                return View(profile);
            }

            var existing = await _ctx.AgencyProfiles.FirstOrDefaultAsync(p => p.UserId == uid);
            
            // Handle image upload
            string? imagePath = null;
            if (profileImage != null && profileImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
                
                if (allowedExtensions.Contains(extension))
                {
                    var fileName = $"{uid}_agency_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                    var uploadPath = Path.Combine("wwwroot", "uploads", "profiles");
                    Directory.CreateDirectory(uploadPath);
                    var fullPath = Path.Combine(uploadPath, fileName);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await profileImage.CopyToAsync(stream);
                    }
                    
                    imagePath = $"/uploads/profiles/{fileName}";
                }
            }
            
            if (existing == null)
            {
                profile.UserId = uid;
                if (imagePath != null) profile.ProfileImagePath = imagePath;
                _ctx.AgencyProfiles.Add(profile);
            }
            else
            {
                existing.AgencyName = profile.AgencyName;
                existing.Description = profile.Description;
                existing.Website = profile.Website;
                existing.TourGuideInfo = profile.TourGuideInfo;
                existing.ServicesOffered = profile.ServicesOffered;
                existing.ContactPhone = profile.ContactPhone;
                existing.Address = profile.Address;
                if (imagePath != null) existing.ProfileImagePath = imagePath;
            }

            await _ctx.SaveChangesAsync();
            TempData["Msg"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Agency));
        }
    }
}