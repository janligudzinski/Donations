using Donations.Database;
using Donations.Entities.Common;
using Donations.Entities.User;
using Microsoft.AspNetCore.Identity;

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