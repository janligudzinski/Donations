using Donations.Entities.Common;
using Donations.Entities.Medical;

namespace Donations.Models;

public class RegisterViewModel
{
    public SortedDictionary<string, BloodType> BloodTypes { get; set; } = new()
    {
        ["AB+"] = BloodType.AbPositive,
        ["AB-"] = BloodType.AbNegative,
        ["O+"] = BloodType.OPositive,
        ["O-"] = BloodType.ONegative,
        ["A+"] = BloodType.APositive,
        ["A-"] = BloodType.ANegative,
        ["B+"] = BloodType.BPositive,
        ["B-"] = BloodType.BNegative
    };
    // filled in by the controller
    public List<Location> EligibleLocations { get; set; } = [];
    public string? ErrorMessage { get; set; }
}