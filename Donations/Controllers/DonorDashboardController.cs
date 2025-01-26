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

    public DonorDashboardController(UserManager<User> userManager, ApplicationDbContext dbContext, DonationCenterService donationCenterService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _donationCenterService = donationCenterService;
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
}