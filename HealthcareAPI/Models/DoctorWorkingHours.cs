namespace HealthcareAPI.Models;

public class DoctorWorkingHours
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsMock { get; set; }
}