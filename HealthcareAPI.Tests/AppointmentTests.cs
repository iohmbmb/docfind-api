using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthcareAPI.Models;
using HealthcareAPI.Tests.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using HealthcareAPI.Tests.Utils;
[assembly: CollectionBehavior(DisableTestParallelization = true)] // Force every tests to run in sequence and not depend on the shared state. Avoid multiple tests running at the same time and falling.

namespace HealthcareAPI.Tests;
public class AppointmentTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions; 
    
    public AppointmentTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
        // Wipe the database structure out completely
        db.Database.EnsureDeleted(); 
            
        // Re-stamp fresh, empty tables instantly
        db.Database.EnsureCreated();
    }

    
    [Fact]
    public async Task GetAppointment_WithValidId_ReturnsAppointment()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Create appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
        }

        // Get appointment
        var response = await _client.GetAsync($"/api/get/user/{meResponseContent.Id}/appointments");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var userAppointments = await response.Content.ReadFromJsonAsync<List<Appointments>>(_jsonOptions);
        if (userAppointments != null) Assert.True(userAppointments.Count > 0); 
    }
    
    [Fact]
    public async Task GetRecord_WithValidAppointmentId_ReturnsRecord()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Create appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
        
            // Create medical record
            var newMedicalRecord = Mocks.CreateMedicalRecordFor(savedAppointment.Id);
            var medicalResponse = await _client.PostAsJsonAsync("/api/create/record", newMedicalRecord, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, medicalResponse.StatusCode);
            var savedMedicalRecord = await medicalResponse.Content.ReadFromJsonAsync<MedicalRecords>(_jsonOptions);
            Assert.NotNull(savedMedicalRecord);
        
            // Get medical records
            var response = await _client.GetAsync($"/api/get/record/{savedAppointment.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var appointmentRecord = await response.Content.ReadFromJsonAsync<List<MedicalRecords>>(_jsonOptions);
            Assert.NotNull(appointmentRecord);
            Assert.True(appointmentRecord.Count > 0);
        }
    }
    
    [Fact]
    public async Task CreateAppointment_WithValidData_ReturnsCreatedStatusCode()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Create appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
        
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
            Assert.True(savedAppointment.Id != Guid.Empty);
        }
    }
    
    [Fact]
    public async Task CreateMedicalRecord_WithValidData_ReturnsCreatedStatusCode()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Create appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
        
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
        
            var newMedicalRecord = Mocks.CreateMedicalRecordFor(savedAppointment.Id);
            var response = await _client.PostAsJsonAsync("/api/create/record", newMedicalRecord, _jsonOptions);
        
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var savedMedicalRecord = await response.Content.ReadFromJsonAsync<MedicalRecords>(_jsonOptions);
            Assert.NotNull(savedMedicalRecord);
            Assert.True(savedMedicalRecord.Id != Guid.Empty);
            Assert.True(savedMedicalRecord.AppointmentId != Guid.Empty);
        }
    }
    
    [Fact]
    public async Task UpdateAppointment_WithValidData_ReturnsOkStatusCode()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Update appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
            Assert.True(savedAppointment.Id != Guid.Empty); 
            Assert.NotNull(savedAppointment);

            savedAppointment.Status = Appointments.AppointmentStatus.Cancelled;
            var updateResponse = await _client.PutAsJsonAsync($"/api/update/appointment/{savedAppointment.Id}", savedAppointment, _jsonOptions);
    
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    
            var verifyResponse = await _client.GetAsync($"/api/get/user/{meResponseContent.Id}/appointments");
            Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
    
            var refreshedAppointment = await verifyResponse.Content.ReadFromJsonAsync<List<Appointments>>(_jsonOptions);
            Assert.NotNull(refreshedAppointment);
            Assert.True(refreshedAppointment.Count > 0);
        
            var specificAppointment = refreshedAppointment.Single(a => a.Id == savedAppointment.Id);
            Assert.Equal(Appointments.AppointmentStatus.Cancelled, specificAppointment.Status);
        }
    }
    
    
    [Fact]
    public async Task UpdateMedicalRecord_WithValidData_ReturnsOkStatusCode()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Update appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment =
                Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment =
                await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
            Assert.True(savedAppointment.Id != Guid.Empty);
            Assert.NotNull(savedAppointment);

            var newMedicalRecord = Mocks.CreateMedicalRecordFor(savedAppointment.Id);
            var createMedicalRecordResponse = await _client.PostAsJsonAsync($"/api/create/record", newMedicalRecord, _jsonOptions);
            var savedMedicalRecord = await createMedicalRecordResponse.Content.ReadFromJsonAsync<MedicalRecords>(_jsonOptions);
            Assert.NotNull(savedMedicalRecord);
        
            savedMedicalRecord.Filename = "test.pdf";
        
            var updateResponse = await _client.PutAsJsonAsync($"/api/update/record/{savedMedicalRecord.Id}", savedMedicalRecord, _jsonOptions);
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    
            var verifyResponse = await _client.GetAsync($"/api/get/record/{savedAppointment.Id}");
            Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
    
            var refreshedMedicalRecords = await verifyResponse.Content.ReadFromJsonAsync<List<MedicalRecords>>(_jsonOptions);
            Assert.NotNull(refreshedMedicalRecords);
            Assert.True(refreshedMedicalRecords.Count > 0);
        
            var specificMedicalRecord = refreshedMedicalRecords.Single(a => a.Id == savedMedicalRecord.Id);
            Assert.True(specificMedicalRecord.Filename == "test.pdf");
        }

    }
    
    [Fact]
    public async Task DeleteAppointment_ReturnsNotFound_WhenAppointmentDoesNotExist()
    {
        // Register user
        var mockUser = Mocks.CreateUser();
        await Mocks.RegisterUser(mockUser, _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        var id = Guid.Empty;
        var deleteResponse = await _client.DeleteAsync($"/api/delete/appointment/{id}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteAppointment_ReturnsNotFound_WhenMedicalRecordDoesNotExist()
    {
        // Register user
        var mockUser = Mocks.CreateUser();
        await Mocks.RegisterUser(mockUser, _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        var id = Guid.Empty;
        var deleteResponse = await _client.DeleteAsync($"/api/delete/record/{id}");
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteAppointment_ReturnsOk_WhenAppointmentExist()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Create appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
        
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
        
            var deleteResponse = await _client.DeleteAsync($"/api/delete/appointment/{savedAppointment.Id}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }
    }
    
    [Fact]
    public async Task DeleteRecord_ReturnsOk_WhenMedicalRecordExist()
    {
        // Register user and doctor
        var mockUser = Mocks.CreateUser();
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUsers([mockUser, mockDoctor], _client);
        
        // Login user
        var login = await Mocks.LoginUser(mockUser, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        Assert.NotNull(loginResponseContent);
        Assert.NotNull(loginResponseContent.Token);
        
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        
        // Get my id
        var meResponse = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meResponseContent = await meResponse.Content.ReadFromJsonAsync<UserIdentityRecord>(_jsonOptions);
        Assert.NotNull(meResponseContent);
        Assert.NotEqual(Guid.Empty, meResponseContent.Id);
        
        // Get the list of doctors
        var getDoctorsResponse = await _client.GetAsync("/api/get/doctors");
        var getDoctorsResponseContent = await getDoctorsResponse.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        
        // Create appointment
        if (getDoctorsResponseContent != null)
        {
            var mockAppointment = Mocks.CreateAppointmentFor(meResponseContent.Id, getDoctorsResponseContent.FirstOrDefault()!.Id);
            var createAppointment = await _client.PostAsJsonAsync("/api/create/appointment", mockAppointment, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, createAppointment.StatusCode);
            var savedAppointment = await createAppointment.Content.ReadFromJsonAsync<Appointments>(_jsonOptions);
            Assert.NotNull(savedAppointment);
        
            // Create medical record
            var newMedicalRecord = Mocks.CreateMedicalRecordFor(savedAppointment.Id);
            var medicalResponse = await _client.PostAsJsonAsync("/api/create/record", newMedicalRecord, _jsonOptions);
            Assert.Equal(HttpStatusCode.Created, medicalResponse.StatusCode);
            var savedMedicalRecord = await medicalResponse.Content.ReadFromJsonAsync<MedicalRecords>(_jsonOptions);
            Assert.NotNull(savedMedicalRecord);
        
            var deleteResponse = await _client.DeleteAsync($"/api/delete/record/{savedMedicalRecord.Id}");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }
    }
}

public record UserIdentityRecord(Guid Id);