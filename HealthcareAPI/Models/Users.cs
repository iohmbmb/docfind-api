namespace HealthcareAPI.Models;

public partial class Users
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public UserRole Role { get; set; }
}