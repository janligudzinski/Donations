using System.ComponentModel.DataAnnotations.Schema;
using Donations.Entities.Medical;
using Donations.Entities.User;

namespace Donations.Entities.Medical;

public class BloodSupply
{
    public Guid Id { get; set; }
    public BloodType BloodType { get; set; }
    public int MillilitersInStock { get; set; }
    public int DesiredMilliliters { get; set; }

    [ForeignKey("DonationCenter")]
    public Guid DonationCenterId { get; set; }
    public DonationCenter DonationCenter { get; set; }

    public int MillilitersNeeded => Math.Max(0, DesiredMilliliters - MillilitersInStock);
}