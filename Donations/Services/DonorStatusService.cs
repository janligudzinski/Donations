using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Donations.Entities.User;
using Donations.Entities.Medical;
using Donations.Database;

namespace Donations.Services;
public class DonorStatusService
{
    private readonly ApplicationDbContext _dbContext;
    private const int WEEKS_BETWEEN_DONATIONS = 8;

    public DonorStatusService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(DateTime? NextAppointment, DateTime? EligibleFrom, bool IsEligible)> GetDonorStatus(Guid donorId)
    {
        var latestAppointment = await _dbContext.Appointments
            .Include(a => a.BloodRequest)
            .Where(a => a.DonorId == donorId &&
                       (a.State == AppointmentState.Accepted || a.State == AppointmentState.Complete))
            .OrderByDescending(a => a.BloodRequest.Date)
            .FirstOrDefaultAsync();

        var upcomingAppointment = await _dbContext.Appointments
            .Include(a => a.BloodRequest)
            .Where(a => a.DonorId == donorId &&
                       a.State == AppointmentState.Accepted &&
                       a.BloodRequest.Date >= DateTime.Today)
            .OrderBy(a => a.BloodRequest.Date)
            .FirstOrDefaultAsync();

        DateTime? nextAppointment = upcomingAppointment?.BloodRequest.Date;

        if (latestAppointment == null)
        {
            return (nextAppointment, null, true);
        }

        var eligibleFrom = latestAppointment.BloodRequest.Date.AddDays(WEEKS_BETWEEN_DONATIONS * 7);
        var isEligible = DateTime.Today >= eligibleFrom;

        return (nextAppointment, eligibleFrom, isEligible);
    }
}