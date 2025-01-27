using Donations.Entities.User;

namespace Donations.Entities.Medical;

public class BloodSupply
{
    public Guid Id { get; set; }
    public BloodType BloodType { get; set; }
    public int MillilitersInStock { get; set; }

    public Guid DonationCenterId { get; set; }
    public DonationCenter DonationCenter { get; set; }
}