using System.ComponentModel.DataAnnotations.Schema;

namespace Donations.Entities.User;

public class Notification
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public bool IsRead { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
}