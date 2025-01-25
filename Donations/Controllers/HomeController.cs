using System.Diagnostics;
using Donations.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Donations.Models;

namespace Donations.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole(Roles.DonationCenter))
            {
                return RedirectToAction("Index", "CenterDashboard");                
            }
            if (User.IsInRole(Roles.Donor))
            {
                return RedirectToAction("Index", "DonorDashboard");
            }
            throw new UnauthorizedAccessException("You are neither a donor nor a donation center. Contact an administator.");
        }
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}