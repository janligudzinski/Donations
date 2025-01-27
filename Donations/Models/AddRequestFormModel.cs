using System.ComponentModel.DataAnnotations;
using Donations.Entities.Medical;

namespace Donations.Models;

public class AddRequestFormModel
{
    [Required(ErrorMessage = "Please select at least one blood type")]
    [MinLength(1, ErrorMessage = "Please select at least one blood type")]
    public HashSet<BloodType> BloodTypes { get; set; } = new();
    public UrgencyLevel UrgencyLevel { get; set; } = UrgencyLevel.Normal;
    public DateTime Date { get; set; } = DateTime.Today;
    public int TargetMilliliters { get; set; } = 450;
}