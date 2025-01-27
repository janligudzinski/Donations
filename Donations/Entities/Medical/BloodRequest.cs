using System.ComponentModel.DataAnnotations.Schema;
using Donations.Entities.User;
using Donations.Entities.Common;

namespace Donations.Entities.Medical;

public class BloodRequest
{
    public Guid Id { get; set; }

    [ForeignKey("DonationCenter")]
    public Guid DonationCenterId { get; set; }
    public virtual DonationCenter DonationCenter { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;
    public UrgencyLevel UrgencyLevel { get; set; }

    // Store blood types as a string of comma-separated values
    // This is because we want a HashSet so the types don't repeat, but we can't store a HashSet in a database directly without tricks like this or at the EF configuration level with a custom converter
    public string BloodTypesString { get; set; }

    [NotMapped] // don't actually make this a column in our DB, it will only exist in memory in the app
    public HashSet<BloodType> BloodTypes
    {
        get => string.IsNullOrEmpty(BloodTypesString)
            ? new HashSet<BloodType>()
            : BloodTypesString.Split(',')
                .Select(bt => Enum.Parse<BloodType>(bt))
                .ToHashSet();
        set => BloodTypesString = string.Join(",", value);
    }

    public int TargetMilliliters { get; set; }
}