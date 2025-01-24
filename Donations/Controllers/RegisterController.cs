using Donations.Models;
using Donations.Services;
using Microsoft.AspNetCore.Mvc;

namespace Donations.Controllers;

public class RegisterController : Controller
{
    private readonly LocationService _locationService;

    public RegisterController(LocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task <IActionResult> Index()
    {
        var locations = await _locationService.GetAllLocations();
        return View(new RegisterViewModel
        {
            EligibleLocations = locations
        });
    }
}