namespace HealthcareAPI.Models;

public class Doctors : Users
{
    public string? Specialty { get; set; }
    public float HourlyRate { get; set; }
}