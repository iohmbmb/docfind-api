using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using HealthcareAPI.Models;
using HealthcareAPI.Tests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace HealthcareAPI.Tests;

public class UserTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions; 
    
    public UserTests(WebApplicationFactory<Program> factory)
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
    public async Task GetUsers_ReturnOK()
    {
        var mockUser1 = Mocks.CreateUser();
        var mockUser2 = Mocks.CreateUser("tom", "doe", "tom@test.com");
        var mockUser3 = Mocks.CreateUser("jae", "doe", "jae@test.com");
        await Mocks.RegisterUsers([mockUser1, mockUser2, mockUser3], _client);
        
        var response = await _client.GetAsync("/api/get/users");
        var responseContent = await response.Content.ReadFromJsonAsync<List<Users>>(_jsonOptions);
        Assert.NotNull(responseContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);       
        Assert.True(responseContent.Count > 0);
    }
    
    [Fact]
    public async Task GetDoctors_ReturnOK()
    {
        var mockDoctor1 = Mocks.CreateDoctor("dale", "smith", "dale.smith@test.com", "General", 60);
        var mockDoctor2 = Mocks.CreateDoctor("emily", "cumberbasch", "emily@test.com", "Pediatric", 40);
        await Mocks.RegisterUsers([mockDoctor1, mockDoctor2], _client);
        
        var response = await _client.GetAsync("/api/get/doctors");
        var responseContent = await response.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        Assert.NotNull(responseContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);       
        Assert.True(responseContent.Count > 0);
    }
    
    [Fact]
    public async Task GetDoctors_BySpecialty_ReturnOK()
    {
        var mockDoctor1 = Mocks.CreateDoctor("fred", "smith", "fred@test.com", "General", 60);
        var mockDoctor2 = Mocks.CreateDoctor("emily", "cumberbasch", "emily@test.com", "Pediatric", 40);
        var mockDoctor3 = Mocks.CreateDoctor("jane", "doe", "jane@test.com", "General", 60);
        await Mocks.RegisterUsers([mockDoctor1, mockDoctor2, mockDoctor3], _client);

        const string specialty = "General";  
        var response = await _client.GetAsync($"/api/get/doctors/{specialty}");
        var responseContent = await response.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        Assert.NotNull(responseContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);       
        Assert.True(responseContent.Count == 2);
    }
    
    [Fact]
    public async Task DeleteUser_ReturnOK()
    {
        var mockUser = Mocks.CreateUser();
        await Mocks.RegisterUser(mockUser, _client);
        var response = await _client.GetAsync("/api/get/users");
        var responseContent = await response.Content.ReadFromJsonAsync<List<Users>>(_jsonOptions);
        Assert.NotNull(responseContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseContent.Count > 0);
        
        var deleteUser = await _client.DeleteAsync($"/api/delete/user/{responseContent.FirstOrDefault()!.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteUser.StatusCode);
        
        var refreshedResponse = await _client.GetAsync("/api/get/users");
        var refreshedResponseContent = await refreshedResponse.Content.ReadFromJsonAsync<List<Users>>(_jsonOptions);
        if (refreshedResponseContent != null) Assert.Empty(refreshedResponseContent);
        Assert.Equal(HttpStatusCode.OK, refreshedResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteDoctor_ReturnOK()
    {
        var mockDoctor = Mocks.CreateDoctor();
        await Mocks.RegisterUser(mockDoctor, _client);
        var response = await _client.GetAsync("/api/get/users");
        var responseContent = await response.Content.ReadFromJsonAsync<List<Doctors>>(_jsonOptions);
        Assert.NotNull(responseContent);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(responseContent.Count > 0);
        
        var deleteUser = await _client.DeleteAsync($"/api/delete/user/{responseContent.FirstOrDefault()!.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteUser.StatusCode);
        
        var refreshedResponse = await _client.GetAsync("/api/get/users");
        var refreshedResponseContent = await refreshedResponse.Content.ReadFromJsonAsync<List<Users>>(_jsonOptions);
        if (refreshedResponseContent != null) Assert.Empty(refreshedResponseContent);
        Assert.Equal(HttpStatusCode.OK, refreshedResponse.StatusCode);
    }
    
}