using Donations.Entities.Medical;

public class AppointmentViewModel
{
    public AppointmentState State { get; set; }
    public string CenterName { get; set; }
    public string CenterLocation { get; set; }
    public DateTime Date { get; set; }
    public string RequestedBloodTypes { get; set; }
}