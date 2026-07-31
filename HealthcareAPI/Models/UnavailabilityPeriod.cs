namespace HealthcareAPI.Models;

public class UnavailabilityPeriod
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}