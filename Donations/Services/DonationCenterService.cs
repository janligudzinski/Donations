using Donations.Database;
using Donations.Entities.Common;
using Donations.Entities.Medical;
using Donations.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Donations.Services;

public class DonationCenterService
{
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public DonationCenterService(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<List<DonationCenter>> GetAll()
    {
        return await _dbContext.DonationCenters
            .ToListAsync();
    }

    public async Task MakeRequest(Guid centerId, HashSet<BloodType> bloodTypes, UrgencyLevel urgencyLevel, DateTime date, int targetMilliliters)
    {
        var center = await _dbContext.DonationCenters
            .Include(dc => dc.BloodSupplies)
            .SingleAsync(dc => dc.Id == centerId);

        var request = new BloodRequest
        {
            DonationCenterId = centerId,
            BloodTypes = bloodTypes,
            UrgencyLevel = urgencyLevel,
            Date = date,
            TargetMilliliters = targetMilliliters
        };

        // Update desired levels for each blood type
        foreach (var bloodType in bloodTypes)
        {
            var supply = center.BloodSupplies.First(bs => bs.BloodType == bloodType);
            supply.DesiredMilliliters += request.TargetMilliliters; // Increase desired level by the target amount
        }

        _dbContext.BloodRequests.Add(request);
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateDonationCenter(string email, string password, string name, Location location)
    {
        var userCreateResult = await _userManager.CreateAsync(new User
        {
            Email = email,
            UserName = email,
            FullName = name
        }, password);
        if (!userCreateResult.Succeeded)
        {
            throw new Exception("User creation for new donation center failed", new Exception(string.Join(", ", userCreateResult.Errors.Select(e => e.Description))));
        }
        var user = await _userManager.FindByEmailAsync(email);

        await _userManager.AddToRoleAsync(user!, Roles.DonationCenter);


        var donationCenter = new DonationCenter
        {
            UserId = user!.Id,
            LocationId = location.Id,
            Name = name,
        };

        await _dbContext.DonationCenters.AddAsync(donationCenter);
        await _dbContext.SaveChangesAsync();
    }
}