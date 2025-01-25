using System.ComponentModel.DataAnnotations.Schema;
using Donations.Entities.Common;
using Donations.Entities.Medical;

namespace Donations.Entities.User;

public class Donor
{
    public Guid Id { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    [ForeignKey("Location")]
    public Guid LocationId { get; set; }
    public virtual Location Location { get; set; }
    
    public string ContactInfo { get; set; }
    public BloodType BloodType { get; set; }
    public int Points {get; set;}
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}