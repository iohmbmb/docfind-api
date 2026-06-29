using HealthcareAPI.Models;

namespace HealthcareAPI.Tests.Models;

public class LoginResponse
{
    public string? Token { get; set; }
    public Users? User { get; set; }
}