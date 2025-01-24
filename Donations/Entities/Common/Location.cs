namespace Donations.Entities.Common;

public class Location
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public static double Distance(Location one, Location other)
    {
        // Haversine distance formula
        double ToRadians(double angle)
        {
            return angle * (Math.PI / 180);
        }
        const double earthRadiusKilometers = 6371; // kilometers
        var differenceLatitudes = ToRadians(one.Latitude - other.Latitude);
        var differenceLongitudes = ToRadians(one.Longitude - other.Longitude);
        
        var a = Math.Sin(differenceLatitudes / 2) * Math.Sin(differenceLatitudes / 2) +
                Math.Cos(ToRadians(one.Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(differenceLongitudes / 2) * Math.Sin(differenceLongitudes / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = earthRadiusKilometers * c;
        return d;
    }
}