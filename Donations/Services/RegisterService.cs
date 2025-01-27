using Donations.Database;
using Donations.Entities.User;
using Donations.Models;
using Microsoft.AspNetCore.Identity;

namespace Donations.Services;

public class RegisterService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ApplicationDbContext _dbContext;
    
    public RegisterService(UserManager<User> userManager, RoleManager<Role> roleManager, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
    }

    public async Task<User> RegisterDonor(RegisterFormModel registerFormModel)
    {
        if (!registerFormModel.Consent)
        {
            throw new Exception(
                "You must consent to the collection of your medical data and attest to your ability to donate.");
        }
        var createResult = await _userManager.CreateAsync(new User
        {
            FullName = registerFormModel.FullName,
            Email = registerFormModel.Email,
            UserName = registerFormModel.Email // we don't actually use this but it's required
        }, registerFormModel.Password);

        if (!createResult.Succeeded)
        {
            throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }

        var user = await _userManager.FindByEmailAsync(registerFormModel.Email);
        await _userManager.AddToRoleAsync(user, Roles.Donor);
        
        _dbContext.Donors.Add(new Donor
        {
            UserId = user.Id,
            LocationId = registerFormModel.LocationId,
            BloodType = registerFormModel.BloodType,
            ContactInfo = registerFormModel.ContactInfo
        });
        
        await _dbContext.SaveChangesAsync();
        
        return user;
    }
}