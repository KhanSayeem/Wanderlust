using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourismApp.Data;
using TourismApp.Models;
using TourismApp.Models.ViewModels;


namespace TourismApp.Controllers
{
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _ctx;
        public ToursController(ApplicationDbContext ctx) => _ctx = ctx;

        // Everyone can browse
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? q)
        {
            var query = _ctx.TourPackages.Include(t => t.AgencyProfile).AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Title.Contains(q) || t.Description.Contains(q));
            return View(await query.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var pkg = await _ctx.TourPackages
                .Include(p => p.AgencyProfile)
                .Include(p => p.TourDates)
                .FirstOrDefaultAsync(p => p.Id == id);
            return pkg == null ? NotFound() : View(pkg);
        }

        // Agency side
        [Authorize(Roles = "Agency")]
       public async Task<IActionResult> MyPackages()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
            if (agency == null) return View(Enumerable.Empty<TourPackage>());

            var pkgs = await _ctx.TourPackages
                .Where(p => p.AgencyProfileId == agency.Id)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(pkgs);
        }

        [Authorize(Roles = "Agency")]
public IActionResult Create() => View(new TourPackageCreateVM());

[HttpPost, Authorize(Roles = "Agency")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(TourPackageCreateVM vm)
{
    if (!ModelState.IsValid) return View(vm);

    var uid = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!;

    // ensure an agency profile exists
    var agency = await _ctx.AgencyProfiles.SingleOrDefaultAsync(a => a.UserId == uid);
    if (agency == null)
    {
        agency = new AgencyProfile { UserId = uid, AgencyName = "My Agency" };
        _ctx.AgencyProfiles.Add(agency);
        await _ctx.SaveChangesAsync();
    }

    var entity = new TourPackage
    {
        Title          = vm.Title,
        Description    = vm.Description,
        DurationDays   = vm.DurationDays,
        Price          = vm.Price,
        GroupSizeLimit = vm.GroupSizeLimit,
        ImagePath      = string.IsNullOrWhiteSpace(vm.ImagePath) ? "/images/placeholder.jpg" : vm.ImagePath,
        AgencyProfileId = agency.Id
    };

    // add any non-empty dates
    foreach (var d in vm.NewDates.Where(d => d.Date.HasValue && d.Capacity.HasValue && d.Capacity > 0))
        entity.TourDates.Add(new TourDate { Date = d.Date.Value.Date, Capacity = d.Capacity.Value });

    _ctx.TourPackages.Add(entity);
    await _ctx.SaveChangesAsync();

    TempData["Msg"] = "Package created.";
    return RedirectToAction(nameof(MyPackages));
}

       [Authorize(Roles = "Agency")]
public async Task<IActionResult> Edit(int id)
{
    var uid = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!;
    var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
    if (agency == null) return Unauthorized();

    var entity = await _ctx.TourPackages
        .Include(p => p.TourDates)
        .FirstOrDefaultAsync(p => p.Id == id && p.AgencyProfileId == agency.Id);
    if (entity == null) return NotFound();

    var vm = new TourPackageEditVM
    {
        Id = entity.Id,
        AgencyProfileId = entity.AgencyProfileId,
        Title = entity.Title,
        Description = entity.Description,
        DurationDays = entity.DurationDays,
        Price = entity.Price,
        GroupSizeLimit = entity.GroupSizeLimit,
        ImagePath = entity.ImagePath,
        ExistingDates = entity.TourDates
            .OrderBy(d => d.Date)
            .Select(d => new TourDateInput { Id = d.Id, Date = d.Date, Capacity = d.Capacity })
            .ToList(),
        NewDates = new() { new TourDateInput() }
    };

    return View(vm);
}

[HttpPost, Authorize(Roles = "Agency")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, TourPackageEditVM vm)
{
    if (id != vm.Id) return BadRequest();
    if (!ModelState.IsValid) return View(vm);

    var uid = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!;
    var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
    if (agency == null) return Unauthorized();

    var entity = await _ctx.TourPackages
        .Include(p => p.TourDates)
        .FirstOrDefaultAsync(p => p.Id == id && p.AgencyProfileId == agency.Id);
    if (entity == null) return NotFound();

    // update package (not FK)
    entity.Title          = vm.Title;
    entity.Description    = vm.Description;
    entity.DurationDays   = vm.DurationDays;
    entity.Price          = vm.Price;
    entity.GroupSizeLimit = vm.GroupSizeLimit;
    entity.ImagePath      = string.IsNullOrWhiteSpace(vm.ImagePath) ? entity.ImagePath : vm.ImagePath;

    // update existing dates or remove
    foreach (var row in vm.ExistingDates)
    {
        if (!row.Id.HasValue) continue;
        var ed = entity.TourDates.FirstOrDefault(t => t.Id == row.Id.Value);
        if (ed == null) continue;

        if (row.Remove)
            _ctx.TourDates.Remove(ed);
        else
        {
            if (row.Date.HasValue)     ed.Date     = row.Date.Value.Date;
            if (row.Capacity.HasValue) ed.Capacity = row.Capacity.Value;
        }
    }

    // add new dates
    foreach (var nd in vm.NewDates.Where(d => d.Date.HasValue && d.Capacity.HasValue && d.Capacity > 0))
        entity.TourDates.Add(new TourDate { Date = nd.Date.Value.Date, Capacity = nd.Capacity.Value });

    await _ctx.SaveChangesAsync();
    TempData["Msg"] = "Package & dates updated.";
    return RedirectToAction(nameof(MyPackages));
}

        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> Delete(int id)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var agency = await _ctx.AgencyProfiles.FirstOrDefaultAsync(a => a.UserId == uid);
            if (agency == null) return Unauthorized();

            var tourPackage = await _ctx.TourPackages
                .FirstOrDefaultAsync(p => p.Id == id && p.AgencyProfileId == agency.Id);
            
            if (tourPackage != null)
            {
                _ctx.TourPackages.Remove(tourPackage);
                await _ctx.SaveChangesAsync();
                TempData["Msg"] = "Tour package deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Tour package not found or you don't have permission to delete it.";
            }
            
            return RedirectToAction(nameof(MyPackages));
        }
    }
}
