namespace HealthcareAPI.Models;

public partial class Appointments
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ScheduleTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? SymptomsDescription { get; set; }
}