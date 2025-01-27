using Donations.Entities.Medical;

public class PendingAppointmentViewModel
{
    public Guid AppointmentId { get; set; }
    public string DonorName { get; set; }
    public string DonorLocation { get; set; }
    public string DonorBloodType { get; set; }
    public string RequestedBloodTypes { get; set; }
    public DateTime RequestDate { get; set; }
    public AppointmentState State { get; set; }
    public UrgencyLevel UrgencyLevel { get; set; }
}