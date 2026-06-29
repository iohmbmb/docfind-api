namespace HealthcareAPI.Models;

public partial class Appointments
{
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
}