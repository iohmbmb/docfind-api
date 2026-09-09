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

public class AuthTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions; 
    
    public AuthTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
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
    public async Task RegisterUser_ReturnOK()
    {
        var user = Mocks.CreateUser();
        var registerUser = await Mocks.RegisterUser(user, _client);
        var responseContent = await registerUser.Content.ReadFromJsonAsync<Users>(_jsonOptions);
        Assert.Equal(HttpStatusCode.OK, registerUser.StatusCode);       
        var beautifulJsonText = JsonSerializer.Serialize(responseContent, new JsonSerializerOptions { 
            WriteIndented = true 
        });
        _testOutputHelper.WriteLine(beautifulJsonText);
    }
    
    [Fact]
    public async Task RegisterDoctor_ReturnOK()
    {
        var user = Mocks.CreateDoctor("dal", "smith", "dale.mith@test.com", PracticeSpecialty.GeneralPractice, 60);
        var registerUser = await Mocks.RegisterDoctor(user, _client);
        var responseContent = await registerUser.Content.ReadFromJsonAsync<Doctors>(_jsonOptions);
        Assert.Equal(HttpStatusCode.OK, registerUser.StatusCode);       
        var beautifulJsonText = JsonSerializer.Serialize(responseContent, new JsonSerializerOptions { 
            WriteIndented = true 
        });
        _testOutputHelper.WriteLine(beautifulJsonText);
    }
    
    [Fact]
    public async Task LoginUser_ReturnOK()
    {
        var user = Mocks.CreateUser();
        var registerUser = await Mocks.RegisterUser(user, _client);
        Assert.Equal(HttpStatusCode.OK, registerUser.StatusCode);       
        var responseContent = await registerUser.Content.ReadFromJsonAsync<Users>(_jsonOptions);
        Assert.NotNull(responseContent);
        Assert.Equal(user.Email, responseContent.Email);

        var login = await Mocks.LoginUser(user, _client);
        if (login != null)
        { 
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
            var loginResponseContent = await login.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
            Assert.NotNull(loginResponseContent);
            Assert.NotNull(loginResponseContent.Token);
            Assert.True(loginResponseContent.Token.Length > 0);
        }
    }
}