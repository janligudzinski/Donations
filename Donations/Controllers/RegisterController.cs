using Donations.Models;
using Microsoft.AspNetCore.Mvc;

namespace Donations.Controllers;

public class RegisterController : Controller
{
    public IActionResult Index()
    {
        return View(new RegisterViewModel());
    }
}