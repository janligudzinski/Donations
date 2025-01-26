using Donations.Database;
using Donations.Entities.User;
using Donations.Models;
using Donations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Donations.Entities.Medical;

namespace Donations.Controllers;

[Authorize(Roles = Roles.DonationCenter)]
public class CenterDashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly DonationCenterService _donationCenterService;

    public CenterDashboardController(ApplicationDbContext context, UserManager<User> userManager, DonationCenterService donationCenterService)
    {
        _context = context;
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
        }
        catch (Exception e)
        {
            ViewData["ErrorMessage"] = e.Message;
            return View(model);
        }
    }

    public async Task<IActionResult> PendingAppointments()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var pendingRequests = await _context.Appointments
            .Include(r => r.Donor)
                .ThenInclude(d => d.Location)
            .Include(r => r.BloodRequest)
            .Where(r => r.BloodRequest.DonationCenterId == currentUser.DonationCenter.Id
                    && r.State == AppointmentState.Pending)
            .Select(r => new PendingAppointmentViewModel
            {
                Id = r.Id,
                DonorName = r.Donor.User.FullName,
                DonorLocation = r.Donor.Location.Name,
                DonorBloodType = r.Donor.BloodType.ToHumanReadableString(),
                RequestedBloodTypes = r.BloodRequest.BloodTypesString,
                RequestDate = r.BloodRequest.Date
            })
            .ToListAsync();

        return View(pendingRequests);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAppointment(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var appointment = await _context.Appointments
            .Include(a => a.BloodRequest)
            .FirstOrDefaultAsync(r => r.Id == id && r.BloodRequest.DonationCenterId == currentUser.DonationCenter.Id);

        if (appointment == null) return NotFound();

        appointment.State = AppointmentState.Accepted;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(PendingAppointments));
    }

    [HttpPost]
    public async Task<IActionResult> RejectAppointment(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var appointment = await _context.Appointments
            .Include(a => a.BloodRequest)
            .FirstOrDefaultAsync(r => r.Id == id && r.BloodRequest.DonationCenterId == currentUser.DonationCenter.Id);

        if (appointment == null) return NotFound();

        appointment.State = AppointmentState.Rejected;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(PendingAppointments));
    }
}