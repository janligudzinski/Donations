using Donations.Database;
using Donations.Entities.User;
using Donations.Models;
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

    public DonorDashboardController(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
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
}