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
    private readonly NotificationService _notificationService;
    private readonly DonorEligibilityService _donorEligibilityService;

    public CenterDashboardController(ApplicationDbContext context, UserManager<User> userManager, DonationCenterService donationCenterService, NotificationService notificationService, DonorEligibilityService donorEligibilityService)
    {
        _context = context;
        _userManager = userManager;
        _donationCenterService = donationCenterService;
        _notificationService = notificationService;
        _donorEligibilityService = donorEligibilityService;
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
        if (!model.BloodTypes.Any())
        {
            ModelState.AddModelError("BloodTypes", "Please select at least one blood type");
        }

        if (!ModelState.IsValid)
        {
            ViewData["ErrorMessage"] = "Please correct the errors and try again.";
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        var donationCenter = user!.DonationCenter!;
        try
        {
            await _donationCenterService.MakeRequest(donationCenter.Id, model.BloodTypes, model.UrgencyLevel, model.Date, model.TargetMilliliters);
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

        var appointments = await _context.Appointments
            .Include(r => r.Donor)
                .ThenInclude(d => d.Location)
            .Include(r => r.BloodRequest)
            .Where(r => r.BloodRequest.DonationCenterId == currentUser.DonationCenter!.Id
                    && (r.State == AppointmentState.Pending || r.State == AppointmentState.Accepted))
            .Select(r => new PendingAppointmentViewModel
            {
                AppointmentId = r.Id,
                DonorName = r.Donor.User.FullName,
                DonorLocation = r.Donor.Location.Name,
                DonorBloodType = r.Donor.BloodType.ToHumanReadableString(),
                RequestedBloodTypes = r.BloodRequest.BloodTypesString,
                RequestDate = r.BloodRequest.Date,
                State = r.State,
                UrgencyLevel = r.BloodRequest.UrgencyLevel
            })
            .OrderBy(r => r.RequestDate)
            .ToListAsync();

        return View(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAppointment(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var appointment = await _context.Appointments
            .Include(a => a.BloodRequest)
                .ThenInclude(r => r.DonationCenter)
            .Include(a => a.Donor)
            .FirstOrDefaultAsync(r => r.Id == id &&
                r.BloodRequest.DonationCenterId == currentUser.DonationCenter!.Id);

        if (appointment == null) return NotFound();

        var (canDonate, _, reason) = await _donorEligibilityService.CanDonate(appointment.DonorId);
        if (!canDonate)
        {
            TempData["ErrorMessage"] = $"Cannot approve: {reason}";
            return RedirectToAction(nameof(PendingAppointments));
        }

        appointment.State = AppointmentState.Accepted;
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotification(
            appointment.Donor.UserId,
            "Appointment accepted",
            $"Your appointment with {appointment.BloodRequest.DonationCenter.Name} on {appointment.BloodRequest.Date.ToShortDateString()} has been accepted"
        );

        return RedirectToAction(nameof(PendingAppointments));
    }

    [HttpPost]
    public async Task<IActionResult> RejectAppointment(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var appointment = await _context.Appointments
            .Include(a => a.BloodRequest).ThenInclude(bloodRequest => bloodRequest.DonationCenter)
            .Include(appointment => appointment.Donor)
            .FirstOrDefaultAsync(r => r.Id == id && r.BloodRequest.DonationCenterId == currentUser.DonationCenter!.Id);

        if (appointment == null) return NotFound();

        appointment.State = AppointmentState.Rejected;
        await _context.SaveChangesAsync();

        await _notificationService.CreateNotification(appointment.Donor.UserId, "Appointment rejected", $"Your appointment with {appointment.BloodRequest.DonationCenter.Name} on {appointment.BloodRequest.Date.ToShortDateString()} has been declined");

        return RedirectToAction(nameof(PendingAppointments));
    }

    [HttpPost]
    public async Task<IActionResult> CompleteAppointment(Guid id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var appointment = await _context.Appointments
            .Include(a => a.BloodRequest)
                .ThenInclude(r => r.DonationCenter)
                    .ThenInclude(dc => dc.BloodSupplies)
            .Include(a => a.Donor)
            .FirstOrDefaultAsync(r => r.Id == id &&
                r.BloodRequest.DonationCenterId == currentUser.DonationCenter!.Id &&
                r.State == AppointmentState.Accepted);

        if (appointment == null) return NotFound();

        // Calculate points based on urgency
        int basePoints = 100;
        double multiplier = appointment.BloodRequest.UrgencyLevel switch
        {
            UrgencyLevel.Urgent => 2.0,
            UrgencyLevel.Campaign => 1.5,
            UrgencyLevel.Normal => 1.0,
            _ => 1.0
        };
        int pointsEarned = (int)(basePoints * multiplier);

        // Update blood supply
        var bloodSupply = appointment.BloodRequest.DonationCenter.BloodSupplies
            .First(bs => bs.BloodType == appointment.Donor.BloodType);
        bloodSupply.MillilitersInStock += 450;

        // Update appointment and donor
        appointment.State = AppointmentState.Complete;
        appointment.Donor.Points += pointsEarned;
        await _context.SaveChangesAsync();

        // Notify the donor
        await _notificationService.CreateNotification(
            appointment.Donor.UserId,
            "Donation Completed - Thank You!",
            $"Thank you for your donation at {appointment.BloodRequest.DonationCenter.Name}! " +
            $"You've earned {pointsEarned} points for this {appointment.BloodRequest.UrgencyLevel.ToString().ToLower()} donation."
        );

        return RedirectToAction(nameof(PendingAppointments));
    }
}