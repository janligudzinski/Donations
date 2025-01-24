namespace Donations.Entities.Medical;

public enum BloodType
{
    AbPositive,
    AbNegative,
    OPositive,
    ONegative,
    APositive,
    ANegative,
    BPositive,
    BNegative
}

public static class BloodTypeExtensions
{
    public static string ToHumanReadableString(this BloodType bloodType)
    {
        return bloodType switch
        {
            BloodType.AbPositive => "AB+",
            BloodType.AbNegative => "AB-",
            BloodType.OPositive => "O+",
            BloodType.ONegative => "O-",
            BloodType.APositive => "A+",
            BloodType.ANegative => "A-",
            BloodType.BPositive => "B+",
            BloodType.BNegative => "B-",
            _ => throw new ArgumentOutOfRangeException(nameof(bloodType), bloodType, null)
        };
    }
}