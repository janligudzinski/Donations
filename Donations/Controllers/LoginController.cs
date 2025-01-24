using Donations.Entities.User;
using Donations.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Donations.Controllers;

public class LoginController : Controller
{
    private readonly SignInManager<User> _signInManager;

    public LoginController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public IActionResult Index(string? loginType)
    {
        LoginViewModel login = new LoginViewModel
        {
            IsDonationCenter = loginType == "DonationCenter"
        };
        return View(login);
    }
    
    [HttpPost]
    public async Task<IActionResult> Index(LoginFormModel loginFormModel)
    {
        var user = await _signInManager.PasswordSignInAsync(loginFormModel.Email, loginFormModel.Password, true, false);
        if (!user.Succeeded)
        {
            return View("Index", new LoginViewModel
            {
                Failed = true
            });
        }
        return RedirectToAction("Index", "Home");
    }
}