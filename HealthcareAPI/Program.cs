using System.Text;
using System.Text.Json.Serialization;
using HealthcareAPI.Endpoints;
using HealthcareAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // Safely extract array of origins from app settings configurations
        var allowedOrigins = builder.Configuration
            .GetSection("CorsSettings:AllowedOrigins")
            .Get<string[]>();

        if (allowedOrigins is { Length: > 0 })
        {
            policy.WithOrigins(allowedOrigins);
        }
        else
        {
            // Fallback mechanism to keep local development intact if section is missing
            policy.WithOrigins("http://localhost:4200");
        }

        // Explicitly specify allowed HTTP verbs instead of allowing everything wildcard style
        policy.WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");

        // Limit headers 
        policy.WithHeaders("Content-Type", "Authorization", "Accept", "X-Requested-With");
    
        // Cache preflight OPTIONS checks for 10 minutes to protect against server resource drain
        policy.SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// Set up a global JSON converter to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        }; 
    });

builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=healthcare.db"));
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Creates the file and tables on the fly
}

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapDoctorEndpoints();
app.MapAppointmentEndpoints();
app.MapScheduleEndpoints();
app.MapSearchEndpoints();

app.Run();

public abstract partial class Program { }
