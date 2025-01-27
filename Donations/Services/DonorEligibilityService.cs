using Donations.Database;
using Donations.Entities.Medical;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Services
{
    public class DonorEligibilityService
    {
        private readonly ApplicationDbContext _context;
        private const int WEEKS_BETWEEN_DONATIONS = 8;

        public DonorEligibilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsEligible, DateTime? NextEligibleDate, string Reason)> CanDonate(Guid donorId)
        {
            var latestAppointment = await _context.Appointments
                .Where(a => a.DonorId == donorId &&
                           (a.State == AppointmentState.Accepted || a.State == AppointmentState.Complete))
                .OrderByDescending(a => a.BloodRequest.Date)
                .FirstOrDefaultAsync();

            if (latestAppointment == null)
                return (true, null, "");

            var nextEligibleDate = latestAppointment.BloodRequest.Date.AddDays(WEEKS_BETWEEN_DONATIONS * 7);
            var isEligible = DateTime.Today >= nextEligibleDate;
            var reason = !isEligible ?
                $"You must wait until {nextEligibleDate:MMM d, yyyy} between donations" : "";

            return (isEligible, nextEligibleDate, reason);
        }

        public async Task<(bool IsEligible, string Reason)> CanDonateForRequest(Guid donorId, BloodRequest request)
        {
            var donor = await _context.Donors
                .FirstAsync(d => d.Id == donorId);

            // Check blood type compatibility
            if (!request.BloodTypes.Contains(donor.BloodType))
            {
                return (false, $"This request is not for your blood type.");
            }

            // Check timing eligibility
            var (isEligible, nextEligibleDate, timeReason) = await CanDonate(donorId);
            if (!isEligible)
            {
                return (false, timeReason);
            }

            // Check if request date is too soon
            if (request.Date < nextEligibleDate)
            {
                return (false, $"You won't be eligible to donate until {nextEligibleDate.Value.ToShortDateString()}");
            }

            return (true, "");
        }
    }
}