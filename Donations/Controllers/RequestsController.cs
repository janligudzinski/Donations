using Donations.Database;
using Donations.Entities.User;
using Donations.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Donations.Controllers;

[Authorize(Roles = Roles.Donor)]
public class RequestsController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public RequestsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Route("Requests/{id}")]
    public async Task<IActionResult> Index(Guid id) {
        var request = await _dbContext.BloodRequests.Include(r => r.DonationCenter)
            .ThenInclude(dc => dc.Location).SingleAsync(r => r.Id == id);
        var viewModel = new RequestViewModel
        {
            Request = request
        };
        return View(viewModel);
    }
}