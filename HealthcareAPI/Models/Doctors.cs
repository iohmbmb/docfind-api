namespace HealthcareAPI.Models;

public class Doctors : Users
{
    public string? PracticeName { get; set; }
    public string? PracticeAddress { get; set; }
    public string? PracticeSuburb { get; set; }
    public string? PracticeState { get; set; }
    public string? PracticePostcode { get; set; }
    public string? PracticePhone { get; set; }
    public string? Biography { get; set; }
    public float HourlyRate { get; set; }
    public Availability? Status { get; set; }
    public LocationPreference? Preference { get; set; }
    public PracticeSpecialty? Specialty { get; set; }
}