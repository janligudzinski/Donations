using System.ComponentModel.DataAnnotations.Schema;

namespace Donations.Entities.User;

public class DonationCenter
{
    public Guid Id { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}