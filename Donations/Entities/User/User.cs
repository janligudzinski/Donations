using Microsoft.AspNetCore.Identity;

namespace Donations.Entities.User;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; }
    public virtual Donor? Donor { get; set; }
    public virtual DonationCenter? DonationCenter { get; set; }
}