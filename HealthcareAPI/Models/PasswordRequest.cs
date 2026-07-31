namespace HealthcareAPI.Models;

public class PasswordRequest
{
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}