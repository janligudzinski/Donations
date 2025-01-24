using Donations.Models;
using Donations.Services;
using Microsoft.AspNetCore.Mvc;

namespace Donations.Controllers;

public class RegisterController : Controller
{
    private readonly LocationService _locationService;
    private readonly RegisterService _registerService;

    public RegisterController(LocationService locationService, RegisterService registerService)
    {
        _locationService = locationService;
        _registerService = registerService;
    }

    public async Task<IActionResult> Index()
    {
        var locations = await _locationService.GetAllLocations();
        return View(new RegisterViewModel
        {
            EligibleLocations = locations
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(RegisterFormModel registerFormModel)
    {
        try
        {
            var user = await _registerService.RegisterDonor(registerFormModel);
            return View("Success");
        } catch (Exception e)
        {
            return View("Index", new RegisterViewModel
            {
                EligibleLocations = await _locationService.GetAllLocations(),
                ErrorMessage = e.Message
            });
        }
    }
}