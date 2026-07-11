namespace HealthcareAPI.Models;

public class WaitlistAppointments
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public Appointments? Appointment { get; set; }
}