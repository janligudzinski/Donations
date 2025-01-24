using Donations.Models;
using Microsoft.AspNetCore.Mvc;

namespace Donations.Controllers;

public class LoginController : Controller
{
    public IActionResult Index(string? loginType)
    {
        LoginViewModel login = new LoginViewModel
        {
            IsDonationCenter = loginType == "DonationCenter"
        };
        return View(login);
    }
}