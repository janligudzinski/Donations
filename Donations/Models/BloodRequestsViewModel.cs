using Donations.Entities.Medical;

namespace Donations.Models;

public class BloodRequestsViewModel
{
    public IEnumerable<BloodRequest> Requests { get; set; } = new List<BloodRequest>();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}