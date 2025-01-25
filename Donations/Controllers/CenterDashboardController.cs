using Donations.Entities.User;
using Donations.Models;
using Donations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Donations.Controllers;

[Authorize(Roles = Roles.DonationCenter)]
public class CenterDashboardController : Controller
{
    private readonly UserManager<User> _userManager;
    public CenterDashboardController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var donationCenter = user!.DonationCenter!;
        var model = new CenterDashboardViewModel
        {
            DonationCenter = donationCenter
        };
        return View(model);
    }
}