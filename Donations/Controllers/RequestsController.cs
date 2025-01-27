using Donations.Database;
using Donations.Entities.Medical;
using Donations.Entities.User;
using Donations.Models;
using Donations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Donations.Controllers;

[Authorize(Roles = Roles.Donor)]
public class RequestsController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly NotificationService _notificationService;
    private readonly ApplicationDbContext _dbContext;

    public RequestsController(ApplicationDbContext dbContext, UserManager<User> userManager, NotificationService notificationService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [Route("Requests/{id}")]
    public async Task<IActionResult> Index(Guid id)
    {
        var donor = (await _userManager.GetUserAsync(User))!.Donor!;
        var request = await _dbContext.BloodRequests.Include(r => r.DonationCenter)
            .ThenInclude(dc => dc.Location).SingleAsync(r => r.Id == id);
        var userHasApplied = await _dbContext.Appointments
            .AnyAsync(a => a.BloodRequestId == request.Id && a.DonorId == donor.Id && a.State == AppointmentState.Pending);
        var viewModel = new RequestViewModel
        {
            Request = request,
            UserHasApplied = userHasApplied
        };
        return View(viewModel);
    }

    [Route("Requests/{id}/Apply")]
    public async Task<IActionResult> Apply(Guid id)
    {
        var donor = (await _userManager.GetUserAsync(User))!.Donor!;
        var request = await _dbContext.BloodRequests.Include(r => r.DonationCenter)
            .ThenInclude(dc => dc.Location).SingleAsync(r => r.Id == id);
        // todo add checks for whether the user can even apply
        var appointment = new Appointment
        {
            BloodRequestId = request.Id,
            DonorId = donor.Id,
            BloodRequest = request,
            State = AppointmentState.Pending
        };
        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync();

        await _dbContext.SaveChangesAsync();

        var viewModel = new RequestViewModel
        {
            Request = request,
            UserHasApplied = true,
            ShowAppliedModal = true
        };

        await _notificationService.CreateNotification(donor.UserId, "Appointment application submitted", $"You have applied to {request.DonationCenter.Name} on {request.Date.ToShortDateString()}");
        return View("Index", viewModel);
    }
}