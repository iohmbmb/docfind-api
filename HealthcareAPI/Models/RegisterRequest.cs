namespace HealthcareAPI.Models;

public class RegisterRequest
{
    public string? Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public Users.UserRole Role { get; set; }
    public PracticeSpecialty? Specialty { get; set; } 
    public Availability? Status { get; set; }
    public LocationPreference? Preference { get; set; }
    public float? HourlyRate { get; set; }
    public string? PracticeName { get; set; }
    public string? PracticeAddress { get; set; }
    public string? PracticeSuburb { get; set; }
    public string? PracticeState { get; set; }
    public string? PracticePostcode { get; set; }
    public string? PracticePhone { get; set; }
    public string? Biography { get; set; }
}