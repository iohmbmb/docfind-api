using System.Net.Http.Json;
using HealthcareAPI.Models;
using Xunit.Sdk;

namespace HealthcareAPI.Tests.Utils;

public static class Mocks
{
    public static Users CreateUser()
    {
        Users mockUser = new Users 
        {
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
            ImagePath = "path/to/image",
            PracticeName = "Test Practice",
            PracticePostcode = "12345",
            PracticeAddress = "123 Test St",
            PracticePhone = "123-456-7890",
            PracticeState = "NSW",
            PracticeSuburb = "Sydney",
            Preference = LocationPreference.Hybrid,
            Biography = "Test Biography",
            Specialty = PracticeSpecialty.GeneralPractice,
            HourlyRate = 60,
            Status = Availability.Available,
            Latitude = 48.8130f,
            Longitude = 2.3882f,
            IsMock = false,
            Role = Users.UserRole.Doctor
        };
        return mockDoctor;
    }
    
    public static Doctors CreateDoctor(string firstname, string lastname, string email, PracticeSpecialty specialty, float hourlyRate)
    {
        Doctors mockDoctor = new Doctors
        {
            FirstName = firstname,
            LastName = lastname,
            Email = email,
            PasswordHash = "averysafepassword", 
            ImagePath = "path/to/image",
            PracticeName = "Test Practice",
            PracticePostcode = "12345",
            PracticeAddress = "123 Test St",
            PracticePhone = "123-456-7890",
            PracticeState = "NSW",
            PracticeSuburb = "Sydney",
            Preference = LocationPreference.Hybrid,
            Biography = "Test Biography",
            Specialty = specialty,
            HourlyRate = hourlyRate,
            Status = Availability.Available,
            Latitude = 48.8130f,
            Longitude = 2.3882f,
            IsMock = false,
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

    public static UnavailabilityPeriod CreateUnavailabilityPeriodFor(Guid id)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var period = new UnavailabilityPeriod()
        {
            DoctorId = id,
            StartDate = today.AddDays(1),
            EndDate = today.AddDays(2),
            IsMock = false
        };
        return period;
    }

    public static List<DoctorWorkingHours> CreateDoctorWorkingHoursFor(Guid id, DayOfWeek day)
    {
        var ret = new List<DoctorWorkingHours>();
        var workHours = new DoctorWorkingHours()
        {
            DoctorId = id,
            Day = day,
            StartTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(9)),
            EndTime = TimeOnly.FromTimeSpan(TimeSpan.FromHours(17))
        };
        ret.Add(workHours);
        return ret;
    }

    private static RegisterRequest CreateRegisterRequest<T>(T user) where T : Users
    {
        return new RegisterRequest
        {
            Email = user.Email,
            Password = user.PasswordHash,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Specialty = user is Doctors doctor ? doctor.Specialty : PracticeSpecialty.GeneralPractice,
            Status = user is Doctors d ? d.Status : Availability.Available,
            Biography = "Test Biography",
            PracticeAddress = "123 Test St",
            PracticePhone = "123-456-7890",
            PracticeName = "Test Practice",
            PracticePostcode = "12345",
            PracticeState = "NSW",
            PracticeSuburb = "Sydney",
            Latitude = user is Doctors doce ? doce.Latitude : 0,
            Longitude = user is Doctors docto ? docto.Longitude : 0,
            Preference = user is Doctors doc ? doc.Preference : LocationPreference.Hybrid,
            HourlyRate = user is Doctors doct ? doct.HourlyRate : 0,
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
    
    public static async Task<HttpResponseMessage> RegisterDoctor(Doctors user, HttpClient client)
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
    
    public static async Task<List<HttpResponseMessage>> RegisterDoctors(List<Doctors> doctors, HttpClient client)
    {
        var ret = new List<HttpResponseMessage>();
        foreach (var registerRequest in doctors.Select(CreateRegisterRequest))
        {
            ret.Add(await client.PostAsJsonAsync("/api/auth/register", registerRequest));
        }

        return ret;
    }
    
    public static async Task<HttpResponseMessage?> LoginUser(Users user, HttpClient client)
    {
        if (user is not { Email: not null, PasswordHash: not null }) return null;
        var loginRequest = CreateLoginRequest(user.Email, user.PasswordHash);
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        return loginResponse;
    }
    
    public static async Task<HttpResponseMessage?> CreateUnavailabilityPeriod(Guid docId, UnavailabilityPeriod period, HttpClient client)
    {
        var response = await client.PostAsJsonAsync($"/api/schedule/post/{docId}/absence", period);
        return response;
    }
    
    public static async Task<HttpResponseMessage?> CreateDoctorWorkHours(Guid docId, List<DoctorWorkingHours> workHours, HttpClient client)
    {
        var response = await client.PostAsJsonAsync($"/api/schedule/post/{docId}/workhours", workHours);
        return response;
    }
}