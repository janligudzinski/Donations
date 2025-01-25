using System.ComponentModel.DataAnnotations.Schema;
using Donations.Entities.Common;
using Donations.Entities.Medical;

namespace Donations.Entities.User;

public class DonationCenter
{
    public Guid Id { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    [ForeignKey("Location")]
    public Guid LocationId { get; set; }
    public virtual Location Location { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();
}