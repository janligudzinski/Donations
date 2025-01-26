using System.ComponentModel.DataAnnotations.Schema;
using Donations.Entities.User;

namespace Donations.Entities.Medical;

public class Appointment
{
    public Guid Id { get; set; }

    [ForeignKey("BloodRequest")]
    public Guid BloodRequestId { get; set; }
    public virtual BloodRequest BloodRequest { get; set; }

    [ForeignKey("Donor")]
    public Guid DonorId { get; set; }
    public virtual Donor Donor { get; set; }

    public AppointmentState State { get; set; }
}
