using Donations.Database;
using Donations.Entities.Common;
using Donations.Entities.Medical;
using Donations.Entities.User;
using Donations.Models;
using Donations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Donations.Controllers;

[Authorize(Roles = Roles.Donor)]
public class DonorDashboardController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly DonationCenterService _donationCenterService;
    private readonly NotificationService _notificationService;

    public DonorDashboardController(UserManager<User> userManager, ApplicationDbContext dbContext, DonationCenterService donationCenterService, NotificationService notificationService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _donationCenterService = donationCenterService;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        var user = _userManager.GetUserAsync(User).Result;
        var donor = await _dbContext.Donors.SingleAsync(d => d.UserId == user.Id);
        return View(new DonorDashboardViewModel
        {
            Donor = donor
        });
    }

    public async Task<IActionResult> Centers()
    {
        var user = _userManager.GetUserAsync(User).Result;
        var donor = await _dbContext.Donors.SingleAsync(d => d.UserId == user.Id);
        var donationCenters = await _donationCenterService.GetAll();
        List<(DonationCenter Center, double Distance)> donationCentersWithDistances = donationCenters.Select(c => (c, Location.Distance(donor.Location, c.Location)))
                .ToList();
        donationCentersWithDistances = donationCentersWithDistances.OrderBy(c => c.Distance).ToList();
        return View("Centers", new NearestDonationCentersViewModel
        {
            DonationCentersWithDistances = donationCentersWithDistances
        });
    }

    public async Task<IActionResult> Urgent()
    {
        var user = await _userManager.GetUserAsync(User);
        var donor = user!.Donor!;
        var requests = await _dbContext.BloodRequests
            .Include(r => r.DonationCenter)
                .ThenInclude(c => c.Location)
            .Where(r => r.UrgencyLevel == UrgencyLevel.Urgent)
            .OrderBy(r => r.Date)
            .ToListAsync();

        return View(new BloodRequestsViewModel
        {
            Title = "Urgent Requests",
            Description = "These requests need immediate attention!",
            Requests = requests
        });
    }

    public async Task<IActionResult> Available()
    {
        var user = await _userManager.GetUserAsync(User);
        var donor = user!.Donor!;
        var requests = await _dbContext.BloodRequests
            .Include(r => r.DonationCenter)
                .ThenInclude(c => c.Location)
            .Where(r => r.UrgencyLevel == UrgencyLevel.Normal)
            .OrderBy(r => r.Date)
            .ToListAsync();

        return View(new BloodRequestsViewModel
        {
            Title = "Available Requests",
            Description = "Current blood donation requests in your area",
            Requests = requests
        });
    }

    public async Task<IActionResult> Campaigns()
    {
        var user = await _userManager.GetUserAsync(User);
        var donor = user!.Donor!;
        var requests = await _dbContext.BloodRequests
            .Include(r => r.DonationCenter)
                .ThenInclude(c => c.Location)
            .Where(r => r.UrgencyLevel == UrgencyLevel.Campaign)
            .OrderBy(r => r.Date)
            .ToListAsync();

        return View(new BloodRequestsViewModel
        {
            Title = "Blood Donation Campaigns",
            Description = "Upcoming blood donation drives and campaigns",
            Requests = requests
        });
    }

    public async Task<IActionResult> Notifications()
    {
        var user = await _userManager.GetUserAsync(User);
        var notifications = await _notificationService.GetUserNotifications(user!.Id);

        // Mark as read after getting them
        await _notificationService.MarkAllAsRead(user.Id);

        return View(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> ClearNotifications()
    {
        var user = await _userManager.GetUserAsync(User);
        await _notificationService.ClearAll(user.Id);
        return RedirectToAction(nameof(Notifications));
    }

    // Add this to get unread count for the layout
    [HttpGet]
    public async Task<JsonResult> UnreadNotificationCount()
    {
        var user = await _userManager.GetUserAsync(User);
        var count = await _notificationService.GetUnreadCount(user.Id);
        return Json(count);
    }

    public IActionResult ProfileOptions()
    {
        return View();
    }

    public async Task<IActionResult> EditInformation()
    {
        var user = await _userManager.GetUserAsync(User);
        var locations = await _dbContext.Locations.OrderBy(l => l.Name).ToListAsync();

        var model = new EditDonorInfoViewModel
        {
            FullName = user!.FullName,
            Email = user.Email!,
            LocationId = user.Donor!.LocationId,
            AvailableLocations = locations
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditInformation(EditDonorInfoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableLocations = await _dbContext.Locations.OrderBy(l => l.Name).ToListAsync();
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var hasPasswordChange = !string.IsNullOrEmpty(model.CurrentPassword) &&
                               !string.IsNullOrEmpty(model.NewPassword);

        if (hasPasswordChange)
        {
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("ConfirmNewPassword", "The new passwords do not match");
                model.AvailableLocations = await _dbContext.Locations.OrderBy(l => l.Name).ToListAsync();
                return View(model);
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user!, model.CurrentPassword!);
            if (!passwordCheck)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                model.AvailableLocations = await _dbContext.Locations.OrderBy(l => l.Name).ToListAsync();
                return View(model);
            }

            var passwordResult = await _userManager.ChangePasswordAsync(user!, model.CurrentPassword!, model.NewPassword!);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.AvailableLocations = await _dbContext.Locations.OrderBy(l => l.Name).ToListAsync();
                return View(model);
            }
        }

        // Update basic info
        user!.FullName = model.FullName;
        if (user.Email != model.Email)
        {
            var emailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!emailResult.Succeeded)
            {
                foreach (var error in emailResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                model.AvailableLocations = await _dbContext.Locations.OrderBy(l => l.Name).ToListAsync();
                return View(model);
            }
            user.UserName = model.Email; // Keep username in sync with email
        }

        // Update location
        user.Donor!.LocationId = model.LocationId;
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Your information has been updated successfully.";
        return RedirectToAction(nameof(EditInformation));
    }

    public IActionResult DonationTerms()
    {
        return View();
    }

    public async Task<IActionResult> Appointments()
    {
        var user = await _userManager.GetUserAsync(User);
        var appointments = await _dbContext.Appointments
            .Include(a => a.BloodRequest)
                .ThenInclude(r => r.DonationCenter)
                    .ThenInclude(c => c.Location)
            .Where(a => a.DonorId == user!.Donor!.Id &&
                       (a.State == AppointmentState.Pending || a.State == AppointmentState.Accepted) &&
                       a.BloodRequest.Date >= DateTime.Today)
            .OrderBy(a => a.BloodRequest.Date)
            .ToListAsync();

        var viewModel = appointments
            .Select(a => new AppointmentViewModel
            {
                State = a.State,
                CenterName = a.BloodRequest.DonationCenter.Name,
                CenterLocation = a.BloodRequest.DonationCenter.Location.Name,
                Date = a.BloodRequest.Date,
                RequestedBloodTypes = string.Join(", ", a.BloodRequest.BloodTypes
                .Select(bt => bt.ToHumanReadableString()))
            });

        return View(viewModel);
    }
}