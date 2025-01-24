using Donations.Entities.User;

namespace Donations.Models;

public class NearestDonationCentersViewModel
{
    public List<(DonationCenter Center, double Distance)> DonationCentersWithDistances { get; set; }
}