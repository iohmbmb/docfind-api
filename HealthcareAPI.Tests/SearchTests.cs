using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthcareAPI.Models;
using HealthcareAPI.Tests.Models;
using HealthcareAPI.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace HealthcareAPI.Tests;

public class SearchTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions; 
    
    public SearchTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
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
    public async Task SearchDoctor_ReturnOK()
    {
        var doctor = Mocks.CreateDoctor();
        var registerDoc = await Mocks.RegisterDoctor(doctor, _client);
        var responseContentReg = await registerDoc.Content.ReadFromJsonAsync<Doctors>(_jsonOptions);
        Assert.Equal(HttpStatusCode.OK, registerDoc.StatusCode);
        const float longitude = 2.3882f;
        const float latitude = 48.8130f;
        var response = await _client.GetAsync($"/api/search/doctor?specialty=GeneralPractice&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContentSearch = await response.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        Assert.NotNull(responseContentSearch);
        Assert.True(responseContentSearch.Count > 0);
    }
    
    [Fact]
    public async Task ShowUnavailableDoctors_ReturnOK()
    {
        var today = DateTime.Today;
        var doctor = Mocks.CreateDoctor();
        var doctor2 = Mocks.CreateDoctor("Emily", "Cumberbasch", "emily.cumb@test.com", PracticeSpecialty.GeneralPractice, 40);
        
        var doctors = await Mocks.RegisterDoctors([doctor, doctor2], _client);
        
        // Got to use those values because they have the correct ids
        var doc1 = await doctors[0]?.Content.ReadFromJsonAsync<Doctors>(_jsonOptions)!;
        var doc2 = await doctors[1]?.Content.ReadFromJsonAsync<Doctors>(_jsonOptions)!;
        
        var login = await Mocks.LoginUser(doctor, _client);
        var loginResponseContent = await login?.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)!;
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent?.Token);
        
        // Create work hours
        var workHoursDoc1 = Mocks.CreateDoctorWorkingHoursFor(doc1!.Id, today.AddDays(1).DayOfWeek);
        var hours1Resp = await Mocks.CreateDoctorWorkHours(doc1!.Id, workHoursDoc1, _client);
        Assert.Equal(HttpStatusCode.Created, hours1Resp?.StatusCode);
        
        var workHoursDoc2 = Mocks.CreateDoctorWorkingHoursFor(doc2!.Id, today.DayOfWeek);
        var hours2Resp = await Mocks.CreateDoctorWorkHours(doc2!.Id, workHoursDoc2, _client);
        Assert.Equal(HttpStatusCode.Created, hours2Resp?.StatusCode);
        
        // Create unavailability period
        var unavailabilityPeriod = Mocks.CreateUnavailabilityPeriodFor(doc1!.Id);
        var periodResponse = await Mocks.CreateUnavailabilityPeriod(doc1!.Id, unavailabilityPeriod, _client);
        Assert.Equal(HttpStatusCode.OK, periodResponse?.StatusCode);
        
        const float longitude = 2.3882f;
        const float latitude = 48.8130f;
        var response = await _client.GetAsync($"/api/search/doctor?specialty=GeneralPractice&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContentSearch = await response.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        Assert.NotNull(responseContentSearch);
        Assert.Equal(responseContentSearch[0].IsUnavailable, true);
    }
}