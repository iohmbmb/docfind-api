using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthcareAPI.Models;
using HealthcareAPI.Tests.Models;
using HealthcareAPI.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HealthcareAPI.Tests;

public class AuthTests  : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions; 
    
    public AuthTests(WebApplicationFactory<Program> factory)
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
    public async Task RegisterUser_ReturnOK()
    {
        var user = Mocks.CreateUser();
        var registerUser = await Mocks.RegisterUser(user, _client);
        var responseContent = await registerUser.Content.ReadFromJsonAsync<string>(_jsonOptions);
        Assert.Equal(HttpStatusCode.OK, registerUser.StatusCode);       
        Assert.Equal("Registration successful!", responseContent);
    }
    
    [Fact]
    public async Task LoginUser_ReturnOK()
    {
        var user = Mocks.CreateUser();
        var registerUser = await Mocks.RegisterUser(user, _client);
        Assert.Equal(HttpStatusCode.OK, registerUser.StatusCode);       
        var responseContent = await registerUser.Content.ReadFromJsonAsync<string>(_jsonOptions);
        Assert.Equal("Registration successful!", responseContent);

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