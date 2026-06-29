namespace HealthcareAPI.Models;

public class MedicalRecords
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public string? Filename { get; set; }
    public string? Filepath { get; set; }
    public DateTime UploadedAt { get; set; }
}