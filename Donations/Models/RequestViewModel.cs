using Donations.Entities.Medical;

namespace Donations.Models;

public class RequestViewModel
{
    public BloodRequest Request { get; set; }
    public bool UserHasApplied { get; set; } = false;
    public bool ShowAppliedModal { get; set; } = false;
    public bool CanDonate { get; set; }
    public string IneligibilityReason { get; set; }
}