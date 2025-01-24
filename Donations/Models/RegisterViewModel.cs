using Donations.Entities.Common;
using Donations.Entities.Medical;

namespace Donations.Models;

public class RegisterViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string ContactInfo { get; set; }
    // sorted dictionary to keep order
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

    public List<Location> EligibleLocations =
    [
        new Location()
        {
            Id = Guid.NewGuid(),
            Name = "Amman",
            Latitude = 1.0,
            Longitude = 1.0
        }
    ];
}