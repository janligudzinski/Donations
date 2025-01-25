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
    private readonly DonationCenterService _donationCenterService;
    public CenterDashboardController(UserManager<User> userManager, DonationCenterService donationCenterService)
    {
        _userManager = userManager;
        _donationCenterService = donationCenterService;
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

    public IActionResult AddRequest()
    {
        return View(new AddRequestFormModel());
    }
    
    [HttpPost]
    public async Task<IActionResult> AddRequest(AddRequestFormModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        var donationCenter = user!.DonationCenter!;
        try
        {
            await _donationCenterService.MakeRequest(donationCenter.Id, model.BloodTypes, model.Urgent, model.Date);
            return RedirectToAction("Index");
        } catch (Exception e)
        {
            ViewData["ErrorMessage"] = e.Message;
            return View(model);
        }

        
    }
}