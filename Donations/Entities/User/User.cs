using Microsoft.AspNetCore.Identity;

namespace Donations.Entities.User;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; }
    
}