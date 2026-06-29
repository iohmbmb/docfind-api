using System.Net.Http.Json;
using HealthcareAPI.Models;

namespace HealthcareAPI.Tests.Utils;

public static class Mocks
{
    public static Users CreateUser()
    {
        Users mockUser = new Users 
        {
            Id = new Guid(), 
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@test.com", 
            PasswordHash = "averysafepassword", 
            Role = Users.UserRole.Patient 
        };
        return mockUser;
    }
    
    public static Users CreateUser(string firstname, string lastname, string email)
    {
        Users mockUser = new Users 
        {
            FirstName = firstname, 
            LastName = lastname, 
            Email = email, 
            PasswordHash = "averysafepassword", 
            Role = Users.UserRole.Patient 
        };
        return mockUser;
    }

    public static Doctors CreateDoctor()
    {
        Doctors mockDoctor = new Doctors
        {
            FirstName = "Dale",
            LastName = "Smith",
            Email = "dale.smith@test.com",
            PasswordHash = "averysafepassword", 
            Specialty = "General",
            HourlyRate = 60,
            Role = Users.UserRole.Doctor
        };
        return mockDoctor;
    }
    
    public static Doctors CreateDoctor(string firstname, string lastname, string email, string specialty, float hourlyRate)
    {
        Doctors mockDoctor = new Doctors
        {
            FirstName = firstname,
            LastName = lastname,
            Email = email,
            PasswordHash = "averysafepassword", 
            Specialty = specialty,
            HourlyRate = hourlyRate,
            Role = Users.UserRole.Doctor
        };
        return mockDoctor;
    }

    public static Appointments CreateAppointmentFor(Guid userId, Guid doctorId)
    {   
        var mockAppointment = new Appointments()
        {
            PatientId = userId,
            DoctorId = doctorId,
            ScheduleTime = DateTime.UtcNow.AddDays(1),
            SymptomsDescription = "Routine Checkup"
        };
        
        return mockAppointment;
    }
    
    public static MedicalRecords CreateMedicalRecordFor(Guid id)
    {
        var mockRecord = new MedicalRecords()
        {
            AppointmentId = id,
            Filename = "card.pdf",
            Filepath = "path/to/file",
            UploadedAt = DateTime.UtcNow
        };
        
        return mockRecord;
    }

    private static RegisterRequest CreateRegisterRequest<T>(T user) where T : Users
    {
        return new RegisterRequest
        {
            Email = user.Email,
            Password = user.PasswordHash,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Specialty = user is Doctors doctor ? doctor.Specialty : "",
            Role = user.Role
        };
    }

    private static LoginRequest CreateLoginRequest(string email, string password)
    {
        return new LoginRequest
        {
            Email = email,
            Password = password
        };
    }

    public static async Task<HttpResponseMessage> RegisterUser(Users user, HttpClient client)
    {
        var registerRequest = CreateRegisterRequest(user);
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        return registerResponse;
    }
    
    public static async Task RegisterUsers(List<Users> users, HttpClient client)
    {
        foreach (var registerRequest in users.Select(CreateRegisterRequest))
        {
            await client.PostAsJsonAsync("/api/auth/register", registerRequest);
        }

    }
    
    public static async Task<HttpResponseMessage?> LoginUser(Users user, HttpClient client)
    {
        if (user is not { Email: not null, PasswordHash: not null }) return null;
        var loginRequest = CreateLoginRequest(user.Email, user.PasswordHash);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        return loginResponse;
    }
}