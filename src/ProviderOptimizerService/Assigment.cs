using System;

/// <summary>
/// Propiedades asociada con la identificacion
/// </summary>
public class Assignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RequestId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public double ETA { get; set; }
}
