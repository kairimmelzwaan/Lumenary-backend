namespace backend.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public Guid TherapistUserId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Timezone { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Mode { get; set; } = null!;
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public DateTime? CanceledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Client Client { get; set; } = null!;
    public User TherapistUser { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}
