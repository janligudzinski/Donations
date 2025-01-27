using Donations.Entities.Medical;

namespace Donations.Models;

public class RegisterFormModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ContactInfo { get; set; }
    public BloodType BloodType { get; set; }
    public Guid LocationId { get; set; }
    public bool Consent { get; set; }
}