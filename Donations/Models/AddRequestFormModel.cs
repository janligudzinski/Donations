using Donations.Entities.Medical;

namespace Donations.Models;

public class AddRequestFormModel
{
    public HashSet<BloodType> BloodTypes { get; set; } = new();
    public bool Urgent { get; set; } = false;
    public DateTime Date { get; set; } = DateTime.UtcNow;
}