using Donations.Entities.Common;

public class EditDonorInfoViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }
    public Guid LocationId { get; set; }
    public List<Location> AvailableLocations { get; set; } = new();
}