using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HealthcareAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HealthcareAPI.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/me", (ClaimsPrincipal claimsUser) =>
            {
                var userIdString = claimsUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return string.IsNullOrEmpty(userIdString) ? Results.Unauthorized() : Results.Ok(new { Id = Guid.Parse(userIdString) });
            })
            .RequireAuthorization()
            .WithTags("Auth")
            .WithSummary("Retrieves the authenticated user's own identity GUID securely.");
        
        app.MapPost("/api/auth/register", async (RegisterRequest request, AppDbContext context) =>
        {
            var ret = new Users();
            
            if (await context.Users.AnyAsync(u => u.Email == request.Email) || await context.Doctors.AnyAsync(d => d.Email == request.Email))
            {
                return Results.BadRequest("Email is already registered.");
            }

            var hasher = new PasswordHasher<Users>();
            var generatedId = Guid.NewGuid();

            switch (request.Role)
            {
                case Users.UserRole.Patient:
                    var patient = new Users
                    {
                        Id = generatedId,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Role = request.Role
                    };
                    if (request.Password != null) patient.PasswordHash = hasher.HashPassword(patient, request.Password);
                    ret = patient;
                    context.Users.Add(ret); 
                    break;

                case Users.UserRole.Doctor:
                    var doctor = new Doctors
                    {
                        Id = generatedId,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Role = request.Role,
                        Specialty = request.Specialty ?? PracticeSpecialty.GeneralPractice,
                        PracticeName = request.PracticeName,
                        PracticeAddress = request.PracticeAddress,
                        PracticeSuburb = request.PracticeSuburb,
                        PracticeState = request.PracticeState,
                        PracticePostcode = request.PracticePostcode,
                        PracticePhone = request.PracticePhone,
                        Biography = request.Biography,
                        Preference = request.Preference ?? LocationPreference.Hybrid,
                        Status = request.Status ?? Availability.Available,
                        HourlyRate = request.HourlyRate ?? 0,
                        Longitude = request.Longitude ?? 0,
                        Latitude = request.Latitude ?? 0,
                        
                    };
                    if (request.Password != null) doctor.PasswordHash = hasher.HashPassword(doctor, request.Password);
                    ret = doctor;
                    context.Doctors.Add(doctor); 
                    break;

                default:
                    return Results.BadRequest("Invalid user role registration request.");
            }

            await context.SaveChangesAsync();
            return Results.Ok(ret);
        }).WithTags("Auth");
        
        app.MapPost("/api/auth/login", async (LoginRequest request, AppDbContext context, IConfiguration config) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null) return Results.Unauthorized();

                // Verify the incoming password against the hashed database block
                var hasher = new PasswordHasher<Users>();
                var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return Results.Unauthorized();
                }

                // Generate claims 
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Role, user.Role.ToString()), // Binds UserRole
                    new Claim("FirstName", user.FirstName ?? "")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Secret"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); 

                var token = new JwtSecurityToken(
                    issuer: config["JwtSettings:Issuer"],
                    audience: config["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7), // Token valid for 1 week
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Return the token payload back to Angular
                return Results.Ok(new { Token = tokenString, User = new { user.Email, user.FirstName, user.Role } });
            }).WithTags("Auth");
        
        app.MapPut("/api/auth/update/password", async (PasswordRequest request, ClaimsPrincipal claimsUser, AppDbContext context) =>
        {
            PasswordHasher<Users> hasher = new PasswordHasher<Users>();
            var userIdString = claimsUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = context.Users.FirstOrDefault(u => userIdString != null && u.Id == Guid.Parse(userIdString));
            
            if (user == null) return Results.Unauthorized();
            
            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash!, request.CurrentPassword);
            if (verificationResult == PasswordVerificationResult.Failed) return Results.Unauthorized();

            if (request.NewPassword != null) user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
            await context.SaveChangesAsync();
            
            return Results.Ok();
        }).RequireAuthorization().WithTags("Auth");
    }
}