using Donations.Entities.Medical;

namespace Donations.Models;

public class AddRequestFormModel
{
    public HashSet<BloodType> BloodTypes { get; set; } = new();
    public UrgencyLevel UrgencyLevel { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
}